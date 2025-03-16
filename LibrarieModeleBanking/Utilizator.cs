using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModeleBanking
{
    public class Utilizator
    {
        public string Nume { get; private set; }
        public List<ContBancar> Conturi { get; private set; }

        public Utilizator(string nume)
        {
            Nume = nume;
            Conturi = new List<ContBancar>();
        }

        public void AdaugaCont(ContBancar cont)
        {
            Conturi.Add(cont);
        }
    }
}
