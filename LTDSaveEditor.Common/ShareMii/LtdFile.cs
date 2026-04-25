using LTDSaveEditor.Core.Extensions;
using System.Text;
using BinaryReader = AeonSake.BinaryTools.BinaryReader;
using BinaryWriter = AeonSake.BinaryTools.BinaryWriter;

namespace LTDSaveEditor.Common.ShareMii;

public sealed class LtdFile
{
    public const uint CanvasHeaderMagic = 0xA3A3A3A3;
    public const uint TexHeaderMagic = 0xA4A4A4A4;

    public byte Version { get; set; }

    public byte[] Mii_CharInfoEx { get; set; } = new byte[0x98];

    public Dictionary<uint, int> IntValues { get; set; } = [];
    public Dictionary<uint, uint> UIntValues { get; set; } = [];

    public int Mii_CharacterParam_Sociability { get; set; }
    public int Mii_CharacterParam_Audaciousness { get; set; }
    public int Mii_CharacterParam_Activeness { get; set; }
    public int Mii_CharacterParam_Commonsense { get; set; }
    public int Mii_CharacterParam_Gaiety { get; set; }
    public int Mii_Voice_Formant { get; set; }
    public int Mii_Voice_Speed { get; set; }
    public int Mii_Voice_Intonation { get; set; }
    public int Mii_Voice_Pitch { get; set; }
    public int Mii_Voice_Tension { get; set; }
    public uint Mii_Voice_PresetType { get; set; } // Enum
    public uint Mii_MiiMisc_FaceInfo_Gender { get; set; } // Enum
    public uint Mii_Name_PronounType { get; set; } // Enum
    public uint Mii_MiiMisc_ClothInfo_ClothStyle { get; set; } // Enum
    public int Mii_MiiMisc_BirthdayInfo_Year { get; set; }
    public int Mii_MiiMisc_BirthdayInfo_Day { get; set; }
    public int Mii_MiiMisc_BirthdayInfo_DirectAge { get; set; }
    public int Mii_MiiMisc_BirthdayInfo_Month { get; set; }

    public string MiiName { get; set; } = string.Empty;
    public string MiiNamePronounced { get; set; } = string.Empty;
    public bool MiiSexuality_Man { get; set; }
    public bool MiiSexuality_Woman { get; set; }
    public bool MiiSexuality_NonBinary { get; set; }

    public byte[]? FacePaint { get; set; }
    public byte[]? TexData { get; set; }

    public static LtdFile Read(string path)
    {
        using var fs = File.OpenRead(path);
        return Read(fs);
    }

    public LtdFile(BinaryReader reader)
    {
        Version = reader.ReadByte();
        var hasFacePaintCanvas = reader.ReadByte() == 1;
        var hasFacePaintTex = reader.ReadByte() == 1;
        _ = reader.ReadByte();

        var charInfoExSize = reader.ReadInt32(); // Should always be 0x98
        Mii_CharInfoEx = reader.ReadByteArray(charInfoExSize);

        Mii_CharacterParam_Sociability = ReadIntValue(reader, "Mii.CharacterParam.Sociability");
        Mii_CharacterParam_Audaciousness = ReadIntValue(reader, "Mii.CharacterParam.Audaciousness");
        Mii_CharacterParam_Activeness = ReadIntValue(reader, "Mii.CharacterParam.Activeness");
        Mii_CharacterParam_Commonsense = ReadIntValue(reader, "Mii.CharacterParam.Commonsense");
        Mii_CharacterParam_Gaiety = ReadIntValue(reader, "Mii.CharacterParam.Gaiety");
        Mii_Voice_Formant = ReadIntValue(reader, "Mii.Voice.Formant");
        Mii_Voice_Speed = ReadIntValue(reader, "Mii.Voice.Speed");
        Mii_Voice_Intonation = ReadIntValue(reader, "Mii.Voice.Intonation");
        Mii_Voice_Pitch = ReadIntValue(reader, "Mii.Voice.Pitch");
        Mii_Voice_Tension = ReadIntValue(reader, "Mii.Voice.Tension");
        Mii_Voice_PresetType = ReadUIntValue(reader, "Mii.Voice.PresetType");
        Mii_MiiMisc_FaceInfo_Gender = ReadUIntValue(reader, "Mii.MiiMisc.FaceInfo.Gender");
        Mii_Name_PronounType = ReadUIntValue(reader, "Mii.Name.PronounType");
        Mii_MiiMisc_ClothInfo_ClothStyle = ReadUIntValue(reader, "Mii.MiiMisc.ClothInfo.ClothStyle");
        Mii_MiiMisc_BirthdayInfo_Year = ReadIntValue(reader, "Mii.MiiMisc.BirthdayInfo.Year");
        Mii_MiiMisc_BirthdayInfo_Day = ReadIntValue(reader, "Mii.MiiMisc.BirthdayInfo.Day");
        Mii_MiiMisc_BirthdayInfo_DirectAge = ReadIntValue(reader, "Mii.MiiMisc.BirthdayInfo.DirectAge");
        Mii_MiiMisc_BirthdayInfo_Month = ReadIntValue(reader, "Mii.MiiMisc.BirthdayInfo.Month");

        MiiName = reader.ReadString(16 * 2, Encoding.Unicode);
        MiiNamePronounced = reader.ReadString(32 * 2, Encoding.Unicode);

        MiiSexuality_Man = reader.ReadByte() == 1;
        MiiSexuality_Woman = reader.ReadByte() == 1;
        MiiSexuality_NonBinary = reader.ReadByte() == 1;
        reader.Skip(1);

        var canvasHeader = reader.ReadUInt32();
        if (canvasHeader != CanvasHeaderMagic)
            throw new InvalidDataException($"Invalid canvas header. Expected 0x{CanvasHeaderMagic:X}, got 0x{canvasHeader:X}.");

        if (hasFacePaintCanvas)
            FacePaint = ReadUntilMagic(reader, TexHeaderMagic);

        var texHeader = reader.ReadUInt32();
        if (texHeader != TexHeaderMagic)
            throw new InvalidDataException($"Invalid texture header. Expected 0x{TexHeaderMagic:X}, got 0x{texHeader:X}.");

        if (hasFacePaintTex)
        {
            var remaining = reader.BaseStream.Length - reader.BaseStream.Position;

            if (remaining > 0)
                TexData = reader.ReadByteArray((int)remaining);
        }
    }

