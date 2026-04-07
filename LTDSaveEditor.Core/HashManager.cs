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

        var hashes = new Dictionary<uint, GameData>();

        foreach (var line in File.ReadLines(hashesCSV))
        {
            if (!TryParseGameData(line, out var data))
                continue;

            hashes[data.Number] = data;
        }

        Hashes = hashes;
    }

    private static bool TryParseGameData(string line, [MaybeNullWhen(false)] out GameData data)
    {
        data = null;

        if (string.IsNullOrWhiteSpace(line))
            return false;

        var parts = line.Split(',', 5, StringSplitOptions.None);
        if (parts.Length < 4)
            return false;

        if (!uint.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var number))
            return false;

        data = new GameData
        {
            Number = number,
            Name = parts[3].Trim(),
            Options = ParseOptions(parts.Length == 5 ? parts[4] : null)
        };

        return true;
    }

    private static Dictionary<uint, string> ParseOptions(string? rawOptions)
    {
        if (string.IsNullOrWhiteSpace(rawOptions))
            return [];

        var options = new Dictionary<uint, string>();

        foreach (var option in rawOptions.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (option.Length == 0)
                continue;

            options[option.ToMurmur()] = option;
        }

        return options;
    }
}
