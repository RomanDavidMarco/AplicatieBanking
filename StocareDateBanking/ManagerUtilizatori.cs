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
            // se incearca deschiderea fisierului in modul OpenOrCreate
            // astfel incat sa fie creat daca nu exista
            Stream streamFisierText = File.Open(numeFisier, FileMode.OpenOrCreate);
            streamFisierText.Close();
        }

        public void AddUtilizator(Utilizator utilizator)
        {
            // instructiunea 'using' va apela la final streamWriterFisierText.Close();
            // al doilea parametru setat la 'true' al constructorului StreamWriter indica
            // modul 'append' de deschidere al fisierului
            using (StreamWriter streamWriterFisierText = new StreamWriter(numeFisier, true))
            {
                streamWriterFisierText.WriteLine(utilizator.ConversieLaSir_PentruFisier());
            }
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
