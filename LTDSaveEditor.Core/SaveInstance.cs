namespace LTDSaveEditor.Core;

public class SaveInstance
{
    public static SaveInstance FromFolder(string folder) => new(folder);

    public string Folder { get; }

    public SaveInstance(string folder)
    {
        if (string.IsNullOrEmpty(folder))
            throw new ArgumentException("Folder cannot be null or empty.", nameof(folder));

        if (!Directory.Exists(folder))
            throw new Exception("Folder does not exist.");

        Folder = folder;
    }
}
