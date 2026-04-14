using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Lab5Lib
{
    public class FileWriter : IWriter
    {
        string filename = Constant.FileName;
        public string FileName { get { return this.filename; } }
        public FileWriter(string? filename = null)
        {
            this.filename = filename ?? Constant.FileName;
        }
        public string? Save(string? message)
        {
            using (StreamWriter writer = new(filename))
            {
                writer.Write(message);
                writer.Flush();
            };
            return this.filename;
        }
    }
}
