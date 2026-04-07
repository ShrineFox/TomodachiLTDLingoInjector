using CommunityToolkit.Mvvm.ComponentModel;
using LTDSaveEditor.Core.SAV;
using System;
using System.Diagnostics.CodeAnalysis;

namespace LTDSaveEditor.Avalonia.ViewModels;

public partial class MiiOptionViewModel(SavFile savFile, int index) : ObservableObject
{
    private readonly SavFile _savFile = savFile;

    public int Index { get; } = index;

    public string Name
    {
        get
        {
            if (TryGetValueFromMiiArray<string>("Mii.Name.Name", out var name))
                return name;

            return "Unknown Mii";
        }
        set
        {
            // TODO: Check limits
            if (TryGetValue<string[]>("Mii.Name.Name", out var names) && names[Index] != value)
            {
                names[Index] = value;
                OnPropertyChanged(nameof(Name));
            }
        }
    }

    public override string ToString() => Name;

    private bool TryGetValue<T>(string key, [MaybeNullWhen(false)] out T value) => _savFile.TryGetValue(key, out value);
    private bool TryGetValueFromMiiArray<T>(string key, [MaybeNullWhen(false)] out T value)
    {
        value = default;

        if (!_savFile.TryGetValue(key, out T[]? array) || array is null || array.Length == 0)
            return false;

        if (Index < 0 || Index >= array.Length)
            return false;

        value = array[Index];
        return true;
    }
}