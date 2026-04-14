using System;

namespace lab1
{
    public class C1
    {
        private const int PRIVATE_CONSTANT = 1;
        public const string PUBLIC_CONSTANT = "Public";
        protected const double PROTECTED_CONSTANT = 3.14;
        private int privateField;
        public string publicField;
        protected bool protectedField;

        private int PrivateProperty { get; set; }
        public string PublicProperty { get; set; }
        protected bool ProtectedProperty { get; set; }
        public string Name { get; set; }

        public void DoSomething(int parameter)
        {
            Console.WriteLine($"C1: Произошло действие над параметром: {parameter}");
        }

        public void DisplayFields()
        {
            Console.WriteLine($"Приватное поле: {privateField}");
        }

        public C1()
        {
            privateField = 0;
            publicField = string.Empty;
            protectedField = false;
        }

        public C1(C1 other)
        {
            privateField = other.privateField;
            publicField = other.publicField;
            protectedField = other.protectedField;
            PrivateProperty = other.PrivateProperty;
            PublicProperty = other.PublicProperty;
            ProtectedProperty = other.ProtectedProperty;
        }
        public C1(int privateValue, string publicValue, bool protectedValue)
        {
            privateField = privateValue;
            publicField = publicValue;
            protectedField = protectedValue;
        }
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

        public void DisplayInfo()
        {
            Console.WriteLine($"Приватное поле: {privateField}");
            Console.WriteLine($"Публичное поле: {publicField}");
            Console.WriteLine($"Защищенное поле: {protectedField}");
            Console.WriteLine($"Приватное свойство: {PrivateProperty}");
            Console.WriteLine($"Публичное свойство: {PublicProperty}");
            Console.WriteLine($"Защищенное свойство: {ProtectedProperty}");
        }
    }
    class Program
    {
        static void Main()
        {
            C1 obj1 = new C1();
            obj1.PublicProperty = "Объект 1";
            obj1.DisplayInfo();

            C1 obj2 = new C1(10, "Объект 2", true);
            obj2.DisplayInfo();

            C1 obj3 = new C1(obj2);
            obj3.publicField = "Объект 3";
            obj3.DisplayInfo();

            obj1.publicField = "Изменено";
            obj1.PublicMethod();

            C2 myObject1 = new C2();
            myObject1.Name = "Test Object 1";
            Console.WriteLine($"Имя объекта 1: {myObject1.Name}");
            myObject1.DoSomething(1);

            myObject1.SomethingHappened += (sender, e) =>
            {
                Console.WriteLine("Событие SomethingHappened было вызвано.");
            };

            C2 myObject2 = new C2("Объект 2");
            myObject2.Name = "Test Object 2";
            Console.WriteLine($"Имя объекта 2: {myObject2.Name}");
            myObject2.DoSomething(2);

            myObject2[0] = "Первый элемент";
            myObject2[1] = "Второй элемент";
            Console.WriteLine($"Элемент под индексом 0: {myObject2[0]}");
            Console.WriteLine($"Элемент под индексом 1: {myObject2[1]}");

            myObject2.DoSomething(3);

            Console.WriteLine("Создание объекта класса C4:");
            C4 obj = new C4();

            Console.WriteLine("\nДемонстрация работы с собственными членами C4:");
            obj.NewMethod();
            Console.WriteLine($"Новая публичная константа C4: {C4.NEW_PUBLIC_CONSTANT}");
            Console.WriteLine($"Новое свойство C4: {obj.NewProperty}");

            Console.WriteLine("\nДемонстрация работы с унаследованными членами:");
            obj.DemonstrateInheritance();

            Console.WriteLine("\nДемонстрация переопределенного метода:");
            obj.VirtualMethod();

            Console.WriteLine("\nДемонстрация работы с индексатором:");
            obj[0] = "Тест";
            Console.WriteLine($"Элемент с индексом 0: {obj[0]}");

            Console.WriteLine("\nДоступ к публичным полям и свойствам:");
            Console.WriteLine($"Новое публичное поле C4: {obj.NewPublicField}");
            Console.WriteLine($"Унаследованное публичное поле: {obj.publicField}");
            obj.PublicProperty = "Измененное публичное свойство";
            Console.WriteLine($"Измененное публичное свойство: {obj.PublicProperty}");
        }
    }
}