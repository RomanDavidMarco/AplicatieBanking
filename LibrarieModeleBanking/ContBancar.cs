using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public string ID { get; private set; }
        private const int IDVAL = 0;
        public string NumarCont { get; private set; }
        private const int NUMARCONT = 1;
        public monede Moneda { get; private set; }
        private const int MONEDA = 2;
        public string NumeCont { get; private set; }
        private const int NUMECONT = 3;
        public decimal Sold { get; private set; }
        private const int SOLD = 4;
        public string PinCriptat;
        private const int PINCRIPTAT = 5;
        private decimal LimitaRetragereZilnica = 5000m;

        public ContBancar(string idBanca, List<ContBancar> conturiExistente, monede moneda, string nume, string pin, decimal soldInitial)
        {
            ID = $"{idBanca}{moneda}{GenereazaIDRandom()}";
            NumarCont = GenereazaNumarCont(conturiExistente);
            Moneda = moneda;
            NumeCont = nume;
            Sold = soldInitial;
            PinCriptat = Securitate.CriptarePin(pin);
        }

        public ContBancar(string dateFisier)
        {
            try
            {
                string[] date = dateFisier.Split(SEPARATOR_PRINCIPAL_FISIER);
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
            int conturiExistenteNr = conturiExistente?.Count ?? 0;
            if (conturiExistenteNr >= 3)
            {
                throw new Exception("Utilizatorul nu poate avea mai mult de 3 conturi.");
            }
            return (conturiExistenteNr + 1).ToString();
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
                return true;
            }
            return false;
        }

        public void Depunere(decimal suma)
        {
            Sold += suma;
        }

        public bool Transfer(ContBancar destinatie, decimal suma)
        {
            if (suma <= Sold)
            {
                Sold -= suma;
                destinatie.Depunere(suma);
                return true;
            }
            return false;
        }

        public string ConversieLaSir_PentruFisier()
        {
            string obiectContPentruFisier = string.Format("{1}{0}{2}{0}{3}{0}{4}{0}{5}{0}{6}{0}",
                 SEPARATOR_PRINCIPAL_FISIER,
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
