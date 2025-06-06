﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilitareBanking;

namespace LibrarieModeleBanking
{
    public enum monede
    {
        RON = 1,
        EUR = 2,
        USD = 3,
    }
    public class ContBancar
    {
        private const char SEPARATOR_PRINCIPAL_FISIER = '|';
        public string CNP {  get; private set; }
        private const int CNPVAL = 0;
        public string ID { get; private set; }
        private const int IDVAL = 1;
        public string NumarCont { get; private set; }
        private const int NUMARCONT = 2;
        public monede Moneda { get; private set; }
        private const int MONEDA = 3;
        public string NumeCont { get; private set; }
        private const int NUMECONT = 4;
        public decimal Sold { get; private set; }
        private const int SOLD = 5;
        public string PinCriptat;
        private const int PINCRIPTAT = 6;
        private decimal LimitaRetragereZilnica = 5000m;

        public ContBancar(string cnp, string idBanca, List<ContBancar> conturiExistente, monede moneda, string nume, string pin, decimal soldInitial)
        {
            CNP = cnp;
            ID = $"{idBanca}{moneda}{GenereazaIDRandom()}";
            NumarCont = GenereazaNumarCont(conturiExistente);
            Moneda = moneda;
            NumeCont = nume.Trim();
            Sold = soldInitial;
            PinCriptat = Securitate.CriptarePin(pin);
        }

        public ContBancar(string dateFisier)
        {
            try
            {
                string[] date = dateFisier.Split(SEPARATOR_PRINCIPAL_FISIER);
                CNP = date[CNPVAL];
                ID = date[IDVAL];
                NumarCont = date[NUMARCONT];
                Moneda = (monede)Enum.Parse(typeof(monede), date[MONEDA]);
                NumeCont = date[NUMECONT];
                Sold = decimal.Parse(date[SOLD]);
                PinCriptat = date[PINCRIPTAT];
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Eroare la citirea datelor: {ex.Message}");
            }
        }

        private string GenereazaNumarCont(List<ContBancar> conturiExistente)
        {
            var numereExistente = conturiExistente
                .Select(c => int.TryParse(c.NumarCont, out int n) ? n : -1)
                .Where(n => n > 0)
                .ToHashSet();

            for (int i = 1; i <= 3; i++)
            {
                if (!numereExistente.Contains(i))
                {
                    return i.ToString();
                }
            }

            return "0";
        }

        private string GenereazaIDRandom()
        {
            Random rnd = new Random();
            return rnd.Next(100000, 999999).ToString();
        }

        public bool VerificaPin(string pin)
        {
            return Securitate.VerificaPin(pin, PinCriptat);
        }

        public bool Retragere(decimal suma)
        {
            if (suma <= Sold && suma <= LimitaRetragereZilnica)
            {
                Sold -= suma;
                Logger.AddLog($"Tranzactie de retragere a sumei {suma} {Moneda} din contul {ID} efectuata cu succes!");
                return true;
            }
            Logger.AddLog($"Tranzactie de retragere a sumei {suma} {Moneda} din contul {ID} esuata!");
            return false;
        }

        public void Depunere(decimal suma)
        {
            Sold += suma;
            Logger.AddLog($"Tranzactie de depunere a sumei {suma} {Moneda} in contul {ID} efectuata cu succes!");
        }

        public bool Transfer(ContBancar destinatie, decimal suma)
        {
            if (Moneda == destinatie.Moneda)
            {
                if (suma <= Sold)
                {
                    Sold -= suma;
                    destinatie.Depunere(suma);
                    Logger.AddLog($"Tranzactie de transfer a sumei {suma} {Moneda} din contul {ID} in contul {destinatie.ID} efectuata cu succes!");
                    return true;
                }
                Logger.AddLog($"Tranzactie de transfer a sumei {suma} {Moneda} din contul {ID} in contul {destinatie.ID} esuata!");
            }
            else
            {
                decimal rataConversie = CursValutar.SchimbValutar(Moneda.ToString(), destinatie.Moneda.ToString());
                decimal sumaSchimbata = suma * rataConversie;
                sumaSchimbata = Math.Round(sumaSchimbata, 2);
                if (suma <= Sold)
                {
                    Sold -= suma;
                    destinatie.Depunere(sumaSchimbata);
                    Logger.AddLog($"Tranzactie de transfer valutar {Moneda} -> {destinatie.Moneda} a sumei {suma} {Moneda} -> {sumaSchimbata} {destinatie.Moneda} din contul {ID} in contul {destinatie.ID} urmand cursul valutar {rataConversie} efectuata cu succes!");
                    return true;
                }
                Logger.AddLog($"Tranzactie de transfer valutar {Moneda} -> {destinatie.Moneda} a sumei {suma} {Moneda} -> {sumaSchimbata} {destinatie.Moneda} din contul {ID} in contul {destinatie.ID} urmand cursul valutar {rataConversie} esuata!");
            }
            return false;
        }

        public bool ModificaContBancar(string nouNume, string pinVechi, string pinNou)
        {
            bool modificat = false;

            if (!string.IsNullOrWhiteSpace(nouNume) && nouNume.Trim() != NumeCont)
            {
                NumeCont = nouNume.Trim();
                Logger.AddLog($"Numele contului {ID} a fost modificat în: {NumeCont}");
                modificat = true;
            }

            if (!string.IsNullOrWhiteSpace(pinVechi) && !string.IsNullOrWhiteSpace(pinNou) &&
                Securitate.VerificaPin(pinVechi, PinCriptat) && pinNou.All(char.IsDigit))
            {
                PinCriptat = Securitate.CriptarePin(pinNou);
                Logger.AddLog($"PIN-ul contului {ID} a fost modificat");
                modificat = true;
            }

            return modificat;
        }

        public string ConversieLaSir_PentruFisier()
        {
            string obiectContPentruFisier = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}{7}{0}",
                 SEPARATOR_PRINCIPAL_FISIER,
                 CNP,
                 ID,
                 NumarCont,
                 Moneda.ToString(),
                 NumeCont,
                 Sold.ToString(),
                 PinCriptat);

            return obiectContPentruFisier;
        }
    }
}
