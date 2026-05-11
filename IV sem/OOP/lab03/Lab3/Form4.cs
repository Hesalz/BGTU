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
    public partial class Form4 : Form
    {
        public Form4() { InitializeComponent(); }
        public void richTextBox1_TextChanged(object sender, EventArgs e) { }

        public Form1 form1;
        List<Discipline> listOfDisciplines = new List<Discipline>();
        List<Discipline> listSearchCourseToSave = new List<Discipline>();
        string path = @"D:\БГТУ\IV сем\ООП\Lab3\Lab3\searchCourse.xml";


        public Form4(Form1 f)
        {
            InitializeComponent();
            form1 = f;
            listOfDisciplines = form1.listOfDisciplines;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataTransfer.courseSearch = richTextBox1.Text;
            string searchCourseOut = "==============================================\n\n";
            string courseSearch = DataTransfer.courseSearch;

            var filteredList = listOfDisciplines
                .Where(dist => dist.Course.Contains(courseSearch))
                .ToList();

            searchCourseOut += string.Join("", filteredList.Select(dis => dis.ToString()));

            MessageBox.Show(searchCourseOut);

            listSearchCourseToSave = filteredList;
            XmlSerializer formatter = new XmlSerializer(typeof(List<Discipline>));

            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fs, listSearchCourseToSave);
            }

            Close();
        }

    }
}
