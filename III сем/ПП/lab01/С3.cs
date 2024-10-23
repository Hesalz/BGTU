using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab01
{
    public class C3
    {
        public int publicField;
        private int privateField;
        protected int protectedField;
        public void PublicMethod()
        {
            Console.WriteLine("Публичный метод класса C3");
        }

        protected void ProtectedMethod()
        {
            Console.WriteLine("Защищённый метод класса C3");
        }
    }
}
