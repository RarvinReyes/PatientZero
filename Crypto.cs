using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace PatientZero {
    public static class Crypto {
        public static string Hash(string value, Guid salt) {
            var sha512 = System.Security.Cryptography.SHA512.Create();
            var hash = Convert.ToBase64String(sha512
                .ComputeHash(Encoding.UTF8.GetBytes(value + salt.ToString())));
            sha512.Dispose();
            return hash;
            
        }
    }
}