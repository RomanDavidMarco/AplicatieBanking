using System;
using System.IO;
using LibrarieModeleBanking;
using StocareDateBanking;
using System.Configuration;
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
            string numeFisier = ConfigurationManager.AppSettings["NumeFisier"];
            ManagerUtilizatori adminUtilizatori = new ManagerUtilizatori(numeFisier);
            Banca banca = new Banca("Banca Transilvania");
            Bancomat bancomat = new Bancomat(banca, 100000000m);

            List<Utilizator> utilizatori = adminUtilizatori.IncarcaUtilizatori();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== Sistem de Operatiuni Bancare ===");
                Console.WriteLine("1. Adauga utilizator");
                Console.WriteLine("2. Sterge utilizator");
                Console.WriteLine("3. Afiseaza utilizatori");
                Console.WriteLine("4. Acceseaza ATM");
                Console.WriteLine("5. Salveaza si iesi");
                Console.Write("Alege o optiune: ");
                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1":
                        AdaugaUtilizator(utilizatori, adminUtilizatori);
                        break;
                    case "2":
                        StergeUtilizator(utilizatori, adminUtilizatori);
                        break;
                    case "3":
                        AfiseazaUtilizatori(utilizatori);
                        break;
                    case "4":
                        AcceseazaATM(bancomat, banca);
                        break;
                    case "5":
                        adminUtilizatori.SalveazaUtilizatori(utilizatori);
                        Console.WriteLine("Date salvate. Iesire...");
                        return;
                    default:
                        Console.WriteLine("Optiune invalida! Apasa ENTER pentru a continua...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        static void AdaugaUtilizator(List<Utilizator> utilizatori, ManagerUtilizatori manager)
        {
            Console.Write("Nume: ");
            string nume = Console.ReadLine();
            Console.Write("Prenume: ");
            string prenume = Console.ReadLine();
            Console.Write("CNP: ");
            string cnp = Console.ReadLine();

            Utilizator utilizator = new Utilizator(nume, prenume, cnp);
            utilizatori.Add(utilizator);
            manager.AddUtilizator(utilizator);

            Console.WriteLine("Utilizator adaugat cu succes!");
            Console.ReadLine();
        }

        static void StergeUtilizator(List<Utilizator> utilizatori, ManagerUtilizatori manager)
        {
            Console.Write("Introduceti CNP-ul utilizatorului de sters: ");
            string cnp = Console.ReadLine();
            Utilizator utilizatorDeSters = utilizatori.FirstOrDefault(u => u.CNP == cnp);

            if (utilizatorDeSters != null)
            {
                utilizatori.Remove(utilizatorDeSters);
                manager.SalveazaUtilizatori(utilizatori);

                Console.WriteLine("Utilizator sters cu succes!");
            }
            else
            {
                Console.WriteLine("Utilizatorul nu a fost gasit!");
            }
            Console.ReadLine();
        }

        static void AfiseazaUtilizatori(List<Utilizator> utilizatori)
        {
            Console.WriteLine("=== Lista Utilizatori ===");
            foreach (var utilizator in utilizatori)
            {
                Console.WriteLine($"{utilizator.Nume} {utilizator.Prenume} - CNP: {utilizator.CNP}");
            }
            Console.ReadLine();
        }

        static void AcceseazaATM(Bancomat bancomat, Banca banca)
        {
            Console.Write("Introduceti numarul contului: ");
            string numarCont = Console.ReadLine();

            Console.Write("Introduceti PIN-ul: ");
            string pin = Console.ReadLine();

            var contUtilizator = banca.CautaCont(numarCont);
            if (contUtilizator != null && contUtilizator.VerificaPin(pin))
            {
                Console.WriteLine("Autentificare reusita!");
                bool running = true;

                while (running)
                {
                    Console.WriteLine("\n1. Retragere Bani");
                    Console.WriteLine("2. Depunere Bani");
                    Console.WriteLine("3. Consultare Sold");
                    Console.WriteLine("4. Iesire");
                    Console.Write("Alegeti optiunea: ");
                    string optiune = Console.ReadLine();

                    switch (optiune)
                    {
                        case "1":
                            Console.Write("Suma: ");
                            decimal suma = Convert.ToDecimal(Console.ReadLine());
                            if (bancomat.RetragereBani(numarCont, suma))
                                Console.WriteLine("Retragere reusita!");
                            else
                                Console.WriteLine("Fonduri insuficiente!");
                            break;
                        case "2":
                            Console.Write("Suma: ");
                            suma = Convert.ToDecimal(Console.ReadLine());
                            contUtilizator.Depunere(suma);
                            Console.WriteLine("Depunere reusita!");
                            break;
                        case "3":
                            Console.WriteLine($"Sold: {contUtilizator.Sold} RON");
                            break;
                        case "4":
                            running = false;
                            break;
                        default:
                            Console.WriteLine("Optiune invalida!");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("Autentificare esuata!");
            }
            Console.ReadLine();
        }
    }
}
