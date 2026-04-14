namespace lab01
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.txtInput = new System.Windows.Forms.TextBox();
            this.btnReplace = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.btnGetChar = new System.Windows.Forms.Button();
            this.btnLength = new System.Windows.Forms.Button();
            this.btnVowels = new System.Windows.Forms.Button();
            this.btnConsonants = new System.Windows.Forms.Button();
            this.btnSentences = new System.Windows.Forms.Button();
            this.btnWords = new System.Windows.Forms.Button();
            this.lblResult = new System.Windows.Forms.Label();
            this.txtOldSubstring = new System.Windows.Forms.TextBox();
            this.txtNewSubstring = new System.Windows.Forms.TextBox();
            this.txtSubstring = new System.Windows.Forms.TextBox();
            this.txtIndex = new System.Windows.Forms.TextBox();
            this.lblOldSubstring = new System.Windows.Forms.Label();
            this.lblNewSubstring = new System.Windows.Forms.Label();
            this.lblSubstring = new System.Windows.Forms.Label();
            this.lblIndex = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtInput
            // 
            this.txtInput.Location = new System.Drawing.Point(76, 13);
            this.txtInput.Margin = new System.Windows.Forms.Padding(4);
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(634, 22);
            this.txtInput.TabIndex = 0;
            // 
            // btnReplace
            // 
            this.btnReplace.Location = new System.Drawing.Point(13, 53);
            this.btnReplace.Margin = new System.Windows.Forms.Padding(4);
            this.btnReplace.Name = "btnReplace";
            this.btnReplace.Size = new System.Drawing.Size(142, 48);
            this.btnReplace.TabIndex = 1;
            this.btnReplace.Text = "Заменить подстроку";
            this.btnReplace.UseVisualStyleBackColor = true;
            this.btnReplace.Click += new System.EventHandler(this.btnReplace_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(14, 333);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(4);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(150, 48);
            this.btnRemove.TabIndex = 2;
            this.btnRemove.Text = "Удалить подстроку";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnGetChar
            // 
            this.btnGetChar.Location = new System.Drawing.Point(13, 212);
            this.btnGetChar.Margin = new System.Windows.Forms.Padding(4);
            this.btnGetChar.Name = "btnGetChar";
            this.btnGetChar.Size = new System.Drawing.Size(138, 48);
            this.btnGetChar.TabIndex = 3;
            this.btnGetChar.Text = "Получить символ по индексу";
            this.btnGetChar.UseVisualStyleBackColor = true;
            this.btnGetChar.Click += new System.EventHandler(this.btnGetChar_Click);
            // 
            // btnLength
            // 
            this.btnLength.Location = new System.Drawing.Point(480, 47);
            this.btnLength.Margin = new System.Windows.Forms.Padding(4);
            this.btnLength.Name = "btnLength";
            this.btnLength.Size = new System.Drawing.Size(147, 48);
            this.btnLength.TabIndex = 4;
            this.btnLength.Text = "Вычислить длину";
            this.btnLength.UseVisualStyleBackColor = true;
            this.btnLength.Click += new System.EventHandler(this.btnLength_Click);
            // 
            // btnVowels
            // 
            this.btnVowels.Location = new System.Drawing.Point(480, 102);
            this.btnVowels.Margin = new System.Windows.Forms.Padding(4);
            this.btnVowels.Name = "btnVowels";
            this.btnVowels.Size = new System.Drawing.Size(147, 48);
            this.btnVowels.TabIndex = 5;
            this.btnVowels.Text = "Количество гласных";
            this.btnVowels.UseVisualStyleBackColor = true;
            this.btnVowels.Click += new System.EventHandler(this.btnVowels_Click);
            // 
            // btnConsonants
            // 
            this.btnConsonants.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.btnConsonants.Location = new System.Drawing.Point(480, 158);
            this.btnConsonants.Margin = new System.Windows.Forms.Padding(4);
            this.btnConsonants.Name = "btnConsonants";
            this.btnConsonants.Size = new System.Drawing.Size(147, 48);
            this.btnConsonants.TabIndex = 6;
            this.btnConsonants.Text = "Количество согласных";
            this.btnConsonants.UseVisualStyleBackColor = false;
            this.btnConsonants.Click += new System.EventHandler(this.btnConsonants_Click);
            // 
            // btnSentences
            // 
            this.btnSentences.Location = new System.Drawing.Point(480, 212);
            this.btnSentences.Margin = new System.Windows.Forms.Padding(4);
            this.btnSentences.Name = "btnSentences";
            this.btnSentences.Size = new System.Drawing.Size(147, 48);
            this.btnSentences.TabIndex = 7;
            this.btnSentences.Text = "Количество предложений";
            this.btnSentences.UseVisualStyleBackColor = true;
            this.btnSentences.Click += new System.EventHandler(this.btnSentences_Click);
            // 
            // btnWords
            // 
            this.btnWords.Location = new System.Drawing.Point(480, 268);
            this.btnWords.Margin = new System.Windows.Forms.Padding(4);
            this.btnWords.Name = "btnWords";
            this.btnWords.Size = new System.Drawing.Size(147, 48);
            this.btnWords.TabIndex = 8;
            this.btnWords.Text = "Количество слов";
            this.btnWords.UseVisualStyleBackColor = true;
            this.btnWords.Click += new System.EventHandler(this.btnWords_Click);
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Location = new System.Drawing.Point(317, 437);
            this.lblResult.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(0, 16);
            this.lblResult.TabIndex = 9;
            // 
            // txtOldSubstring
            // 
            this.txtOldSubstring.Location = new System.Drawing.Point(163, 73);
            this.txtOldSubstring.Margin = new System.Windows.Forms.Padding(4);
            this.txtOldSubstring.Name = "txtOldSubstring";
            this.txtOldSubstring.Size = new System.Drawing.Size(132, 22);
            this.txtOldSubstring.TabIndex = 10;
            // 
            // txtNewSubstring
            // 
            this.txtNewSubstring.Location = new System.Drawing.Point(163, 122);
            this.txtNewSubstring.Margin = new System.Windows.Forms.Padding(4);
            this.txtNewSubstring.Name = "txtNewSubstring";
            this.txtNewSubstring.Size = new System.Drawing.Size(132, 22);
            this.txtNewSubstring.TabIndex = 11;
            // 
            // txtSubstring
            // 
            this.txtSubstring.Location = new System.Drawing.Point(185, 353);
            this.txtSubstring.Margin = new System.Windows.Forms.Padding(4);
            this.txtSubstring.Name = "txtSubstring";
            this.txtSubstring.Size = new System.Drawing.Size(132, 22);
            this.txtSubstring.TabIndex = 12;
            // 
            // txtIndex
            // 
            this.txtIndex.Location = new System.Drawing.Point(172, 231);
            this.txtIndex.Margin = new System.Windows.Forms.Padding(4);
            this.txtIndex.Name = "txtIndex";
            this.txtIndex.Size = new System.Drawing.Size(132, 22);
            this.txtIndex.TabIndex = 13;
            // 
            // lblOldSubstring
            // 
            this.lblOldSubstring.AutoSize = true;
            this.lblOldSubstring.Location = new System.Drawing.Point(163, 53);
            this.lblOldSubstring.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblOldSubstring.Name = "lblOldSubstring";
            this.lblOldSubstring.Size = new System.Drawing.Size(129, 16);
            this.lblOldSubstring.TabIndex = 14;
            this.lblOldSubstring.Text = "Старая подстрока:";
            this.lblOldSubstring.Click += new System.EventHandler(this.lblOldSubstring_Click);
            // 
            // lblNewSubstring
            // 
            this.lblNewSubstring.AutoSize = true;
            this.lblNewSubstring.Location = new System.Drawing.Point(163, 102);
            this.lblNewSubstring.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblNewSubstring.Name = "lblNewSubstring";
            this.lblNewSubstring.Size = new System.Drawing.Size(123, 16);
            this.lblNewSubstring.TabIndex = 15;
            this.lblNewSubstring.Text = "Новая подстрока:";
            // 
            // lblSubstring
            // 
            this.lblSubstring.AutoSize = true;
            this.lblSubstring.Location = new System.Drawing.Point(185, 333);
            this.lblSubstring.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblSubstring.Name = "lblSubstring";
            this.lblSubstring.Size = new System.Drawing.Size(81, 16);
            this.lblSubstring.TabIndex = 16;
            this.lblSubstring.Text = "Подстрока:";
            // 
            // lblIndex
            // 
            this.lblIndex.AutoSize = true;
            this.lblIndex.Location = new System.Drawing.Point(172, 212);
            this.lblIndex.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblIndex.Name = "lblIndex";
            this.lblIndex.Size = new System.Drawing.Size(58, 16);
            this.lblIndex.TabIndex = 17;
            this.lblIndex.Text = "Индекс:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(230, 437);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 16);
            this.label1.TabIndex = 19;
            this.label1.Text = "Результат:";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(723, 554);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblIndex);
            this.Controls.Add(this.lblSubstring);
            this.Controls.Add(this.lblNewSubstring);
            this.Controls.Add(this.lblOldSubstring);
            this.Controls.Add(this.txtIndex);
            this.Controls.Add(this.txtSubstring);
            this.Controls.Add(this.txtNewSubstring);
            this.Controls.Add(this.txtOldSubstring);
            this.Controls.Add(this.lblResult);
            this.Controls.Add(this.btnWords);
            this.Controls.Add(this.btnSentences);
            this.Controls.Add(this.btnConsonants);
            this.Controls.Add(this.btnVowels);
            this.Controls.Add(this.btnLength);
            this.Controls.Add(this.btnGetChar);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnReplace);
            this.Controls.Add(this.txtInput);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Текстовый калькулятор";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Button btnReplace;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Button btnGetChar;
        private System.Windows.Forms.Button btnLength;
        private System.Windows.Forms.Button btnVowels;
        private System.Windows.Forms.Button btnConsonants;
        private System.Windows.Forms.Button btnSentences;
        private System.Windows.Forms.Button btnWords;
        private System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.TextBox txtOldSubstring;
        private System.Windows.Forms.TextBox txtNewSubstring;
        private System.Windows.Forms.TextBox txtSubstring;
        private System.Windows.Forms.TextBox txtIndex;
        private System.Windows.Forms.Label lblOldSubstring;
        private System.Windows.Forms.Label lblNewSubstring;
        private System.Windows.Forms.Label lblSubstring;
        private System.Windows.Forms.Label lblIndex;
        private System.Windows.Forms.Label label1;
    }
}
