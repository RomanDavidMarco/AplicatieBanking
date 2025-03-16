using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModeleBanking
{
    public class Bancomat
    {
        private Banca banca;
        public decimal SoldBancomat { get; private set; }

        public Bancomat(Banca banca, decimal soldInitial)
        {
            this.banca = banca;
            SoldBancomat = soldInitial;
        }

        public bool RetragereBani(string numarCont, decimal suma)
        {
            var cont = banca.CautaCont(numarCont);
            if (cont != null && cont.Retragere(suma) && SoldBancomat >= suma)
            {
                SoldBancomat -= suma;
                return true;
            }
            return false;
        }
    }
}
