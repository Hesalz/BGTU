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

    public const string University = "BGTU";

    private static int _studentCount;

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
            if (value < 1 || value > 5)
                throw new ArgumentException("Группа должна быть от 1 до 10.");
            _group = value;
        }
    }

    static Student()
    {
        _studentCount = 0;
    }

    private Student(int id)
    {
        _id = id;
    }

    public Student(string surname, string name, string patronymic, DateTime birthDate, string address, string phone, string faculty, int course, int group)
    {
        _id = GenerateUniqueID();
        _surname = surname;
        _name = name;
        _patronymic = patronymic;
        _birthDate = birthDate;
        _address = address;
        _phone = phone;
        _faculty = faculty;
        Course = course;
        _group = group;

        _studentCount++;
    }

    public Student()
    {
        _id = GenerateUniqueID();
        _surname = "Unknown";
        _name = "Unknown";
        _patronymic = "Unknown";
        _birthDate = DateTime.Now;
        _address = "Unknown";
        _phone = "Unknown";
        _faculty = "Unknown";
        _course = 1;
        _group = 0;

        _studentCount++;
    }

    public Student(string surname, string name)
    {
        _id = GenerateUniqueID();
        _surname = surname;
        _name = name;
        _patronymic = "Unknown";
        _birthDate = DateTime.Now;
        _address = "Unknown";
        _phone = "Unknown";
        _faculty = "Unknown";
        _course = 1;
        _group = 0;

        _studentCount++;
    }

    private static int GenerateUniqueID()
    {
        return new Random().Next(1000, 9999); 
    }

    
    public static void ShowClassInfo()
    {
        Console.WriteLine($"Объектов класса Student создано: {_studentCount}");
    }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Student))
            return false;

        Student other = (Student)obj;
        return ID == other.ID;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ID, Surname, Name, Patronymic);
    }

    public override string ToString()
    {
        return $"ID: {ID}, ФИО: {Surname} {Name} {Patronymic}, Факультет: {Faculty}, Курс: {Course}, Группа: {Group}";
    }

    public int CalculateAge()
    {
        DateTime now = DateTime.Now;
        int age = now.Year - BirthDate.Year;
        if (now < BirthDate.AddYears(age))
            age--;
        return age;
    }

    public void UpdateCourse(ref int newCourse, out bool isUpdated)
    {
        if (newCourse < 1 || newCourse > 5)
        {
            isUpdated = false;
        }
        else
        {
            _course = newCourse;
            isUpdated = true;
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Student[] students = new Student[0];
        Array.Resize(ref students, students.Length + 1);
        students[students.Length - 1] = new Student("Бабашинский", "Глеб", "Александрович", new DateTime(2006, 03, 16), "г. Витебск", "295119753", "ИТ", 3, 1);

        Array.Resize(ref students, students.Length + 1);
        students[students.Length - 1] = new Student("Харченко", "Евгений", "Игоревич", new DateTime(2005, 08, 26), "г. Могилёв", "297104549", "ИТ", 1, 1);

        Console.WriteLine("Задание 2:");
            Student student1 = new Student("Иванов", "Иван", "Иванович", new DateTime(2000, 5, 15), "г. Минск", "123456789", "ФИТ", 2, 3);
            Student student2 = new Student("Петров", "Петр", "Петрович", new DateTime(2001, 8, 23), "г. Витебск", "987654321", "ФИТ", 3, 2);
            Student student3 = new Student("Жеребьев", "Антон");
            Console.WriteLine("Студенты: ");
            Console.WriteLine(student1);
            Console.WriteLine(student2); 
            Console.WriteLine(student3); Console.WriteLine();
            Console.WriteLine($"Возраст студента {student1.Surname}: {student1.CalculateAge()} лет");
            Console.WriteLine("Равны ли student1 и student2? " + (student1.Equals(student2) ? "Да" : "Нет"));
            Console.WriteLine("Тип объекта student1: " + student1.GetType());

        Console.WriteLine(); Console.WriteLine("Задание 4: ");
        var anonymousStudent = new
        {
            ID = 1,
            FullName = "Лезбиков Артём Сергеевич",
            Faculty = "ХТиТ",
            Course = 3,
            Group = 4
        };
        Console.WriteLine($"ID: {anonymousStudent.ID}, ФИО: {anonymousStudent.FullName}, Факультет: {anonymousStudent.Faculty}, Курс: {anonymousStudent.Course}, Группа: {anonymousStudent.Group}"); Console.WriteLine();

        while (true)
        {
            Console.WriteLine("\nВыберите действие:");
            Console.WriteLine("1 - Добавить студента");
            Console.WriteLine("2 - Показать всех студентов");
            Console.WriteLine("3 - Найти студентов по факультету");
            Console.WriteLine("4 - Найти студентов по группе");
            Console.WriteLine("5 - Узнать кол-во созданных объектов класса Student");
            Console.WriteLine("0 - Выйти");

            Console.Write("Ваш выбор: "); string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddStudent(ref students);
                    break;
                case "2":
                    ShowAllStudents(students);
                    break;
                case "3":
                    FindByFaculty(students);
                    break;
                case "4":
                    FindByGroup(students);
                    break;
                case "5":
                    Student.ShowClassInfo();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Некорректный выбор. Попробуйте снова.");
                    break;
            }
        }
    }

    public static void AddStudent(ref Student[] students)
    {
        Console.WriteLine("\nДобавление студента...");

        Console.Write("Введите фамилию: ");
        string surname = Console.ReadLine();

        Console.Write("Введите имя: ");
        string name = Console.ReadLine();

        Console.Write("Введите отчество: ");
        string patronymic = Console.ReadLine();

        Console.Write("Введите дату рождения (гггг-мм-дд): ");
        DateTime birthDate;
        while (!DateTime.TryParse(Console.ReadLine(), out birthDate))
        {
            Console.Write("Некорректный ввод. Введите дату рождения (гггг-мм-дд): ");
        }

        Console.Write("Введите адрес: ");
        string address = Console.ReadLine();

        Console.Write("Введите телефон: ");
        string phone = Console.ReadLine();

        Console.Write("Введите факультет: ");
        string faculty = Console.ReadLine();

        Console.Write("Введите курс (1-5): ");
        int course;
        while (!int.TryParse(Console.ReadLine(), out course) || course < 1 || course > 5)
        {
            Console.Write("Некорректный ввод. Введите курс (1-5): ");
        }

        Console.Write("Введите группу (1-10): ");
        int group;
        while (!int.TryParse(Console.ReadLine(), out group) || group < 1 || group > 10)
        {
            Console.Write("Некорректный ввод. Введите группу (1-10): ");
        }

        Student newStudent = new Student(surname, name, patronymic, birthDate, address, phone, faculty, course, group);

        Array.Resize(ref students, students.Length + 1);
        students[students.Length - 1] = newStudent;

        Console.WriteLine("\nСтудент добавлен успешно.");
    }

    public static void ShowAllStudents(Student[] students)
    {
        Console.WriteLine("\nСписок всех студентов:");
        foreach (Student student in students)
        {
            Console.WriteLine(student.ToString());
        }
    }

    public static void FindByFaculty(Student[] students)
    {
        Console.Write("\nВведите факультет для поиска: ");
        string faculty = Console.ReadLine();
        bool fac = false;

        Console.WriteLine($"\nСтуденты факультета {faculty}:");
        foreach (Student student in students)
        {
            if (student.Faculty == faculty)
            {
                Console.WriteLine(student.ToString());
                fac = true;
            }
        }
        if (!fac) { Console.WriteLine($"Студенты факультета {faculty} не найдены."); }
    }

    public static void FindByGroup(Student[] students)
    {
        Console.Write("\nВведите номер группы для поиска: ");
        int group;

        while (!int.TryParse(Console.ReadLine(), out group))
        {
            Console.Write("Некорректный ввод. Введите номер группы (число): ");
        }

        Console.WriteLine($"\nСтуденты группы {group}:");
        bool found = false;
        foreach (Student student in students)
        {
            if (student.Group == group)
            {
                Console.WriteLine(student.ToString());
                found = true;
            }
        }

        if (!found)
        {
            Console.WriteLine($"Студенты в группе {group} не найдены.");
        }
    }
}
