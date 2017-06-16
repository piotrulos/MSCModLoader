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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ADlabel = new System.Windows.Forms.Label();
            this.GFlabel = new System.Windows.Forms.Label();
            this.MDlabel = new System.Windows.Forms.Label();
            this.ADradio = new System.Windows.Forms.RadioButton();
            this.GFradio = new System.Windows.Forms.RadioButton();
            this.MDradio = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.logBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
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
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.ADlabel);
            this.groupBox2.Controls.Add(this.GFlabel);
            this.groupBox2.Controls.Add(this.MDlabel);
            this.groupBox2.Controls.Add(this.ADradio);
            this.groupBox2.Controls.Add(this.GFradio);
            this.groupBox2.Controls.Add(this.MDradio);
            this.groupBox2.Location = new System.Drawing.Point(12, 97);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(447, 139);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Mods location Folder";
            // 
            // ADlabel
            // 
            this.ADlabel.AutoSize = true;
            this.ADlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.ADlabel.ForeColor = System.Drawing.Color.Orange;
            this.ADlabel.Location = new System.Drawing.Point(26, 109);
            this.ADlabel.Name = "ADlabel";
            this.ADlabel.Size = new System.Drawing.Size(14, 13);
            this.ADlabel.TabIndex = 5;
            this.ADlabel.Text = "?";
            // 
            // GFlabel
            // 
            this.GFlabel.AutoSize = true;
            this.GFlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.GFlabel.ForeColor = System.Drawing.Color.Orange;
            this.GFlabel.Location = new System.Drawing.Point(26, 73);
            this.GFlabel.Name = "GFlabel";
            this.GFlabel.Size = new System.Drawing.Size(14, 13);
            this.GFlabel.TabIndex = 4;
            this.GFlabel.Text = "?";
            // 
            // MDlabel
            // 
            this.MDlabel.AutoSize = true;
            this.MDlabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.MDlabel.ForeColor = System.Drawing.Color.Orange;
            this.MDlabel.Location = new System.Drawing.Point(26, 37);
            this.MDlabel.Name = "MDlabel";
            this.MDlabel.Size = new System.Drawing.Size(14, 13);
            this.MDlabel.TabIndex = 3;
            this.MDlabel.Text = "?";
            // 
            // ADradio
            // 
            this.ADradio.AutoSize = true;
            this.ADradio.Location = new System.Drawing.Point(9, 92);
            this.ADradio.Name = "ADradio";
            this.ADradio.Size = new System.Drawing.Size(152, 30);
            this.ADradio.TabIndex = 2;
            this.ADradio.Text = "Appdata (MSC save folder)\r\n \r\n";
            this.ADradio.UseVisualStyleBackColor = true;
            // 
            // GFradio
            // 
            this.GFradio.AutoSize = true;
            this.GFradio.Location = new System.Drawing.Point(9, 56);
            this.GFradio.Name = "GFradio";
            this.GFradio.Size = new System.Drawing.Size(108, 30);
            this.GFradio.TabIndex = 1;
            this.GFradio.Text = "MSC Game folder\r\n \r\n";
            this.GFradio.UseVisualStyleBackColor = true;
            // 
            // MDradio
            // 
            this.MDradio.AutoSize = true;
            this.MDradio.Checked = true;
            this.MDradio.Location = new System.Drawing.Point(9, 20);
            this.MDradio.Name = "MDradio";
            this.MDradio.Size = new System.Drawing.Size(99, 30);
            this.MDradio.TabIndex = 0;
            this.MDradio.TabStop = true;
            this.MDradio.Text = "My Documents \r\n ";
            this.MDradio.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.logBox);
            this.groupBox3.Location = new System.Drawing.Point(12, 242);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(447, 137);
            this.groupBox3.TabIndex = 4;
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
            this.logBox.Size = new System.Drawing.Size(441, 118);
            this.logBox.TabIndex = 0;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 391);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "ModLoader for My Summer Car";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton ADradio;
        private System.Windows.Forms.RadioButton GFradio;
        private System.Windows.Forms.RadioButton MDradio;
        private System.Windows.Forms.Label MDlabel;
        private System.Windows.Forms.Label ADlabel;
        private System.Windows.Forms.Label GFlabel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox logBox;
        private System.Windows.Forms.Button button4;
    }
}

