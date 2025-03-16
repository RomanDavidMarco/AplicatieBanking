using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModeleBanking
{
    public class Utilizator
    {
        private const char SEPARATOR_PRINCIPAL_FISIER = ';';
        public string Nume { get; private set; }
        public string Prenume { get; private set; }
        public string CNP { get; private set; }
        public List<ContBancar> Conturi { get; private set; }

        public Utilizator(string nume, string prenume, string cnp)
        {
            Nume = nume;
            Prenume = prenume;
            CNP = cnp;
            Conturi = new List<ContBancar>();
        }
        
        public Utilizator (string dateFisier)
        {
            string[] date = dateFisier.Split(SEPARATOR_PRINCIPAL_FISIER);
            Nume = date[0];
            Prenume = date[1];
            CNP = date[2];
        }

        public bool AdaugaCont(ContBancar cont)
        {
            if (Conturi.Find(lbda => lbda.Moneda == cont.Moneda) == null)
            {
                Conturi.Add(cont);
                return true;
            }
            return false;
        }

        public string ConversieLaSir_PentruFisier()
        {
            string obiectUtilizatorPentruFisier = string.Format("{1}{0}{2}{0}{3}{0}",
                 SEPARATOR_PRINCIPAL_FISIER,
                 (Nume ?? " NECUNOSCUT "),
                 (Prenume ?? " NECUNOSCUT "),
                 CNP.ToString());

            return obiectUtilizatorPentruFisier;
        }
    }
}
