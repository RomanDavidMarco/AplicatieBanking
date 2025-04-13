using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModeleBanking
{
    public class Banca
    {
        private const string prefixTara = "RO";
        private const char SEPARATOR_ID = ';';
        private const char SEPARATOR_PRINCIPAL_FISIER = '|';
        public string Nume { get; private set; }
        private const int NUME = 0; 
        public string IDBanca { get; private set; }
        private const int IDBANCA = 1;
        public List<Utilizator> Utilizatori;
        //public Bancomat Bancomat { get; set; }

        public Banca(string nume, string initiale)
        {
            IDBanca = $"{prefixTara}{initiale}";
            Nume = nume;
            Utilizatori = new List<Utilizator>();
            //Bancomat = new Bancomat(Nume, 10000m);
        }

        public Banca (string dateFisier)
        {
            string[] date = dateFisier.Split(SEPARATOR_PRINCIPAL_FISIER);

            Utilizatori = new List<Utilizator>();
            //Bancomat = new Bancomat(Nume, 10000m);

            Nume = date[NUME];
            IDBanca= date[IDBANCA];
        }

        public void AdaugaUtilizator(Utilizator utilizator)
        {
            Utilizatori.Add(utilizator);
        }

        public ContBancar CautaCont(string numarCont)
        {
            return Utilizatori.SelectMany(u => u.Conturi)
                              .FirstOrDefault(c => c.NumarCont == numarCont);
        }

        public List<string> GetCnpUtilizatori()
        {
            return Utilizatori.Select(u => u.CNP).ToList();
        }

        public static bool ValidareNumeBanca(string nume)
        {
            return string.IsNullOrWhiteSpace(nume);
        }

        public static bool ValidarePrefixBanca(string initiale)
        {
            if (initiale.Length != 4 || !initiale.All(char.IsLetter))
            {
                return true;
            }
            return false;
        }

        public string ConversieLaSir_PentruFisier()
        {
            string obiectUtilizatorPentruFisier = string.Format("{1}{0}{2}{0}",
                 SEPARATOR_PRINCIPAL_FISIER,
                 (Nume ?? " NECUNOSCUT "),
                 (IDBanca ?? " NECUNOSCUT "));

            return obiectUtilizatorPentruFisier;
        }
    }
}
