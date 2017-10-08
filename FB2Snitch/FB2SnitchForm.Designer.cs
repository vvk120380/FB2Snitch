namespace FB2Snitch
{
    partial class FB2SnitchForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tvMain = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // tvMain
            // 
            this.tvMain.Location = new System.Drawing.Point(12, 12);
            this.tvMain.Name = "tvMain";
            this.tvMain.Size = new System.Drawing.Size(287, 464);
            this.tvMain.TabIndex = 0;
            // 
            // FB2SnitchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(889, 534);
            this.Controls.Add(this.tvMain);
            this.Name = "FB2SnitchForm";
            this.Text = "FB2SnitchForm";
            this.Load += new System.EventHandler(this.FB2SnitchForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvMain;
    }
}

