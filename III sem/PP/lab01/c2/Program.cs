using System;

public class C2 : I1
{
    // Константы из C1
    private const int PRIVATE_CONSTANT = 1;
    public const string PUBLIC_CONSTANT = "Public";
    protected const double PROTECTED_CONSTANT = 3.14;

    // Поля из C1
    private int privateField;
    public string publicField;
    protected bool protectedField;

    // Свойства из C1
    private int PrivateProperty { get; set; }
    public string PublicProperty { get; set; }
    protected bool ProtectedProperty { get; set; }

    // Реализация свойства из I1
    public string Name { get; set; }

    // Конструкторы
    public C2()
    {
        privateField = 0;
        publicField = string.Empty;
        protectedField = false;
    }

    public C2(C2 other)
    {
        privateField = other.privateField;
        publicField = other.publicField;
        protectedField = other.protectedField;
        PrivateProperty = other.PrivateProperty;
        PublicProperty = other.PublicProperty;
        ProtectedProperty = other.ProtectedProperty;
        Name = other.Name;
    }

    public C2(int privateValue, string publicValue, bool protectedValue)
    {
        privateField = privateValue;
        publicField = publicValue;
        protectedField = protectedValue;
    }

    // Методы из C1
    private void PrivateMethod()
    {
        Console.WriteLine("Это приватный метод");
    }

    public void PublicMethod()
    {
        Console.WriteLine("Это публичный метод");
        PrivateMethod();
    }

    protected void ProtectedMethod()
    {
        Console.WriteLine("Это защищенный метод");
    }

    // Реализация метода из I1
    public void DoSomething(int parameter)
    {
        Console.WriteLine($"Выполняется действие с параметром: {parameter}");
        OnSomethingHappened(EventArgs.Empty);
    }

    // Реализация события из I1
    public event EventHandler<EventArgs> SomethingHappened;

    protected virtual void OnSomethingHappened(EventArgs e)
    {
        SomethingHappened?.Invoke(this, e);
    }

    // Реализация индексатора из I1
    private string[] items = new string[10];
    public string this[int index]
    {
        get => items[index];
        set => items[index] = value;
    }

    // Метод для отображения информации
    public void DisplayInfo()
    {
        Console.WriteLine($"Приватное поле: {privateField}");
        Console.WriteLine($"Публичное поле: {publicField}");
        Console.WriteLine($"Защищенное поле: {protectedField}");
        Console.WriteLine($"Приватное свойство: {PrivateProperty}");
        Console.WriteLine($"Публичное свойство: {PublicProperty}");
        Console.WriteLine($"Защищенное свойство: {ProtectedProperty}");
        Console.WriteLine($"Имя (из I1): {Name}");
    }
}