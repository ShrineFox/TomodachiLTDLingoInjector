using System.Collections.ObjectModel;

namespace LTDSaveEditor.Avalonia.Views;

public class TreeViewNode
{
    public string Label { get; set; } = string.Empty;
    public uint Hash { get; set; }
    public ObservableCollection<TreeViewNode> Nodes { get; set; } = [];
}
