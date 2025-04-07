using LibrarieModeleBanking;
using StocareDateBanking;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AplicatieBankingForms
{
    public partial class Form1 : Form
    {
        List<Banca> banci;
        List<Utilizator> utilizatori;
        List<ContBancar> conturi;

        private Banca bancaSelectata;
        private Utilizator utilizatorSelectat;
        private ContBancar contSelectat;
        private Panel panelMain;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelMain = new Panel
            {
                Dock = DockStyle.Fill
            };
            this.Controls.Add(panelMain);
            IncarcaDate();
            AfiseazaMeniuBanci();
        }

        private void IncarcaDate()
        {
            string locatieFisierSolutie = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;

            // Initializare manageri
            ManagerBanci managerBanci = new ManagerBanci(locatieFisierSolutie + "\\banci.txt");
            ManagerUtilizatori managerUtilizatori = new ManagerUtilizatori(locatieFisierSolutie + "\\utilizatori.txt");
            ManagerConturi managerConturi = new ManagerConturi(locatieFisierSolutie + "\\conturi.txt");

            // Incarcare date
            banci = managerBanci.IncarcaBanci();
            utilizatori = managerUtilizatori.IncarcaUtilizatori();
            conturi = managerConturi.IncarcaConturi();

            System.Diagnostics.Debug.WriteLine("DATE CITITE!");

            /*managerBanci.AfiseazaBanci(banci);
            managerUtilizatori.AfiseazaUtilizatori(utilizatori);
            managerConturi.AfiseazaConturi(conturi);*/

            // Legarea conturilor la utilizatori prin metoda AddCont()
            foreach (var utilizator in utilizatori)
            {
                foreach (var cont in conturi)
                {
                    if (cont.CNP == utilizator.CNP)
                    {
                        utilizator.AdaugaCont(cont);
                    }
                }
            }

            // Legarea utilizatorilor la bănci prin metoda AdaugaUtilizator()
            foreach (var banca in banci)
            {
                foreach (var utilizator in utilizatori)
                {
                    if (utilizator.NumeBanca == banca.Nume)
                    {
                        banca.AdaugaUtilizator(utilizator);
                    }
                }
            }
        }
        private void AfiseazaMeniuBanci()
        {
            if(panelMain != null)
                panelMain.Controls.Clear();

            var lbl = new Label
            {
                Text = "=== MENIU BĂNCI ===",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lbl);

            // Afișezi lista băncilor într-un ListBox
            ListBox lstBanci = new ListBox { Location = new Point(20, 60), Size = new Size(300, 150) };
            foreach (var banca in banci)
                lstBanci.Items.Add(banca.Nume);

            panelMain.Controls.Add(lstBanci);

            // Buton "Selectează Banca"
            Button btnSelecteaza = new Button
            {
                Text = "Selectează Banca",
                Location = new Point(20, 230)
            };
            btnSelecteaza.Click += (s, e) =>
            {
                if (lstBanci.SelectedIndex != -1)
                {
                    bancaSelectata = banci[lstBanci.SelectedIndex];
                    AfiseazaMeniuUtilizatori();
                }
            };
            panelMain.Controls.Add(btnSelecteaza);
        }

        private void AfiseazaMeniuUtilizatori()
        {
            panelMain.Controls.Clear();

            var lbl = new Label
            {
                Text = $"=== UTILIZATORI BANCA {bancaSelectata.Nume} ===",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lbl);

            ListBox lstUtilizatori = new ListBox { Location = new Point(20, 60), Size = new Size(300, 150) };
            foreach (var utilizator in bancaSelectata.Utilizatori)
                lstUtilizatori.Items.Add(utilizator.Nume);

            panelMain.Controls.Add(lstUtilizatori);

            Button btnSelecteaza = new Button
            {
                Text = "Selectează Utilizator",
                Location = new Point(20, 230)
            };
            btnSelecteaza.Click += (s, e) =>
            {
                if (lstUtilizatori.SelectedIndex != -1)
                {
                    utilizatorSelectat = bancaSelectata.Utilizatori[lstUtilizatori.SelectedIndex];
                    AfiseazaMeniuConturi();
                }
            };
            panelMain.Controls.Add(btnSelecteaza);
            Button btnInapoi = new Button { Text = "Înapoi la banci", Location = new Point(20, 270), AutoSize = true};
            btnInapoi.Click += (s, e) => AfiseazaMeniuBanci();
            panelMain.Controls.Add(btnInapoi);
        }
        private void AfiseazaMeniuConturi()
        {
            panelMain.Controls.Clear();

            var lbl = new Label
            {
                Text = $"=== CONTURI {utilizatorSelectat.Nume} ===",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lbl);

            ListBox lstConturi = new ListBox { Location = new Point(20, 60), Size = new Size(300, 150) };
            foreach (var cont in utilizatorSelectat.Conturi)
                lstConturi.Items.Add(cont.NumarCont);

            panelMain.Controls.Add(lstConturi);

            Button btnSelecteaza = new Button
            {
                Text = "Selectează Cont",
                Location = new Point(20, 230)
            };
            btnSelecteaza.Click += (s, e) =>
            {
                if (lstConturi.SelectedIndex != -1)
                {
                    contSelectat = utilizatorSelectat.Conturi[lstConturi.SelectedIndex];
                    AfiseazaMeniuATM();
                }
            };
            panelMain.Controls.Add(btnSelecteaza);
            Button btnInapoi = new Button { Text = "Înapoi la utilizatori", Location = new Point(20, 270), AutoSize = true };
            btnInapoi.Click += (s, e) => AfiseazaMeniuUtilizatori();
            panelMain.Controls.Add(btnInapoi);
        }
        private void AfiseazaMeniuATM()
        {
            panelMain.Controls.Clear();

            var lbl = new Label
            {
                Text = $"=== ATM CONT {contSelectat.NumarCont} ===",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lbl);

            // Afișează soldul
            Label lblSold = new Label
            {
                Text = $"Sold: {contSelectat.Sold} lei",
                Location = new Point(20, 60),
                AutoSize = true
            };
            panelMain.Controls.Add(lblSold);

            // Butoane Depunere / Retragere / Transfer / Înapoi
            // De exemplu:
            Button btnDepunere = new Button { Text = "Depunere", Location = new Point(20, 100) };
            btnDepunere.Click += (s, e) => { /* cod depunere */ };
            panelMain.Controls.Add(btnDepunere);

            Button btnRetragere = new Button { Text = "Retragere", Location = new Point(20, 140) };
            btnRetragere.Click += (s, e) => { /* cod retragere */ };
            panelMain.Controls.Add(btnRetragere);

            Button btnInapoi = new Button { Text = "Înapoi la conturi", Location = new Point(20, 180), AutoSize = true };
            btnInapoi.Click += (s, e) => AfiseazaMeniuConturi();
            panelMain.Controls.Add(btnInapoi);
        }


    }
}
