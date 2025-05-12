using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarieModeleBanking;
using UtilitareBanking;

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

            Logger.AddLog($"Banca adaugata: {bancaNoua.Nume} cu ID: {bancaNoua.IDBanca}");
        }

        public bool StergeBanca(List<Banca> banci, List<Utilizator> utilizatori, List<ContBancar> conturi, ManagerConturi managerConturi, ManagerUtilizatori managerUtilizatori, string idBanca)
        {
            Banca bancaDeSters = banci.FirstOrDefault(b => b.IDBanca.ToUpper() == idBanca || b.Nume.ToUpper() == idBanca);

            if (bancaDeSters != null)
            {
                List<string> cnpUtilizatoriDeSters = bancaDeSters.GetCnpUtilizatori();

                foreach (var cnp in cnpUtilizatoriDeSters)
                {
                    Utilizator utilizatorDeSters = bancaDeSters.Utilizatori.FirstOrDefault(u => u.CNP == cnp);

                    if (utilizatorDeSters != null && utilizatorDeSters.NumeBanca.ToUpper() == bancaDeSters.Nume.ToUpper())
                    {
                        List<ContBancar> conturiUtilizator = utilizatorDeSters.Conturi;

                        foreach (var cont in conturiUtilizator)
                        {
                            // Sterge contul din lista generala
                            conturi.Remove(cont);
                            Logger.AddLog($"Cont sters: IBAN {cont.ID} pentru utilizatorul cu CNP {cnp}");
                        }

                        utilizatori.Remove(utilizatorDeSters);
                        Console.WriteLine($"Utilizatorul cu CNP {cnp} si conturile sale au fost sterse cu succes!");
                        Logger.AddLog($"Utilizator sters: CNP {cnp}");
                    }
                }

                banci.Remove(bancaDeSters);
                SalveazaBanci(banci);

                managerConturi.SalveazaConturi(conturi);
                managerUtilizatori.SalveazaUtilizatori(utilizatori);

                Logger.AddLog($"Banca stearsa: {bancaDeSters.Nume} cu ID: {bancaDeSters.IDBanca}");

                return true;
            }
            else
            {
                Logger.AddLog($"Incercare esuata de stergere banca cu ID inexistent: {idBanca}");
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
