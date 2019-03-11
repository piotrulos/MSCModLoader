namespace MSCPatcher
{
    partial class Form1
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.mscPathLabel = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.statusLabelText = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.mscPathL = new System.Windows.Forms.Label();
            this.selectPathToMSC = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.folderPage = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ADlabel = new System.Windows.Forms.Label();
            this.GFlabel = new System.Windows.Forms.Label();
            this.MDlabel = new System.Windows.Forms.Label();
            this.ADradio = new System.Windows.Forms.RadioButton();
            this.GFradio = new System.Windows.Forms.RadioButton();
            this.MDradio = new System.Windows.Forms.RadioButton();
            this.settingPage = new System.Windows.Forms.TabPage();
            this.basicDebugInfo = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.debugStatus2 = new System.Windows.Forms.Label();
            this.disDebug = new System.Windows.Forms.Button();
            this.enDebug = new System.Windows.Forms.Button();
            this.debugStatus = new System.Windows.Forms.Label();
            this.debugCheckbox = new System.Windows.Forms.CheckBox();
            this.linkDebug = new System.Windows.Forms.LinkLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.resDialogLabel = new System.Windows.Forms.Label();
            this.resDialogCheck = new System.Windows.Forms.CheckBox();
            this.engineButton = new System.Windows.Forms.Button();
            this.OutputlogLabel = new System.Windows.Forms.Label();
            this.enOutputlog = new System.Windows.Forms.CheckBox();
            this.logPage = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.logBox = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusBarLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.folderPage.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.settingPage.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.logPage.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(89, 49);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(123, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Install MSCLoader";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.mscPathLabel);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.statusLabelText);
            this.groupBox1.Controls.Add(this.statusLabel);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.mscPathL);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(447, 78);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Info";
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(347, 49);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(90, 23);
            this.button4.TabIndex = 5;
            this.button4.Text = "Launch MSC";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // mscPathLabel
            // 
            this.mscPathLabel.AutoSize = true;
            this.mscPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.mscPathLabel.Location = new System.Drawing.Point(68, 16);
            this.mscPathLabel.Name = "mscPathLabel";
            this.mscPathLabel.Size = new System.Drawing.Size(0, 13);
            this.mscPathLabel.TabIndex = 4;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(218, 49);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(123, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "Remove MSCLoader";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // statusLabelText
            // 
            this.statusLabelText.AutoSize = true;
            this.statusLabelText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.statusLabelText.Location = new System.Drawing.Point(43, 33);
            this.statusLabelText.Name = "statusLabelText";
            this.statusLabelText.Size = new System.Drawing.Size(0, 13);
            this.statusLabelText.TabIndex = 3;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(6, 33);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(40, 13);
            this.statusLabel.TabIndex = 2;
            this.statusLabel.Text = "Status:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(9, 49);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(74, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "MSC Folder";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // mscPathL
            // 
            this.mscPathL.AutoSize = true;
            this.mscPathL.Location = new System.Drawing.Point(6, 16);
            this.mscPathL.Name = "mscPathL";
            this.mscPathL.Size = new System.Drawing.Size(65, 13);
            this.mscPathL.TabIndex = 0;
            this.mscPathL.Text = "MSC Folder:";
            // 
            // selectPathToMSC
            // 
            this.selectPathToMSC.DefaultExt = "mysummercar.exe";
            this.selectPathToMSC.FileName = "mysummercar.exe";
            this.selectPathToMSC.Filter = "mysummercar.exe|mysummercar.exe";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.folderPage);
            this.tabControl1.Controls.Add(this.settingPage);
            this.tabControl1.Controls.Add(this.logPage);
            this.tabControl1.Location = new System.Drawing.Point(12, 96);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(447, 165);
            this.tabControl1.TabIndex = 5;
            // 
            // folderPage
            // 
            this.folderPage.Controls.Add(this.groupBox2);
            this.folderPage.Location = new System.Drawing.Point(4, 22);
            this.folderPage.Name = "folderPage";
            this.folderPage.Padding = new System.Windows.Forms.Padding(3);
            this.folderPage.Size = new System.Drawing.Size(439, 139);
            this.folderPage.TabIndex = 0;
            this.folderPage.Text = "Mods folder";
            this.folderPage.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.ADlabel);
            this.groupBox2.Controls.Add(this.GFlabel);
            this.groupBox2.Controls.Add(this.MDlabel);
            this.groupBox2.Controls.Add(this.ADradio);
            this.groupBox2.Controls.Add(this.GFradio);
            this.groupBox2.Controls.Add(this.MDradio);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(433, 133);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Select mod folder";
            // 
            // ADlabel
            // 
            this.ADlabel.AutoSize = true;
            this.ADlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ADlabel.ForeColor = System.Drawing.Color.Orange;
            this.ADlabel.Location = new System.Drawing.Point(23, 108);
            this.ADlabel.Name = "ADlabel";
            this.ADlabel.Size = new System.Drawing.Size(14, 13);
            this.ADlabel.TabIndex = 29;
            this.ADlabel.Text = "?";
            // 
            // GFlabel
            // 
            this.GFlabel.AutoSize = true;
            this.GFlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GFlabel.ForeColor = System.Drawing.Color.Orange;
            this.GFlabel.Location = new System.Drawing.Point(23, 72);
            this.GFlabel.Name = "GFlabel";
            this.GFlabel.Size = new System.Drawing.Size(14, 13);
            this.GFlabel.TabIndex = 28;
            this.GFlabel.Text = "?";
            // 
            // MDlabel
            // 
            this.MDlabel.AutoSize = true;
            this.MDlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MDlabel.ForeColor = System.Drawing.Color.Orange;
            this.MDlabel.Location = new System.Drawing.Point(23, 36);
            this.MDlabel.Name = "MDlabel";
            this.MDlabel.Size = new System.Drawing.Size(14, 13);
            this.MDlabel.TabIndex = 27;
            this.MDlabel.Text = "?";
            // 
            // ADradio
            // 
            this.ADradio.AutoSize = true;
            this.ADradio.Location = new System.Drawing.Point(6, 91);
            this.ADradio.Name = "ADradio";
            this.ADradio.Size = new System.Drawing.Size(152, 30);
            this.ADradio.TabIndex = 26;
            this.ADradio.Text = "Appdata (MSC save folder)\r\n \r\n";
            this.ADradio.UseVisualStyleBackColor = true;
            // 
            // GFradio
            // 
            this.GFradio.AutoSize = true;
            this.GFradio.Location = new System.Drawing.Point(6, 55);
            this.GFradio.Name = "GFradio";
            this.GFradio.Size = new System.Drawing.Size(108, 30);
            this.GFradio.TabIndex = 25;
            this.GFradio.Text = "MSC Game folder\r\n \r\n";
            this.GFradio.UseVisualStyleBackColor = true;
            // 
            // MDradio
            // 
            this.MDradio.AutoSize = true;
            this.MDradio.Checked = true;
            this.MDradio.Location = new System.Drawing.Point(6, 19);
            this.MDradio.Name = "MDradio";
            this.MDradio.Size = new System.Drawing.Size(99, 30);
            this.MDradio.TabIndex = 24;
            this.MDradio.TabStop = true;
            this.MDradio.Text = "My Documents \r\n ";
            this.MDradio.UseVisualStyleBackColor = true;
            // 
            // settingPage
            // 
            this.settingPage.Controls.Add(this.basicDebugInfo);
            this.settingPage.Controls.Add(this.groupBox5);
            this.settingPage.Controls.Add(this.groupBox4);
            this.settingPage.Location = new System.Drawing.Point(4, 22);
            this.settingPage.Name = "settingPage";
            this.settingPage.Padding = new System.Windows.Forms.Padding(3);
            this.settingPage.Size = new System.Drawing.Size(439, 139);
            this.settingPage.TabIndex = 2;
            this.settingPage.Text = "Settings";
            this.settingPage.ToolTipText = "Enable output_log.txt useful for debugging and troubleshooting";
            this.settingPage.UseVisualStyleBackColor = true;
            // 
            // basicDebugInfo
            // 
            this.basicDebugInfo.AutoSize = true;
            this.basicDebugInfo.ForeColor = System.Drawing.Color.Red;
            this.basicDebugInfo.Location = new System.Drawing.Point(166, 63);
            this.basicDebugInfo.Name = "basicDebugInfo";
            this.basicDebugInfo.Size = new System.Drawing.Size(107, 13);
            this.basicDebugInfo.TabIndex = 32;
            this.basicDebugInfo.Text = "No Debug files found";
            this.basicDebugInfo.Visible = false;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.debugStatus2);
            this.groupBox5.Controls.Add(this.disDebug);
            this.groupBox5.Controls.Add(this.enDebug);
            this.groupBox5.Controls.Add(this.debugStatus);
            this.groupBox5.Controls.Add(this.debugCheckbox);
            this.groupBox5.Controls.Add(this.linkDebug);
            this.groupBox5.Location = new System.Drawing.Point(162, 6);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(278, 127);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Debug Stuff";
            // 
            // debugStatus2
            // 
            this.debugStatus2.AutoSize = true;
            this.debugStatus2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.debugStatus2.ForeColor = System.Drawing.Color.Orange;
            this.debugStatus2.Location = new System.Drawing.Point(6, 69);
            this.debugStatus2.Name = "debugStatus2";
            this.debugStatus2.Size = new System.Drawing.Size(14, 13);
            this.debugStatus2.TabIndex = 36;
            this.debugStatus2.Text = "?";
            // 
            // disDebug
            // 
            this.disDebug.Enabled = false;
            this.disDebug.Location = new System.Drawing.Point(136, 98);
            this.disDebug.Name = "disDebug";
            this.disDebug.Size = new System.Drawing.Size(135, 23);
            this.disDebug.TabIndex = 35;
            this.disDebug.Text = "Disable Debugging";
            this.disDebug.UseVisualStyleBackColor = true;
            this.disDebug.Click += new System.EventHandler(this.disDebug_Click);
            // 
            // enDebug
            // 
            this.enDebug.Enabled = false;
            this.enDebug.Location = new System.Drawing.Point(9, 98);
            this.enDebug.Name = "enDebug";
            this.enDebug.Size = new System.Drawing.Size(121, 23);
            this.enDebug.TabIndex = 34;
            this.enDebug.Text = "Enable Debugging";
            this.enDebug.UseVisualStyleBackColor = true;
            this.enDebug.Click += new System.EventHandler(this.enDebug_Click);
            // 
            // debugStatus
            // 
            this.debugStatus.AutoSize = true;
            this.debugStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.debugStatus.ForeColor = System.Drawing.Color.Orange;
            this.debugStatus.Location = new System.Drawing.Point(6, 55);
            this.debugStatus.Name = "debugStatus";
            this.debugStatus.Size = new System.Drawing.Size(14, 13);
            this.debugStatus.TabIndex = 33;
            this.debugStatus.Text = "?";
            // 
            // debugCheckbox
            // 
            this.debugCheckbox.AutoSize = true;
            this.debugCheckbox.Location = new System.Drawing.Point(9, 35);
            this.debugCheckbox.Name = "debugCheckbox";
            this.debugCheckbox.Size = new System.Drawing.Size(140, 17);
            this.debugCheckbox.TabIndex = 32;
            this.debugCheckbox.Text = "I understand what is this";
            this.debugCheckbox.UseVisualStyleBackColor = true;
            // 
            // linkDebug
            // 
            this.linkDebug.AutoSize = true;
            this.linkDebug.Location = new System.Drawing.Point(6, 16);
            this.linkDebug.Name = "linkDebug";
            this.linkDebug.Size = new System.Drawing.Size(224, 13);
            this.linkDebug.TabIndex = 31;
            this.linkDebug.TabStop = true;
            this.linkDebug.Text = "Click here to read more info about Debugging!";
            this.linkDebug.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDebug_LinkClicked);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox4.Controls.Add(this.resDialogLabel);
            this.groupBox4.Controls.Add(this.resDialogCheck);
            this.groupBox4.Controls.Add(this.engineButton);
            this.groupBox4.Controls.Add(this.OutputlogLabel);
            this.groupBox4.Controls.Add(this.enOutputlog);
            this.groupBox4.Location = new System.Drawing.Point(5, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(153, 127);
            this.groupBox4.TabIndex = 29;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Settings";
            // 
            // resDialogLabel
            // 
            this.resDialogLabel.AutoSize = true;
            this.resDialogLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.resDialogLabel.ForeColor = System.Drawing.Color.Orange;
            this.resDialogLabel.Location = new System.Drawing.Point(22, 69);
            this.resDialogLabel.Name = "resDialogLabel";
            this.resDialogLabel.Size = new System.Drawing.Size(14, 13);
            this.resDialogLabel.TabIndex = 33;
            this.resDialogLabel.Text = "?";
            // 
            // resDialogCheck
            // 
            this.resDialogCheck.AutoSize = true;
            this.resDialogCheck.Location = new System.Drawing.Point(6, 52);
            this.resDialogCheck.Name = "resDialogCheck";
            this.resDialogCheck.Size = new System.Drawing.Size(140, 30);
            this.resDialogCheck.TabIndex = 32;
            this.resDialogCheck.Text = "Disable resolution dialog\r\n ";
            this.toolTip1.SetToolTip(this.resDialogCheck, "If checked it disables resolution dialog.\r\nGame will run directly without showing" +
        " \"resolution dialog\"");
            this.resDialogCheck.UseVisualStyleBackColor = true;
            // 
            // engineButton
            // 
            this.engineButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.engineButton.Location = new System.Drawing.Point(6, 98);
            this.engineButton.Name = "engineButton";
            this.engineButton.Size = new System.Drawing.Size(141, 23);
            this.engineButton.TabIndex = 31;
            this.engineButton.Text = "Apply these settings";
            this.engineButton.UseVisualStyleBackColor = true;
            this.engineButton.Click += new System.EventHandler(this.engineButton_Click);
            // 
            // OutputlogLabel
            // 
            this.OutputlogLabel.AutoSize = true;
            this.OutputlogLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.OutputlogLabel.ForeColor = System.Drawing.Color.Orange;
            this.OutputlogLabel.Location = new System.Drawing.Point(22, 36);
            this.OutputlogLabel.Name = "OutputlogLabel";
            this.OutputlogLabel.Size = new System.Drawing.Size(14, 13);
            this.OutputlogLabel.TabIndex = 30;
            this.OutputlogLabel.Text = "?";
            // 
            // enOutputlog
            // 
            this.enOutputlog.AutoSize = true;
            this.enOutputlog.Checked = true;
            this.enOutputlog.CheckState = System.Windows.Forms.CheckState.Checked;
            this.enOutputlog.Location = new System.Drawing.Point(6, 19);
            this.enOutputlog.Name = "enOutputlog";
            this.enOutputlog.Size = new System.Drawing.Size(126, 30);
            this.enOutputlog.TabIndex = 29;
            this.enOutputlog.Text = "Enable output_log.txt\r\n ";
            this.toolTip1.SetToolTip(this.enOutputlog, "Enables unity output_log.txt\r\nThis file is helpfull with debugging and troublesho" +
        "oting\r\n\r\nRecomended to turn this on.");
            this.enOutputlog.UseVisualStyleBackColor = true;
            // 
            // logPage
            // 
            this.logPage.Controls.Add(this.groupBox3);
            this.logPage.Location = new System.Drawing.Point(4, 22);
            this.logPage.Name = "logPage";
            this.logPage.Padding = new System.Windows.Forms.Padding(3);
            this.logPage.Size = new System.Drawing.Size(439, 139);
            this.logPage.TabIndex = 1;
            this.logPage.Text = "Log";
            this.logPage.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.logBox);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(433, 133);
            this.groupBox3.TabIndex = 5;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Log";
            // 
            // logBox
            // 
            this.logBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logBox.Location = new System.Drawing.Point(3, 16);
            this.logBox.Multiline = true;
            this.logBox.Name = "logBox";
            this.logBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logBox.Size = new System.Drawing.Size(427, 114);
            this.logBox.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusBarLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 264);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(471, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 6;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusBarLabel
            // 
            this.statusBarLabel.Name = "statusBarLabel";
            this.statusBarLabel.Size = new System.Drawing.Size(38, 17);
            this.statusBarLabel.Text = "status";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 286);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "ModLoader for My Summer Car";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.folderPage.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.settingPage.ResumeLayout(false);
            this.settingPage.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.logPage.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label mscPathL;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.OpenFileDialog selectPathToMSC;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label statusLabelText;
        private System.Windows.Forms.Label mscPathLabel;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage folderPage;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label ADlabel;
        private System.Windows.Forms.Label GFlabel;
        private System.Windows.Forms.Label MDlabel;
        private System.Windows.Forms.RadioButton ADradio;
        private System.Windows.Forms.RadioButton GFradio;
        private System.Windows.Forms.RadioButton MDradio;
        private System.Windows.Forms.TabPage logPage;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.TabPage settingPage;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusBarLabel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label OutputlogLabel;
        private System.Windows.Forms.CheckBox enOutputlog;
        private System.Windows.Forms.Button engineButton;
        private System.Windows.Forms.LinkLabel linkDebug;
        private System.Windows.Forms.Label debugStatus;
        private System.Windows.Forms.CheckBox debugCheckbox;
        private System.Windows.Forms.Button enDebug;
        private System.Windows.Forms.Button disDebug;
        private System.Windows.Forms.CheckBox resDialogCheck;
        private System.Windows.Forms.Label resDialogLabel;
        private System.Windows.Forms.Label debugStatus2;
        private System.Windows.Forms.Label basicDebugInfo;
    }
}

