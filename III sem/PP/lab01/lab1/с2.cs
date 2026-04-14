using System;

namespace lab1
{

    public class C2 : C1, I1
    {
        private string[] data = new string[10];
        public string this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }

        public event EventHandler<EventArgs> SomethingHappened;

        public new void DoSomething(int parameter)
        {
            Console.WriteLine($"C2: Произошло действие над параметром: {parameter}");
            OnSomethingHappened(EventArgs.Empty);
        }

        protected virtual void OnSomethingHappened(EventArgs e)
        {
            SomethingHappened?.Invoke(this, e);
        }

        public C2()
        {
            Console.WriteLine("C2: Конструктор вызван.");
        }

        public C2(string param)
        {
            Console.WriteLine($"C2: Конструктор вызван с параметром: {param}");
        }
    }
}