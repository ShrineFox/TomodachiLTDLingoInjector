using CsvHelper;
using CsvHelper.Configuration;
using LTDSaveEditor.Core.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace LTDSaveEditor.Core;

public class GameData
{
    public uint Number { get; set; }
    public string? Name { get; set; }
    public Dictionary<uint, string> Options { get; set; } = [];
}

public class GameDataMap : ClassMap<GameData>
{
    public GameDataMap()
    {
        Map(m => m.Number).Index(1);
        Map(m => m.Name).Index(3);
        Map(m => m.Options).Index(4).Convert(args =>
        {
            if (args.Row.Parser.Count <= 4)
                return [];

            var field = args.Row.GetField(4);

            if (string.IsNullOrWhiteSpace(field))
                return [];

            var options = field.Split(';', StringSplitOptions.RemoveEmptyEntries);

            var dict = new Dictionary<uint, string>();

            foreach (var option in options)
            {
                var trimmed = option.Trim();
                uint crc = trimmed.ToMurmur();
                dict[crc] = trimmed;
            }

            return dict;
        });
    }
}

public static class HashManager
{
    public static Dictionary<uint, GameData> Hashes { get; private set; } = [];
    public static bool IsInitialized => Hashes.Count > 0;

    public static string GetName(uint hash)
    {
        if (Hashes.TryGetValue(hash, out var data) && data.Name != null)
            return data.Name;
        return "< Unknown >";
    }

    public static bool TryGetData(uint hash, [MaybeNullWhen(false)] out GameData result)
    {
        if (Hashes.TryGetValue(hash, out var data))
        {
            result = data;
            return true;
        }

        result = null;
        return false;
    }

    public static void Initialize(string hashesCSV)
    {
        Hashes.Clear();

        if (!File.Exists(hashesCSV))
            throw new FileNotFoundException($"The specified file '{hashesCSV}' does not exist.");

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
        };

        using var reader = new StreamReader(hashesCSV);
        using var csv = new CsvReader(reader, config);

        csv.Context.RegisterClassMap<GameDataMap>();

        Hashes = csv.GetRecords<GameData>().ToDictionary(x => x.Number, y => y);
    }
}
