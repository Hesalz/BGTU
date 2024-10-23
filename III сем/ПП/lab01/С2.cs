using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab01
{
    public class C2 : C1, I1 // Задание 3
    {
        public int MyProperty { get; set; }
        public event EventHandler MyEvent;
        public int this[int index]
        {
            get { return index; }
            set { Console.WriteLine($"Для индекса {index} установлено значение {value}"); }
        }

        public void MyMethod()
        {
            Console.WriteLine("Метод интерфейса 1");
        }
    }
}
