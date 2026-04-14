using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab5Lib
{
    public class Decorator : IWriter
    {
        protected IWriter? writer;
        public Decorator(IWriter? writer)
        {
            this.writer = writer;
        }
        public virtual string? Save(string? message)
        {
            return writer?.Save(message);
        }
    }
}
