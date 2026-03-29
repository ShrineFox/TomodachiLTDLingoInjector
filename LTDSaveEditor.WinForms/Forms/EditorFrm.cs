using LTDSaveEditor.Core;
using LTDSaveEditor.WinForms.Utility;

namespace LTDSaveEditor.WinForms.Forms;

public partial class EditorFrm : Form
{
    public SaveInstance SaveInstance { get; }
    public EditorFrm(SaveInstance instance)
    {
        InitializeComponent();

        SaveInstance = instance;       
    }

    protected override async void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        if (!HashManager.IsInitialized)
        {

            var path = Path.Combine("Data", "GameDataListFull.csv");

            if (File.Exists(path))
                HashManager.Initialize(path);
            else
            {
                WinFormsUtility.ErrorMessage("Failed to load hashes. The file 'GameDataListFull.csv' was not found in the 'Data' folder.");
                return;
            }
        }

        LoadGameData();
    }

    public async void LoadGameData()
    {
        gamedataTree.BeginUpdate();
        foreach (var gameData in HashManager.Hashes)
        {
            var name = gameData.Name ?? "< Unknown >";
            var parts = name.Split('.');

            TreeNodeCollection currentLevel = gamedataTree.Nodes;
            TreeNode? currentNode = null;

            foreach (var part in parts)
            {
                // Try to find existing node at this level
                currentNode = null;

                foreach (TreeNode node in currentLevel)
                {
                    if (node.Text == part)
                    {
                        currentNode = node;
                        break;
                    }
                }

                // If not found, create it
                if (currentNode == null)
                {
                    currentNode = new TreeNode(part);
                    currentLevel.Add(currentNode);
                }

                // Move down one level
                currentLevel = currentNode.Nodes;
            }

            // Assign tag only to the final node
            currentNode?.Text += $" ({gameData.Number})";
            currentNode?.Tag = gameData.Number;
        }
        gamedataTree.EndUpdate();
    }

    private void EditorFrm_FormClosing(object sender, FormClosingEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to exit? Any unsaved changes will be lost.", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        e.Cancel = result == DialogResult.No;
    }
}
