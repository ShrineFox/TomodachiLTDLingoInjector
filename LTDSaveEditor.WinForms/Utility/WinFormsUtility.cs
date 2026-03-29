using System;
using System.Collections.Generic;
using System.Text;

namespace LTDSaveEditor.WinForms.Utility;

public class WinFormsUtility
{
    public static void ErrorMessage(string message)
    {
        MessageBox.Show(message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
