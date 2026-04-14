using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Lab2
{
    public partial class LiteratureForm : Form
    {
        public List<Literature> LiteratureList { get; set; }

        public LiteratureForm()
        {
            InitializeComponent();
            LiteratureList = new List<Literature>();
        }

        private void LiteratureForm_Load(object sender, EventArgs e)
        {
            // Инициализация компонентов формы при загрузке
        }

        private void buttonAddLiterature_Click(object sender, EventArgs e)
        {
            string title = textBoxTitle.Text;
            string author = textBoxAuthor.Text;
            string yearText = textBoxYear.Text;

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(yearText))
            {
                MessageBox.Show("Пожалуйста, заполните все поля для литературы.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (int.TryParse(yearText, out int year))
            {
                LiteratureList.Add(new Literature(title, author, year));
                listBoxLiterature.Items.Add($"{title}; {author}; {year}");
                textBoxTitle.Clear();
                textBoxAuthor.Clear();
                textBoxYear.Clear();
            }
            else
            {
                MessageBox.Show("Некорректный год издания.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
