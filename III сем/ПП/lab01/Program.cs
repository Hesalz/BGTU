using System;
namespace lab01;

class Program
{
    static void Main(string[] args)
    {
        C1 obj1 = new C1(); // 1.2
        obj1.PublicMethod();
        obj1.PublicProperty = 100;
        Console.WriteLine($"Публичное свойство: {obj1.PublicProperty}");
        C1 obj2 = new C1(1, 2, 3);
        obj2.PublicMethod();
        C1 obj3 = new C1(obj2);
        obj3.PublicMethod(); Console.WriteLine();  Console.WriteLine("Задание 3.8");

        C2 objC2 = new C2(); // 3.8
        objC2.PublicMethod();
        objC2.MyMethod();
        objC2.MyProperty = 10;
        Console.WriteLine($"Свойство класса C2: {objC2.MyProperty}");
        objC2[5] = 100; Console.WriteLine(); Console.WriteLine("Задание 4.9");


        C4 objC4 = new C4(); // 4.9
        objC4.PublicMethod();
        objC4.ChildPublicMethod();
    }
}
