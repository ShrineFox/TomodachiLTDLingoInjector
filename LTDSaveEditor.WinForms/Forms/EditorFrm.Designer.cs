namespace LTDSaveEditor.WinForms.Forms
{
    partial class EditorFrm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditorFrm));
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            saveToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            closeToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            discordToolStripMenuItem = new ToolStripMenuItem();
            wikiToolStripMenuItem = new ToolStripMenuItem();
            lingoToolStripMenuItem = new ToolStripMenuItem();
            injectToolStripMenuItem = new ToolStripMenuItem();
            fromTxtFileToolStripMenuItem = new ToolStripMenuItem();
            fromTomolingoToolStripMenuItem = new ToolStripMenuItem();
            replaceExistingLingoToolStripMenuItem = new ToolStripMenuItem();
            onlyUseFamilyFriendlyToolStripMenuItem = new ToolStripMenuItem();
            setGrammarForaAndanToolStripMenuItem = new ToolStripMenuItem();
            importLingoListToolStripMenuItem = new ToolStripMenuItem();
            exportLingoListToolStripMenuItem = new ToolStripMenuItem();
            clearLingoListToolStripMenuItem = new ToolStripMenuItem();
            creditToTomoLingoToolStripMenuItem = new ToolStripMenuItem();
            dockPanel = new WeifenLuo.WinFormsUI.Docking.DockPanel();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, optionsToolStripMenuItem, helpToolStripMenuItem, lingoToolStripMenuItem, creditToTomoLingoToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 3, 0, 3);
            menuStrip1.Size = new Size(914, 30);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { saveToolStripMenuItem, toolStripSeparator1, closeToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(173, 26);
            saveToolStripMenuItem.Text = "Save";
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(170, 6);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new Size(173, 26);
            closeToolStripMenuItem.Text = "Close";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(75, 24);
            optionsToolStripMenuItem.Text = "Options";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { discordToolStripMenuItem, wikiToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(55, 24);
            helpToolStripMenuItem.Text = "Help";
            // 
            // discordToolStripMenuItem
            // 
            discordToolStripMenuItem.Name = "discordToolStripMenuItem";
            discordToolStripMenuItem.Size = new Size(143, 26);
            discordToolStripMenuItem.Text = "Discord";
            // 
            // wikiToolStripMenuItem
            // 
            wikiToolStripMenuItem.Name = "wikiToolStripMenuItem";
            wikiToolStripMenuItem.Size = new Size(143, 26);
            wikiToolStripMenuItem.Text = "Wiki";
            // 
            // lingoToolStripMenuItem
            // 
            lingoToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { injectToolStripMenuItem, replaceExistingLingoToolStripMenuItem, onlyUseFamilyFriendlyToolStripMenuItem, setGrammarForaAndanToolStripMenuItem, importLingoListToolStripMenuItem, exportLingoListToolStripMenuItem, clearLingoListToolStripMenuItem });
            lingoToolStripMenuItem.Name = "lingoToolStripMenuItem";
            lingoToolStripMenuItem.Size = new Size(60, 24);
            lingoToolStripMenuItem.Text = "Lingo";
            // 
            // injectToolStripMenuItem
            // 
            injectToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { fromTxtFileToolStripMenuItem, fromTomolingoToolStripMenuItem });
            injectToolStripMenuItem.Name = "injectToolStripMenuItem";
            injectToolStripMenuItem.Size = new Size(286, 26);
            injectToolStripMenuItem.Text = "Inject...";
            // 
            // fromTxtFileToolStripMenuItem
            // 
            fromTxtFileToolStripMenuItem.Name = "fromTxtFileToolStripMenuItem";
            fromTxtFileToolStripMenuItem.Size = new Size(202, 26);
            fromTxtFileToolStripMenuItem.Text = "From Txt File";
            fromTxtFileToolStripMenuItem.Click += InjectLingoFromFile_Click;
            // 
            // fromTomolingoToolStripMenuItem
            // 
            fromTomolingoToolStripMenuItem.Name = "fromTomolingoToolStripMenuItem";
            fromTomolingoToolStripMenuItem.Size = new Size(202, 26);
            fromTomolingoToolStripMenuItem.Text = "From Tomolingo";
            fromTomolingoToolStripMenuItem.Click += InjectFromTomolingo_Click;
            // 
            // replaceExistingLingoToolStripMenuItem
            // 
            replaceExistingLingoToolStripMenuItem.CheckOnClick = true;
            replaceExistingLingoToolStripMenuItem.Name = "replaceExistingLingoToolStripMenuItem";
            replaceExistingLingoToolStripMenuItem.Size = new Size(286, 26);
            replaceExistingLingoToolStripMenuItem.Text = "Replace Existing Lingo";
            // 
            // onlyUseFamilyFriendlyToolStripMenuItem
            // 
            onlyUseFamilyFriendlyToolStripMenuItem.CheckOnClick = true;
            onlyUseFamilyFriendlyToolStripMenuItem.Name = "onlyUseFamilyFriendlyToolStripMenuItem";
            onlyUseFamilyFriendlyToolStripMenuItem.Size = new Size(286, 26);
            onlyUseFamilyFriendlyToolStripMenuItem.Text = "SFW Only TomoLingo Text";
            // 
            // setGrammarForaAndanToolStripMenuItem
            // 
            setGrammarForaAndanToolStripMenuItem.Checked = true;
            setGrammarForaAndanToolStripMenuItem.CheckOnClick = true;
            setGrammarForaAndanToolStripMenuItem.CheckState = CheckState.Checked;
            setGrammarForaAndanToolStripMenuItem.Name = "setGrammarForaAndanToolStripMenuItem";
            setGrammarForaAndanToolStripMenuItem.Size = new Size(286, 26);
            setGrammarForaAndanToolStripMenuItem.Text = "Set grammar for \"a\" and \"an\"";
            // 
            // importLingoListToolStripMenuItem
            // 
            importLingoListToolStripMenuItem.Name = "importLingoListToolStripMenuItem";
            importLingoListToolStripMenuItem.Size = new Size(286, 26);
            importLingoListToolStripMenuItem.Text = "Import Lingo List";
            importLingoListToolStripMenuItem.Click += ImportLingo_Click;
            // 
            // exportLingoListToolStripMenuItem
            // 
            exportLingoListToolStripMenuItem.Name = "exportLingoListToolStripMenuItem";
            exportLingoListToolStripMenuItem.Size = new Size(286, 26);
            exportLingoListToolStripMenuItem.Text = "Export Lingo List";
            exportLingoListToolStripMenuItem.Click += ExportLingo_Click;
            // 
            // clearLingoListToolStripMenuItem
            // 
            clearLingoListToolStripMenuItem.Name = "clearLingoListToolStripMenuItem";
            clearLingoListToolStripMenuItem.Size = new Size(286, 26);
            clearLingoListToolStripMenuItem.Text = "Clear Lingo List";
            clearLingoListToolStripMenuItem.Click += ClearLingo_Click;
            // 
            // creditToTomoLingoToolStripMenuItem
            // 
            creditToTomoLingoToolStripMenuItem.Font = new Font("Segoe UI", 9F, FontStyle.Underline);
            creditToTomoLingoToolStripMenuItem.ForeColor = SystemColors.MenuHighlight;
            creditToTomoLingoToolStripMenuItem.Name = "creditToTomoLingoToolStripMenuItem";
            creditToTomoLingoToolStripMenuItem.Size = new Size(256, 24);
            creditToTomoLingoToolStripMenuItem.Text = "Credit to TomoLingo by ryderflyder";
            creditToTomoLingoToolStripMenuItem.Click += TomoLingoLink_Click;
            // 
            // dockPanel
            // 
            dockPanel.Dock = DockStyle.Fill;
            dockPanel.Location = new Point(0, 30);
            dockPanel.Margin = new Padding(3, 4, 3, 4);
            dockPanel.Name = "dockPanel";
            dockPanel.Size = new Size(914, 570);
            dockPanel.TabIndex = 3;
            // 
            // EditorFrm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 600);
            Controls.Add(dockPanel);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "EditorFrm";
            Text = "Living the Dream: Save Editor (TomoLingo Mod v1.0)";
            FormClosing += EditorFrm_FormClosing;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem closeToolStripMenuItem;
        private WeifenLuo.WinFormsUI.Docking.DockPanel dockPanel;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem discordToolStripMenuItem;
        private ToolStripMenuItem wikiToolStripMenuItem;
        private ToolStripMenuItem lingoToolStripMenuItem;
        private ToolStripMenuItem injectToolStripMenuItem;
        private ToolStripMenuItem fromTxtFileToolStripMenuItem;
        private ToolStripMenuItem fromTomolingoToolStripMenuItem;
        private ToolStripMenuItem replaceExistingLingoToolStripMenuItem;
        private ToolStripMenuItem creditToTomoLingoToolStripMenuItem;
        private ToolStripMenuItem onlyUseFamilyFriendlyToolStripMenuItem;
        private ToolStripMenuItem importLingoListToolStripMenuItem;
        private ToolStripMenuItem exportLingoListToolStripMenuItem;
        private ToolStripMenuItem clearLingoListToolStripMenuItem;
        private ToolStripMenuItem setGrammarForaAndanToolStripMenuItem;
    }
}