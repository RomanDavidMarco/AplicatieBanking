using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilitareBanking
{
    public class Logger
    {
        private static string numeFisierLogs;

        public Logger(string numeFisier)
        {
            numeFisierLogs = numeFisier;

            Stream streamFisierTextLogs = File.Open(numeFisierLogs, FileMode.OpenOrCreate);
            streamFisierTextLogs.Close();
        }

        public static void AddLog(string mesaj)
        {
            using (StreamWriter writer = new StreamWriter(numeFisierLogs, true))
            {
                writer.WriteLine($"[{DateTime.Now}] {mesaj}");
            }
        }
    }
}
