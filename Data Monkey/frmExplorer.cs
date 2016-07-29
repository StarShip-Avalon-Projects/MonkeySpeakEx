using System;
using System.Drawing;
using System.Windows.Forms;
using System.Data;
using SilverMonkey.SQLiteEditor.Controls;
using MonkeyCore;
using FastColoredTextBoxNS;
using MonkeyCore.Controls;
using System.ComponentModel;

namespace SQLiteEditor
{

    public class frmExplorer : Form
    {
        #region Members
        MainMenu mainMenu1;
        MenuItem menuItem1;
        MenuItem OpenDBmenu;
        private System.ComponentModel.IContainer components;
        private MenuItem menuItem2;
        private MenuItem AddAreaMenu;

        //Database String
        static string ActiveDatabaseLocation;

        //Menu Members
        ContextMenu TreeViewContextMenu;
        ContextMenu TreeViewTablesMenu;

        //Text Box Menu
        MenuItem objExecuteSQL;

        //Treeview Menu
        MenuItem objOpenTableSQL;
        MenuItem objRenameTableSQL;
        MenuItem objAddColumnSQL;
        MenuItem objRemoveColumnSQL;
        MenuItem objDeleteTableSQL;
        MenuItem objCreateTableSQL;

        //Listview Menu
        MenuItem objDeleteRowSQL;
        private MenuItem ExitAppMenu;
        private ToolBar toolBar1;
        private ImageList ToolBarImages;
        private ToolBarButton OpenDatabase;
        private ToolBarButton ExecuteSQL;
        private ToolBarButton Separator;
        private ToolBarButton IntegrityCheck;
        private MenuItem CheckIntegrity;
        private MenuItem menuItem3;
        private MenuItem CreateDBMenu;

        //Only access from BuildSqlResultsListView
        string TableName = "";
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel StatusStripLog;
        private SplitContainer splitContainer1;
        private TabControl tabControl1;
        private TabPage tabPage2;
        private TreeView DatabaseTreeView;
        private TabPage tabPage3;
        private SplitContainer splitContainer2;
        private TabControlEx SQLAreaTabControl;
        private TabPage tabPage1;
        private SilverMonkeyFCTB silverMonkeyFCTB1;
        public ListView_NoFlicker SqlResultsListView;
        private FastColoredTextBox sqlStatementTextBox;
        #endregion

        #region Constructor / Destructor

        public frmExplorer()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            //
            //sqlStatementTextBox 
            //
            GenerateTabPage();

