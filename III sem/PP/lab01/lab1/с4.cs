using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab1
{
    public class C4 : C3
    {
        public const string NEW_PUBLIC_CONSTANT = "Новая публичная константа C4";
        public string NewPublicField = "Новое публичное поле C4";

        public int NewProperty { get; set; } = 100;

        public override string this[int index]
        {
            get => base[index];
            set => base[index] = $"C4: {value}";
        }

        public C4() : base()
        {
            Console.WriteLine("Конструктор C4");
        }

        public void NewMethod()
        {
            Console.WriteLine("Новый метод C4");
            Console.WriteLine($"Доступ к защищенной константе базового класса: {PROTECTED_CONSTANT}");
            Console.WriteLine($"Доступ к защищенному полю базового класса: {protectedField}");
            Console.WriteLine($"Доступ к защищенному свойству базового класса: {ProtectedProperty}");
            ProtectedMethod();
        }

        public override void VirtualMethod()
        {
            base.VirtualMethod();
            Console.WriteLine("Переопределенный виртуальный метод в C4");
        }

        public void DemonstrateInheritance()
        {
            Console.WriteLine($"Публичная константа базового класса: {PUBLIC_CONSTANT}");
            Console.WriteLine($"Публичное поле базового класса: {publicField}");
            Console.WriteLine($"Публичное свойство базового класса: {PublicProperty}");
            PublicMethod();
        }
    }
}
