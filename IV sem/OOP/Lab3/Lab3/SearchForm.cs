using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Lab3
{
    public partial class SearchForm : Form
    {
        public Form1 form1;
        List<Discipline> listOfDisciplines = new List<Discipline>();
        List<Discipline> searchResults = new List<Discipline>();

        public SearchForm(Form1 f)
        {
            InitializeComponent();
            form1 = f;
            listOfDisciplines = form1.listOfDisciplines;
        }

        public SearchForm(List<Discipline> listOfDisciplines)
        {
            this.listOfDisciplines = listOfDisciplines;
            listBoxResults.Visible = false;
        }

        public string GetSearchCriteria()
        {
            List<string> criteria = new List<string>();

            var selectedCourses = checkedListBox1.CheckedItems.Cast<string>().ToArray();
            if (selectedCourses.Length > 0)
                criteria.Add($"Курс: {string.Join(", ", selectedCourses)}");

            if (radioButton1.Checked)
                criteria.Add("Форма контроля: Зачёт");
            else if (radioButton2.Checked)
                criteria.Add("Форма контроля: Экзамен");

            return criteria.Count > 0 ? string.Join("; ", criteria) : "Критерии поиска не заданы.";
        }

        private void PerformSearch()
        {
            searchResults.Clear();

            var selectedCourses = checkedListBox1.CheckedItems.Cast<string>().ToArray();
            bool courseFilter = selectedCourses.Length > 0;
            bool controlFilter = radioButton1.Checked || radioButton2.Checked;

            foreach (var discipline in listOfDisciplines)
            {
                bool match = true;

                if (courseFilter)
                {
                    match = discipline.Course.Intersect(selectedCourses).Any();
                }

                if (controlFilter)
                {
                    string controlType = radioButton1.Checked ? "Зачёт" : "Экзамен";
                    match = match && discipline.Control == controlType;
                }

                if (match)
                {
                    searchResults.Add(discipline);
                }
            }

            DisplaySearchResults();
        }

        private void DisplaySearchResults()
        {
            listBoxResults.Visible = true;
            listBoxResults.Items.Clear();
            foreach (var discipline in searchResults)
            {
                listBoxResults.Items.Add(discipline.Name);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PerformSearch();
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {

        }
    }
}
