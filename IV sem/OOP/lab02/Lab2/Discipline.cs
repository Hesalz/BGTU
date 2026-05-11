using Lab2;
using System.Collections.Generic;
using System;
using System.Linq;
[Serializable]
public class Discipline
{
    public string Name { get; set; }
    public string Semester { get; set; }
    public List<string> Course { get; set; }
    public List<string> Speciality { get; set; }
    public int NumberOfLections { get; set; }
    public int NumberOfLabs { get; set; }
    public string Control { get; set; }
    public Lector Lector { get; set; }
    public List<Literature> LiteratureList { get; set; }
    public string Difficulty { get; set; } // Новое поле для сложности курса

    public Discipline()
    {
        Course = new List<string>();
        Speciality = new List<string>();
        Lector = new Lector();
        LiteratureList = new List<Literature>();
    }

    public Discipline(string name, string sem, List<string> course, List<string> spec,
        int numLect, int numLabs, string ctrl, Lector lect, List<Literature> literatureList, string difficulty)
    {
        Name = name;
        Semester = sem;
        Course = course;
        Speciality = spec;
        NumberOfLections = numLect;
        NumberOfLabs = numLabs;
        Control = ctrl;
        Lector = lect;
        LiteratureList = literatureList;
        Difficulty = difficulty; // Инициализация нового поля
    }

    public override string ToString()
    {
        string course = string.Join("; ", Course);
        string speciality = string.Join("; ", Speciality);
        string literature = string.Join("; ", LiteratureList.Select(l => $"{l.Title} by {l.Author} ({l.Year})"));

        return $"Название: {Name}\nКурс: {course}\nСеместр: {Semester}\n" +
               $"Специальность: {speciality}\nЧасов лекций: {NumberOfLections}\n" +
               $"Часов лабораторных: {NumberOfLabs}\nТип контроля: {Control}\n" +
               $"Сложность: {Difficulty}\n" + // Вывод нового поля
               $"ФИО лектора: {Lector.Name}\nКафедра: {Lector.Department}\n" +
               $"Аудитория: {Lector.Auditorium}\nЛитература: {literature}\n\n" +
               $"==============================================\n\n";
    }
}
