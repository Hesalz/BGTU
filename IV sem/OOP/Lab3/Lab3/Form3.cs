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

namespace Lab3
{
    public partial class Form3 : Form
    {
        public Form3() { InitializeComponent(); }
        public void richTextBox1_TextChanged(object sender, EventArgs e) { }

        public Form1 form1;
        List<Discipline> listOfDisciplines = new List<Discipline>();
        List<Discipline> listSearchSemesterToSave = new List<Discipline>();
        string path = @"D:\БГТУ\IV сем\ООП\Lab3\Lab3\searchSemester.xml";


        public Form3(Form1 f)
        {
            InitializeComponent();
            form1 = f;
            listOfDisciplines = form1.listOfDisciplines;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            DataTransfer.semesterSearch = richTextBox1.Text;
            string searchSemesterOut = "==============================================\n\n";

            string semesterSearch = DataTransfer.semesterSearch;

            var filteredAndSortedList = listOfDisciplines
                .Where(dist => dist.Semester == semesterSearch)
                .OrderBy(dist => dist.Name)
                .ToList();

            foreach (Discipline dis in filteredAndSortedList)
            {
                searchSemesterOut += dis.ToString();
            }

            MessageBox.Show(searchSemesterOut);

            listSearchSemesterToSave = filteredAndSortedList;
            XmlSerializer formatter = new XmlSerializer(typeof(List<Discipline>));

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fs, listSearchSemesterToSave);
            }

            Close();
        }
    }
}
