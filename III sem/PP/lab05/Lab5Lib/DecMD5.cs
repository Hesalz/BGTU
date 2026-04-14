using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab5Lib
{
    public class DecMD5 : Decorator
    {
        public DecMD5(IWriter writer) : base(writer) { }

        public override string? Save(string? message)
        {
            message = message ?? string.Empty;
            byte[] hash;
            MD5 md5 = MD5.Create();
            hash = md5.ComputeHash(Encoding.ASCII.GetBytes(message));
            message += Constant.Delimiter + Convert.ToBase64String(hash);

            return base.Save(message);
        }
    }
}