    public static LtdFile Read(Stream stream)
    {
        using var reader = new BinaryReader(stream);

        return new LtdFile(reader);
    }

    public void Write(string path)
    {
        using var fs = File.Create(path);
        Write(fs);
    }

    public void Write(Stream stream)
    {
        using var writer = new BinaryWriter(stream);

        writer.Write(Version);
        writer.Write((byte) (FacePaint != null ? 1 : 0));
        writer.Write((byte) (TexData != null ? 1 : 0));
        writer.Skip(1);

        writer.Write(Mii_CharInfoEx.Length);
        writer.Write(Mii_CharInfoEx);
  
        writer.Write(Mii_CharacterParam_Sociability);
        writer.Write(Mii_CharacterParam_Audaciousness);
        writer.Write(Mii_CharacterParam_Activeness);
        writer.Write(Mii_CharacterParam_Commonsense);
        writer.Write(Mii_CharacterParam_Gaiety);
        writer.Write(Mii_Voice_Formant);
        writer.Write(Mii_Voice_Speed);
        writer.Write(Mii_Voice_Intonation);
        writer.Write(Mii_Voice_Pitch);
        writer.Write(Mii_Voice_Tension);
        writer.Write(Mii_Voice_PresetType);
        writer.Write(Mii_MiiMisc_FaceInfo_Gender);
        writer.Write(Mii_Name_PronounType);
        writer.Write(Mii_MiiMisc_ClothInfo_ClothStyle);
        writer.Write(Mii_MiiMisc_BirthdayInfo_Year);
        writer.Write(Mii_MiiMisc_BirthdayInfo_Day);
        writer.Write(Mii_MiiMisc_BirthdayInfo_DirectAge);
        writer.Write(Mii_MiiMisc_BirthdayInfo_Month);

        writer.Write(MiiName, 16 * 2, Encoding.Unicode);
        writer.Write(MiiNamePronounced, 32 * 2, Encoding.Unicode);

        writer.Write((byte)(MiiSexuality_Man ? 1 : 0));
        writer.Write((byte)(MiiSexuality_Woman ? 1 : 0));
        writer.Write((byte)(MiiSexuality_NonBinary ? 1 : 0));
        writer.Skip(1);

        writer.Write(CanvasHeaderMagic);

        if (FacePaint != null)
            writer.Write(FacePaint);

        writer.Write(TexHeaderMagic);

        if (TexData != null)
            writer.Write(TexData);
        
    }

    private int ReadIntValue(BinaryReader reader, string name)
    {
        var hash = name.ToMurmur();
        var value = reader.ReadInt32();

        IntValues.TryAdd(hash, value);

        return value;
    }

    private uint ReadUIntValue(BinaryReader reader, string name)
    {
        var hash = name.ToMurmur();
        var value = reader.ReadUInt32();

        UIntValues.TryAdd(hash, value);

        return value;
    }

    private static byte[] ReadUntilMagic(BinaryReader reader, uint magic)
    {
        using var ms = new MemoryStream();

        while (true)
        {
            if (reader.BaseStream.Position + 4 > reader.BaseStream.Length)
                throw new InvalidDataException($"Could not find magic 0x{magic:X8} while reading FacePaint.");

            long pos = reader.BaseStream.Position;
            uint value = reader.ReadUInt32();

            if (value == magic)
                return ms.ToArray();
            
            ms.WriteByte(reader.ReadByteAt(pos));
        }
    }
}