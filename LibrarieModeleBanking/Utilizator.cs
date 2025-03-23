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
        public string Nume { get; private set; }
        private const int NUME = 0;
        public string Prenume { get; private set; }
        private const int PRENUME = 1;
        public string CNP { get; private set; }
        private const int CNPVAL = 2;
        public List<ContBancar> Conturi { get; private set; }
        public List<string> IDConturi { get; private set; }
        private const int IDCONTURI = 3;
        public string parolaCriptata;
        private const int PAROLACRIPTATA = 4;


        public Utilizator(string nume, string prenume, string cnp, string parola)
        {
            Nume = nume;
            Prenume = prenume;
            CNP = cnp;
            Conturi = new List<ContBancar>();
            IDConturi = new List<string>();
            parolaCriptata = Securitate.CriptarePin(parola);
        }
        
        public Utilizator (string dateFisier)
        {
            string[] date = dateFisier.Split(SEPARATOR_PRINCIPAL_FISIER);

            Conturi = new List<ContBancar>();

            Nume = date[NUME];
            Prenume = date[PRENUME];
            CNP = date[CNPVAL];
            IDConturi = date[IDCONTURI].Split(SEPARATOR_ID).ToList();
            parolaCriptata = date[PAROLACRIPTATA];
        }

        public bool AdaugaCont(ContBancar cont)
        {
            try
            {
                
                if (Conturi.Count >= 3)
                {
                    throw new Exception("Utilizatorul nu poate avea mai mult de 3 conturi.");
                }
                
                if (Conturi.Find(lbda => lbda.Moneda == cont.Moneda) != null)
                {
                    throw new Exception("Contul cu aceasta moneda exista deja.");
                }
                
                Conturi.Add(cont);

                return true;
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Eroare la adaugarea contului: {ex.Message}");
                return false; 
            }
        }

        public List<string> GetIdConturi()
        {
            return Conturi.Select(u => u.ID).ToList();
        }

        public string ConversieLaSir_PentruFisier()
        {
            string idConturiStr = string.Join(SEPARATOR_ID.ToString(), Conturi.Select(cont => cont.ID));

            string obiectUtilizatorPentruFisier = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}",
                 SEPARATOR_PRINCIPAL_FISIER,
                 (Nume ?? " NECUNOSCUT "),
                 (Prenume ?? " NECUNOSCUT "),
                 CNP.ToString(),
                 idConturiStr,
                 (parolaCriptata ?? "NECUNOSCUT"));
            return obiectUtilizatorPentruFisier;
        }
    }
}
