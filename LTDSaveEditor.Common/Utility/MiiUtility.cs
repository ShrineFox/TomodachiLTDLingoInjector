using LTDSaveEditor.Common.ShareMii;
using LTDSaveEditor.Core;
using LTDSaveEditor.Core.Extensions;
using LTDSaveEditor.Core.SAV;
using LTDSaveEditor.Core.Types;

namespace LTDSaveEditor.Common.Utility;

public static class MiiUtility
{
    /// <summary>
    /// Tries to add a Mii as a new entry, checking "Mii.CharInfoEx" for an available slot.
    /// If there is no available slot, it will return false.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="ltdFile"></param>
    /// <returns></returns>
    public static bool TryAddMii(this SaveInstance instance, LtdFile ltdFile) 
    {
        var save = instance.Mii;

        if (save.TryGetValue("Mii.CharInfoEx", out Binary[]? charInfoEx))
        {
            if (charInfoEx == null) return false;

            var nextMii = Array.FindIndex(charInfoEx, x => x.Bytes[0] == 0);
            var miiCount = charInfoEx.Count(y =>  y.Bytes[0] != 0);

            // We have space for a new mii yippiee
            if (nextMii >= 0)
            {
                // Mii.MiiMisc.EntryInfo.SortIndex[nextMii] = miiCount - 1;
                // Add other mii fields...

                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Replaces a Mii on a specific slot.
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="ltdFile"></param>
    /// <param name="miiIndex">The slot being replaced, use <c>-1</c> for the temporary slot.</param>
    public static void ReplaceMii(this SaveInstance instance, LtdFile ltdFile, int miiIndex = -1)
    {
        var save = instance.Mii;

        var charInfoEx = new Binary(ltdFile.Mii_CharInfoEx);

        if (miiIndex >= 0)
            save.SetMiiValue("Mii.CharInfoEx", miiIndex, charInfoEx);
        else
            save.SetValue("UGC.SuspendMii.CharInfoEx", charInfoEx);

        save.SetMiiValue("Mii.Name.Name", miiIndex, ltdFile.MiiName);
        save.SetMiiValue("Mii.Name.HowToCallName", miiIndex, ltdFile.MiiNamePronounced);

        // Copy all the int and uint values from the ltd file to the save file
        foreach (var (hash, value) in ltdFile.IntValues)
            save.SetMiiValue(hash, miiIndex, value);

        foreach (var (hash, value) in ltdFile.UIntValues)
            save.SetMiiValue(hash, miiIndex, value);

        if (save.TryGetValue<bool[]>("Mii.MiiMisc.FaceInfo.IsLoveGender", out var sexuality))
        {
            sexuality[(3 * miiIndex) + 0] = ltdFile.MiiSexuality_Man;
            sexuality[(3 * miiIndex) + 1] = ltdFile.MiiSexuality_Woman;
            sexuality[(3 * miiIndex) + 2] = ltdFile.MiiSexuality_NonBinary;
        }

        // TODO: Check if there is a .canvas.zs  and .ugctex.zs in the ltdFile location
        if (ltdFile.FacePaint != null /* || File.Exists(Path.Combine(ltdFile.Path, $"{ltdFile.FileName}.canvas.zs")) */)
        {
            var ugcPath = Path.Combine(save.Path, "Ugc");
            Directory.CreateDirectory(ugcPath);

            sbyte facepaintIndex = 70;

            // Temp slot (-1) uses the facepaintIndex 70.
            if (miiIndex >= 0 && save.TryGetValue<int[]>("Mii.MiiMisc.FaceInfo.FacePaintIndex", out var facepaintIndexes))
            {
                //facepaintIndex = (sbyte)Array.FindIndex(facepaintIndexes, x => x >= 0);
                facepaintIndex = (sbyte) facepaintIndexes[miiIndex];

                // Mii don't have a facepaint yet, assign one
                if (facepaintIndex == -1)
                {

                }
            }

            if (instance.Player.TryGetValue<int[]>("UGC.Facepaint.Price", out var prices))
                prices[facepaintIndex] = 500;

            if (instance.Player.TryGetValue<uint[]>("UGC.Facepaint.TextureSourceType", out var textureTypes))
                textureTypes[facepaintIndex] = "SaveDataUgcBc1".ToMurmur();

            if (instance.Player.TryGetValue<uint[]>("UGC.Facepaint.State", out var facepaints))
                facepaints[facepaintIndex] = "New".ToMurmur();

            // Unknown hash
            if (instance.Player.TryGetValue<uint[]>(0xFFC750B6, out var unks))
                unks[facepaintIndex] = 32768;

            if (instance.Player.TryGetValue<uint[]>("UGC.FacePaint.Hash", out var hashes))
            {
                var array = new byte[] { (byte) facepaintIndex, 0, 8, 0 };
                hashes[facepaintIndex] = BitConverter.ToUInt32(array, 0);
            }

        }
    }

    public static void SetMiiValue<T>(this SavFile save, uint hash, int index, T value)
    {
        if (save.TryGetValue<T[]>(hash, out var array))
            array[index] = value;
    }

    public static void SetMiiValue<T>(this SavFile save, string name, int index, T value)
    {
        if (save.TryGetValue<T[]>(name, out var array))
            array[index] = value;
    }
}