using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModeleBanking
{
    public class Utilizator
    {
        private const char SEPARATOR_PRINCIPAL_FISIER = '|';
        private const char SEPARATOR_ID = ';';
        public string NumeBanca {  get; private set; }
        private const int NUMEBANCA = 0;
        public string Nume { get; private set; }
        private const int NUME = 1;
        public string Prenume { get; private set; }
        private const int PRENUME = 2;
        public string CNP { get; private set; }
        private const int CNPVAL = 3;
        public List<ContBancar> Conturi { get; private set; }
        public string parolaCriptata;
        private const int PAROLACRIPTATA = 4;


        public Utilizator(string banca, string nume, string prenume, string cnp, string parola)
        {
            NumeBanca = banca;
            Nume = nume;
            Prenume = prenume;
            CNP = cnp;
            Conturi = new List<ContBancar>();
            parolaCriptata = Securitate.CriptarePin(parola);
        }
        
        public Utilizator (string dateFisier)
        {
            string[] date = dateFisier.Split(SEPARATOR_PRINCIPAL_FISIER);

            Conturi = new List<ContBancar>();
            NumeBanca = date[NUMEBANCA];
            Nume = date[NUME];
            Prenume = date[PRENUME];
            CNP = date[CNPVAL];
            parolaCriptata = date[PAROLACRIPTATA];
        }

        public void AdaugaCont(ContBancar cont)
        {
                Conturi.Add(cont);
        }

        public List<string> GetIdConturi()
        {
            return Conturi.Select(u => u.ID).ToList();
        }

        public string ConversieLaSir_PentruFisier()
        {
            string obiectUtilizatorPentruFisier = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}",
                 SEPARATOR_PRINCIPAL_FISIER,
                 (NumeBanca ?? " NECUNOSCUT "),
                 (Nume ?? " NECUNOSCUT "),
                 (Prenume ?? " NECUNOSCUT "),
                 CNP.ToString(),
                 (parolaCriptata ?? "NECUNOSCUT"));
            return obiectUtilizatorPentruFisier;
        }
    }
}
