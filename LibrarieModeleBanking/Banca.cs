using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibrarieModeleBanking
{
    public class Banca
    {
        public string Nume { get; private set; }
        private List<Utilizator> utilizatori;

        public Banca(string nume)
        {
            Nume = nume;
            utilizatori = new List<Utilizator>();
        }

        public void AdaugaUtilizator(Utilizator utilizator)
        {
            utilizatori.Add(utilizator);
        }

        public ContBancar CautaCont(string numarCont)
        {
            return utilizatori.SelectMany(u => u.Conturi)
                              .FirstOrDefault(c => c.NumarCont == numarCont);
        }
    }
}
