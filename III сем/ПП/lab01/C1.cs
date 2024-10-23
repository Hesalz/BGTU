using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab01
{
    public class C1 // Задание 1
    {
        private const int PrivateConstant = 10;
        public const int PublicConstant = 20;
        protected const int ProtectedConstant = 30;
        private int privateField;
        public int publicField;
        protected int protectedField;
        private int PrivateProperty { get; set; }
        public int PublicProperty { get; set; }
        protected int ProtectedProperty { get; set; }
        public C1()
        {
            privateField = 0;
            publicField = 0;
            protectedField = 0;
            PrivateProperty = 0;
            PublicProperty = 0;
            ProtectedProperty = 0;
        }
        public C1(int publicField, int protectedField, int privateField)
        {
            this.publicField = publicField;
            this.protectedField = protectedField;
            this.privateField = privateField;
        }
        public C1(C1 obj)   
        {
            this.publicField = obj.publicField;
            this.protectedField = obj.protectedField;
            this.privateField = obj.privateField;
        }
        private void PrivateMethod()
        {
            Console.Write("Приватный метод: ");
            Console.WriteLine($"Значение privateField: {privateField}");
        }
        public void PublicMethod()
        {
            Console.Write("Публичный метод: ");
            Console.WriteLine($"Значение publicField: {publicField}");
            ProtectedMethod();
            PrivateMethod();
        }
        protected void ProtectedMethod()
        {
            Console.Write("Защищённый метод: ");
            Console.WriteLine($"Значение protectedField: {protectedField}");
        }
    }
}
