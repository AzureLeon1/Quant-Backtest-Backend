using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Quant_BackTest_Backend.NameHash {
    public class HashOperator {
        public string HashGivenString(string input) {
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var hash = new System.Text.StringBuilder();
            byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input));
            foreach (byte theByte in crypto) {
                hash.Append(theByte.ToString("x2"));
            }
            return hash.ToString();
        }
    }
}