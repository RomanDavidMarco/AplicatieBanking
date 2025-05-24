using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarieModeleBanking;
using UtilitareBanking;

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
            Nume = nume.Trim();
            Prenume = prenume.Trim();
            CNP = cnp.Trim();
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

        public static bool ValidareCNP(string cnp)
        {
            if (string.IsNullOrEmpty(cnp) || cnp.Length != 13 || !cnp.All(char.IsDigit))
            {
                return true;
            }

            string constanta = "279146358279";
            int suma = 0;

            for (int i = 0; i < 12; i++)
            {
                suma += (cnp[i] - '0') * (constanta[i] - '0');
            }

            int rest = suma % 11;
            int cifraControl = (rest < 10) ? rest : 1;

            // verificam daca cifra de control calculata coincide cu ultima cifra (a 13-a)
            if ((cnp[12] - '0') != cifraControl)
            {
                return true;
            }

            return false;
        }

        public static bool ValidareExistaUtilizator(List<Utilizator> utilizatori, string cnp)
        {
            if (utilizatori != null && utilizatori.Any(u => u.CNP == cnp))
            {
                return true;
            }
            return false;
        }

        public static bool ValidareNumePrenumeParolaUtilizator(string nume)
        {
            return string.IsNullOrWhiteSpace(nume);
        }

        public bool VerificarePinCriptat(string parola)
        {
            if (parolaCriptata != Securitate.CriptarePin(parola))
            {
                return true;
            }
            return false;
        }

        public bool ModificaUtilizator(string nouNume, string nouPrenume, string vecheParola, string nouParola)
        {
            bool modificat = false;

            if (!string.IsNullOrWhiteSpace(nouNume) && nouNume.Trim() != Nume)
            {
                Nume = nouNume.Trim();
                Logger.AddLog($"Numele utilizatorului {CNP} - {NumeBanca} a fost modificat în: {Nume}");
                modificat = true;
            }

            if (!string.IsNullOrWhiteSpace(nouPrenume) && nouPrenume.Trim() != Prenume)
            {
                Prenume = nouPrenume.Trim();
                Logger.AddLog($"Prenumele utilizatorului {CNP} - {NumeBanca} a fost modificat în: {Prenume}");
                modificat = true;
            }

            // Verificăm parola
            if (!string.IsNullOrWhiteSpace(vecheParola) && !string.IsNullOrWhiteSpace(nouParola))
            {
                if (Securitate.VerificaPin(vecheParola, parolaCriptata) && nouParola != vecheParola)
                {
                    parolaCriptata = Securitate.CriptarePin(nouParola);
                    Logger.AddLog($"Parola utilizatorului {CNP} - {NumeBanca} a fost modificata");
                    modificat = true;
                }
            }

            return modificat;
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
