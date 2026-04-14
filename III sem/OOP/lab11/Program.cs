using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

public static class Reflector
{
    // a
    public static string GetAssemblyName(string className)
    {
        Type type = Type.GetType(className);
        if (type == null) throw new ArgumentException($"Класс {className} не найден.");
        return type.Assembly.FullName;
    }

    // b
    public static bool HasPublicConstructors(string className)
    {
        Type type = Type.GetType(className);
        if (type == null) throw new ArgumentException($"Класс {className} не найден.");
        return type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any();
    }

    // c
    public static IEnumerable<string> GetPublicMethods(string className)
    {
        Type type = Type.GetType(className);
        if (type == null) throw new ArgumentException($"Класс {className} не найден.");
        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                   .Select(m => m.Name);
    }

    // d
    public static IEnumerable<string> GetFieldsAndProperties(string className)
    {
        Type type = Type.GetType(className);
        if (type == null) throw new ArgumentException($"Класс {className} не найден.");
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                         .Select(f => $"Field: {f.Name}");
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                             .Select(p => $"Property: {p.Name}");
        return fields.Concat(properties);
    }

    // e
    public static IEnumerable<string> GetImplementedInterfaces(string className)
    {
        Type type = Type.GetType(className);
        if (type == null) throw new ArgumentException($"Класс {className} не найден.");
        return type.GetInterfaces().Select(i => i.Name);
    }

    // f
    public static IEnumerable<string> GetMethodsByParameterType(string className, string parameterTypeName)
    {
        Type type = Type.GetType(className);
        if (type == null) throw new ArgumentException($"Класс {className} не найден.");
        Type parameterType = Type.GetType(parameterTypeName);
        if (parameterType == null) throw new ArgumentException($"Тип параметра {parameterTypeName} не найден.");
        return type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                   .Where(m => m.GetParameters().Any(p => p.ParameterType == parameterType))
                   .Select(m => m.Name);
    }

    // g
    public static object InvokeMethod(object obj, string methodName, params object[] parameters)
    {
        MethodInfo method = obj.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if (method == null) throw new ArgumentException($"Метод {methodName} не найден.");
        return method.Invoke(obj, parameters);
    }

    public static object InvokeMethodFromFile(string className, string methodName, string filePath)
    {
        Type type = Type.GetType(className);
        if (type == null) throw new ArgumentException($"Класс {className} не найден.");
        MethodInfo method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
        if (method == null) throw new ArgumentException($"Метод {methodName} не найден.");

        var parameters = File.ReadAllLines(filePath)
                             .Select(line => Convert.ChangeType(line, Type.GetType(line.GetType().ToString())))
                             .ToArray();
        var obj = Activator.CreateInstance(type);
        return method.Invoke(obj, parameters);
    }

    public static T Create<T>() where T : new()
    {
        return new T();
    }

    public static void WriteToFile(string filePath, IEnumerable<string> data)
    {
        File.WriteAllLines(filePath, data);
    }
}

public class Class1
{
    public int Number { get; set; }
    public string Text { get; set; }

    public Class1() { }
    public Class1(int number, string text)
    {
        Number = number;
        Text = text;
    }

    public void PrintInfo(string message)
    {
        Console.WriteLine($"Сообщение: {message}, Номер: {Number}, Текст: {Text}");
    }

    public int AddNumbers(int a, int b) => a + b;
}

class Program
{
    static void Main()
    {
        string className = "Class1";
        string assemblyName = Reflector.GetAssemblyName(className);
        Console.WriteLine("Имя сборки: " + assemblyName);

        bool hasConstructors = Reflector.HasPublicConstructors(className);
        Console.WriteLine("Есть ли публичные конструкторы: " + hasConstructors);

        var methods = Reflector.GetPublicMethods(className);
        Console.WriteLine("Общедоступные методы:");
        foreach (var method in methods) Console.WriteLine(method);

        var fieldsAndProperties = Reflector.GetFieldsAndProperties(className);
        Console.WriteLine("Поля и свойства:");
        foreach (var item in fieldsAndProperties) Console.WriteLine(item);

        var interfaces = Reflector.GetImplementedInterfaces(className);
        Console.WriteLine("Реализованные интерфейсы:");
        foreach (var i in interfaces) Console.WriteLine(i);

        var methodsWithStringParam = Reflector.GetMethodsByParameterType(className, "System.String");
        Console.WriteLine("Методы с параметром типа string:");
        foreach (var method in methodsWithStringParam) Console.WriteLine(method);

        Class1 sample = new Class1(42, "Hello");
        Reflector.InvokeMethod(sample, "PrintInfo", "Test test test");

        var newSample = Reflector.Create<Class1>();
        Console.WriteLine("Создан новый экземпляр класса.");
        Console.WriteLine(newSample.ToString());

        string filePath = "output.txt";
        var dataToWrite = new List<string>
        {
            $"Имя сборки: {assemblyName}",
            $"Есть ли публичные конструкторы: {hasConstructors}",
            "Публичные методы: " + string.Join(", ", methods),
            "Поля и свойства: " + string.Join(", ", fieldsAndProperties),
            "Реализованные интерфейсы: " + string.Join(", ", interfaces)
        };
        Reflector.WriteToFile(filePath, dataToWrite);

        Console.WriteLine($"Данные записаны в {filePath}");
    }
}
