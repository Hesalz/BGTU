using System;
namespace lab1
{
    public class C3
    {
        private const int PRIVATE_CONSTANT = 10;
        protected const string PROTECTED_CONSTANT = "Защищенная константа C3";
        public const double PUBLIC_CONSTANT = 3.14;

        private string privateField = "Приватное поле C3";
        protected string protectedField = "Защищенное поле C3";
        public string publicField = "Публичное поле C3";

        public string PublicProperty { get; set; } = "Публичное свойство C3";
        protected string ProtectedProperty { get; set; } = "Защищенное свойство C3";

        private string[] items = new string[5];
        public virtual string this[int index]
        {
            get => items[index];
            set => items[index] = value;
        }

        public C3()
        {
            Console.WriteLine("Конструктор C3");
        }

        public void PublicMethod()
        {
            Console.WriteLine("Публичный метод C3");
            Console.WriteLine($"Приватная константа: {PRIVATE_CONSTANT}");
        }

        protected void ProtectedMethod()
        {
            Console.WriteLine("Защищенный метод C3");
        }

        private void PrivateMethod()
        {
            Console.WriteLine("Приватный метод C3");
        }

        public virtual void VirtualMethod()
        {
            Console.WriteLine("Виртуальный метод C3");
        }
    }
}