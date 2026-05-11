using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lab3
{
    [Serializable]
    public class Discipline
    {
        [Required(ErrorMessage = "Название дисциплины обязательно.")]
        [RegularExpression(@"^[а-яА-Я\s]{2,50}$", ErrorMessage = "Название должно содержать только кириллические символы и быть длиной от 2 до 50 символов.")]
        public string Name { get; set; }          
        [SemesterValidate]
        public string Semester { get; set; }            
        public List<string> Course { get; set; }       
        public List<string> Speciality { get; set; }    
        [RegularExpression(@"^\d+$")]
        [Range(1, 200, ErrorMessage = "Количество лекций должно быть в диапазоне от 1 до 200.")]
        public int NumberOfLections { get; set; }   
        [RegularExpression(@"^\d+$")]
        [Range(1, 200, ErrorMessage = "Количество лабораторных должно быть в диапазоне от 1 до 200.")]
        public int NumberOfLabs { get; set; }      
        public string Control { get; set; }
        [Required(ErrorMessage = "Лектор обязателен.")]
        public Lector lector { get; set; }            

        public Discipline()                          
        {
            Course = new List<string>();
            Speciality = new List<string>();
            lector = new Lector();
        }

        public Discipline(string name, string sem, List<string> course, List<string> spec,
            int numLect, int numLabs, string ctrl, Lector lect)
        {
            Name = name;
            Semester = sem;
            Course = course;
            Speciality = spec;
            NumberOfLections = numLect;
            NumberOfLabs = numLabs;
            Control = ctrl;
            lector = lect;
        }

        public override string ToString()  
        {
            string course = "";
            string speciality = "";
            foreach (string c in Course)
                course += c + "; ";
            foreach (string s in Speciality)
                speciality += s + "; ";

            string res = $"Название: {Name}\nКурс: {course}\nСеместр: {Semester}\n" +
                $"Специальность: {speciality}\nЧасов лекций: {NumberOfLections}\n" +
                $"Часов лабораторных: {NumberOfLabs}\nТип контроля: {Control}\n" +
                $"ФИО лектора: {lector.Name}\nКафедра: {lector.Department}\n" +
                $"Аудитория: {lector.Auditorium}\n\n==============================================\n\n";
            return res;
        }
    }




    public class SemesterValidateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is string semesterName)
            {
                if (semesterName == "1" || semesterName == "2")
                    return true;
                else
                    ErrorMessage = "Некорректный номер семестра.";
            }
            return false;
        }
    }
}
