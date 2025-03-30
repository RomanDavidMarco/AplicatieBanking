using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibrarieModeleBanking;

namespace StocareDateBanking
{
    public class ManagerUtilizatori
    {
        private string numeFisier;

        public ManagerUtilizatori(string numeFisier)
        {
            this.numeFisier = numeFisier;
            Stream streamFisierText = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisierText.Close();
        }

        public void AddUtilizator(List<Utilizator> utilizatori, Banca banca, string nume, string prenume, string cnp, string parola)
        {
            Utilizator utilizator = new Utilizator(banca.Nume, nume, prenume, cnp, parola);

            utilizatori.Add(utilizator);
            banca.AdaugaUtilizator(utilizator);

            using (StreamWriter streamWriterFisierText = new StreamWriter(numeFisier, true))
            {
                streamWriterFisierText.WriteLine(utilizator.ConversieLaSir_PentruFisier());
            }

        }

        public bool StergeUtilizator(Banca banca, List<Utilizator> utilizatori, List<ContBancar> conturi, ManagerConturi managerConturi, string cnp)
        {
            Utilizator utilizatorDeSters = utilizatori.FirstOrDefault(u => u.CNP == cnp);

            if (utilizatorDeSters != null)
            {
                banca.Utilizatori.Remove(utilizatorDeSters);

                conturi.RemoveAll(cont => utilizatorDeSters.Conturi.Contains(cont));

                utilizatori.Remove(utilizatorDeSters);

                SalveazaUtilizatori(utilizatori);
                managerConturi.SalveazaConturi(conturi);

                return true;
            }
            else
            {
                return false;
            }
        }

        public void AfiseazaUtilizatori(List<Utilizator> utilizatori)
        {
            Console.WriteLine("=== Lista Utilizatori ===");
            if (utilizatori != null)
            {
                foreach (var utilizator in utilizatori)
                {
                    Console.WriteLine($"{utilizator.Nume} {utilizator.Prenume} - CNP: {utilizator.CNP}");
                }
            }
            Console.ReadLine();
        }

        public List<Utilizator> IncarcaUtilizatori()
        {
            List<Utilizator> utilizatori = new List<Utilizator>();

            if (File.Exists(numeFisier))
            {
                string[] linii = File.ReadAllLines(numeFisier);
                foreach (string linie in linii)
                {
                    if (!string.IsNullOrWhiteSpace(linie))
                    {
                        utilizatori.Add(new Utilizator(linie));
                    }
                }
            }

            return utilizatori;
        }
        public void SalveazaUtilizatori(List<Utilizator> utilizatori)
        {
            using (StreamWriter writer = new StreamWriter(numeFisier, false)) //rescrie (dupa incarcare)
            {
                foreach (var utilizator in utilizatori)
                {
                    writer.WriteLine(utilizator.ConversieLaSir_PentruFisier());
                }
            }
        }
    }
}
