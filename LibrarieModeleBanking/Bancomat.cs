using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModeleBanking
{
    public class Bancomat
    {
        public string numeBanca { get; set; }
        public decimal SoldBancomat { get; private set; }

        public Bancomat(string numeBanca, decimal soldInitial)
        {
            this.numeBanca = numeBanca;
            SoldBancomat = soldInitial;
        }

        public bool RetragereBani(Utilizator utilizator, string numarCont, decimal suma)
        {
            var cont = utilizator.Conturi.FirstOrDefault(c => c.NumarCont == numarCont);
            if (cont != null && cont.Retragere(suma) && SoldBancomat >= suma)
            {
                SoldBancomat -= suma;
                return true;
            }
            return false;
        }

        public bool DepunereBani(Utilizator utilizator, string numarCont, decimal suma)
        {
            var cont = utilizator.Conturi.FirstOrDefault(c => c.NumarCont == numarCont);
            if (cont != null && suma > 0)
            {
                cont.Depunere(suma);
                SoldBancomat += suma;
                return true;
            }
            return false;
        }
    }
}
