using System.Diagnostics.CodeAnalysis;
using BinaryReader = AeonSake.BinaryTools.BinaryReader;

namespace LTDSaveEditor.Core.SAV;

public class SavFile
{
    public static byte[] Magic = { 4,3,2,1 };

    public int Version { get; set; }
    public int SaveDataOffset { get; set; }

    public Dictionary<uint, SavFileEntry> Entries = [];

    public SavFile(Stream stream)
    {
        using var reader = new BinaryReader(stream);

        var magic = reader.ReadByteArray(4);

        if (!Magic.SequenceEqual(magic))
            throw new Exception("Invalid save file format.");

        Version = reader.ReadInt32();
        SaveDataOffset = reader.ReadInt32();

        reader.Align(0x20);

        var currentData = DataType.Bool;
        while (reader.BaseStream.Position < SaveDataOffset)
        {
            var hash = reader.ReadUInt32();
            if (hash == 0)
            {
                currentData = (DataType) reader.ReadUInt32();
                continue;
            }

            var entry = new SavFileEntry(hash, currentData, reader);
            Entries.TryAdd(hash, entry);
        }
    }

    public bool TryGetValue(uint hash, [MaybeNullWhen(false)] out SavFileEntry entry)
    {
        if (Entries.TryGetValue(hash, out var val))
        {
            entry = val;
            return true;
        }

        entry = null;
        return false;
    }
}
