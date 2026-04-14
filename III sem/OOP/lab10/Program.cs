using System;
public partial class Student
{
    private readonly int _id;
    private string _surname;
    private string _name;
    private string _patronymic;
    private DateTime _birthDate;
    private string _address;
    private string _phone;
    private string _faculty;
    private int _course;
    private int _group;

    public int ID => _id;
    public string Surname { get => _surname; set => _surname = value; }
    public string Name { get => _name; set => _name = value; }
    public string Patronymic { get => _patronymic; set => _patronymic = value; }
    public DateTime BirthDate { get => _birthDate; set => _birthDate = value; }
    public string Address { get => _address; set => _address = value; }
    public string Phone { get => _phone; set => _phone = value; }
    public string Faculty { get => _faculty; set => _faculty = value; }
    public int Course
    {
        get => _course;
        set
        {
            if (value < 1 || value > 5)
                throw new ArgumentException("Курс должен быть от 1 до 5.");
            _course = value;
        }
    }
    public int Group
    {
        get => _group;
        set
        {
            if (value < 1 || value > 10)
                throw new ArgumentException("Группа должна быть от 1 до 10.");
            _group = value;
        }
    }

    public Student(string surname, string name, string patronymic, DateTime birthDate, string address, string phone, string faculty, int course, int group)
    {
        _id = new Random().Next(1000, 9999);
        _surname = surname;
        _name = name;
        _patronymic = patronymic;
        _birthDate = birthDate;
        _address = address;
        _phone = phone;
        _faculty = faculty;
        Course = course;
        Group = group;
    }

    public int CalculateAge()
    {
        DateTime now = DateTime.Now;
        int age = now.Year - BirthDate.Year;
        if (now < BirthDate.AddYears(age))
            age--;
        return age;
    }

    public override string ToString()
    {
        return $"{Surname} {Name}, Факультет: {Faculty}, Курс: {Course}, Группа: {Group}";
    }
}

class Program
{
    static void Main(string[] args)
    {
        // Задание 1
        string[] months = { "June", "July", "May", "December", "January", "February", "March", "April", "August", "September", "October", "November" };

        int n = 4;
        var monthsWithLengthN = months.Where(month => month.Length == n);
        Console.WriteLine("Месяцы с длиной строки равной " + n + ": " + string.Join(", ", monthsWithLengthN));

        var summerWinterMonths = months.Where(month => new[] { "June", "July", "August", "December", "January", "February" }.Contains(month));
        Console.WriteLine("Летние и зимние месяцы: " + string.Join(", ", summerWinterMonths));

        var alphabeticalOrder = months.OrderBy(month => month);
        Console.WriteLine("Месяцы в алфавитном порядке: " + string.Join(", ", alphabeticalOrder));

        var monthsWithU = months.Where(month => month.Contains('u') && month.Length >= 4).Count();
        Console.WriteLine("Количество месяцев с буквой 'u' и длиной >= 4: " + monthsWithU);

        // Задание 2
        Console.WriteLine(); Console.WriteLine();
        List<Student> students = new List<Student>
        {
            new Student("Бабашинский", "Глеб", "Александрович", new DateTime(2003, 5, 12), "Витебск", "123456789", "ИТ", 2, 1),
            new Student("Жинак", "Евгений", "Игоревич", new DateTime(2002, 3, 15), "Минск", "987654321", "ХТиТ", 3, 2),
            new Student("Харчук", "Денис", "Павлович", new DateTime(2001, 7, 20), "Могилев", "123123123", "ИТ", 4, 3),
            new Student("Денак", "Александра", "Валерьявна", new DateTime(2004, 9, 25), "Гродно", "456456456", "ЛесХоз", 1, 4),
            new Student("Машедо", "Полина", "Александровна", new DateTime(2003, 1, 30), "Молодечно", "789789789", "ИТ", 2, 1),
            new Student("Кумабойчик", "Константин", "Олегович", new DateTime(2001, 11, 10), "Могилев", "321321321", "ЦД", 4, 5),
            new Student("Левицкий", "Павел", "Константинович", new DateTime(2002, 4, 18), "Браслав", "654654654", "ИТ", 3, 2),
            new Student("Молот", "Игорь", "Викторович", new DateTime(2005, 8, 22), "Брест", "987987987", "ХТиТ", 1, 4),
            new Student("Волков", "Николай", "Ефремович", new DateTime(2000, 6, 6), "Пинск", "159159159", "ИТ", 5, 3),
            new Student("Шилов", "Роман", "Андреевич", new DateTime(2003, 12, 5), "Заславль", "753753753", "ЦД", 2, 1)
        };

        var itStudents = students.Where(student => student.Faculty == "ИТ").OrderBy(student => student.Name);
        Console.WriteLine("Студенты факультета ИТ:");
        foreach (var student in itStudents)
        {
            Console.WriteLine(student);
        }

        var youngStudent = students.OrderByDescending(s => s.BirthDate).First();
        Console.WriteLine("Самый молодой студент: " + youngStudent);

        // Задание 3
        Console.WriteLine(); Console.WriteLine();
        var specificSpecialty = students.Where(s => s.Faculty == "ИТ").OrderBy(s => s.Surname);
        Console.WriteLine("Студенты специальности ИТ (по алфавиту): " + string.Join(", ", specificSpecialty.Select(s => s.Surname)));

        int group = 1;
        string faculty = "ИТ";
        var specificGroupAndFaculty = students.Where(s => s.Group == group && s.Faculty == faculty);
        Console.WriteLine($"Студенты группы {group} факультета {faculty}:");
        foreach (var student in specificGroupAndFaculty)
        {
            Console.WriteLine(student);
        }

        var groupStudents = students.Where(s => s.Group == group).OrderBy(s => s.Surname);
        Console.WriteLine($"Студенты группы {group} (по фамилии): " + string.Join(", ", groupStudents.Select(s => s.Surname)));

        string name = "Глеб";
        var firstStudentWithName = students.FirstOrDefault(s => s.Name == name);
        Console.WriteLine("Первый студент с именем " + name + ": " + (firstStudentWithName != null ? firstStudentWithName.ToString() : "Не найден"));

        // Задание 4
        Console.WriteLine(); Console.WriteLine();
        var customQuery = students
            .Where(s => s.Course > 1)
            .OrderBy(s => s.Surname)
            .GroupBy(s => s.Faculty)
            .Select(group => new
            {
                Faculty = group.Key,
                Count = group.Count(),
                AverageAge = group.Average(s => s.CalculateAge())
            })
            .Where(result => result.Count > 2)
            .ToList();

        foreach (var result in customQuery)
        {
            Console.WriteLine($"Факультет: {result.Faculty}, Кол-во: {result.Count}, Средний возраст: {result.AverageAge}");
        }

        // Задание 5
        Console.WriteLine(); Console.WriteLine();
        var faculties = new[]
        {
            new { Faculty = "ИТ", Dean = "Шаман" },
            new { Faculty = "ХТиТ", Dean = "Баркович" },
            new { Faculty = "ЦД", Dean = "Демидович" },
            new { Faculty = "ЛесХоз", Dean = "Яковлев" }
        };

        var studentsWithDeans = students
            .Join(faculties,
                  student => student.Faculty,
                  faculty => faculty.Faculty,
                  (student, faculty) => new
                  {
                      student.Name,
                      student.Surname,
                      student.Faculty,
                      faculty.Dean
                  });

        Console.WriteLine("Студенты и их деканы:");
        foreach (var item in studentsWithDeans)
        {
            Console.WriteLine($"{item.Surname} {item.Name}, Факультет: {item.Faculty}, Декан: {item.Dean}");
        }
    }
}
