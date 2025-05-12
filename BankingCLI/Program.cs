using System;
using System.Collections.Generic;
using System.Linq;
using StocareDateBanking;
using LibrarieModeleBanking;
using UtilitareBanking;
using System.IO;

namespace AplicatieBanking
{
    class Program
    {
        static void Main(string[] args)
        {
            string locatieFisierSolutie = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            // Initializare manageri
            Logger logger = new Logger(locatieFisierSolutie + "\\logs.txt");
            CursValutar cursValutar = new CursValutar(locatieFisierSolutie + "\\CursuriValutare.json");
            ManagerBanci managerBanci = new ManagerBanci(locatieFisierSolutie + "\\banci.txt");
            ManagerUtilizatori managerUtilizatori = new ManagerUtilizatori(locatieFisierSolutie + "\\utilizatori.txt");
            ManagerConturi managerConturi = new ManagerConturi(locatieFisierSolutie + "\\conturi.txt");

            // Incarcare date
            List<Banca> banci = managerBanci.IncarcaBanci();
            List<Utilizator> utilizatori = managerUtilizatori.IncarcaUtilizatori();
            List<ContBancar> conturi = managerConturi.IncarcaConturi();

            /*managerBanci.AfiseazaBanci(banci);
            managerUtilizatori.AfiseazaUtilizatori(utilizatori);
            managerConturi.AfiseazaConturi(conturi);*/

            // Legarea conturilor la utilizatori prin metoda AddCont()
            foreach (var utilizator in utilizatori)
            {
                Banca banca = banci.FirstOrDefault(b => b.Nume.ToUpper() == utilizator.NumeBanca.ToUpper());

                if (banca != null)
                {
                    foreach (var cont in conturi)
                    {
                        if (cont.CNP == utilizator.CNP && cont.ID.Substring(0, 6) == banca.IDBanca)
                        {
                            utilizator.AdaugaCont(cont);
                        }
                    }
                }
            }

            // Legarea utilizatorilor la bănci prin metoda AdaugaUtilizator()
            foreach (var banca in banci)
            {
                foreach (var utilizator in utilizatori)
                {
                    if (utilizator.NumeBanca == banca.Nume)
                    {
                        banca.AdaugaUtilizator(utilizator);
                    }
                }
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== MENIU PRINCIPAL ===");
                Console.WriteLine("1. Adauga Banca");
                Console.WriteLine("2. Sterge Banca");
                Console.WriteLine("3. Afisează Banci");
                Console.WriteLine("4. Selectează Banca");
                Console.WriteLine("5. Iesire");
                Console.Write("Alege o optiune: ");
                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1":
                        AdaugaBanca(managerBanci, banci);
                        break;
                    case "2":
                        StergeBanca(managerBanci, banci, managerUtilizatori, utilizatori, managerConturi, conturi);
                        break;
                    case "3":
                        managerBanci.AfiseazaBanci(banci);
                        break;
                    case "4":
                        SelecteazaBanca(banci, utilizatori, conturi, managerUtilizatori, managerConturi);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Optiune invalida.");
                        break;
                }
            }
        }

        // ===================== MENIU PRINCIPAL =====================
        static void AdaugaBanca(ManagerBanci managerBanci, List<Banca> banci)
        {
            Console.Write("Introdu numele bancii: ");
            string nume = Console.ReadLine();

            if (Banca.ValidareNumeBanca(nume))
            {
                Console.WriteLine("Eroare: Nici un nume valid introdus");
                Console.ReadLine();
                return;
            }

            Console.Write("Introdu initialele bancii (4 litere): ");
            string initiale = Console.ReadLine().ToUpper();

            if (Banca.ValidarePrefixBanca(initiale))
            {
                Console.WriteLine("Eroare: Initialele trebuie sa contina exact 4 litere.");
                Console.ReadLine();
                return;
            }

            if (Banca.ValidareExistaBanca(banci, nume, "RO"+initiale))
            {
                Console.WriteLine("Banca exista deja!");
                Console.ReadLine();
                return;
            }

            managerBanci.AddBanca(banci, nume, initiale);
            Console.WriteLine("Banca a fost adaugata cu succes!");
            Console.ReadLine();
        }

