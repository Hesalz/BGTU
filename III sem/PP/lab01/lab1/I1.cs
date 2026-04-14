using System;

namespace lab1
{
    public interface I1
    {
        string Name { get; set; }
        void DoSomething(int parameter);
        event EventHandler<EventArgs> SomethingHappened;
        string this[int index] { get; set; }
    }
}