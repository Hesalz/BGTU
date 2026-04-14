using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab5Lib
{
    public class DecSHA512 : Decorator
    {
        public DecSHA512(IWriter writer) : base(writer) { }

        public override string? Save(string? message)
        {
            message = message ?? string.Empty;

            byte[] hash;
            SHA512 shaM = SHA512.Create();
            hash = shaM.ComputeHash(Encoding.ASCII.GetBytes(message));

            message += Constant.Delimiter + Convert.ToBase64String(hash);

            return base.Save(message);
        }
    }
}