        static void StergeBanca(ManagerBanci managerBanci, List<Banca> banci, ManagerUtilizatori managerUtilizatori, List<Utilizator> utilizatori, ManagerConturi managerConturi, List<ContBancar> conturi)
        {
            Console.Write("Introdu ID-ul / numele bancii: ");
            string id = Console.ReadLine().ToUpper();

            bool OK = managerBanci.StergeBanca(banci, utilizatori, conturi, managerConturi, managerUtilizatori, id);
            if (OK)
            {
                Console.WriteLine("Banca si toti utilizatorii sai au fost stersi cu succes!");
            }
            else
            {
                Console.WriteLine("Eroare la stergerea bancii!");
            }
            Console.ReadLine();
        }

        // ===================== MENIU UTILIZATORI =====================
        static void SelecteazaBanca(List<Banca> banci, List<Utilizator> utilizatori, List<ContBancar> conturi, ManagerUtilizatori managerUtilizatori, ManagerConturi managerConturi)
        {
            Console.Write("Introdu ID-ul sau numele bancii: ");
            string input = Console.ReadLine().ToUpper();

            Banca banca = banci.FirstOrDefault(b =>
                b.IDBanca.ToUpper() == input || b.Nume.ToUpper() == input);

            if (banca == null)
            {
                Console.WriteLine("Banca nu exista!");
                Console.ReadLine();
                return;
            }

            MeniuUtilizatori(banca, utilizatori, conturi, managerUtilizatori, managerConturi);
        }

