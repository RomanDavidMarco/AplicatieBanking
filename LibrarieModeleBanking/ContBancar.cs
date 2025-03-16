using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModeleBanking
{
    public class ContBancar
    {
        public string NumarCont { get; private set; }
        public decimal Sold { get; private set; }
        private string PinCriptat;
        private decimal LimitaRetragereZilnica = 5000m;

        public ContBancar(string numarCont, decimal soldInitial, string pin)
        {
            NumarCont = numarCont;
            Sold = soldInitial;
            PinCriptat = Securitate.CriptarePin(pin);
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
    }
}
