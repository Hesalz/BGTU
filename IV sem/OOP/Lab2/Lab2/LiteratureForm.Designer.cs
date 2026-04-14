namespace Lab2
{
    partial class LiteratureForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox textBoxTitle;
        private System.Windows.Forms.TextBox textBoxAuthor;
        private System.Windows.Forms.TextBox textBoxYear;
        private System.Windows.Forms.ListBox listBoxLiterature;
        private System.Windows.Forms.Button buttonAddLiterature;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Label labelTitle;
        private System.Windows.Forms.Label labelAuthor;
        private System.Windows.Forms.Label labelYear;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.textBoxTitle = new System.Windows.Forms.TextBox();
            this.textBoxAuthor = new System.Windows.Forms.TextBox();
            this.textBoxYear = new System.Windows.Forms.TextBox();
            this.listBoxLiterature = new System.Windows.Forms.ListBox();
            this.buttonAddLiterature = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.labelTitle = new System.Windows.Forms.Label();
            this.labelAuthor = new System.Windows.Forms.Label();
            this.labelYear = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // textBoxTitle
            //
            this.textBoxTitle.Location = new System.Drawing.Point(100, 30);
            this.textBoxTitle.Name = "textBoxTitle";
            this.textBoxTitle.Size = new System.Drawing.Size(200, 22);
            this.textBoxTitle.TabIndex = 0;
            //
            // textBoxAuthor
            //
            this.textBoxAuthor.Location = new System.Drawing.Point(100, 70);
            this.textBoxAuthor.Name = "textBoxAuthor";
            this.textBoxAuthor.Size = new System.Drawing.Size(200, 22);
            this.textBoxAuthor.TabIndex = 1;
            //
            // textBoxYear
            //
            this.textBoxYear.Location = new System.Drawing.Point(100, 110);
            this.textBoxYear.Name = "textBoxYear";
            this.textBoxYear.Size = new System.Drawing.Size(200, 22);
            this.textBoxYear.TabIndex = 2;
            //
            // listBoxLiterature
            //
            this.listBoxLiterature.FormattingEnabled = true;
            this.listBoxLiterature.ItemHeight = 16;
            this.listBoxLiterature.Location = new System.Drawing.Point(350, 30);
            this.listBoxLiterature.Name = "listBoxLiterature";
            this.listBoxLiterature.Size = new System.Drawing.Size(320, 164);
            this.listBoxLiterature.TabIndex = 3;
            //
            // buttonAddLiterature
            //
            this.buttonAddLiterature.Location = new System.Drawing.Point(100, 150);
            this.buttonAddLiterature.Name = "buttonAddLiterature";
            this.buttonAddLiterature.Size = new System.Drawing.Size(100, 23);
            this.buttonAddLiterature.TabIndex = 4;
            this.buttonAddLiterature.Text = "Добавить книгу";
            this.buttonAddLiterature.UseVisualStyleBackColor = true;
            this.buttonAddLiterature.Click += new System.EventHandler(this.buttonAddLiterature_Click);
            //
            // buttonSave
            //
            this.buttonSave.Location = new System.Drawing.Point(220, 150);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(100, 23);
            this.buttonSave.TabIndex = 5;
            this.buttonSave.Text = "Сохранить";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.buttonSave_Click);
            //
            // labelTitle
            //
            this.labelTitle.AutoSize = true;
            this.labelTitle.Location = new System.Drawing.Point(20, 30);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(74, 16);
            this.labelTitle.TabIndex = 6;
            this.labelTitle.Text = "Название:";
            //
            // labelAuthor
            //
            this.labelAuthor.AutoSize = true;
            this.labelAuthor.Location = new System.Drawing.Point(20, 70);
            this.labelAuthor.Name = "labelAuthor";
            this.labelAuthor.Size = new System.Drawing.Size(53, 16);
            this.labelAuthor.TabIndex = 7;
            this.labelAuthor.Text = "Автор:";
            //
            // labelYear
            //
            this.labelYear.AutoSize = true;
            this.labelYear.Location = new System.Drawing.Point(20, 110);
            this.labelYear.Name = "labelYear";
            this.labelYear.Size = new System.Drawing.Size(34, 16);
            this.labelYear.TabIndex = 8;
            this.labelYear.Text = "Год:";
            //
            // LiteratureForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.labelYear);
            this.Controls.Add(this.labelAuthor);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.buttonAddLiterature);
            this.Controls.Add(this.listBoxLiterature);
            this.Controls.Add(this.textBoxYear);
            this.Controls.Add(this.textBoxAuthor);
            this.Controls.Add(this.textBoxTitle);
            this.Name = "LiteratureForm";
            this.Text = "Литература";
            this.Load += new System.EventHandler(this.LiteratureForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
