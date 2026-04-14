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
using System.Text.RegularExpressions;

namespace Lab3
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e) { }

        // Поле ввода
        private void richTextBox1_TextChanged(object sender, EventArgs e) { }

        public Form1 form1;

        List<Discipline> listOfDisciplines = new List<Discipline>();
        List<Discipline> listSearchLectorNameToSave = new List<Discipline>();
        string path = @"D:\БГТУ\IV сем\ООП\Lab3\Lab3\searchLectorName.xml";

        public Form2(Form1 f)
        {                           // передаю первую форму в качестве параметра
            InitializeComponent();  // при инициализации второй формы и задаю
            form1 = f;              // полю form1 значение этого параметра
            listOfDisciplines = form1.listOfDisciplines;
        }

        // Кнопка поиска
        private void button1_Click(object sender, EventArgs e)
        {
            DataTransfer.lectorSearch = richTextBox1.Text;
            string searchLectorOut = "==============================================\n\n";
            string lectorSearchPattern = DataTransfer.lectorSearch;
            List<Discipline> lectorList = new List<Discipline>();

            // Используем регулярное выражение для поиска
            Regex regex = new Regex(lectorSearchPattern, RegexOptions.IgnoreCase);

            var res = from dist in listOfDisciplines
                      where regex.IsMatch(dist.lector.Name)
                      select dist;

            foreach (Discipline dis in res)
                searchLectorOut += dis.ToString();

            MessageBox.Show(searchLectorOut);

            listSearchLectorNameToSave = res.ToList();
            XmlSerializer formatter = new XmlSerializer(typeof(List<Discipline>));

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fs, listSearchLectorNameToSave);
            }

            Close();
        }
    }
}