        static void MeniuUtilizatori(Banca banca, List<Utilizator> utilizatori, List<ContBancar> conturi, ManagerUtilizatori managerUtilizatori, ManagerConturi managerConturi)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== {banca.Nume} ===");
                Console.WriteLine("1. Afiseaza Utilizatori");
                Console.WriteLine("2. Adauga Utilizator");
                Console.WriteLine("3. Sterge Utilizator");
                Console.WriteLine("4. Logare Utilizator");
                Console.WriteLine("5. Inapoi");
                Console.Write("Alege o optiune: ");
                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1":
                        managerUtilizatori.AfiseazaUtilizatori(banca.Utilizatori);
                        break;
                    case "2":
                        AdaugaUtilizator(managerUtilizatori, banca, utilizatori);
                        break;
                    case "3":
                        StergeUtilizator(managerUtilizatori, managerConturi, banca, utilizatori, conturi);
                        break;
                    case "4":
                        SelecteazaUtilizator(managerUtilizatori, managerConturi, banca, conturi);
                        break;
                    case "5":
                        return;
                    default:
                        Console.WriteLine("Optiune invalida.");
                        break;
                }
            }
        }

        static void AdaugaUtilizator(ManagerUtilizatori managerUtilizatori, Banca banca, List<Utilizator> utilizatori)
        {
            Console.Write("Introdu CNP-ul utilizatorului: ");
            string cnp = Console.ReadLine();

            if (Utilizator.ValidareCNP(cnp))
            {
                Console.WriteLine("CNP-ul introdus este invalid. Asigura-te ca are exact 13 cifre si ca nu contine litere.");
                Console.ReadLine();
                return;
            }

            if (Utilizator.ValidareExistaUtilizator(banca.Utilizatori, cnp))
            {
                Console.WriteLine("Utilizatorul exista deja!");
                Console.ReadLine();
                return;
            }

            string nume;

            while (true)
            {
                Console.Write("Introdu numele utilizatorului: ");
                nume = Console.ReadLine();

                if (!Utilizator.ValidareNumePrenumeParolaUtilizator(nume))
                {
                    break;
                }

                Console.WriteLine("Eroare: Numele nu poate fi gol sau doar spatii. Te rog sa introduci un nume valid.");
            }

            string prenume;

            while (true)
            {
                Console.Write("Introdu prenumele utilizatorului: ");
                prenume = Console.ReadLine();

                if (!Utilizator.ValidareNumePrenumeParolaUtilizator(prenume))
                {
                    break;
                }

                Console.WriteLine("Eroare: Preumele nu poate fi gol sau doar spatii. Te rog sa introduci un nume valid.");
            }

            string parola;

            while (true)
            {
                Console.Write("Introdu parola: ");
                parola = Console.ReadLine();

                if (!Utilizator.ValidareNumePrenumeParolaUtilizator(parola))
                {
                    break;
                }

                Console.WriteLine("Eroare: Parola nu poate fi goala sau doar spatii. Te rog sa introduci o parola valida.");
            }

            managerUtilizatori.AddUtilizator(utilizatori, banca, nume, prenume, cnp, parola);

            Console.WriteLine("Utilizator adaugat cu succes!");
            Console.ReadLine();
        }

        // ===================== ȘTERGE UTILIZATOR =====================
        static void StergeUtilizator(ManagerUtilizatori managerUtilizatori, ManagerConturi managerConturi, Banca banca, List<Utilizator> utilizatori, List<ContBancar> conturi)
        {
            Console.Write("Introdu CNP-ul utilizatorului de sters: ");
            string cnp = Console.ReadLine();

            if (Utilizator.ValidareCNP(cnp))
            {
                Console.WriteLine("CNP-ul introdus este invalid. Asigura-te ca are exact 13 cifre si ca nu contine litere.");
                Console.ReadLine();
                return;
            }

            var utilizator = utilizatori.FirstOrDefault(u => u.CNP == cnp);

            if (utilizator == null)
            {
                Console.WriteLine("Utilizatorul nu exista!");
                Console.ReadLine();
                return;
            }

            Console.Write("Introdu Parola utilizatorului: ");
            string parola = Console.ReadLine();


            if (utilizator.VerificarePinCriptat(parola))
            {
                Console.WriteLine("Parola gresita!");
                Console.ReadLine();
                return;
            }

            bool OK = managerUtilizatori.StergeUtilizator(banca, utilizatori, conturi, managerConturi, cnp);
            if (OK)
            {
                Console.WriteLine("Utilizator si conturile asociate au fost sterse cu succes!");
            }
            else
            {
                Console.WriteLine("Eroare la adaugarea utilizatorului!");
            }
            Console.ReadLine();
        }

        // ===================== MENIU CONTURI BANCARE =====================
        static void SelecteazaUtilizator(ManagerUtilizatori managerUtilizatori, ManagerConturi managerConturi, Banca banca, List<ContBancar> conturi)
        {
            Console.Write("Introdu CNP-ul utilizatorului: ");
            string cnp = Console.ReadLine();

            if (Utilizator.ValidareCNP(cnp))
            {
                Console.WriteLine("CNP-ul introdus este invalid. Asigura-te ca are exact 13 cifre si ca nu contine litere.");
                Console.ReadLine();
                return;
            }

            var utilizator = banca.Utilizatori.FirstOrDefault(u => u.CNP == cnp);
            if (utilizator == null)
            {
                Console.WriteLine("Utilizatorul nu exista!");
                Console.ReadLine();
                return;
            }

            Console.Write("Introdu parola: ");
            string parola = Console.ReadLine();

            if (utilizator.VerificarePinCriptat(parola))
            {
                Console.WriteLine("Parola incorecta!");
                Console.ReadLine();
                return;
            }

            MeniuConturi(managerConturi, banca, utilizator, conturi);
        }

        static void MeniuConturi(ManagerConturi managerConturi, Banca banca, Utilizator utilizator, List<ContBancar> conturi)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== MENIU CONTURI - ({utilizator.Nume} {utilizator.Prenume})  ===");
                Console.WriteLine("1. Afiseaza conturi");
                Console.WriteLine("2. Adauga cont");
                Console.WriteLine("3. Sterge cont");
                Console.WriteLine("4. Spre ATM");
                Console.WriteLine("5. Inapoi");
                Console.Write("Alege o optiune: ");
                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1":
                        managerConturi.AfiseazaConturi(utilizator.Conturi);
                        break;

                    case "2":
                        AdaugaCont(managerConturi, banca, utilizator, conturi);
                        break;

                    case "3":
                        StergeCont(managerConturi, utilizator, conturi);
                        break;

                    case "4":
                        SpreATM(managerConturi, utilizator, conturi);
                        break;

                    case "5":
                        return;

                    default:
                        Console.WriteLine("Optiune invalida.");
                        Console.ReadLine();
                        break;
                }
            }
        }
        static void AdaugaCont(ManagerConturi managerConturi, Banca banca, Utilizator utilizator, List<ContBancar> conturi)
        {
            if (utilizator.Conturi.Count >= 3)
            {
                Console.WriteLine("Utilizatorul nu poate avea mai mult de 3 conturi.");
                Console.ReadLine();
                return;
            }

            string nume;

            while (true)
            {
                Console.Write("Introdu numele contului: ");
                nume = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(nume))
                {
                    break;
                }

                Console.WriteLine("Eroare: Numele nu poate fi gol sau doar spatii. Te rog să introduci un nume valid.");
            }

            if (utilizator.Conturi.Any(c => c.NumeCont == nume))
            {
                Console.WriteLine("Numele contului exista deja!");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Alegeti tipul de moneda:");
            Console.WriteLine("1 - RON");
            Console.WriteLine("2 - EUR");
            Console.WriteLine("3 - USD");

            int optiune;
            while (true)
            {
                Console.Write("Alegeti: ");
                if (int.TryParse(Console.ReadLine(), out optiune) && Enum.IsDefined(typeof(monede), optiune))
                    break;

                Console.WriteLine("Optiune invalida. Introduceti 1 pentru RON, 2 pentru EUR, 3 pentru USD:");
            }

            monede moneda = (monede)optiune;

            if (utilizator.Conturi.Any(c => c.Moneda == moneda))
            {
                Console.WriteLine($"Utilizatorul are deja un cont in moneda {moneda}.");
                Console.ReadLine();
                return;
            }

            Console.Write("Depunere initiala: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal sold) || sold < 0)
            {
                Console.WriteLine("Soldul introdus nu este valid!");
                Console.ReadLine();
                return;
            }

            string pin;

            while (true)
            {
                Console.Write("Introdu PIN-ul noului cont: ");
                pin = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(pin) && pin.All(char.IsDigit))
                {
                    break;
                }

                Console.WriteLine("Eroare: PIN-ul nu poate fi gol, doar spatii sau sa contina litere. Te rog sa introduci un PIN valid.");
            }

            managerConturi.AddCont(conturi, utilizator, banca.IDBanca, moneda, nume, pin, sold);

            Console.WriteLine("Cont creat cu succes!");
            Console.ReadLine();
        }

        static void StergeCont(ManagerConturi managerConturi, Utilizator utilizator, List<ContBancar> conturi)
        {
            if (utilizator.Conturi == null || utilizator.Conturi.Count == 0)
            {
                Console.WriteLine("Utilizatorul nu are conturi bancare.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("=== Selecteaza un cont pentru a-l sterge ===");

            // Afișează lista conturilor disponibile
            for (int i = 0; i < utilizator.Conturi.Count; i++)
            {
                var contSelectat = utilizator.Conturi[i];
                Console.WriteLine($"{i + 1}. {contSelectat.NumeCont} (ID/IBAN: {contSelectat.ID}, Moneda: {contSelectat.Moneda}, Sold: {contSelectat.Sold})");
            }

            // Permite utilizatorului să aleagă un cont
            Console.Write($"Alege un cont (1 - {utilizator.Conturi.Count}): ");
            if (!int.TryParse(Console.ReadLine(), out int indexSelectat) ||
                indexSelectat < 1 || indexSelectat > utilizator.Conturi.Count)
            {
                Console.WriteLine("Optiune invalida!");
                Console.ReadLine();
                return;
            }

            var cont = utilizator.Conturi[indexSelectat - 1];

            Console.Write("Introdu PIN-ul contului: ");
            string pin = Console.ReadLine();

            if (cont.VerificaPin(pin))
            {
                Console.WriteLine("PIN gresit!");
                Console.ReadLine();
                return;
            }

            managerConturi.StergeCont(utilizator, conturi, cont.ID);

            Console.WriteLine("Contul a fost sters cu succes!");
            Console.ReadLine();
        }

        static void SpreATM(ManagerConturi managerConturi, Utilizator utilizator, List<ContBancar> conturi)
        {
            if (utilizator.Conturi == null || utilizator.Conturi.Count == 0)
            {
                Console.WriteLine("Utilizatorul nu are conturi bancare.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("=== Selecteaza un cont ===");
            for (int i = 0; i < utilizator.Conturi.Count; i++)
            {
                var contSelectat = utilizator.Conturi[i];
                Console.WriteLine($"{i + 1}. {contSelectat.NumeCont} (ID/IBAN: {contSelectat.ID}, Moneda: {contSelectat.Moneda}, Sold: {contSelectat.Sold})");
            }

            Console.Write($"Alege un cont (1 - {utilizator.Conturi.Count}): ");

            if (!int.TryParse(Console.ReadLine(), out int indexSelectat) ||
                indexSelectat < 1 || indexSelectat > utilizator.Conturi.Count)
            {
                Console.WriteLine("Optiune invalida!");
                Console.ReadLine();
                return;
            }

            var cont = utilizator.Conturi[indexSelectat - 1];

            Console.Write("Introdu PIN-ul contului: ");
            string pin = Console.ReadLine();

            if (cont.PinCriptat != Securitate.CriptarePin(pin))
            {
                Console.WriteLine("PIN gresit!");
                Console.ReadLine();
                return;
            }

            MeniuATM(managerConturi, utilizator, cont, conturi);
        }

        static void MeniuATM (ManagerConturi managerConturi, Utilizator utilizator, ContBancar cont, List<ContBancar> conturi)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"=== ATM - {cont.ID} ({cont.NumeCont}) ===");
                Console.WriteLine($"1. Vizualizare Sold ({cont.Moneda})");
                Console.WriteLine("2. Depunere Bani");
                Console.WriteLine("3. Retragere Bani");
                Console.WriteLine("4. Transfer Bani");
                Console.WriteLine("5. Schimb Valutar");
                Console.WriteLine("6. Inchide sesiunea");
                Console.Write("Alege o optiune: ");

                string optiune = Console.ReadLine();
                switch (optiune)
                {
                    case "1":
                        AfisareSold(cont);
                        break;

                    case "2":
                        DepunereBani(managerConturi, cont, conturi);
                        break;

                    case "3":
                        RetragereBani(managerConturi, cont, conturi);
                        break;

                    case "4":
                        TransferBani(managerConturi, cont, conturi);
                        break;

                    case "5":
                        SchimbValutar(managerConturi, utilizator, cont, conturi);
                        break;

                    case "6":
                        return;

                    default:
                        Console.WriteLine("Optiune invalida.");
                        Console.ReadLine();
                        break;
                }
            }
        }
        static void AfisareSold(ContBancar cont)
        {
            Console.WriteLine($"\nSold curent: {cont.Sold} {cont.Moneda}");
            Console.ReadLine();
        }
        static void DepunereBani (ManagerConturi managerConturi, ContBancar cont, List<ContBancar> conturi)
        {
            Console.Write("\nIntrodu suma de depus: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal sumaDepunere) && sumaDepunere > 0)
            {
                cont.Depunere(sumaDepunere);
                Console.WriteLine($"Depunere reusita! Sold nou: {cont.Sold} {cont.Moneda}");
                managerConturi.SalveazaConturi(conturi);
            }
            else
            {
                Console.WriteLine("Suma introdusa nu este valida!");
            }
            Console.ReadLine();
        }
        static void RetragereBani(ManagerConturi managerConturi, ContBancar cont, List<ContBancar> conturi)
        {
            Console.Write("\nIntrodu suma de retras: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal sumaRetragere) && sumaRetragere > 0)
            {
                if (cont.Retragere(sumaRetragere))
                {
                    Console.WriteLine($"Retragere reusita! Sold nou: {cont.Sold} {cont.Moneda}");
                    managerConturi.SalveazaConturi(conturi);
                }
                else
                {
                    Console.WriteLine("Fonduri insuficiente sau limita depasita!");
                }
            }
            else
            {
                Console.WriteLine("Suma introdusa nu este valida!");
            }
            Console.ReadLine();
        }
        static void TransferBani(ManagerConturi managerConturi, ContBancar cont, List<ContBancar> conturi)
        {
            Console.Write("\nIntrodu ID/IBAN-ul contului destinatar: ");
            string idDestinatar = Console.ReadLine();

            var contDestinatar = conturi.FirstOrDefault(c => c.ID == idDestinatar);

            if (contDestinatar == null)
            {
                Console.WriteLine("Cont destinatar inexistent.");
            }
            else if (cont.Moneda != contDestinatar.Moneda)
            {
                Console.WriteLine($"Moneda diferita! Va fi efectuata o conversie valutara: {cont.Moneda} -> {contDestinatar.Moneda} urmand cursul valutar : {CursValutar.SchimbValutar(cont.Moneda.ToString(), contDestinatar.Moneda.ToString())}");
                Console.Write("Introdu suma de transferat: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal sumaTransfer) && sumaTransfer > 0)
                {
                    if (cont.Transfer(contDestinatar, sumaTransfer))
                    {
                        Console.WriteLine($"Transfer reusit! Sold nou: {cont.Sold} {cont.Moneda}");
                        managerConturi.SalveazaConturi(conturi);
                    }
                    else
                    {
                        Console.WriteLine("Fonduri insuficiente pentru transfer!");
                    }
                }
                else
                {
                    Console.WriteLine("Suma introdusa nu este valida!");
                }
            }
            else if (cont.Moneda == contDestinatar.Moneda)
            {
                Console.Write("Introdu suma de transferat: ");
                if (decimal.TryParse(Console.ReadLine(), out decimal sumaTransfer) && sumaTransfer > 0)
                {
                    if (cont.Transfer(contDestinatar, sumaTransfer))
                    {
                        Console.WriteLine($"Transfer reusit! Sold nou: {cont.Sold} {cont.Moneda}");
                        managerConturi.SalveazaConturi(conturi);
                    }
                    else
                    {
                        Console.WriteLine("Fonduri insuficiente pentru transfer!");
                    }
                }
                else
                {
                    Console.WriteLine("Suma introdusa nu este valida!");
                }
            }
            Console.ReadLine();
        }
        static void SchimbValutar(ManagerConturi managerConturi, Utilizator utilizator, ContBancar cont, List<ContBancar> conturi)
        {
            var alteConturi = utilizator.Conturi.Where(c => c != cont).ToList();

            if (alteConturi.Count == 0)
            {
                Console.WriteLine("Nu exista alte conturi pentru schimb valutar.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("=== Selecteaza contul destinatie pentru schimb valutar ===");
            for (int i = 0; i < alteConturi.Count; i++)
            {
                var c = alteConturi[i];
                Console.WriteLine($"{i + 1}. {c.NumeCont} (ID: {c.ID}, Moneda: {c.Moneda}, Sold: {c.Sold})");
            }

            Console.Write($"Alege contul destinatie (1 - {alteConturi.Count}): ");
            if (!int.TryParse(Console.ReadLine(), out int idxDest) || idxDest < 1 || idxDest > alteConturi.Count)
            {
                Console.WriteLine("Optiune invalida!");
                Console.ReadLine();
                return;
            }

            var contDest = alteConturi[idxDest - 1];

            Console.WriteLine($"Va fi efectuata o conversie valutara: {cont.Moneda} -> {contDest.Moneda} urmand cursul valutar : {CursValutar.SchimbValutar(cont.Moneda.ToString(), contDest.Moneda.ToString())}");
            Console.Write("Introdu suma de schimbat: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal suma) || suma <= 0)
            {
                Console.WriteLine("Suma invalida.");
                Console.ReadLine();
                return;
            }

            bool rezultat = cont.Transfer(contDest, suma);
            if (rezultat)
            {
                Console.WriteLine("Transferul valutar a fost efectuat cu succes.");
            }
            else
            {
                Console.WriteLine("Transfer esuat. Verifica suma sau soldul disponibil.");
            }
            Console.ReadLine();
        }
    }
}
