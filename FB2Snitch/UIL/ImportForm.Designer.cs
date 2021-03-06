﻿namespace FB2Snitch
{
    partial class ImportForm
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
            this.tbPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectPath = new System.Windows.Forms.Button();
            this.lvFiles = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnStart = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.slStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.slProgress = new System.Windows.Forms.ToolStripStatusLabel();
            this.cbAutoDelete = new System.Windows.Forms.CheckBox();
            this.tsTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsError = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbPath
            // 
            this.tbPath.BackColor = System.Drawing.SystemColors.Info;
            this.tbPath.Location = new System.Drawing.Point(57, 22);
            this.tbPath.Name = "tbPath";
            this.tbPath.ReadOnly = true;
            this.tbPath.Size = new System.Drawing.Size(492, 20);
            this.tbPath.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Папка:";
            // 
            // btnSelectPath
            // 
            this.btnSelectPath.Location = new System.Drawing.Point(555, 20);
            this.btnSelectPath.Name = "btnSelectPath";
            this.btnSelectPath.Size = new System.Drawing.Size(75, 23);
            this.btnSelectPath.TabIndex = 2;
            this.btnSelectPath.Text = "<<<";
            this.btnSelectPath.UseVisualStyleBackColor = true;
            this.btnSelectPath.Click += new System.EventHandler(this.btnSelectPath_Click);
            // 
            // lvFiles
            // 
            this.lvFiles.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.lvFiles.Location = new System.Drawing.Point(12, 48);
            this.lvFiles.Name = "lvFiles";
            this.lvFiles.Size = new System.Drawing.Size(619, 240);
            this.lvFiles.TabIndex = 3;
            this.lvFiles.UseCompatibleStateImageBehavior = false;
            this.lvFiles.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Имя файла";
            this.columnHeader1.Width = 490;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Состояние";
            this.columnHeader2.Width = 121;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 295);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(103, 23);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(556, 295);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 6;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.slStatus,
            this.tsProgress,
            this.slProgress,
            this.tsTime,
            this.tsError});
            this.statusStrip1.Location = new System.Drawing.Point(0, 358);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(643, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.AutoSize = false;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(69, 17);
            this.toolStripStatusLabel1.Text = "Состояние:";
            this.toolStripStatusLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // slStatus
            // 
            this.slStatus.AutoSize = false;
            this.slStatus.Name = "slStatus";
            this.slStatus.Size = new System.Drawing.Size(130, 17);
            this.slStatus.Text = "Ожидает обработки";
            this.slStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsProgress
            // 
            this.tsProgress.AutoSize = false;
            this.tsProgress.Name = "tsProgress";
            this.tsProgress.Size = new System.Drawing.Size(150, 16);
            this.tsProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.tsProgress.Value = 100;
            // 
            // slProgress
            // 
            this.slProgress.Name = "slProgress";
            this.slProgress.Size = new System.Drawing.Size(124, 17);
            this.slProgress.Text = "10000 из 10000 (100%)";
            // 
            // cbAutoDelete
            // 
            this.cbAutoDelete.AutoSize = true;
            this.cbAutoDelete.Location = new System.Drawing.Point(20, 335);
            this.cbAutoDelete.Name = "cbAutoDelete";
            this.cbAutoDelete.Size = new System.Drawing.Size(458, 17);
            this.cbAutoDelete.TabIndex = 8;
            this.cbAutoDelete.Text = "Автоматически удалять успешно обработанные файлы (вновь и ранее добавленные)";
            this.cbAutoDelete.UseVisualStyleBackColor = true;
            // 
            // tsTime
            // 
            this.tsTime.AutoSize = false;
            this.tsTime.Name = "tsTime";
            this.tsTime.Size = new System.Drawing.Size(64, 17);
            this.tsTime.Text = "00:00:00.00";
            // 
            // tsError
            // 
            this.tsError.AutoSize = false;
            this.tsError.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.tsError.ForeColor = System.Drawing.Color.Red;
            this.tsError.Name = "tsError";
            this.tsError.Size = new System.Drawing.Size(80, 17);
            this.tsError.Text = "0";
            this.tsError.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ImportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(643, 380);
            this.ControlBox = false;
            this.Controls.Add(this.cbAutoDelete);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lvFiles);
            this.Controls.Add(this.btnSelectPath);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.Name = "ImportForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Импорт fb2 файлов";
            this.Load += new System.EventHandler(this.ImportForm_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectPath;
        private System.Windows.Forms.ListView lvFiles;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel slStatus;
        private System.Windows.Forms.ToolStripStatusLabel slProgress;
        private System.Windows.Forms.ToolStripProgressBar tsProgress;
        private System.Windows.Forms.CheckBox cbAutoDelete;
        private System.Windows.Forms.ToolStripStatusLabel tsTime;
        private System.Windows.Forms.ToolStripStatusLabel tsError;
    }
}