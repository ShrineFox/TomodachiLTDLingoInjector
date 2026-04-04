namespace LTDSaveEditor.Avalonia.Views;

public class DataGridRowWrapper : System.ComponentModel.INotifyPropertyChanged
{
    private object? _value;
    private EnumOption? _selectedOption;

    public int Index { get; set; }
    public string? Label { get; set; }
    public object? Tag { get; set; } // Stores the SavFileEntry
    public System.Collections.Generic.IEnumerable<EnumOption>? Options { get; set; }

    public EnumOption? SelectedOption
    {
        get => _selectedOption;
        set
        {
            if (_selectedOption != value)
            {
                _selectedOption = value;
                OnPropertyChanged(nameof(SelectedOption));
                if (value != null)
                {
                    Value = value.Value;
                }
            }
        }
    }

    public object? Value
    {
        get => _value;
        set
        {
            if (_value != value)
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
    }

    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
        => PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
}
