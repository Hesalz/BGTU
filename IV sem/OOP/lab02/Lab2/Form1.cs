using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Lab2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public List<Discipline> listOfDisciplines = new List<Discipline>();
        Discipline discipline = new Discipline();   // в этот объект пишем всю инфу из формы
        string path = @"D:\БГТУ\IV сем\ООП\Lab2\Lab2\out.xml";
        // путь по которому будем записывать в xml-файл объект

        public void ExceptionsCheck()
        {
            if (richTextBox1.Text == "")
                throw new Exception("Введите название дисциплины.");
            if (listBox1.SelectedItems.Count == 0)
                throw new Exception("Выберите специальность.");
            if (checkedListBox1.SelectedItems.Count == 0)
                throw new Exception("Выберите курс.");
            if (comboBox1.Text == "")
                throw new Exception("Выберите номер семестра.");
            if (!radioButton1.Checked && !radioButton2.Checked)
                throw new Exception("Выберите тип контроля.");
            if (richTextBox2.Text == "")
                throw new Exception("Введите кол-во лекций.");
            if (richTextBox3.Text == "")
                throw new Exception("Введите кол-во лабораторных.");
            if (maskedTextBox1.Text == "")
                throw new Exception("Введите преподавателя.");
            if (listBox2.SelectedItems.Count == 0)
                throw new Exception("Выберите кафедру.");
            if (richTextBox4.Text == "")
                throw new Exception("Введите номер аудитории.");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ExceptionsCheck();

            List<string> tempCourse = new List<string>();
            List<string> tempSpec = new List<string>();
            string tempRadio = "";
            Lector tempLect = new Lector((string)listBox2.SelectedItem, maskedTextBox1.Text, richTextBox4.Text);
            List<Literature> tempLiterature = new List<Literature>();

            foreach (string item in checkedListBox1.CheckedItems)
                tempCourse.Add(item);
            foreach (string item in listBox1.SelectedItems)
                tempSpec.Add(item);

            if (radioButton1.Checked)
                tempRadio = radioButton1.Text;
            else if (radioButton2.Checked)
                tempRadio = radioButton2.Text;

            // Добавление литературы из интерфейса
            foreach (var item in listBoxLiterature.Items)
            {
                string[] parts = item.ToString().Split(';');
                if (parts.Length == 3)
                {
                    tempLiterature.Add(new Literature(parts[0].Trim(), parts[1].Trim(), int.Parse(parts[2].Trim())));
                }
            }

            // Получение сложности курса
            string difficulty = comboBoxDifficulty.SelectedItem?.ToString(); // Получение выбранного уровня сложности

            discipline = new Discipline(richTextBox1.Text, comboBox1.Text,
                tempCourse, tempSpec, Int32.Parse(richTextBox2.Text),
                Int32.Parse(richTextBox3.Text), tempRadio, tempLect, tempLiterature, difficulty);

            listOfDisciplines.Add(discipline);

            // Расчет и вывод стоимости обучения
            double cost = CalculateCost(discipline);
            MessageBox.Show($"Стоимость обучения: {cost} у.е.", "DisciplineRedact", MessageBoxButtons.OK);
        }







        private void button2_Click(object sender, EventArgs e)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<Discipline>));

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fs, listOfDisciplines);
            }

            MessageBox.Show("Информация записана в XML!", "DisciplineRedact", MessageBoxButtons.OK);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            XmlSerializer formatter = new XmlSerializer(typeof(List<Discipline>));
            List<Discipline> disOut = new List<Discipline>();

            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                disOut = (List<Discipline>)formatter.Deserialize(fs);
            }

            // Заполнение полей формы данными из первого элемента списка
            if (disOut.Count > 0)
            {
                Discipline firstDiscipline = disOut[0];
                richTextBox1.Text = firstDiscipline.Name;
                comboBox1.Text = firstDiscipline.Semester;
                richTextBox2.Text = firstDiscipline.NumberOfLections.ToString();
                richTextBox3.Text = firstDiscipline.NumberOfLabs.ToString();

                checkedListBox1.ClearSelected();
                foreach (string course in firstDiscipline.Course)
                {
                    int index = checkedListBox1.Items.IndexOf(course);
                    if (index != -1)
                        checkedListBox1.SetItemCheckState(index, CheckState.Checked);
                }

                listBox1.ClearSelected();
                foreach (string spec in firstDiscipline.Speciality)
                {
                    int index = listBox1.Items.IndexOf(spec);
                    if (index != -1)
                        listBox1.SetSelected(index, true);
                }

                radioButton1.Checked = firstDiscipline.Control == "Зачёт";
                radioButton2.Checked = firstDiscipline.Control == "Экзамен";

                maskedTextBox1.Text = firstDiscipline.Lector.Name;
                listBox2.ClearSelected();
                int departmentIndex = listBox2.Items.IndexOf(firstDiscipline.Lector.Department);
                if (departmentIndex != -1)
                    listBox2.SetSelected(departmentIndex, true);
                richTextBox4.Text = firstDiscipline.Lector.Auditorium;

                listBoxLiterature.Items.Clear();
                foreach (var literature in firstDiscipline.LiteratureList)
                {
                    listBoxLiterature.Items.Add($"{literature.Title}; {literature.Author}; {literature.Year}");
                }

                comboBoxDifficulty.SelectedItem = firstDiscipline.Difficulty; // Заполнение поля сложности

                // Расчет и вывод стоимости обучения
                double cost = CalculateCost(firstDiscipline);
                MessageBox.Show($"Стоимость обучения: {cost} у.е.", "DisciplineRedact", MessageBoxButtons.OK);
            }

            MessageBox.Show("Информация выведена!", "DisciplineRedact", MessageBoxButtons.OK);
        }



        private double CalculateCost(Discipline discipline)
        {
            double costPerLection = 5.0; // Стоимость за один час лекции
            double costPerLab = 10.0; // Стоимость за один час лабораторной работы

            // Коэффициенты для сложности
            double difficultyMultiplier = 1.0;
            switch (discipline.Difficulty)
            {
                case "Средний":
                    difficultyMultiplier = 1.5;
                    break;
                case "Сложный":
                    difficultyMultiplier = 2.0;
                    break;
                case "Легкий":
                default:
                    difficultyMultiplier = 1.0;
                    break;
            }

            double totalCost = (discipline.NumberOfLections * costPerLection + discipline.NumberOfLabs * costPerLab) * difficultyMultiplier;
            return totalCost;
        }



        private void button4_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox2.Clear();
            richTextBox3.Clear();
            richTextBox4.Clear();
            maskedTextBox1.Clear();
            comboBoxDifficulty.SelectedIndex = -1; // Очистка выбора сложности

            // Очистка выбора в списках
            listBox1.ClearSelected();
            checkedListBox1.ClearSelected();
            comboBox1.SelectedIndex = -1; // Очистка выбора семестра
            listBox2.ClearSelected();

            // Очистка выбора типа контроля
            radioButton1.Checked = false;
            radioButton2.Checked = false;

            // Очистка списка литературы
            listBoxLiterature.Items.Clear();

            MessageBox.Show("Вся введенная информация очищена!", "DisciplineRedact", MessageBoxButtons.OK);
        }


        private void label1_Click(object sender, EventArgs e)
        {

        }

        // Название дисциплины
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.belstu.by/fakultety");
        }

        // Выбор спецухи
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Выбор курса
        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Выбор семестра
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Выбор либо зачет, либо экз
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        // Лекционные часы
        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            
        }

        // Лабораторные часы
        private void richTextBox3_TextChanged(object sender, EventArgs e)
        {

        }

        // Преподаватель
        private void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {

        }

        // Кафедры
        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        // Аудитория
        private void richTextBox4_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void buttonOpenLiteratureForm_Click(object sender, EventArgs e)
        {
            using (LiteratureForm literatureForm = new LiteratureForm())
            {
                if (literatureForm.ShowDialog() == DialogResult.OK)
                {
                    listBoxLiterature.Items.Clear();
                    foreach (var literature in literatureForm.LiteratureList)
                    {
                        listBoxLiterature.Items.Add($"{literature.Title}; {literature.Author}; {literature.Year}");
                    }
                }
            }
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void label9_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void comboBoxDifficulty_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void textBoxTitle_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
