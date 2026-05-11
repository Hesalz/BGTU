using System;
using System.Collections.Generic;

namespace Lab2
{
    [Serializable]
    public class Literature
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }

        public Literature() { }

        public Literature(string title, string author, int year)
        {
            Title = title;
            Author = author;
            Year = year;
        }

    }
}
