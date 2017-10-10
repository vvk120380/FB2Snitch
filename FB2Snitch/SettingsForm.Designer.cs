namespace FB2Snitch
{
    partial class SettingsForm
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
            this.lDBPath = new System.Windows.Forms.Label();
            this.tbDBPath = new System.Windows.Forms.TextBox();
            this.btnDBDir = new System.Windows.Forms.Button();
            this.lConnectionString = new System.Windows.Forms.Label();
            this.lArcPath = new System.Windows.Forms.Label();
            this.tbConnectionString = new System.Windows.Forms.TextBox();
            this.tbArcDir = new System.Windows.Forms.TextBox();
            this.btnArcDir = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SuspendLayout();
            // 
            // lDBPath
            // 
            this.lDBPath.AutoSize = true;
            this.lDBPath.Location = new System.Drawing.Point(20, 13);
            this.lDBPath.Name = "lDBPath";
            this.lDBPath.Size = new System.Drawing.Size(108, 13);
            this.lDBPath.TabIndex = 0;
            this.lDBPath.Text = "Файл базы данных:";
            // 
            // tbDBPath
            // 
            this.tbDBPath.BackColor = System.Drawing.Color.PapayaWhip;
            this.tbDBPath.Location = new System.Drawing.Point(129, 9);
            this.tbDBPath.Name = "tbDBPath";
            this.tbDBPath.ReadOnly = true;
            this.tbDBPath.Size = new System.Drawing.Size(320, 20);
            this.tbDBPath.TabIndex = 1;
            // 
            // btnDBDir
            // 
            this.btnDBDir.Location = new System.Drawing.Point(455, 8);
            this.btnDBDir.Name = "btnDBDir";
            this.btnDBDir.Size = new System.Drawing.Size(47, 23);
            this.btnDBDir.TabIndex = 2;
            this.btnDBDir.Text = "<<<";
            this.btnDBDir.UseVisualStyleBackColor = true;
            this.btnDBDir.Click += new System.EventHandler(this.btnDBDir_Click);
            // 
            // lConnectionString
            // 
            this.lConnectionString.AutoSize = true;
            this.lConnectionString.Location = new System.Drawing.Point(12, 39);
            this.lConnectionString.Name = "lConnectionString";
            this.lConnectionString.Size = new System.Drawing.Size(116, 13);
            this.lConnectionString.TabIndex = 3;
            this.lConnectionString.Text = "Строка подключения:";
            // 
            // lArcPath
            // 
            this.lArcPath.AutoSize = true;
            this.lArcPath.Location = new System.Drawing.Point(8, 65);
            this.lArcPath.Name = "lArcPath";
            this.lArcPath.Size = new System.Drawing.Size(120, 13);
            this.lArcPath.TabIndex = 4;
            this.lArcPath.Text = "Архивная директория:";
            // 
            // tbConnectionString
            // 
            this.tbConnectionString.BackColor = System.Drawing.Color.PapayaWhip;
            this.tbConnectionString.Location = new System.Drawing.Point(129, 35);
            this.tbConnectionString.Name = "tbConnectionString";
            this.tbConnectionString.ReadOnly = true;
            this.tbConnectionString.Size = new System.Drawing.Size(373, 20);
            this.tbConnectionString.TabIndex = 5;
            // 
            // tbArcDir
            // 
            this.tbArcDir.BackColor = System.Drawing.Color.PapayaWhip;
            this.tbArcDir.Location = new System.Drawing.Point(129, 61);
            this.tbArcDir.Name = "tbArcDir";
            this.tbArcDir.ReadOnly = true;
            this.tbArcDir.Size = new System.Drawing.Size(320, 20);
            this.tbArcDir.TabIndex = 6;
            // 
            // btnArcDir
            // 
            this.btnArcDir.Location = new System.Drawing.Point(456, 60);
            this.btnArcDir.Name = "btnArcDir";
            this.btnArcDir.Size = new System.Drawing.Size(47, 23);
            this.btnArcDir.TabIndex = 7;
            this.btnArcDir.Text = "<<<";
            this.btnArcDir.UseVisualStyleBackColor = true;
            this.btnArcDir.Click += new System.EventHandler(this.btnArcDir_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(347, 101);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 8;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(428, 101);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Location = new System.Drawing.Point(2, 87);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(509, 4);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 132);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnArcDir);
            this.Controls.Add(this.tbArcDir);
            this.Controls.Add(this.tbConnectionString);
            this.Controls.Add(this.lArcPath);
            this.Controls.Add(this.lConnectionString);
            this.Controls.Add(this.btnDBDir);
            this.Controls.Add(this.tbDBPath);
            this.Controls.Add(this.lDBPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "SettingsForm";
            this.ShowIcon = false;
            this.Text = "Настройки";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lDBPath;
        private System.Windows.Forms.TextBox tbDBPath;
        private System.Windows.Forms.Button btnDBDir;
        private System.Windows.Forms.Label lConnectionString;
        private System.Windows.Forms.Label lArcPath;
        private System.Windows.Forms.TextBox tbConnectionString;
        private System.Windows.Forms.TextBox tbArcDir;
        private System.Windows.Forms.Button btnArcDir;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}