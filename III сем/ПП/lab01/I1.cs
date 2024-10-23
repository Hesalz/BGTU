using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab01
{
        public interface I1 // Задание 2
        {
            int MyProperty { get; set; }
            void MyMethod();         
            event EventHandler MyEvent;
            int this[int index] { get; set; }
        }
    }
