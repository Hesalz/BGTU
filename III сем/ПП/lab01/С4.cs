using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab01
{
    public class C4 : C3
    {
        public int childPublicField;
        public void ChildPublicMethod()
        {
            Console.WriteLine("Публичный метод класса C4");
            ProtectedMethod();
        }
    }
}
