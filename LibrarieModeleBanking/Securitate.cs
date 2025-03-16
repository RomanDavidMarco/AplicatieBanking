using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModeleBanking
{
    public static class Securitate
    {
        public static string CriptarePin(string pin)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(pin);
                byte[] hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        public static bool VerificaPin(string pin, string pinCriptat)
        {
            return CriptarePin(pin) == pinCriptat;
        }
    }
}
