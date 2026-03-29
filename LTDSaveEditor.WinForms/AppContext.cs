using LTDSaveEditor.WinForms;

class AppContext : ApplicationContext
{
    public AppContext()
    {
        MainForm = new MainFrm();
        MainForm.Show();
    }

    public void ChangeMainForm(Form newForm)
    {
        MainForm = newForm;
    }
}