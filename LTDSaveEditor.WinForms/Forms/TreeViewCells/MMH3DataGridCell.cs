using LTDSaveEditor.Core.Extensions;
using System.ComponentModel;

namespace LTDSaveEditor.WinForms.Forms.TreeViewCells;

public class MMH3DataGridCell(Dictionary<uint, string> hashes) : DataGridViewTextBoxCell
{
    public MMH3DataGridCell() : this([])
    {
    }

    private Dictionary<uint, string> Hashes { get; set; } = hashes;

    public override Type EditType => typeof(DataGridViewComboBoxEditingControl);
    public override Type ValueType => typeof(uint);
    public override object DefaultNewRowValue => 0u;

    public override object Clone()
    {
        var clone = (MMH3DataGridCell) base.Clone();
        clone.Hashes = Hashes;
        return clone;
    }

    public override object? ParseFormattedValue(object? formattedValue, DataGridViewCellStyle cellStyle, TypeConverter? formattedValueTypeConverter, TypeConverter? valueTypeConverter)
    {
        if (formattedValue == null) return 0u;

        if (formattedValue is string s)
            return s.ToMurmur();

        return base.ParseFormattedValue(formattedValue, cellStyle, formattedValueTypeConverter, valueTypeConverter);
    }

    public override void InitializeEditingControl(int rowIndex, object? initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    {
        base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

        if (DataGridView != null && OwningColumn != null && DataGridView.EditingControl is DataGridViewComboBoxEditingControl control)
        {
            control.DropDownStyle = ComboBoxStyle.DropDown;
            control.AutoCompleteSource = AutoCompleteSource.ListItems;
            control.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            control.MaxDropDownItems = 10;

            
            var options = Hashes.Values.ToList();
            if (control.DataSource != options)
            {
                control.DataSource = options;
                control.TextChanged -= Control_TextChanged;
                control.TextChanged += Control_TextChanged;
            }

            if (initialFormattedValue is string text) control.Text = text;
        }
    }

    private void Control_TextChanged(object? sender, EventArgs e)
    {
        // Since we are using 
        DataGridView?.NotifyCurrentCellDirty(true);
    }
}