            sqlStatementTextBox.ContextMenu = new ContextMenu();
            sqlStatementTextBox.ContextMenu.MenuItems.Add(objExecuteSQL);
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #endregion

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(frmExplorer));
            mainMenu1 = new MainMenu(components);
            menuItem1 = new MenuItem();
            OpenDBmenu = new MenuItem();
            CreateDBMenu = new MenuItem();
            CheckIntegrity = new MenuItem();
            menuItem3 = new MenuItem();
            ExitAppMenu = new MenuItem();
            menuItem2 = new MenuItem();
            AddAreaMenu = new MenuItem();
            objExecuteSQL = new MenuItem();
            objOpenTableSQL = new MenuItem();
            objRenameTableSQL = new MenuItem();
            objAddColumnSQL = new MenuItem();
            objRemoveColumnSQL = new MenuItem();
            objDeleteRowSQL = new MenuItem();
            objCreateTableSQL = new MenuItem();
            objDeleteTableSQL = new MenuItem();
            TreeViewContextMenu = new ContextMenu();
            TreeViewTablesMenu = new ContextMenu();
            toolBar1 = new ToolBar();
            OpenDatabase = new ToolBarButton();
            IntegrityCheck = new ToolBarButton();
            Separator = new ToolBarButton();
            ExecuteSQL = new ToolBarButton();
            ToolBarImages = new ImageList(components);
            sqlStatementTextBox = new FastColoredTextBox();
            statusStrip1 = new StatusStrip();
            StatusStripLog = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            splitContainer2 = new SplitContainer();
            SQLAreaTabControl = new TabControlEx();
            tabPage1 = new TabPage();
            silverMonkeyFCTB1 = new SilverMonkeyFCTB();
            SqlResultsListView = new ListView_NoFlicker();
            tabControl1 = new TabControl();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            DatabaseTreeView = new TreeView();
            ((ISupportInitialize)(sqlStatementTextBox)).BeginInit();
            statusStrip1.SuspendLayout();
            ((ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((ISupportInitialize)(splitContainer2)).BeginInit();
            splitContainer2.Panel1.SuspendLayout();
            splitContainer2.Panel2.SuspendLayout();
            splitContainer2.SuspendLayout();
            SQLAreaTabControl.SuspendLayout();
            tabPage1.SuspendLayout();
            ((ISupportInitialize)(silverMonkeyFCTB1)).BeginInit();
            tabControl1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // mainMenu1
            // 
            mainMenu1.MenuItems.AddRange(new MenuItem[] {
            menuItem1,
            menuItem2});
            // 
            // menuItem1
            // 
            menuItem1.Index = 0;
            menuItem1.MenuItems.AddRange(new MenuItem[] {
            OpenDBmenu,
            CreateDBMenu,
            CheckIntegrity,
            menuItem3,
            ExitAppMenu});
            menuItem1.Text = "File";
            // 
            // OpenDBmenu
            // 
            OpenDBmenu.Index = 0;
            OpenDBmenu.Text = "Open Database";
            OpenDBmenu.Click += new EventHandler(OpenDBmenu_Click);
            // 
            // CreateDBMenu
            // 
            CreateDBMenu.Index = 1;
            CreateDBMenu.Text = "Create Database";
            CreateDBMenu.Click += new EventHandler(CreateDBMenu_Click);
            // 
            // CheckIntegrity
            // 
            CheckIntegrity.Index = 2;
            CheckIntegrity.Text = "Check DB Integrity";
            CheckIntegrity.Click += new EventHandler(CheckIntegrity_Click);
            // 
            // menuItem3
            // 
            menuItem3.Index = 3;
            menuItem3.Text = "-";
            // 
            // ExitAppMenu
            // 
            ExitAppMenu.Index = 4;
            ExitAppMenu.Text = "Exit";
            ExitAppMenu.Click += new EventHandler(ExitAppMenu_Click);
            // 
            // menuItem2
            // 
            menuItem2.Index = 1;
            menuItem2.MenuItems.AddRange(new MenuItem[] {
            AddAreaMenu});
            menuItem2.Text = "SQLArea";
            // 
            // AddAreaMenu
            // 
            AddAreaMenu.Index = 0;
            AddAreaMenu.Text = "Add Area";
            AddAreaMenu.Click += new EventHandler(AddAreaMenu_Click);
            // 
            // objExecuteSQL
            // 
            objExecuteSQL.Index = -1;
            objExecuteSQL.Text = "Execute";
            objExecuteSQL.Click += new EventHandler(objExecuteSQL_Click);
            // 
            // objOpenTableSQL
            // 
            objOpenTableSQL.Index = 0;
            objOpenTableSQL.Text = "Open Table";
            objOpenTableSQL.Click += new EventHandler(objOpenTableSQL_Click);
            // 
            // objRenameTableSQL
            // 
            objRenameTableSQL.Index = 1;
            objRenameTableSQL.Text = "Rename";
            objRenameTableSQL.Click += new EventHandler(objRenameTableSQL_Click);
            // 
            // objAddColumnSQL
            // 
            objAddColumnSQL.Index = 2;
            objAddColumnSQL.Text = "Add Column";
            objAddColumnSQL.Click += new EventHandler(objAddColumnSQL_Click);
            // 
            // objRemoveColumnSQL
            // 
            objRemoveColumnSQL.Index = 3;
            objRemoveColumnSQL.Text = "Remove Column";
            objRemoveColumnSQL.Click += new EventHandler(objRemoveColumnSQL_Click);
            // 
            // objDeleteRowSQL
            // 
            objDeleteRowSQL.Index = -1;
            objDeleteRowSQL.Text = "Delete Row";
            objDeleteRowSQL.Click += new EventHandler(objDeleteRowSQL_Click);
            // 
            // objCreateTableSQL
            // 
            objCreateTableSQL.Index = 0;
            objCreateTableSQL.Text = "Create Table";
            objCreateTableSQL.Click += new EventHandler(objCreateTableSQL_Click);
            // 
            // objDeleteTableSQL
            // 
            objDeleteTableSQL.Index = 4;
            objDeleteTableSQL.Text = "Delete Table";
            objDeleteTableSQL.Click += new EventHandler(objDeleteTableSQL_Click);
            // 
            // TreeViewContextMenu
            // 
            TreeViewContextMenu.MenuItems.AddRange(new MenuItem[] {
            objOpenTableSQL,
            objRenameTableSQL,
            objAddColumnSQL,
            objRemoveColumnSQL,
            objDeleteTableSQL});
            // 
            // TreeViewTablesMenu
            // 
            TreeViewTablesMenu.MenuItems.AddRange(new MenuItem[] {
            objCreateTableSQL});
            // 
            // toolBar1
            // 
            toolBar1.Buttons.AddRange(new ToolBarButton[] {
            OpenDatabase,
            IntegrityCheck,
            Separator,
            ExecuteSQL});
            toolBar1.DropDownArrows = true;
            toolBar1.ImageList = ToolBarImages;
            toolBar1.Location = new Point(0, 0);
            toolBar1.Name = "toolBar1";
            toolBar1.ShowToolTips = true;
            toolBar1.Size = new Size(840, 28);
            toolBar1.TabIndex = 11;
            toolBar1.ButtonClick += new ToolBarButtonClickEventHandler(toolBar1_ButtonClick);
            // 
            // OpenDatabase
            // 
            OpenDatabase.ImageIndex = 0;
            OpenDatabase.Name = "OpenDatabase";
            OpenDatabase.Tag = "OpenDatabase";
            OpenDatabase.ToolTipText = "Open Database";
            // 
            // IntegrityCheck
            // 
            IntegrityCheck.ImageIndex = 3;
            IntegrityCheck.Name = "IntegrityCheck";
            IntegrityCheck.Tag = "IntegrityCheck";
            IntegrityCheck.ToolTipText = "Integrity Check";
            // 
            // Separator
            // 
            Separator.Name = "Separator";
            Separator.Style = ToolBarButtonStyle.Separator;
            // 
            // ExecuteSQL
            // 
            ExecuteSQL.ImageIndex = 1;
            ExecuteSQL.Name = "ExecuteSQL";
            ExecuteSQL.Tag = "ExecuteSQL";
            ExecuteSQL.ToolTipText = "Execute SQL";
            // 
            // ToolBarImages
            // 
            ToolBarImages.ImageStream = ((ImageListStreamer)(resources.GetObject("ToolBarImages.ImageStream")));
            ToolBarImages.TransparentColor = System.Drawing.Color.Transparent;
            ToolBarImages.Images.SetKeyName(0, "");
            ToolBarImages.Images.SetKeyName(1, "");
            ToolBarImages.Images.SetKeyName(2, "");
            ToolBarImages.Images.SetKeyName(3, "");
            // 
            // sqlStatementTextBox
            // 
            sqlStatementTextBox.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            sqlStatementTextBox.AutoScrollMinSize = new Size(2, 14);
            sqlStatementTextBox.BackBrush = null;
            sqlStatementTextBox.CharHeight = 14;
            sqlStatementTextBox.CharWidth = 8;
            sqlStatementTextBox.Cursor = Cursors.IBeam;
            sqlStatementTextBox.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            sqlStatementTextBox.Font = new System.Drawing.Font("Courier New", 9.75F);
            sqlStatementTextBox.IsReplaceMode = false;
            sqlStatementTextBox.Location = new Point(0, 0);
            sqlStatementTextBox.Name = "sqlStatementTextBox";
            sqlStatementTextBox.Paddings = new Padding(0);
            sqlStatementTextBox.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            sqlStatementTextBox.ServiceColors = ((ServiceColors)(resources.GetObject("sqlStatementTextBox.ServiceColors")));
            sqlStatementTextBox.Size = new Size(150, 150);
            sqlStatementTextBox.TabIndex = 0;
            sqlStatementTextBox.Zoom = 100;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] {
            StatusStripLog});
            statusStrip1.Location = new Point(0, 361);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(840, 22);
            statusStrip1.TabIndex = 18;
            statusStrip1.Text = "statusStrip1";
            // 
            // StatusStripLog
            // 
            StatusStripLog.Name = "StatusStripLog";
            StatusStripLog.Size = new Size(50, 17);
            StatusStripLog.Text = "Execute: Ready";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 28);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(tabControl1);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(splitContainer2);
            splitContainer1.Size = new Size(840, 333);
            splitContainer1.SplitterDistance = 280;
            splitContainer1.TabIndex = 19;
            // 
            // splitContainer2
            // 
            splitContainer2.Dock = DockStyle.Fill;
            splitContainer2.Location = new Point(0, 0);
            splitContainer2.Name = "splitContainer2";
            splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            splitContainer2.Panel1.Controls.Add(SQLAreaTabControl);
            splitContainer2.Panel1.Padding = new Padding(5);
            // 
            // splitContainer2.Panel2
            // 
            splitContainer2.Panel2.Controls.Add(SqlResultsListView);
            splitContainer2.Size = new Size(556, 333);
            splitContainer2.SplitterDistance = 158;
            splitContainer2.TabIndex = 21;
            // 
            // SQLAreaTabControl
            // 
            SQLAreaTabControl.Controls.Add(tabPage1);
            SQLAreaTabControl.Dock = DockStyle.Fill;
            SQLAreaTabControl.Location = new Point(5, 5);
            SQLAreaTabControl.Name = "SQLAreaTabControl";
            SQLAreaTabControl.SelectedIndex = 0;
            SQLAreaTabControl.Size = new Size(546, 148);
            SQLAreaTabControl.TabIndex = 0;
            SQLAreaTabControl.CloseButtonClick += SQLAreaTabControl_CloseButtonClick;
            SQLAreaTabControl.MouseDown += new MouseEventHandler(SQLAreaTabControl_MouseDown);
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(silverMonkeyFCTB1);
            tabPage1.Location = new Point(4, 22);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(538, 122);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "SQL     ";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // silverMonkeyFCTB1
            // 
            silverMonkeyFCTB1.AutoCompleteBracketsList = new char[] {
        '(',
        ')',
        '{',
        '}',
        '[',
        ']',
        '\"',
        '\"',
        '\'',
        '\''};
            silverMonkeyFCTB1.AutoIndentCharsPatterns = "";
            silverMonkeyFCTB1.AutoScrollMinSize = new Size(27, 14);
            silverMonkeyFCTB1.BackBrush = null;
            silverMonkeyFCTB1.CharHeight = 14;
            silverMonkeyFCTB1.CharWidth = 8;
            silverMonkeyFCTB1.CommentPrefix = "--";
            silverMonkeyFCTB1.Cursor = Cursors.IBeam;
            silverMonkeyFCTB1.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            silverMonkeyFCTB1.Dock = DockStyle.Fill;
            silverMonkeyFCTB1.IsReplaceMode = false;
            silverMonkeyFCTB1.Language = FastColoredTextBoxNS.Language.SQL;
            silverMonkeyFCTB1.LeftBracket = '(';
            silverMonkeyFCTB1.Location = new Point(3, 3);
            silverMonkeyFCTB1.Name = "silverMonkeyFCTB1";
            silverMonkeyFCTB1.Paddings = new Padding(0);
            silverMonkeyFCTB1.RightBracket = ')';
            silverMonkeyFCTB1.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            silverMonkeyFCTB1.ServiceColors = ((ServiceColors)(resources.GetObject("silverMonkeyFCTB1.ServiceColors")));
            silverMonkeyFCTB1.Size = new Size(532, 116);
            silverMonkeyFCTB1.TabIndex = 0;
            silverMonkeyFCTB1.Zoom = 100;
            // 
            // SqlResultsListView
            // 
            SqlResultsListView.Dock = DockStyle.Fill;
            SqlResultsListView.FullRowSelect = true;
            SqlResultsListView.GridLines = true;
            SqlResultsListView.LabelEdit = true;
            SqlResultsListView.LargeImageList = ToolBarImages;
            SqlResultsListView.Location = new Point(0, 0);
            SqlResultsListView.Name = "SqlResultsListView";
            SqlResultsListView.Size = new Size(556, 171);
            SqlResultsListView.TabIndex = 0;
            SqlResultsListView.UseCompatibleStateImageBehavior = false;
            SqlResultsListView.View = View.Details;
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(280, 333);
            tabControl1.TabIndex = 1;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(DatabaseTreeView);
            tabPage2.Location = new Point(4, 22);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(272, 307);
            tabPage2.TabIndex = 0;
            tabPage2.Text = "Database";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            tabPage3.Location = new Point(4, 22);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(272, 307);
            tabPage3.TabIndex = 1;
            tabPage3.Text = "Templates";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // DatabaseTreeView
            // 
            DatabaseTreeView.Dock = DockStyle.Fill;
            DatabaseTreeView.Location = new Point(3, 3);
            DatabaseTreeView.Name = "DatabaseTreeView";
            DatabaseTreeView.Size = new Size(266, 301);
            DatabaseTreeView.TabIndex = 1;
            DatabaseTreeView.MouseDown += new MouseEventHandler(DatabaseTreeView_MouseDown);
            DatabaseTreeView.MouseDoubleClick += DatabaseTreeView_MouseDoubleClick;
            DatabaseTreeView.AfterExpand += DatabaseTreeView_AfterExpand;
            // 
            // frmExplorer
            // 
            AutoScaleBaseSize = new Size(5, 13);
            ClientSize = new Size(840, 383);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Controls.Add(toolBar1);
            Icon = ((Icon)(resources.GetObject("$this.Icon")));
            Menu = mainMenu1;
            Name = "frmExplorer";
            Text = "TSProjects: Data Monkey";
            ((ISupportInitialize)(sqlStatementTextBox)).EndInit();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            splitContainer2.Panel1.ResumeLayout(false);
            splitContainer2.Panel2.ResumeLayout(false);
            ((ISupportInitialize)(splitContainer2)).EndInit();
            splitContainer2.ResumeLayout(false);
            SQLAreaTabControl.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((ISupportInitialize)(silverMonkeyFCTB1)).EndInit();
            tabControl1.ResumeLayout(false);
            tabPage2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }


        private void DatabaseTreeView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (DatabaseTreeView.SelectedNode != null &&
                DatabaseTreeView.SelectedNode.Parent != null &&
                DatabaseTreeView.SelectedNode.Parent.Text.ToLower().Equals("tables"))
            {
                objOpenTableSQL.PerformClick();
            }
        }

        private void SQLAreaTabControl_MouseDown(object sender, MouseEventArgs e)
        {
            Control z = (Control)sender;
            if (e.Button == MouseButtons.Right)
            {
                ContextMenuStrip x = new ContextMenuStrip();
                ToolStripItem s = x.Items.Add("New Tab", null, FNewTab_Click);
                s.Tag = sender;
                ToolStripItem t = x.Items.Add("Close All Other Tabs", null, FCloseAllTab_Click);
                ToolStripItem v = x.Items.Add("Close Tab", null, FCloseTab_Click);


                x.Show(z, e.Location);
                int tabPageIndex = 0;
                for (int i = 0; i <= SQLAreaTabControl.TabPages.Count - 1; i++)
                {
                    if (SQLAreaTabControl.GetTabRect(i).Contains(e.X, e.Y))
                    {
                        tabPageIndex = i;
                        break; 
                    }

                }
                t.Tag = tabPageIndex;
                v.Tag = tabPageIndex;
            }
            else if (e.Button == MouseButtons.Middle)
            {
                for (int i = 0; i <= SQLAreaTabControl.TabPages.Count - 1; i++)
                {
                    if (SQLAreaTabControl.GetTabRect(i).Contains(e.X, e.Y))
                    {
                        CloseTab(i);
                        break; 
                    }
                }
            }
        }

        private void FNewTab_Click(object sender, EventArgs e)
        {
            SQLAreaTabControl.TabPages.Add(GenerateTabPage());
        }

        private void CloseAllButThis(ref int i)
        {
            int j = 0;
            for (j = SQLAreaTabControl.TabPages.Count - 1; j >= 0; j += -1)
            {
                if (i != j)
                    CloseTab(j);
            }
        }

        private void FCloseAllTab_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            int i = (int)t.Tag;
            CloseAllButThis(ref i);
        }

        private void FCloseTab_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem t = (ToolStripMenuItem)sender;
            int i = (int)t.Tag;
            CloseTab(i);
        }

        private void SQLAreaTabControl_CloseButtonClick(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Button t = (Button)sender;
            CloseTab(t.TabIndex);
            SQLAreaTabControl.RePositionCloseButtons();
        }

        private void CloseTab(int i)
        {
            if (i > SQLAreaTabControl.TabCount - 1)
                return;
            SQLAreaTabControl.TabPages.RemoveAt(i);
            SQLAreaTabControl.RePositionCloseButtons();
            if (SQLAreaTabControl.TabPages.Count == 0 & Disposing == false)
            {
                SQLAreaTabControl.TabPages.Add(GenerateTabPage());
            }

        }


        #endregion


        #region Private Methods

        #region PopulateDatabaseTreeView
        public void PopulateDatabaseTreeView()
        {
            DatabaseTreeView.Nodes.Clear();

            int LastSlash = ActiveDatabaseLocation.LastIndexOf("\\");
            string DatabaseName = ActiveDatabaseLocation.Substring(LastSlash + 1, ActiveDatabaseLocation.Length - LastSlash - 1);

            TreeNode topNode = new TreeNode();
            topNode.Text = DatabaseName;
            topNode.Tag = "DatabaseName";

            TreeNode tablesNode = new TreeNode();
            tablesNode.Text = "Tables";
            tablesNode.Tag = "Tables";

            DataSet ds = null;
            string message;
            StatementParser.ReturnResults(StatementBuilder.BuildMasterQuery(), ActiveDatabaseLocation, ref ds, out message);
            StatusStripLog.Text = message;
            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    string TableName = dr[0].ToString();
                    TreeNode tableNode = new TreeNode();
                    tableNode.Text = TableName;
                    tableNode.Tag = TableName;
                    tableNode.Nodes.Add(new TreeNode("Columns"));

                    tablesNode.Nodes.Add(tableNode);
                }
            }

            tablesNode.Expand();
            topNode.Expand();

            //Add the Tables Node to the Top Node
            topNode.Nodes.Add(tablesNode);

            //Add the topNode to the TreeView
            DatabaseTreeView.Nodes.Add(topNode);
        }
        #endregion

        #region GenerateSQLArea TabPage

        private TabPage GenerateTabPage()
        {
            FastColoredTextBox tempTextBox = new SilverMonkeyFCTB();
            TabPage tempTabPage = new TabPage();
            // 
            // sqlStatementTextBox
            // 
            tempTextBox.Dock = DockStyle.Fill;
            tempTextBox.Location = new Point(0, 0);
            tempTextBox.Multiline = true;
            tempTextBox.Size = new Size(608, 200);
            tempTextBox.ContextMenu = new ContextMenu();
            tempTextBox.ContextMenu.MenuItems.Add(objExecuteSQL.CloneMenu());
            tempTextBox.Language = Language.SQL;

            tempTabPage.Controls.Add(tempTextBox);
            tempTabPage.Location = new Point(4, 22);
            tempTabPage.Size = new Size(608, 158);
            tempTabPage.Text = (SQLAreaTabControl.TabCount + 1).ToString();

            return tempTabPage;
        }
        #endregion

        #region BuildSqlResultsListView

        private void BuildSqlResultsListView(DataSet ds, string tableName)
        {
            SqlResultsListView.Items.Clear();
            SqlResultsListView.Columns.Clear();

            if (ds != null)
            {
                TableName = tableName;
                foreach (DataColumn dc in ds.Tables[0].Columns)
                {
                    SqlResultsListView.Columns.Add(dc.ColumnName, 50, HorizontalAlignment.Left);
                }

                int iCounter = 0;

                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    SqlResultsListView.Items.Add(dr[0].ToString(), 0);

                    for (int i = 1; i < dr.ItemArray.Length; i++)
                    {
                        SqlResultsListView.Items[iCounter].SubItems.Add(dr[i].ToString());
                    }

                    //-- Assign alternating backcolor
                    if (iCounter % 2 == 0)
                    {
                        SqlResultsListView.Items[iCounter].BackColor = Color.AliceBlue;
                    }

                    iCounter++;
                }

                foreach (ColumnHeader ch in SqlResultsListView.Columns)
                {
                    ch.Width = -2;
                }
                SqlResultsListView.Visible = true;
            }
        }
        #endregion

        #region IntegrityCheck
        private void IntegrityCheckSQL()
        {
            //GetTable Names
            DataSet ds = null;
            string sqlStatement = StatementBuilder.BuildIntegrityCheckSQL();

            //Place sqlstatement into the text box
            ((SilverMonkeyFCTB)SQLAreaTabControl.SelectedTab.Controls[0]).Text = sqlStatement;

            //Parse Results
            string message;
            StatementParser.ReturnResults(sqlStatement, ActiveDatabaseLocation, ref ds, out message);
            StatusStripLog.Text = message;

            //Build ListView
            BuildSqlResultsListView(ds, (DatabaseTreeView.SelectedNode == null ? "" : DatabaseTreeView.SelectedNode.Text));
        }
        #endregion

        #region ExecuteTextBoxSQL

        private void ExecuteTextBoxSQL()
        {
            //GetTable Names
            DataSet ds = null;
            string sqlStatement = ((SilverMonkeyFCTB)SQLAreaTabControl.SelectedTab.Controls[0]).Text;
            if (string.IsNullOrEmpty(sqlStatement))
                return;
            //Parse Results
            string message = null;
            StatementParser.ReturnResults(sqlStatement, ActiveDatabaseLocation, ref ds, out message);

            //Get the tablename out of the txtbox Sqlstatement
            string TableName = ParseTableName(sqlStatement);

            //Build ListView
            BuildSqlResultsListView(ds, TableName);
            StatusStripLog.Text = message;
        }

        #endregion

        #region OpenDBFileLocator
        private void OpenDBFileLocator()
        {
            using (OpenFileDialog oFile = new OpenFileDialog())
            {
                oFile.Title = "Data Monkey Database Locator";
                oFile.InitialDirectory = Paths.SilverMonkeyBotPath;
                oFile.Filter = "All files (*.*)|*.*|DB Files (*.db)|*.db";
                oFile.FilterIndex = 2;
                oFile.RestoreDirectory = true;
                if (oFile.ShowDialog() == DialogResult.OK)
                {
                    ActiveDatabaseLocation = oFile.FileName;
                    SQLiteDatabase db = new SQLiteDatabase(ActiveDatabaseLocation);
                    PopulateDatabaseTreeView();
                }
            }
        }
        #endregion

        #region CreateDBFile()

        private void CreateDBFile()
        {
            using (SaveFileDialog oFile = new SaveFileDialog())
            {
                oFile.Title = "Data Monkey Database Locator";
                oFile.InitialDirectory = Paths.SilverMonkeyBotPath;
                oFile.Filter = "All files (*.*)|*.*|DB Files (*.db)|*.db";
                oFile.FilterIndex = 2;
                oFile.RestoreDirectory = true;
                if (oFile.ShowDialog() == DialogResult.OK)
                {
                    ActiveDatabaseLocation = oFile.FileName;
                    SQLiteDatabase db = new SQLiteDatabase(ActiveDatabaseLocation);
                    PopulateDatabaseTreeView();
                }
            }
        }

        #endregion


        #endregion

        #region Menu Event
        private void OpenDBmenu_Click(object sender, EventArgs e)
        {
            OpenDBFileLocator();
        }

        private void CreateDBMenu_Click(object sender, EventArgs e)
        {
            CreateDBFile();
        }
        private void AddAreaMenu_Click(object sender, EventArgs e)
        {
            SQLAreaTabControl.Controls.Add(GenerateTabPage());
        }

        private void ExitAppMenu_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void CheckIntegrity_Click(object sender, EventArgs e)
        {
            IntegrityCheckSQL();
        }
        #endregion

        #region TreeView Events
        private void DatabaseTreeView_LostFocus(object sender, EventArgs e)
        {
            if (DatabaseTreeView.SelectedNode != null)
                DatabaseTreeView.SelectedNode.BackColor = Color.LightGray;
        }

        private void DatabaseTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (DatabaseTreeView.SelectedNode != null)
                DatabaseTreeView.SelectedNode.BackColor = Color.White;
        }

        private void DatabaseTreeView_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = new Point(e.X, e.Y);
            if (e.Button == MouseButtons.Right)
            {
                DatabaseTreeView.SelectedNode = DatabaseTreeView.GetNodeAt(p);
            }

            if (DatabaseTreeView.SelectedNode != null &&
                DatabaseTreeView.SelectedNode.Parent != null &&
                DatabaseTreeView.SelectedNode.Parent.Text.ToLower().Equals("tables"))
            {
                DatabaseTreeView.ContextMenu = TreeViewContextMenu;
            }
            else if (DatabaseTreeView.SelectedNode != null &&
                DatabaseTreeView.SelectedNode.Text.ToLower().Equals("tables"))
            {
                DatabaseTreeView.ContextMenu = TreeViewTablesMenu;
            }
            else
            {
                DatabaseTreeView.ContextMenu = null;
            }
        }

        private void DatabaseTreeView_AfterExpand(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Parent != null && e.Node.Parent.Text == "Tables")
            {
                string SQLStatement = "PRAGMA table_info(" + e.Node.Text + ")";

                //GetTable Names
                DataSet ds = null;

                string message;
                StatementParser.ReturnResults(SQLStatement, ActiveDatabaseLocation, ref ds, out message);
                StatusStripLog.Text = message;

                TreeNode columnsNode = e.Node.Nodes[0];
                columnsNode.Tag = "Columns";

                columnsNode.Nodes.Clear();

                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        string ColumnName = dr[1].ToString();
                        string ColumnType = dr[2].ToString();
                        TreeNode columnNode = new TreeNode();
                        columnNode.Text = ColumnName != null ? ColumnName + ", " + ColumnType : ColumnName;
                        columnNode.Tag = ColumnName;

                        columnsNode.Nodes.Add(columnNode);
                    }
                }

                columnsNode.Expand();
            }
        }
        private void objCreateTableSQL_Click(object sender, EventArgs e)
        {
            using (AddTable pAddTable = new AddTable())
            {
                string message;
                pAddTable.ShowInTaskbar = false;
                if (DialogResult.OK == pAddTable.ShowDialog())
                {
                    DataSet ds = null;

                    StatementParser.ReturnResults(StatementBuilder.BuildAddTableSQL(pAddTable.TableName), ActiveDatabaseLocation, ref ds, out message);

                    //Build TreeView
                    PopulateDatabaseTreeView();

                }
                else
                    message = "Create Table Aborted";
                StatusStripLog.Text = message;
            }
        }

        private void objOpenTableSQL_Click(object sender, EventArgs e)
        {
            //GetTable Names
            DataSet ds = null;
            string sqlStatement = StatementBuilder.BuildTableOpenSql(DatabaseTreeView.SelectedNode.Text);

            //Place sqlstatement into the text box
            if (!string.IsNullOrEmpty(((SilverMonkeyFCTB)SQLAreaTabControl.SelectedTab.Controls[0]).Text))
            {
                SQLAreaTabControl.TabPages.Add(GenerateTabPage());
                SQLAreaTabControl.SelectTab(SQLAreaTabControl.TabCount - 1);
            }
                ((SilverMonkeyFCTB)SQLAreaTabControl.SelectedTab.Controls[0]).Text = sqlStatement;

            //Parse Results
            string LogMessage;
            StatementParser.ReturnResults(sqlStatement, ActiveDatabaseLocation, ref ds, out LogMessage);

            //Build ListView
            BuildSqlResultsListView(ds, DatabaseTreeView.SelectedNode.Text);
            StatusStripLog.Text = LogMessage;
        }

        private void objAddColumnSQL_Click(object sender, EventArgs e)
        {
            using (AddColumn pAddColumn = new AddColumn())
            {
                pAddColumn.ShowInTaskbar = false;
                if (DialogResult.OK == pAddColumn.ShowDialog() & !string.IsNullOrEmpty(pAddColumn.ColumnName))
                {
                    string LogMessage;
                    StatementParser.ReturnResults(StatementBuilder.BuildAddColumnSQL(DatabaseTreeView.SelectedNode.Text, pAddColumn.ColumnName, pAddColumn.ColumnType), ActiveDatabaseLocation, out LogMessage);

                    //Add new column to the tree if it is expanded
                    if (DatabaseTreeView.SelectedNode.IsExpanded)
                    {
                        TreeNode columnNode = new TreeNode();
                        columnNode.Text = pAddColumn.ColumnName + ", " + pAddColumn.ColumnType;
                        columnNode.Tag = pAddColumn.ColumnName;

                        DatabaseTreeView.SelectedNode.Nodes[0].Nodes.Add(columnNode);
                    }
                    StatusStripLog.Text = LogMessage;
                }
            }
        }

        private void objRemoveColumnSQL_Click(object sender, EventArgs e)
        {
            using (RemoveColumn pRemoveColumn = new RemoveColumn())
            {
                pRemoveColumn.ShowInTaskbar = false;
                string message = null;
                if (DialogResult.OK == pRemoveColumn.ShowDialog() & !string.IsNullOrEmpty(pRemoveColumn.ColumnName))
                {
                    // StatementParser.ReturnResults(StatementBuilder.BuildAddColumnSQL(DatabaseTreeView.SelectedNode.Text, pRemoveColumn.ColumnName, pRemoveColumn.ColumnType), ActiveDatabaseLocation);
                    SQLiteDatabase db = new SQLiteDatabase(ActiveDatabaseLocation);
                    int Records = db.removeColumn(DatabaseTreeView.SelectedNode.Text, pRemoveColumn.ColumnName);

                    //Add new column to the tree if it is expanded
                    if (DatabaseTreeView.SelectedNode.IsExpanded & Records > -1)
                    {
                        TreeNode columnNode = new TreeNode();
                        columnNode.Text = pRemoveColumn.ColumnName;
                        columnNode.Tag = pRemoveColumn.ColumnName;
                        DatabaseTreeView.SelectedNode.Nodes[0].Nodes.Remove(columnNode);

                        DatabaseTreeView.SelectedNode.Collapse();
                        DatabaseTreeView.SelectedNode.Expand();

                    }
                    message = String.Format("ExecuteNonQurey: Records updated {0}", Records);
                }
                else
                    message = "Remove Column Aborted";
                StatusStripLog.Text = message;
            }
        }

        private void objRenameTableSQL_Click(object sender, EventArgs e)
        {
            using (RenameTable pRenameTable = new RenameTable())
            {
                pRenameTable.ShowInTaskbar = false;
                string message;
                if (DialogResult.OK == pRenameTable.ShowDialog())
                {

                    if (StatementParser.ReturnResults(StatementBuilder.BuildRenameTableSQL(DatabaseTreeView.SelectedNode.Text, pRenameTable.NewTableName), ActiveDatabaseLocation, out message))
                        DatabaseTreeView.SelectedNode.Text = pRenameTable.NewTableName;
                }
                else
                    message = "Rename Table aborted";
                StatusStripLog.Text = message;
            }
        }


        private void objDeleteTableSQL_Click(object sender, EventArgs e)
        {
            string TableName = DatabaseTreeView.SelectedNode.Text;
            string message;
            if (MessageBox.Show("Do you want to delete this table?", "Waring", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                StatementParser.ReturnResults(StatementBuilder.BuildTableDeleteSQL(TableName), ActiveDatabaseLocation, out message);
                DatabaseTreeView.SelectedNode.Remove();
            }
            else
                message = "Delete Table Aborted";
            StatusStripLog.Text = message;
        }
        #endregion

        #region TextBox Events

        private void objExecuteSQL_Click(object sender, EventArgs e)
        {
            ExecuteTextBoxSQL();
            PopulateDatabaseTreeView();
        }

        private string ParseTableName(string SQLStatement)
        {
            string tableName = "";
            int iofTable = 0;
            int iofEndTableName = 0;

            iofTable = SQLStatement.ToLower().IndexOf(" from ");
            if (iofTable > -1)
                iofTable += 6;
            else
            {
                iofTable = SQLStatement.ToLower().IndexOf(" table ");
                if (iofTable > -1)
                    iofTable += 7;
            }

            if (iofTable > -1)
            {
                string t = SQLStatement.Substring(iofTable, SQLStatement.Length - iofTable);
                iofEndTableName = t.IndexOf(" ");

                if (iofEndTableName > -1)
                    tableName = SQLStatement.Substring(iofTable, iofEndTableName);
                else
                    tableName = SQLStatement.Substring(iofTable, SQLStatement.Length - iofTable);
            }

            return tableName;
        }

        #endregion

        #region ListView Events
        private void SqlResultsListView_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                ListViewItem lvi = SqlResultsListView.GetItemAt(e.X, e.Y);
                if (lvi != null)
                {
                    SqlResultsListView.ContextMenu = new ContextMenu();
                    SqlResultsListView.ContextMenu.MenuItems.Add(objDeleteRowSQL);
                }
                else
                {
                    SqlResultsListView.ContextMenu = null;
                }
            }
        }

        private void objDeleteRowSQL_Click(object sender, EventArgs e)
        {
            string ColumnName = SqlResultsListView.Columns[0].Text;
            foreach (ListViewItem lvi in SqlResultsListView.SelectedItems)
            {
                string message;
                StatementParser.ReturnResults(StatementBuilder.BuildRowDeleteSQL(TableName, ColumnName, lvi.Text), ActiveDatabaseLocation, out message);
                StatusStripLog.Text = message;
                lvi.Remove();
            }
        }

        #endregion

        private void toolBar1_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
        {
            if (e.Button.Tag != null)
            {
                switch (e.Button.Tag.ToString())
                {
                    case "OpenDatabase":
                        OpenDBFileLocator();
                        break;
                    case "IntegrityCheck":
                        IntegrityCheckSQL();
                        break;
                    case "ExecuteSQL":
                        ExecuteTextBoxSQL();
                        break;
                    default:
                        break;
                }

            }
        }




    }
}
