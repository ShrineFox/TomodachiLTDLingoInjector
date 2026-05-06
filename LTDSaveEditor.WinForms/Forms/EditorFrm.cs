using LTDSaveEditor.Common.Utility;
using LTDSaveEditor.Core;
using LTDSaveEditor.Core.SAV;
using LTDSaveEditor.WinForms.Utility;
using OpenQA.Selenium.Chrome;
using WeifenLuo.WinFormsUI.Docking;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace LTDSaveEditor.WinForms.Forms;

public partial class EditorFrm : Form
{
    public SaveInstance SaveInstance { get; }
    public BackupManager BackupManager { get; }
    public bool sessionBackupCreated = false;

    public EditorFrm(SaveInstance instance)
    {
        InitializeComponent();

        SaveInstance = instance;
        BackupManager = new BackupManager(SaveInstance.Folder);

        saveToolStripMenuItem.Click += (_, _) =>
        {
            if (!sessionBackupCreated)
            {
                BackupManager.CreateBackup(SaveInstance.Player.Path, SaveInstance.Mii.Path, SaveInstance.Map.Path);
                sessionBackupCreated = true;
            }

            try
            {
                if (!Directory.Exists(SaveInstance.Folder))
                    throw new Exception("Save folder does not exist.");

                var playerPath = Path.Combine(SaveInstance.Folder, "Player.sav");
                SaveInstance.Player.SaveTo(playerPath);

                var miiPath = Path.Combine(SaveInstance.Folder, "Mii.sav");
                SaveInstance.Mii.SaveTo(miiPath);

                var mapPath = Path.Combine(SaveInstance.Folder, "Map.sav");
                SaveInstance.Map.SaveTo(mapPath);
            }
            catch (Exception ex)
            {
                WinFormsUtility.ErrorMessage($"Failed to save: {ex.Message}");
            }
        };

        closeToolStripMenuItem.Click += (_, _) => Close();
        optionsToolStripMenuItem.Click += (_, _) => new OptionsFrm().ShowDialog();
        discordToolStripMenuItem.Click += (_, _) => WinFormsUtility.OpenUrl("https://discord.gg/YHFNTvXrdE");
        wikiToolStripMenuItem.Click += (_, _) => WinFormsUtility.OpenUrl("https://tlmodding.github.io/");

        dockPanel.Theme = new VS2015DarkTheme();
        var playerTab = CreateTab("Player", SaveInstance.Player);
        CreateTab("Mii", SaveInstance.Mii);
        CreateTab("Map", SaveInstance.Map);

        playerTab.Activate();
    }

    private DockableControl<EditorPage> CreateTab(string tabName, SavFile savFile)
    {
        var page = new EditorPage(savFile);
        var dock = DockableControl.Create(page, tabName);

        dock.Show(dockPanel, DockState.Document);

        return dock;
    }

    private void EditorFrm_FormClosing(object sender, FormClosingEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to exit? Any unsaved changes will be lost.", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        e.Cancel = result == DialogResult.No;
    }

    public class LingoWord
    {
        public string Text { get; set; } = "";
        public string Type { get; set; } = "activities";
    }

    public void InjectFromTomolingo(object sender, EventArgs e)
    {
        List<LingoWord> lingo = new List<LingoWord>();

        string tomoLingoTxt = GetTomolingoText();
        if (tomoLingoTxt == "")
            return;

        foreach(var line in tomoLingoTxt.Split('\n').Where(x => x.StartsWith("  { w: \"")))
        {
            LingoWord word = new LingoWord();
            word.Text = line.Replace("  { w: \"", "").Split('\"')[0];
            word.Type = line.Split("t: \"")[1].Replace("\" },","");
        }
        
        /* uncategorized
        foreach (var line in tomoLingoTxt.Split('\n').Where(x => x.StartsWith("  \"")))
        {
            LingoWord word = new LingoWord();
            word.Text = line.Replace("  \"", "").Replace("\",", "").Replace("\"", "");
        }
        */

        gameDataTree.Nodes.Clear();
    }

    private string GetTomolingoText()
    {
        string url = "https://tomolingo.neocities.org/lingo-data.js";
        string text = "";

        try
        {
            var options = new ChromeOptions();

            options.AddArgument("--headless=new");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-dev-shm-usage");

            using (IWebDriver driver = new ChromeDriver(options))
            {
                driver.Navigate().GoToUrl(url);

                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
                return driver.PageSource;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}");
        }

        return text;
    }
}