using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarieModeleBanking;

namespace StocareDateBanking
{
    public class ManagerConturi
    {
        private string numeFisier;

        public ManagerConturi(string numeFisier)
        {
            this.numeFisier = numeFisier;
            Stream streamFisierText = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisierText.Close();
        }

        public void AddCont(List<ContBancar> conturi, Utilizator utilizator, string idBanca,monede moneda, string numeCont, string pin, decimal soldInitial)
        {
            ContBancar contNou = new ContBancar(utilizator.CNP, idBanca, utilizator.Conturi, moneda, numeCont, pin, soldInitial);

            conturi.Add(contNou);
            utilizator.AdaugaCont(contNou);

            using (StreamWriter streamWriterFisierText = new StreamWriter(numeFisier, true))
            {
                streamWriterFisierText.WriteLine(contNou.ConversieLaSir_PentruFisier());
            }
        }

        public bool StergeCont(Utilizator utilizator, List<ContBancar> conturi, string idCont)
        {
            ContBancar contDeSters = conturi.FirstOrDefault(c => c.ID == idCont);

            if (contDeSters != null)
            {
                utilizator.Conturi.Remove(contDeSters);

                conturi.Remove(contDeSters);

                SalveazaConturi(conturi);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void AfiseazaConturi(List<ContBancar> conturi)
        {
            Console.WriteLine("=== Lista Conturi ===");
            foreach (var cont in conturi)
            {
                Console.WriteLine($"Cont: {cont.NumarCont} | ID (IBAN): {cont.ID} | Nume Cont: {cont.NumeCont} | Moneda: {cont.Moneda} | Sold: {cont.Sold} {cont.Moneda}");
            }
            Console.ReadLine();
        }

        public List<ContBancar> IncarcaConturi()
        {
            List<ContBancar> conturi = new List<ContBancar>();

            if (File.Exists(numeFisier))
            {
                string[] linii = File.ReadAllLines(numeFisier);
                foreach (string linie in linii)
                {
                    if (!string.IsNullOrWhiteSpace(linie))
                    {
                        conturi.Add(new ContBancar(linie));
                    }
                }
            }

            return conturi;
        }
        public void SalveazaConturi(List<ContBancar> conturi)
        {
            using (StreamWriter writer = new StreamWriter(numeFisier, false)) //rescrie (dupa incarcare)
            {
                foreach (var cont in conturi)
                {
                    writer.WriteLine(cont.ConversieLaSir_PentruFisier());
                }
            }
        }
    }
}
