using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StocareDateBanking
{
    public static class ManagerDate
    {
        private static string filePath = "banca_data.txt";

        public static void SalveazaDate(string data)
        {
            File.WriteAllText(filePath, data);
        }

        public static string IncarcaDate()
        {
            return File.Exists(filePath) ? File.ReadAllText(filePath) : "";
        }
    }
}
