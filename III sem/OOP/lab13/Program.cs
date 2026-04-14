using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Soap;

/* public class BinarySerializer : ISerializer
{
    public void Serialize<T>(T obj, string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Create);
        var formatter = new BinaryFormatter();
        formatter.Serialize(stream, obj);
    }

    public T Deserialize<T>(string filePath)
    {
        using var stream = new FileStream(filePath, FileMode.Open);
        var formatter = new BinaryFormatter();
        return (T)formatter.Deserialize(stream);
    }
} */

[Serializable]
public class Address
{
    public string Street { get; set; }
    public string City { get; set; }
}

[Serializable]
public class Person
{
    public string Name { get; set; }
    public int Age { get; set; }

    [NonSerialized]
    public string SensitiveData;

    public Address Address { get; set; }
}

[Serializable]
public class Student : Person
{
    public string University { get; set; }
}

public interface ISerializer
{
    void Serialize<T>(T obj, string filePath);
    T Deserialize<T>(string filePath);
}



public class JSONSerializer : ISerializer
{
    public void Serialize<T>(T obj, string filePath)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
    }

    public T Deserialize<T>(string filePath)
    {
        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<T>(json);
    }
}

public class XMLSerializer : ISerializer
{
    public void Serialize<T>(T obj, string filePath)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stream = new FileStream(filePath, FileMode.Create);
        serializer.Serialize(stream, obj);
    }

    public T Deserialize<T>(string filePath)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var stream = new FileStream(filePath, FileMode.Open);
        return (T)serializer.Deserialize(stream);
    }
}


public class SOAPSerializer : ISerializer
{
    public void Serialize<T>(T obj, string filePath)
    {
        var serializer = new SoapFormatter();
        using var stream = new FileStream(filePath, FileMode.Create);
        serializer.Serialize(stream, obj);
    }

    public T Deserialize<T>(string filePath)
    {
        var serializer = new SoapFormatter();
        using var stream = new FileStream(filePath, FileMode.Open);
        return (T)serializer.Deserialize(stream);
    } 
} 

public class CustomSerializer
{
    private readonly Dictionary<string, ISerializer> _serializers = new();

    public void RegisterSerializer(string format, ISerializer serializer)
    {
        _serializers[format] = serializer;
    }

    public void Serialize<T>(T obj, string format, string filePath)
    {
        if (_serializers.TryGetValue(format, out var serializer))
        {
            serializer.Serialize(obj, filePath);
        }
        else
        {
            throw new Exception($"Сериализатор для формата '{format}' не найден.");
        }
    }

    public T Deserialize<T>(string format, string filePath)
    {
        if (_serializers.TryGetValue(format, out var serializer))
        {
            return serializer.Deserialize<T>(filePath);
        }
        else
        {
            throw new Exception($"Сериализатор для формата '{format}' не найден.");
        }
    }
}

class Program
{
    static void Main()
    {
        var customSerializer = new CustomSerializer();
        //  customSerializer.RegisterSerializer("binary", new BinarySerializer());
        customSerializer.RegisterSerializer("json", new JSONSerializer());
        customSerializer.RegisterSerializer("xml", new XMLSerializer());
        customSerializer.RegisterSerializer("soap", new SOAPSerializer());

        var students = new[]
        {
            new Student { Name = "Василий", Age = 20, University = "БГТУ", Address = new Address { Street = "1-ая Белорусская", City = "Минск" } },
            new Student { Name = "Генадий", Age = 22, University = "БГЭУ", Address = new Address { Street = "Будёного", City = "Минск" } }
        };

        const string jsonFilePath = "students.json";
        customSerializer.Serialize(students, "json", jsonFilePath);
        var deserializedStudents = customSerializer.Deserialize<Student[]>("json", jsonFilePath);

        Console.WriteLine("Десериализованные студенты из JSON:");
        foreach (var student in deserializedStudents)
        {
            Console.WriteLine($"Имя: {student.Name}, Возраст: {student.Age}, Университет: {student.University}");
        }

        var xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(@"
        <Students>
            <Student>
                <Name>Василий</Name>
                <Age>20</Age>
                <University>БГТУ</University>
            </Student>
            <Student>
                <Name>Генадий</Name>
                <Age>22</Age>
                <University>БГЭУ</University>
            </Student>
        </Students>");

        var nav = xmlDoc.CreateNavigator();

        var olderStudents = nav.Select("/Students/Student[Age > 21]");
        Console.WriteLine("\nXPath: студенты старше 21 года:");
        while (olderStudents.MoveNext())
        {
            Console.WriteLine(olderStudents.Current.SelectSingleNode("Name").Value);
        }

        var mitStudents = nav.Select("/Students/Student[University='БГТУ']/Name");
        Console.WriteLine("\nXPath: студенты из БГТУ:");
        while (mitStudents.MoveNext())
        {
            Console.WriteLine(mitStudents.Current.Value);
        }

        var studentsXml = new XElement("Students",
            new XElement("Student",
                new XElement("Name", "Василий"),
                new XElement("Age", 20),
                new XElement("University", "БГТУ")
            ),
            new XElement("Student",
                new XElement("Name", "Генадий"),
                new XElement("Age", 22),
                new XElement("University", "БГЭУ")
            )
        );

        var mitLinqStudents = studentsXml.Elements("Student")
            .Where(x => x.Element("University")?.Value == "БГТУ")
            .Select(x => x.Element("Name")?.Value);

        Console.WriteLine("\nLinq to XML: студенты из БГТУ:");
        foreach (var student in mitLinqStudents)
        {
            Console.WriteLine(student);
        }
    }
}
