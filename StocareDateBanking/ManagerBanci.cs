using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarieModeleBanking;

namespace StocareDateBanking
{
    public class ManagerBanci
    {
        private string numeFisier;

        public ManagerBanci(string numeFisier)
        {
            this.numeFisier = numeFisier;
            Stream streamFisierText = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisierText.Close();
        }

        public void AddBanca(List<Banca> banci, string numeBanca, string initiale)
        {
            initiale = initiale.ToUpper();

            Banca bancaNoua = new Banca(numeBanca, initiale);
            banci.Add(bancaNoua);

            using (StreamWriter streamWriterFisierText = new StreamWriter(numeFisier, true))
            {
                streamWriterFisierText.WriteLine(bancaNoua.ConversieLaSir_PentruFisier());
            }
        }

        public bool StergeBanca(List<Banca> banci, List<Utilizator> utilizatori, List<ContBancar> conturi, ManagerConturi managerConturi, ManagerUtilizatori managerUtilizatori, string idBanca)
        {
            Banca bancaDeSters = banci.FirstOrDefault(b => b.IDBanca == idBanca);

            if (bancaDeSters != null)
            {
                List<string> cnpUtilizatoriDeSters = bancaDeSters.GetCnpUtilizatori();

                foreach (var cnp in cnpUtilizatoriDeSters)
                {
                    Utilizator utilizatorDeSters = utilizatori.FirstOrDefault(u => u.CNP == cnp);

                    if (utilizatorDeSters != null)
                    {
                        conturi.RemoveAll(cont => utilizatorDeSters.Conturi.Contains(cont));

                        utilizatori.Remove(utilizatorDeSters);

                        Console.WriteLine($"Utilizatorul cu CNP {cnp} si conturile sale au fost șterse cu succes!");
                    }
                }

                banci.Remove(bancaDeSters);
                SalveazaBanci(banci);

                managerConturi.SalveazaConturi(conturi);
                managerUtilizatori.SalveazaUtilizatori(utilizatori);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void AfiseazaBanci(List<Banca> banci)
        {
            Console.WriteLine("=== Lista Banci ===");
            foreach (var banca in banci)
            {
                Console.WriteLine($"Banca: {banca.Nume} | ID Banca: {banca.IDBanca}");
            }
            Console.ReadLine();
        }

        public List<Banca> IncarcaBanci()
        {
            List<Banca> banci = new List<Banca>();

            if (File.Exists(numeFisier))
            {
                string[] linii = File.ReadAllLines(numeFisier);
                foreach (string linie in linii)
                {
                    if (!string.IsNullOrWhiteSpace(linie))
                    {
                        banci.Add(new Banca(linie));
                    }
                }
            }

            return banci;
        }

        public void SalveazaBanci(List<Banca> banci)
        {
            using (StreamWriter writer = new StreamWriter(numeFisier, false)) // rescrie (dupa incarcare)
            {
                foreach (var banca in banci)
                {
                    writer.WriteLine(banca.ConversieLaSir_PentruFisier());
                }
            }
        }
    }
}
