using LTDSaveEditor.Common.Utility;
using LTDSaveEditor.Core;
using LTDSaveEditor.Core.Extensions;
using LTDSaveEditor.Core.SAV;
using LTDSaveEditor.WinForms.Utility;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Diagnostics;
using WeifenLuo.WinFormsUI.Docking;
using LTDSaveEditor.WinForms.Settings;

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
        public string SubmissionCredit { get; set; } = "";
    }

    public void InjectFromTomolingo_Click(object sender, EventArgs e)
    {
        var lingo = DownloadTomolingoText();
        InjectLingo(lingo);
    }

    private void InjectLingoFromFile_Click(object sender, EventArgs e)
    {
        var lingo = GetLingoFromTextFile();
        InjectLingo(lingo);
    }

    private List<LingoWord> GetLingoFromTextFile()
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Text files (*.txt)|*.txt";
        openFileDialog.Title = "Load lingo from Text file";

        openFileDialog.ShowDialog();
        if (!File.Exists(openFileDialog.FileName))
            return null;

        List<LingoWord> lingo = new List<LingoWord>();

        foreach (var line in File.ReadAllLines(openFileDialog.FileName))
        {
            LingoWord word = new LingoWord();
            if (line.Contains('|'))
            {
                word.Text = line.Split('|')[0].Trim();
                word.Type = line.Split('|')[1].Trim();
            }
            else
            {
                word.Text = line.Trim();
                word.Type = "Action";
            }
            lingo.Add(word);
        }

        return lingo;
    }

    private void InjectLingo(List<LingoWord> lingo)
    {
        if (lingo == null || lingo.Count < 1)
            return;

        ShuffleLingo(lingo);

        Array arr = GetArrayToEdit(out EditorPage page, out DataGridView dgv);
        int li = 0;
        int injected = 0;
        bool replace = replaceExistingLingoToolStripMenuItem.Checked;
        bool saveCredits = false;
        string creditsTxt = "Lingo sourced from users who submitted to https://tomolingo.neocities.org/ \r\n\r\n" +
            "TomoLingo|Submission Credits\r\n";

        /* 
         * if (dgv.Columns.Count == 1)
            dgv.Columns.Add("Credit", "Submission Credit");
        */

        // Set other arrays values
        for (int i = 0; i < arr.Length; i++)
        {
            var val = arr.GetValue(i);
            bool empty = val == null || (val is string s && string.IsNullOrWhiteSpace(s));
            if (empty || replace)
            {
                bool a = false;
                bool an = false;

                // Set lingo text
                var pick = lingo[li % lingo.Count];

                if (pick.SubmissionCredit != "")
                {
                    saveCredits = true;
                    creditsTxt += $"{pick.Text} | {pick.SubmissionCredit}\r\n";
                }

                if (setGrammarForaAndanToolStripMenuItem.Checked)
                {
                    if (pick.Text.StartsWith("a "))
                    {
                        a = true;
                        pick.Text = pick.Text.Substring(2);

                    }
                    else if (pick.Text.StartsWith("an "))
                    {
                        an = true;
                        pick.Text = pick.Text.Substring(3);
                    }
                }

                // Set TomoLingo credit
                // dgv.Rows[i].Cells[1].Value = pick.SubmissionCredit;

                arr.SetValue(pick.Text, i);

                // Set lingo genre
                var mapped = MapGenre(pick.Type);
                TrySetEnumArray(page, "UGC.Text.TextData.Genre", i, mapped);

                // Set default values for other arrays
                TrySetNumericArray(page, "UGC.Text.TextData.AddTime", i, 1778139330u);
                TrySetEnumArray(page, "UGC.Text.TextData.Attribute", i, "Neutral");
                TrySetEnumArray(page, "UGC.Text.TextData.RegionLanguageID", i, "USen");
                TrySetStringArray(page, "UGC.Text.TextData.HowToCallText", i, "");

                // Try to set grammar from lingo text
                if (setGrammarForaAndanToolStripMenuItem.Checked)
                {
                    if (a)
                        TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, "cMasculine");
                    else if (an)
                        TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, "cFeminine");
                    else
                        TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, "cNone");
                }
                else
                    TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, "cNone");

                injected++;
                li++;
            }
        }

        dgv.Refresh();

        MessageBox.Show($"Injected {injected} entries of lingo.",
            "Lingo Injected", MessageBoxButtons.OK, MessageBoxIcon.Information);

        if (saveCredits)
        {
            string creditsPath = Path.GetFullPath(".\\" + "tomolingo_submission_credits.txt");
            File.WriteAllText(creditsPath, creditsTxt);
            Process.Start("notepad.exe", creditsPath);
        }

    }

    private Array GetArrayToEdit(out EditorPage outPage, out DataGridView outDgv)
    {
        // Target path in the treeview
        const string targetPath = "UGC.Text.TextData.Text";

        // Find tree node by path
        TreeNode? FindNodeByFullPath(TreeView tree, string fullPath)
        {
            var parts = fullPath.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries);
            TreeNodeCollection currentLevel = tree.Nodes;
            TreeNode? currentNode = null;

            foreach (var part in parts)
            {
                currentNode = null;
                foreach (TreeNode node in currentLevel)
                {
                    if (!string.IsNullOrEmpty(node.Text) && node.Text.StartsWith(part, StringComparison.OrdinalIgnoreCase))
                    {
                        currentNode = node;
                        break;
                    }
                }

                if (currentNode == null)
                    return null;

                currentLevel = currentNode.Nodes;
            }

            return currentNode;
        }

        // Find first DGV child
        DataGridView? FindDataGridView(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is DataGridView dgv) return dgv;
                var nested = FindDataGridView(c);
                if (nested != null) return nested;
            }

            return null;
        }

        // Find target node and inject lingo
        foreach (var content in dockPanel.Contents)
        {
            if (content is DockableControl<EditorPage> dockControl && dockControl.Control is EditorPage page)
            {
                var tree = page.gamedataTree;
                var node = FindNodeByFullPath(tree, targetPath);
                if (node == null)
                    continue;

                // Select the node to load DGV
                tree.SelectedNode = node;
                var dgv = FindDataGridView(page);

                // Update DGV data with lingo values
                if (node.Tag is uint hash && page.SaveFile.TryGetEntry(hash, out var entry))
                {
                    // int li = 0;
                    if (entry.Value is Array arr)
                    {
                        outPage = page;
                        outDgv = dgv;
                        return arr;
                    }
                }
            }
        }

        outPage = null;
        outDgv = null;
        return null;
    }

    private void ShuffleLingo(List<LingoWord> lingo)
    {
        // Shuffle lingo
        var rng = new Random();
        for (int i = lingo.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            var tmp = lingo[i];
            lingo[i] = lingo[j];
            lingo[j] = tmp;
        }
    }

    private List<LingoWord> DownloadTomolingoText()
    {
        List<LingoWord> lingo = new List<LingoWord>();

        string url = "https://tomolingo.neocities.org/lingo-data.js";
        if (onlyUseFamilyFriendlyToolStripMenuItem.Checked)
            url = "https://tomolingo.neocities.org/lingo-safe.js";

        System.Net.WebClient wc = new System.Net.WebClient();
        byte[] raw = wc.DownloadData(url);

        string tomoLingoTxt = System.Text.Encoding.UTF8.GetString(raw);

        // Parse words from the JS file
        foreach (var line in tomoLingoTxt.Split('\n').Where(x => x.StartsWith("  { w: \"")))
        {
            LingoWord word = new LingoWord();
            word.Text = line.Replace("  { w: \"", "").Split('\"')[0];

            if (line.Split("t: \"").Length > 1)
                word.Type = line.Split("t: \"")[1].Replace("\" },", "");

            if (line.Split("c: \"").Length > 1)
                word.SubmissionCredit = line.Split("c: \"")[1].Split('\"')[0];

            lingo.Add(word);
        }

        return lingo;
    }

    private void TomoLingoLink_Click(object sender, EventArgs e)
    {
        System.Diagnostics.Process.Start(new ProcessStartInfo
        {
            FileName = "https://tomolingo.neocities.org/",
            UseShellExecute = true
        });
    }

    private void ImportLingo_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new OpenFileDialog();
        openFileDialog.Filter = "Tsv files (*.tsv)|*.tsv";
        openFileDialog.Title = "Import lingo from file";

        openFileDialog.ShowDialog();
        if (!File.Exists(openFileDialog.FileName))
            return;

        List<LingoWord> lingo = new List<LingoWord>();

        Array arr = GetArrayToEdit(out EditorPage page, out DataGridView dgv);
        var lines = File.ReadAllLines(openFileDialog.FileName);

        for (int i = 0; i < arr.Length; i++)
        {
            if (i >= lines.Length)
            {
                arr.SetValue("", i);
                TrySetStringArray(page, "UGC.Text.TextData.HowToCallText", i, "");
                TrySetEnumArray(page, "UGC.Text.TextData.Genre", i, "None");
                TrySetNumericArray(page, "UGC.Text.TextData.AddTime", i, 0);
                TrySetEnumArray(page, "UGC.Text.TextData.Attribute", i, "Neutral");
                TrySetEnumArray(page, "UGC.Text.TextData.RegionLanguageID", i, "USen");
                TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, "cNone");
            }
            else
            {
                var splitLine = lines[i].Split('\t');

                string lingoWord = splitLine[0];
                bool a = false;
                bool an = false;
                if (setGrammarForaAndanToolStripMenuItem.Checked)
                {
                    if (lingoWord.StartsWith("a "))
                    {
                        a = true;
                        lingoWord = lingoWord.Substring(2);

                    }
                    else if (lingoWord.StartsWith("an "))
                    {
                        an = true;
                        lingoWord = lingoWord.Substring(3);
                    }
                }

                arr.SetValue(lingoWord, i);
                TrySetEnumArray(page, "UGC.Text.TextData.Genre", i, splitLine[1]);
                TrySetNumericArray(page, "UGC.Text.TextData.AddTime", i, Convert.ToUInt32(splitLine[2]));
                TrySetEnumArray(page, "UGC.Text.TextData.Attribute", i, splitLine[3]);
                TrySetEnumArray(page, "UGC.Text.TextData.RegionLanguageID", i, splitLine[4]);

                // Try to set grammar from lingo text
                if (setGrammarForaAndanToolStripMenuItem.Checked)
                {
                    if (a)
                        TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, "cMasculine");
                    else if (an)
                        TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, "cFeminine");
                    else
                        TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, "cNone");
                }
                else
                    TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, splitLine[5]);

                TrySetStringArray(page, "UGC.Text.TextData.HowToCallText", i, splitLine[6]);
            }
        }

        MessageBox.Show($"Imported lingo from selected TSV file.",
            "Lingo Imported", MessageBoxButtons.OK, MessageBoxIcon.Information);

        dgv.Refresh();
    }

    private void ExportLingo_Click(object sender, EventArgs e)
    {
        SaveFileDialog saveFileDialog = new SaveFileDialog();
        saveFileDialog.Filter = "Tsv files (*.tsv)|*.tsv";
        saveFileDialog.Title = "Export Lingo to file";
        saveFileDialog.FileName = "lingo.tsv";

        saveFileDialog.ShowDialog();
        if (saveFileDialog.FileName != "")
        {
            string lingoTsv = "";

            Array arr = GetArrayToEdit(out EditorPage page, out DataGridView dgv);

            for (int i = 0; i < arr.Length; i++)
            {
                var val = arr.GetValue(i);
                bool empty = val == null || (val is string s && string.IsNullOrWhiteSpace(s));
                if (!empty)
                {
                    string lingoText = arr.GetValue(i).ToString();

                    string lingoGenre = TryGetEnumArray(page, "UGC.Text.TextData.Genre", i);
                    string lingoAddTime = TryGetNumericArray(page, "UGC.Text.TextData.AddTime", i);
                    string lingoAttr = TryGetEnumArray(page, "UGC.Text.TextData.Attribute", i);
                    string lingoRegionID = TryGetEnumArray(page, "UGC.Text.TextData.RegionLanguageID", i);
                    string lingoGrammarAttr = TryGetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i);
                    string howToCall = TryGetStringArray(page, "UGC.Text.TextData.HowToCallText", i);
                    lingoTsv += $"{lingoText}\t{lingoGenre}\t{lingoAddTime}\t{lingoAttr}\t{lingoRegionID}\t{lingoGrammarAttr}\t{howToCall}\n";
                }
            }

            File.WriteAllText(saveFileDialog.FileName, lingoTsv);
            MessageBox.Show("Saved lingo to: " + saveFileDialog.FileName, "Lingo Exported");
        }
    }

    private void ClearLingo_Click(object sender, EventArgs e)
    {
        Array arr = GetArrayToEdit(out EditorPage page, out DataGridView dgv);

        for (int i = 0; i < arr.Length; i++)
        {
            arr.SetValue("", i);
            TrySetStringArray(page, "UGC.Text.TextData.HowToCallText", i, "");
            TrySetEnumArray(page, "UGC.Text.TextData.Genre", i, "None");
            TrySetNumericArray(page, "UGC.Text.TextData.AddTime", i, 0);
            TrySetEnumArray(page, "UGC.Text.TextData.Attribute", i, "Neutral");
            TrySetEnumArray(page, "UGC.Text.TextData.RegionLanguageID", i, "USen");
            TrySetEnumArray(page, "UGC.Text.TextData.WordAttrGrammaticality", i, "cNone");
        }

        MessageBox.Show("Cleared all lingo!", "Lingo Cleared");
        dgv.Refresh();
    }

    public void TrySetEnumArray(EditorPage page, string name, int idx, string enumName)
    {
        try
        {
            uint h = enumName.ToMurmur();
            if (page.SaveFile.TryGetValue<uint[]>(name, out var uarr) && idx < uarr.Length)
                uarr[idx] = h;
        }
        catch { }
    }

    public void TrySetNumericArray(EditorPage page, string name, int idx, uint value)
    {

        if (page.SaveFile.TryGetValue<uint[]>(name, out var uarr) && idx < uarr.Length)
        {
            uarr[idx] = value;
            return;
        }

        if (page.SaveFile.TryGetValue<int[]>(name, out var iarr) && idx < iarr.Length)
        {
            iarr[idx] = (int)value;
            return;
        }

        if (page.SaveFile.TryGetValue<long[]>(name, out var larr) && idx < larr.Length)
        {
            larr[idx] = value;
            return;
        }
    }

    public void TrySetStringArray(EditorPage page, string name, int idx, string value)
    {
        try
        {
            if (page.SaveFile.TryGetValue<string[]>(name, out var sarr) && idx < sarr.Length)
            {
                sarr[idx] = value;
                return;
            }
        }
        catch { }
    }

    public string TryGetEnumArray(EditorPage page, string name, int idx)
    {
        try
        {
            if (page == null) return string.Empty;

            if (page.SaveFile.TryGetValue<uint[]>(name, out var uarr) && idx < uarr.Length)
            {
                var enumHash = uarr[idx];
                if (enumHash == 0) return string.Empty;

                return UserOptions.Instance.EnumDisplayMode switch
                {
                    EnumDisplayMode.Name =>
                        (HashManager.TryGetData(name.ToMurmur(), out var gd) && gd.Options.TryGetValue(enumHash, out var opt)) ? opt : enumHash.ToString("X"),
                    EnumDisplayMode.Hash => enumHash.ToString("X"),
                    EnumDisplayMode.Number => enumHash.ToString(),
                    _ => enumHash.ToString("X"),
                };
            }
        }
        catch { }

        return string.Empty;
    }

    public string TryGetStringArray(EditorPage page, string name, int idx)
    {
        try
        {
            if (page == null) return string.Empty;

            if (page.SaveFile.TryGetValue<string[]>(name, out var sarr) && idx < sarr.Length)
                return sarr[idx] ?? string.Empty;
        }
        catch { }

        return string.Empty;
    }

    public string TryGetNumericArray(EditorPage page, string name, int idx)
    {
        try
        {
            if (page == null) return string.Empty;

            if (page.SaveFile.TryGetValue<uint[]>(name, out var uarr) && idx < uarr.Length)
                return uarr[idx].ToString();

            if (page.SaveFile.TryGetValue<int[]>(name, out var iarr) && idx < iarr.Length)
                return iarr[idx].ToString();

            if (page.SaveFile.TryGetValue<long[]>(name, out var larr) && idx < larr.Length)
                return larr[idx].ToString();
        }
        catch { }

        return string.Empty;
    }

    public string MapGenre(string t)
    {
        if (string.IsNullOrWhiteSpace(t)) return "Phrase";
        return t.Trim().ToLowerInvariant() switch
        {
            "people" => "Person",
            "things" => "Object",
            "activities" => "Action",
            "topics" => "Topic",
            "phrases" => "Phrase",
            _ => char.ToUpper(t[0]) + t.Substring(1)
        };
    }
}