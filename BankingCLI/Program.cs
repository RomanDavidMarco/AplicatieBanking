using System;
using LibrarieModeleBanking;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            Banca banca = new Banca("Banca Transilvania");
            Bancomat bancomat = new Bancomat(banca, 10000m);

            // Creare utilizator
            Utilizator utilizator = new Utilizator("Ion Popescu");
            ContBancar cont = new ContBancar("1", 5000m, "1234");
            utilizator.AdaugaCont(cont);
            banca.AdaugaUtilizator(utilizator);

            Console.WriteLine("=== Bine ai venit la ATM! ===");
            Console.Write("Introduceți numărul contului: ");
            string numarCont = Console.ReadLine();

            Console.Write("Introduceți PIN-ul: ");
            string pin = Console.ReadLine();

            var contUtilizator = banca.CautaCont(numarCont);
            if (contUtilizator != null && contUtilizator.VerificaPin(pin))
            {
                Console.WriteLine("Autentificare reușită!");
                bool running = true;

                while (running)
                {
                    Console.WriteLine("\n1. Retragere Bani");
                    Console.WriteLine("2. Depunere Bani");
                    Console.WriteLine("3. Consultare Sold");
                    Console.WriteLine("4. Ieșire");
                    Console.Write("Alegeți opțiunea: ");
                    string optiune = Console.ReadLine();

                    switch (optiune)
                    {
                        case "1":
                            Console.Write("Sumă: ");
                            decimal suma = Convert.ToDecimal(Console.ReadLine());
                            if (bancomat.RetragereBani(numarCont, suma))
                                Console.WriteLine("Retragere reușită!");
                            else
                                Console.WriteLine("Fonduri insuficiente!");
                            break;
                        case "2":
                            Console.Write("Sumă: ");
                            suma = Convert.ToDecimal(Console.ReadLine());
                            contUtilizator.Depunere(suma);
                            Console.WriteLine("Depunere reușită!");
                            break;
                        case "3":
                            Console.WriteLine($"Sold: {contUtilizator.Sold} RON");
                            break;
                        case "4":
                            running = false;
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Autentificare eșuată!");
            }
        }
    }
}
