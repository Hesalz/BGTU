using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Lab5Lib
{
    public class DecRSA : Decorator
    {
        public DecRSA(IWriter writer) : base(writer) { }

        public override string? Save(string? message)
        {
            message = message ?? string.Empty;
            int hashIndex = message.LastIndexOf(Constant.Delimiter);
            if (hashIndex != -1)
            {
                string mess = message.Substring(0, hashIndex);
                string hash = message.Substring(hashIndex + 1);

                byte[] dataToEncrypt = Convert.FromBase64String(hash);
                byte[] encryptedData;

                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    encryptedData = rsa.Encrypt(dataToEncrypt, false);

                    string encHash = Convert.ToBase64String(encryptedData);
                    string rsaXml = rsa.ToXmlString(true);
                    message = mess + Constant.Delimiter + encHash + Constant.Delimiter + rsaXml;
                }
            }
            return base.Save(message);
        }
    }
}