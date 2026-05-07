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

        // Target path in the treeview
        const string targetPath = "UGC.Text.TextData.Text";

        int injected = 0;
        bool replace = replaceExistingLingoToolStripMenuItem.Checked;

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
                    int li = 0;
                    if (entry.Value is Array arr)
                    {
                        // Set other arrays values
                        void TrySetEnumArray(string name, int idx, string enumName)
                        {
                            try
                            {
                                uint h = enumName.ToMurmur();
                                if (page.SaveFile.TryGetValue<uint[]>(name, out var uarr) && idx < uarr.Length)
                                    uarr[idx] = h;
                            }
                            catch { }
                        }

                        void TrySetNumericArray(string name, int idx, uint value)
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

                        string MapGenre(string t)
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

                        for (int i = 0; i < arr.Length; i++)
                        {
                            var val = arr.GetValue(i);
                            bool empty = val == null || (val is string s && string.IsNullOrWhiteSpace(s));
                            if (empty || replace)
                            {
                                // Set lingo text
                                var pick = lingo[li % lingo.Count];
                                arr.SetValue(pick.Text, i);

                                // Set lingo genre
                                var mapped = MapGenre(pick.Type);
                                TrySetEnumArray("UGC.Text.TextData.Genre", i, mapped);

                                // Set default values for other arrays
                                TrySetNumericArray("UGC.Text.TextData.AddTime", i, 1778139330u);
                                TrySetEnumArray("UGC.Text.TextData.Attribute", i, "Neutral");
                                TrySetEnumArray("UGC.Text.TextData.RegionLanguageID", i, "USen");
                                TrySetEnumArray("UGC.Text.TextData.WordAttrGramatically", i, "cNone");

                                injected++;
                                li++;
                            }
                        }
                    }
                    else
                    {
                        var val = entry.Value;
                        bool empty = val == null || (val is string s && string.IsNullOrWhiteSpace(s));
                        if (empty || replace)
                        {
                            entry.Value = lingo[0].Text;
                            injected++;
                        }
                    }

                    dgv.Refresh();
                }
            }
        }

        MessageBox.Show($"Injected {injected} entries of lingo.",
            "Lingo Injected", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            word.Type = line.Split("t: \"")[1].Replace("\" },", "");

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
}