using LibrarieModeleBanking;
using StocareDateBanking;
using UtilitareBanking;
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

        //locatie fisiere
        static string locatieFisierSolutie = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        // Initializare manageri
        Logger logger = new Logger(locatieFisierSolutie + "\\logs.txt");
        CursValutar cursValutar = new CursValutar(locatieFisierSolutie + "\\CursuriValutare.json");
        ManagerBanci managerBanci = new ManagerBanci(locatieFisierSolutie + "\\banci.txt");
        ManagerUtilizatori managerUtilizatori = new ManagerUtilizatori(locatieFisierSolutie + "\\utilizatori.txt");
        ManagerConturi managerConturi = new ManagerConturi(locatieFisierSolutie + "\\conturi.txt");

        private Banca bancaSelectata;
        private Utilizator utilizatorSelectat;
        private ContBancar contSelectat;
        private ContBancar contSelectatATM;
        private Panel panelMain;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            panelMain = new Panel
            {
                Dock = DockStyle.None,
                Size = new Size(600, 600)
            };

            this.Size = new Size(1200, 600);
            // Adaugă panelul la formă
            this.Controls.Add(panelMain);

            IncarcaDate();
            AfiseazaMeniuBanci();

            CenterPanel();

            this.Resize += Form1_Resize;
        }

        private void CenterPanel()
        {
            panelMain.Location = new Point(
                (this.ClientSize.Width - panelMain.Width + panelMain.Width / 4) / 2, 0
            //(this.ClientSize.Height - panelMain.Height) / 2
            );
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            CenterPanel();
        }

        private void IncarcaDate()
        {
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
                Banca banca = banci.FirstOrDefault(b => b.Nume.ToUpper() == utilizator.NumeBanca.ToUpper());

                if (banca != null)
                {
                    foreach (var cont in conturi)
                    {
                        if (cont.CNP == utilizator.CNP && cont.ID.Substring(0, 6) == banca.IDBanca)
                        {
                            utilizator.AdaugaCont(cont);
                        }
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
            if (panelMain != null)
                panelMain.Controls.Clear();

            var lbl = new Label
            {
                Text = "=== MENIU BĂNCI ===",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lbl);

            // Căutare
            Label lblCautare = new Label
            {
                Text = "Caută bancă:",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };
            panelMain.Controls.Add(lblCautare);

            TextBox txtCautare = new TextBox
            {
                Location = new Point(20, 85),
                Width = 300
            };
            panelMain.Controls.Add(txtCautare);

            // ListBox banci
            ListBox lstBanci = new ListBox
            {
                Location = new Point(20, 120),
                Size = new Size(300, 150)
            };
            panelMain.Controls.Add(lstBanci);

            // Funcția de actualizare listă
            void UpdateListaBanci(string criteriu)
            {
                lstBanci.Items.Clear();
                foreach (var banca in banci)
                {
                    if (string.IsNullOrEmpty(criteriu) ||
                        banca.Nume.IndexOf(criteriu, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        banca.IDBanca.IndexOf(criteriu, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        lstBanci.Items.Add($"{banca.Nume} - {banca.IDBanca}");
                    }
                }
            }

            UpdateListaBanci(""); // inițial afișăm toate băncile

            txtCautare.TextChanged += (s, e) =>
            {
                UpdateListaBanci(txtCautare.Text);
            };

            // Buton "Selectează Banca"
            Button btnSelecteaza = new Button
            {
                Text = "Selectează Banca",
                Location = new Point(20, 290)
            };
            btnSelecteaza.Click += (s, e) =>
            {
                if (lstBanci.SelectedIndex != -1)
                {
                    string selectedText = lstBanci.SelectedItem.ToString();
                    string idBancaSelectata = selectedText.Split('-').Last().Trim();
                    bancaSelectata = banci.FirstOrDefault(b => b.IDBanca == idBancaSelectata);

                    if (bancaSelectata != null)
                    {
                        AfiseazaMeniuUtilizatori();
                    }
                }
            };
            panelMain.Controls.Add(btnSelecteaza);

            // Buton "Adaugă Bancă"
            Button btnAdaugaBanca = new Button
            {
                Text = "Adaugă Bancă",
                Location = new Point(150, 290)
            };
            btnAdaugaBanca.Click += (s, e) => AfiseazaFormularAdaugareBanca();
            panelMain.Controls.Add(btnAdaugaBanca);

            // Buton "Șterge Bancă"
            Button btnStergeBanca = new Button
            {
                Text = "Șterge Bancă",
                Location = new Point(280, 290)
            };
            btnStergeBanca.Click += (s, e) => AfiseazaFormularStergereBanca();
            panelMain.Controls.Add(btnStergeBanca);
        }


        private void AfiseazaFormularAdaugareBanca()
        {
            panelMain.Controls.Clear();

            Label lblTitlu = new Label
            {
                Text = "Adaugare Bancă Nouă",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            Label lblNume = new Label { Text = "Nume Bancă:", Location = new Point(20, 70), AutoSize = true };
            TextBox txtNume = new TextBox { Location = new Point(150, 70), Width = 200 };
            panelMain.Controls.Add(lblNume);
            panelMain.Controls.Add(txtNume);

            Label lblInitiale = new Label { Text = "Initiale Bancă (4 litere):", Location = new Point(20, 110), AutoSize = true };
            TextBox txtInitiale = new TextBox { Location = new Point(150, 110), Width = 200 };
            panelMain.Controls.Add(lblInitiale);
            panelMain.Controls.Add(txtInitiale);

            Button btnSalveaza = new Button
            {
                Text = "Salvează Banca",
                Location = new Point(150, 160)
            };
            panelMain.Controls.Add(btnSalveaza);

            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(20, 160)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuBanci();
            panelMain.Controls.Add(btnInapoi);

            btnSalveaza.Click += (s, e) =>
            {
                bool valid = true;

                // Resetăm culorile
                txtNume.BackColor = Color.White;
                txtInitiale.BackColor = Color.White;

                if (Banca.ValidareNumeBanca(txtNume.Text))
                {
                    txtNume.BackColor = Color.LightCoral;
                    valid = false;
                }

                if (Banca.ValidarePrefixBanca(txtInitiale.Text))
                {
                    txtInitiale.BackColor = Color.LightCoral;
                    valid = false;
                }

                if (Banca.ValidareExistaBanca(banci, txtNume.Text, "RO"+txtInitiale.Text))
                {
                    valid = false;
                }

                if (valid)
                {
                    managerBanci.AddBanca(banci, txtNume.Text, txtInitiale.Text);
                    MessageBox.Show("Banca adăugată cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    AfiseazaMeniuBanci(); // revenim la meniul principal
                }
                else
                {
                    MessageBox.Show("Datele introduse nu sunt valide / Banca exista deja! Corectează câmpurile marcate.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }

        private void AfiseazaFormularStergereBanca()
        {
            panelMain.Controls.Clear();

            Label lblTitlu = new Label
            {
                Text = "Ștergere Bancă",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            Label lblCautare = new Label
            {
                Text = "Caută bancă (după nume sau prefix):",
                Location = new Point(20, 70),
                AutoSize = true
            };
            panelMain.Controls.Add(lblCautare);

            TextBox txtCautare = new TextBox
            {
                Location = new Point(20, 95),
                Width = 300
            };
            panelMain.Controls.Add(txtCautare);

            ListBox lstBanci = new ListBox
            {
                Location = new Point(20, 130),
                Size = new Size(300, 150)
            };
            panelMain.Controls.Add(lstBanci);

            // Inițial încărcăm toate băncile
            void IncarcaListaBanci(string criteriu)
            {
                lstBanci.Items.Clear();
                foreach (var banca in banci)
                {
                    if (string.IsNullOrEmpty(criteriu) ||
                        banca.Nume.IndexOf(criteriu, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        banca.IDBanca.IndexOf(criteriu, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        lstBanci.Items.Add($"{banca.Nume} - {banca.IDBanca}");
                    }
                }
            }

            IncarcaListaBanci(""); // La început afișăm toate băncile

            // Eveniment când tastezi în search box
            txtCautare.TextChanged += (s, e) =>
            {
                IncarcaListaBanci(txtCautare.Text);
            };

            Button btnSterge = new Button
            {
                Text = "Șterge Banca",
                Location = new Point(20, 300)
            };
            panelMain.Controls.Add(btnSterge);

            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 300)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuBanci();
            panelMain.Controls.Add(btnInapoi);

            btnSterge.Click += (s, e) =>
            {
                if (lstBanci.SelectedIndex != -1)
                {
                    string bancaInfo = lstBanci.SelectedItem.ToString();
                    string idBancaSelectata = bancaInfo.Split('-').Last().Trim();

                    var bancaDeSters = banci.FirstOrDefault(b => b.IDBanca == idBancaSelectata);

                    if (bancaDeSters != null)
                    {
                        var confirmare = MessageBox.Show(
                            $"Sigur dorești să ștergi banca '{bancaDeSters.Nume}' și toți utilizatorii săi?",
                            "Confirmare Ștergere Bancă",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question
                        );

                        if (confirmare == DialogResult.Yes)
                        {
                            bool OK = managerBanci.StergeBanca(banci, utilizatori, conturi, managerConturi, managerUtilizatori, bancaDeSters.IDBanca);

                            if (OK)
                            {
                                MessageBox.Show("Banca și toți utilizatorii săi au fost șterși cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Eroare la ștergerea băncii!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            AfiseazaMeniuBanci();
                        }
                        else
                        {
                            MessageBox.Show("Ștergerea băncii a fost anulată.", "Anulare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Selectați o bancă!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }

        private void AfiseazaMeniuUtilizatori()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = $"=== UTILIZATORI BANCA {bancaSelectata.Nume} ===",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            // Label pentru căutare
            var lblCautare = new Label
            {
                Text = "Căutare utilizator:",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };
            panelMain.Controls.Add(lblCautare);

            // TextBox pentru căutare
            TextBox txtSearch = new TextBox
            {
                Location = new Point(20, 85),
                Width = 300
            };
            panelMain.Controls.Add(txtSearch);

            // ListBox pentru utilizatori
            ListBox lstUtilizatori = new ListBox
            {
                Location = new Point(20, 120),
                Size = new Size(300, 150)
            };
            panelMain.Controls.Add(lstUtilizatori);

            // Funcție pentru a încărca utilizatorii în ListBox
            void IncarcaUtilizatori(string search = "")
            {
                lstUtilizatori.Items.Clear();
                foreach (var utilizator in bancaSelectata.Utilizatori)
                {
                    if (string.IsNullOrWhiteSpace(search) ||
                        utilizator.Nume.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        utilizator.CNP.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        lstUtilizatori.Items.Add($"{utilizator.Nume} {utilizator.Prenume} - {utilizator.CNP}");
                    }
                }
            }

            // Încărcare inițială utilizatori
            IncarcaUtilizatori();

            // Căutare în timp real
            txtSearch.TextChanged += (s, e) =>
            {
                IncarcaUtilizatori(txtSearch.Text);
            };

            // Buton "Selectează Utilizator"
            Button btnSelecteaza = new Button
            {
                Text = "Loghează Utilizator",
                Location = new Point(20, 290)
            };
            btnSelecteaza.Click += (s, e) =>
            {
                if (lstUtilizatori.SelectedIndex != -1)
                {
                    utilizatorSelectat = bancaSelectata.Utilizatori[lstUtilizatori.SelectedIndex];
                    AfiseazaFormularAutentificareUtilizator();
                }
            };
            panelMain.Controls.Add(btnSelecteaza);

            // Buton "Adaugă Utilizator"
            Button btnAdauga = new Button
            {
                Text = "Adaugă Utilizator",
                Location = new Point(160, 290)
            };
            btnAdauga.Click += (s, e) => AfiseazaFormularAdaugareUtilizator();
            panelMain.Controls.Add(btnAdauga);

            // Buton "Sterge Utilizator"
            Button btnSterge = new Button
            {
                Text = "Sterge Utilizator",
                Location = new Point(300, 290)
            };
            btnSterge.Click += (s, e) => AfiseazaFormularStergereUtilizator();
            panelMain.Controls.Add(btnSterge);

            // Buton "Înapoi"
            Button btnInapoi = new Button
            {
                Text = "Înapoi la Banci",
                Location = new Point(440, 290),
                AutoSize = true
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuBanci();
            panelMain.Controls.Add(btnInapoi);
        }

        private void AfiseazaFormularAdaugareUtilizator()
        {
            panelMain.Controls.Clear();

            Label lblTitlu = new Label
            {
                Text = "Adaugă Utilizator Nou",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            // CNP
            Label lblCNP = new Label { Text = "CNP:", Location = new Point(20, 70), AutoSize = true };
            TextBox txtCNP = new TextBox { Location = new Point(150, 70), Width = 200 };
            panelMain.Controls.Add(lblCNP);
            panelMain.Controls.Add(txtCNP);

            // Nume
            Label lblNume = new Label { Text = "Nume:", Location = new Point(20, 110), AutoSize = true };
            TextBox txtNume = new TextBox { Location = new Point(150, 110), Width = 200 };
            panelMain.Controls.Add(lblNume);
            panelMain.Controls.Add(txtNume);

            // Prenume
            Label lblPrenume = new Label { Text = "Prenume:", Location = new Point(20, 150), AutoSize = true };
            TextBox txtPrenume = new TextBox { Location = new Point(150, 150), Width = 200 };
            panelMain.Controls.Add(lblPrenume);
            panelMain.Controls.Add(txtPrenume);

            // Parola
            Label lblParola = new Label { Text = "Parola:", Location = new Point(20, 190), AutoSize = true };
            TextBox txtParola = new TextBox { Location = new Point(150, 190), Width = 200, PasswordChar = '*' };
            panelMain.Controls.Add(lblParola);
            panelMain.Controls.Add(txtParola);

            // Checkbox Termeni și Condiții
            CheckBox chkTermeni = new CheckBox
            {
                Text = "Sunt de acord și am citit ",
                Location = new Point(20, 230),
                AutoSize = true
            };
            panelMain.Controls.Add(chkTermeni);

            // LinkLabel pentru Termeni
            LinkLabel linkTermeni = new LinkLabel
            {
                Text = "Termenii și Condițiile",
                Location = new Point(chkTermeni.Right + 5, 230),
                AutoSize = true
            };
            linkTermeni.Click += (s, e) =>
            {
                // Poți schimba linkul spre orice pagină sau fișier .txt local
                System.Diagnostics.Process.Start("explorer", "https://example.com/termeni.pdf");
                // Sau local: System.Diagnostics.Process.Start("notepad.exe", @"C:\Termeni.txt");
            };
            panelMain.Controls.Add(linkTermeni);

            // Buton Adaugă
            Button btnAdauga = new Button
            {
                Text = "Adaugă Utilizator",
                Location = new Point(20, 270)
            };
            panelMain.Controls.Add(btnAdauga);

            // Buton Înapoi
            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 270)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuUtilizatori();
            panelMain.Controls.Add(btnInapoi);

            // Eveniment click Adaugă
            btnAdauga.Click += (s, e) =>
            {
                bool eroare = false;

                txtCNP.BackColor = SystemColors.Window;
                txtNume.BackColor = SystemColors.Window;
                txtPrenume.BackColor = SystemColors.Window;
                txtParola.BackColor = SystemColors.Window;

                if (Utilizator.ValidareCNP(txtCNP.Text))
                {
                    txtCNP.BackColor = Color.LightCoral;
                    eroare = true;
                }
                if (Utilizator.ValidareNumePrenumeParolaUtilizator(txtNume.Text))
                {
                    txtNume.BackColor = Color.LightCoral;
                    eroare = true;
                }
                if (Utilizator.ValidareNumePrenumeParolaUtilizator(txtPrenume.Text))
                {
                    txtPrenume.BackColor = Color.LightCoral;
                    eroare = true;
                }
                if (Utilizator.ValidareNumePrenumeParolaUtilizator(txtParola.Text))
                {
                    txtParola.BackColor = Color.LightCoral;
                    eroare = true;
                }
                if (Utilizator.ValidareExistaUtilizator(bancaSelectata.Utilizatori, txtCNP.Text))
                {
                    MessageBox.Show("Utilizatorul cu acest CNP există deja!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!chkTermeni.Checked)
                {
                    MessageBox.Show("Trebuie să accepți Termenii și Condițiile pentru a continua!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (eroare)
                {
                    MessageBox.Show("Datele introduse nu sunt valide! Corectează câmpurile marcate.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Salvăm utilizatorul
                managerUtilizatori.AddUtilizator(utilizatori, bancaSelectata, txtNume.Text, txtPrenume.Text, txtCNP.Text, txtParola.Text);
                MessageBox.Show("Utilizator adăugat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AfiseazaMeniuUtilizatori();
            };
        }


        private void AfiseazaFormularStergereUtilizator()
        {
            panelMain.Controls.Clear();

            // Titlu
            Label lblTitlu = new Label
            {
                Text = "Ștergere Utilizator",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            // Label Căutare
            Label lblCautare = new Label
            {
                Text = "Căutare utilizator:",
                Location = new Point(20, 70),
                AutoSize = true
            };
            panelMain.Controls.Add(lblCautare);

            // TextBox Căutare
            TextBox txtCautare = new TextBox
            {
                Location = new Point(20, 95),
                Width = 300
            };
            panelMain.Controls.Add(txtCautare);

            // ListBox Utilizatori
            ListBox lstUtilizatori = new ListBox
            {
                Location = new Point(20, 130),
                Size = new Size(300, 150)
            };
            panelMain.Controls.Add(lstUtilizatori);

            // Populate ListBox inițial
            foreach (var utilizator in bancaSelectata.Utilizatori)
                lstUtilizatori.Items.Add($"{utilizator.Nume} {utilizator.Prenume} - {utilizator.CNP}");

            // Căutare live
            txtCautare.TextChanged += (s, e) =>
            {
                lstUtilizatori.Items.Clear();
                foreach (var utilizator in bancaSelectata.Utilizatori)
                {
                    if (utilizator.Nume.ToLower().Contains(txtCautare.Text.ToLower()) ||
                        utilizator.Prenume.ToLower().Contains(txtCautare.Text.ToLower()) ||
                        utilizator.CNP.Contains(txtCautare.Text))
                    {
                        lstUtilizatori.Items.Add($"{utilizator.Nume} {utilizator.Prenume} - {utilizator.CNP}");
                    }
                }
            };

            // Label și TextBox pentru CNP
            Label lblCnp = new Label
            {
                Text = "Introdu CNP-ul utilizatorului:",
                Location = new Point(20, 300),
                AutoSize = true
            };
            panelMain.Controls.Add(lblCnp);

            TextBox txtCnp = new TextBox
            {
                Location = new Point(20, 325),
                Width = 300
            };
            panelMain.Controls.Add(txtCnp);

            // Label și TextBox pentru Parolă
            Label lblParola = new Label
            {
                Text = "Introdu parola utilizatorului:",
                Location = new Point(20, 370),
                AutoSize = true
            };
            panelMain.Controls.Add(lblParola);

            TextBox txtParola = new TextBox
            {
                Location = new Point(20, 395),
                Width = 300,
                PasswordChar = '*'
            };
            panelMain.Controls.Add(txtParola);

            // Buton Șterge Utilizator
            Button btnSterge = new Button
            {
                Text = "Șterge Utilizator",
                Location = new Point(20, 450)
            };
            btnSterge.Click += (s, e) =>
            {
                bool valid = true;

                var utilizatorDeSters = bancaSelectata.Utilizatori.FirstOrDefault(u => u.CNP == txtCnp.Text);

                // Resetăm culorile
                txtCnp.BackColor = Color.White;
                txtParola.BackColor = Color.White;

                // Validare CNP
                if (Utilizator.ValidareCNP(txtCnp.Text))
                {
                    txtCnp.BackColor = Color.LightCoral;
                    valid = false;
                }

                // Validare parolă
                if (Utilizator.ValidareNumePrenumeParolaUtilizator(txtParola.Text) || utilizatorDeSters == null || utilizatorDeSters.VerificarePinCriptat(txtParola.Text))
                {
                    txtParola.BackColor = Color.LightCoral;
                    valid = false;
                }

                if (valid)
                {
                    // Confirmare ștergere
                    var confirmare = MessageBox.Show(
                        $"Sigur dorești să ștergi utilizatorul {utilizatorDeSters.Nume} {utilizatorDeSters.Prenume}?",
                        "Confirmare Ștergere",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question
                    );

                    if (confirmare == DialogResult.Yes)
                    {
                        bool OK = managerUtilizatori.StergeUtilizator(bancaSelectata, utilizatori, conturi, managerConturi, utilizatorDeSters.CNP);
                        
                        /*bancaSelectata.Utilizatori.Remove(utilizatorDeSters);
                        utilizatori.Remove(utilizatorDeSters);
                        managerUtilizatori.SalveazaUtilizatori(utilizatori);*/

                        if (OK)
                        {
                            MessageBox.Show("Utilizator șters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Eroare la ștergerea utilizatorului!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        AfiseazaMeniuUtilizatori();
                    }
                    else
                    {
                        // Dacă utilizatorul apasă No, nu facem nimic
                        MessageBox.Show("Ștergerea a fost anulată.", "Anulare", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                else
                {
                    MessageBox.Show("Datele introduse nu sunt valide! Corectează câmpurile marcate.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            panelMain.Controls.Add(btnSterge);

            // Buton Înapoi
            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 450)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuUtilizatori();
            panelMain.Controls.Add(btnInapoi);
        }

        private void AfiseazaFormularAutentificareUtilizator()
        {
            panelMain.Controls.Clear();

            Label lblTitlu = new Label
            {
                Text = "Autentificare Utilizator",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            // Label CNP
            Label lblCnp = new Label { Text = "CNP Utilizator:", Location = new Point(20, 70), AutoSize = true };
            TextBox txtCnp = new TextBox { Location = new Point(20, 95), Width = 300 };
            panelMain.Controls.Add(lblCnp);
            panelMain.Controls.Add(txtCnp);

            // Label Parolă
            Label lblParola = new Label { Text = "Parola:", Location = new Point(20, 140), AutoSize = true };
            TextBox txtParola = new TextBox { Location = new Point(20, 165), Width = 300, PasswordChar = '*' };
            panelMain.Controls.Add(lblParola);
            panelMain.Controls.Add(txtParola);

            // Buton Autentificare
            Button btnAutentificare = new Button
            {
                Text = "Autentificare",
                Location = new Point(20, 220)
            };
            panelMain.Controls.Add(btnAutentificare);

            // Buton Înapoi
            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 220)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuUtilizatori();
            panelMain.Controls.Add(btnInapoi);

            btnAutentificare.Click += (s, e) =>
            {
                bool valid = true;

                txtCnp.BackColor = Color.White;
                txtParola.BackColor = Color.White;

                // Validare CNP
                if (Utilizator.ValidareCNP(txtCnp.Text))
                {
                    txtCnp.BackColor = Color.LightCoral;
                    valid = false;
                }

                // Validare parola (nu goală + ulterior și verificare)
                if (Utilizator.ValidareNumePrenumeParolaUtilizator(txtParola.Text))
                {
                    txtParola.BackColor = Color.LightCoral;
                    valid = false;
                }

                if (valid)
                {
                    var utilizatorGasit = bancaSelectata.Utilizatori.FirstOrDefault(u => u.CNP == txtCnp.Text);

                    if (utilizatorGasit == null)
                    {
                        MessageBox.Show("Utilizatorul nu există!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        // Verificăm dacă parola este corectă
                        if (utilizatorGasit.VerificarePinCriptat(txtParola.Text))
                        {
                            txtParola.BackColor = Color.LightCoral;
                            MessageBox.Show("Parolă incorectă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            utilizatorSelectat = utilizatorGasit;
                            MessageBox.Show("Autentificare reușită!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            AfiseazaMeniuConturi(); // mergem la conturi
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Datele introduse nu sunt valide! Corectează câmpurile marcate.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
        }


        private void AfiseazaMeniuConturi()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = $"=== MENIU CONTURI - ({utilizatorSelectat.Nume} {utilizatorSelectat.Prenume}) ===",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            // Label căutare
            var lblCautare = new Label
            {
                Text = "Căutare cont (după nume sau IBAN):",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Arial", 10, FontStyle.Regular)
            };
            panelMain.Controls.Add(lblCautare);

            // TextBox căutare
            TextBox txtSearch = new TextBox
            {
                Location = new Point(20, 85),
                Width = 300
            };
            panelMain.Controls.Add(txtSearch);

            // ListBox pentru conturi
            ListBox lstConturi = new ListBox
            {
                Location = new Point(20, 120),
                Size = new Size(400, 150)
            };
            panelMain.Controls.Add(lstConturi);

            // Funcție pentru încărcare conturi
            void IncarcaConturi(string search = "")
            {
                lstConturi.Items.Clear();
                foreach (var cont in utilizatorSelectat.Conturi)
                {
                    if (string.IsNullOrWhiteSpace(search) ||
                        cont.NumeCont.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        cont.ID.IndexOf(search, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        lstConturi.Items.Add($"{cont.NumeCont} - {cont.ID} | Sold: {cont.Sold} {cont.Moneda}");
                    }
                }
            }

            IncarcaConturi();

            txtSearch.TextChanged += (s, e) =>
            {
                IncarcaConturi(txtSearch.Text);
            };

            // Buton "Adaugă Cont"
            Button btnAdaugaCont = new Button
            {
                Text = "Adaugă Cont",
                Location = new Point(20, 290)
            };
            btnAdaugaCont.Click += (s, e) => AfiseazaFormularAdaugareCont();
            panelMain.Controls.Add(btnAdaugaCont);

            // Buton "Sterge Cont"
            Button btnStergeCont = new Button
            {
                Text = "Șterge Cont",
                Location = new Point(150, 290)
            };
            btnStergeCont.Click += (s, e) => AfiseazaFormularStergereCont();
            panelMain.Controls.Add(btnStergeCont);

            // Buton "Spre ATM"
            Button btnATM = new Button
            {
                Text = "Spre ATM",
                Location = new Point(280, 290)
            };
            btnATM.Click += (s, e) => AfiseazaFormularAutentificareATM();
            panelMain.Controls.Add(btnATM);

            // Buton "Înapoi"
            Button btnInapoi = new Button
            {
                Text = "Înapoi la utilizatori",
                Location = new Point(410, 290)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuUtilizatori();
            panelMain.Controls.Add(btnInapoi);
        }

        /*private void AfiseazaFormularAdaugareCont()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Adaugare Cont Bancar",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            if (utilizatorSelectat.Conturi.Count >= 3)
            {
                MessageBox.Show("Utilizatorul nu poate avea mai mult de 3 conturi.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                AfiseazaMeniuConturi();
                return;
            }

            // Nume Cont
            var lblNume = new Label { Text = "Nume cont:", Location = new Point(20, 70), AutoSize = true };
            var txtNume = new TextBox { Location = new Point(20, 95), Width = 300 };
            panelMain.Controls.Add(lblNume);
            panelMain.Controls.Add(txtNume);

            // Moneda
            var lblMoneda = new Label { Text = "Alege moneda:", Location = new Point(20, 130), AutoSize = true };
            var cmbMoneda = new ComboBox
            {
                Location = new Point(20, 155),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbMoneda.Items.AddRange(new string[] { "RON", "EUR", "USD" });
            panelMain.Controls.Add(lblMoneda);
            panelMain.Controls.Add(cmbMoneda);

            // Sold inițial
            var lblSold = new Label { Text = "Depunere inițială:", Location = new Point(20, 190), AutoSize = true };
            var txtSold = new TextBox { Location = new Point(20, 215), Width = 300 };
            panelMain.Controls.Add(lblSold);
            panelMain.Controls.Add(txtSold);

            // PIN
            var lblPin = new Label { Text = "PIN cont (doar cifre):", Location = new Point(20, 250), AutoSize = true };
            var txtPin = new TextBox { Location = new Point(20, 275), Width = 300, PasswordChar = '*' };
            panelMain.Controls.Add(lblPin);
            panelMain.Controls.Add(txtPin);

            // Buton Salvare
            var btnSalveaza = new Button
            {
                Text = "Crează cont",
                Location = new Point(20, 320)
            };
            btnSalveaza.Click += (s, e) =>
            {
                bool valid = true;

                txtNume.BackColor = Color.White;
                txtSold.BackColor = Color.White;
                txtPin.BackColor = Color.White;

                string numeCont = txtNume.Text.Trim();
                if (string.IsNullOrWhiteSpace(numeCont) || utilizatorSelectat.Conturi.Any(c => c.NumeCont == numeCont))
                {
                    txtNume.BackColor = Color.LightCoral;
                    valid = false;
                }

                if (cmbMoneda.SelectedIndex == -1)
                {
                    MessageBox.Show("Selectează o monedă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    valid = false;
                }

                monede monedaSelectata = (monede)cmbMoneda.SelectedIndex;
                if (utilizatorSelectat.Conturi.Any(c => c.Moneda == monedaSelectata))
                {
                    MessageBox.Show("Utilizatorul are deja un cont în această monedă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    valid = false;
                }

                if (!decimal.TryParse(txtSold.Text, out decimal soldInitial) || soldInitial < 0)
                {
                    txtSold.BackColor = Color.LightCoral;
                    valid = false;
                }

                string pin = txtPin.Text.Trim();
                if (string.IsNullOrWhiteSpace(pin) || !pin.All(char.IsDigit))
                {
                    txtPin.BackColor = Color.LightCoral;
                    valid = false;
                }

                if (valid)
                {
                    managerConturi.AddCont(conturi, utilizatorSelectat, bancaSelectata.IDBanca, monedaSelectata, numeCont, pin, soldInitial);
                    MessageBox.Show("Cont creat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AfiseazaMeniuConturi();
                }
                else
                {
                    MessageBox.Show("Corectează câmpurile marcate!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            panelMain.Controls.Add(btnSalveaza);

            // Buton Înapoi
            var btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 320)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuConturi();
            panelMain.Controls.Add(btnInapoi);
        }*/

        private void AfiseazaFormularAdaugareCont()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Adăugare Cont Bancar",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            if (utilizatorSelectat.Conturi.Count >= 3)
            {
                MessageBox.Show("Utilizatorul nu poate avea mai mult de 3 conturi.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                AfiseazaMeniuConturi();
                return;
            }

            // Nume cont
            var lblNume = new Label { Text = "Nume cont:", Location = new Point(20, 70), AutoSize = true };
            var txtNume = new TextBox { Location = new Point(20, 95), Width = 300 };
            panelMain.Controls.Add(lblNume);
            panelMain.Controls.Add(txtNume);

            // Monedă - RadioButtons
            var lblMoneda = new Label { Text = "Alege moneda:", Location = new Point(20, 130), AutoSize = true };
            var rbRON = new RadioButton { Text = "RON", Location = new Point(20, 155), AutoSize = true };
            var rbEUR = new RadioButton { Text = "EUR", Location = new Point(80, 155), AutoSize = true };
            var rbUSD = new RadioButton { Text = "USD", Location = new Point(150, 155), AutoSize = true };
            panelMain.Controls.Add(lblMoneda);
            panelMain.Controls.Add(rbRON);
            panelMain.Controls.Add(rbEUR);
            panelMain.Controls.Add(rbUSD);

            // Sold inițial
            var lblSold = new Label { Text = "Depunere inițială:", Location = new Point(20, 190), AutoSize = true };
            var txtSold = new TextBox { Location = new Point(20, 215), Width = 300 };
            panelMain.Controls.Add(lblSold);
            panelMain.Controls.Add(txtSold);

            // PIN
            var lblPin = new Label { Text = "PIN cont (doar cifre):", Location = new Point(20, 250), AutoSize = true };
            var txtPin = new TextBox { Location = new Point(20, 275), Width = 300, PasswordChar = '*' };
            panelMain.Controls.Add(lblPin);
            panelMain.Controls.Add(txtPin);

            // Buton Salvare
            var btnSalveaza = new Button
            {
                Text = "Crează cont",
                Location = new Point(20, 320)
            };

            btnSalveaza.Click += (s, e) =>
            {
                bool valid = true;

                txtNume.BackColor = Color.White;
                txtSold.BackColor = Color.White;
                txtPin.BackColor = Color.White;

                string numeCont = txtNume.Text.Trim();
                if (string.IsNullOrWhiteSpace(numeCont) || utilizatorSelectat.Conturi.Any(c => c.NumeCont == numeCont))
                {
                    txtNume.BackColor = Color.LightCoral;
                    valid = false;
                }

                monede? monedaAleasa = null;
                if (rbRON.Checked) monedaAleasa = monede.RON;
                else if (rbEUR.Checked) monedaAleasa = monede.EUR;
                else if (rbUSD.Checked) monedaAleasa = monede.USD;

                if (monedaAleasa == null)
                {
                    MessageBox.Show("Selectează o monedă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    valid = false;
                }
                else if (utilizatorSelectat.Conturi.Any(c => c.Moneda == monedaAleasa))
                {
                    MessageBox.Show($"Utilizatorul are deja un cont în moneda {monedaAleasa}.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    valid = false;
                }

                if (!decimal.TryParse(txtSold.Text, out decimal soldInitial) || soldInitial < 0)
                {
                    txtSold.BackColor = Color.LightCoral;
                    valid = false;
                }

                string pin = txtPin.Text.Trim();
                if (string.IsNullOrWhiteSpace(pin) || !pin.All(char.IsDigit))
                {
                    txtPin.BackColor = Color.LightCoral;
                    valid = false;
                }

                if (valid && monedaAleasa.HasValue)
                {
                    managerConturi.AddCont(conturi, utilizatorSelectat, bancaSelectata.IDBanca, monedaAleasa.Value, numeCont, pin, soldInitial);
                    MessageBox.Show("Cont creat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AfiseazaMeniuConturi();
                }
                else
                {
                    MessageBox.Show("Corectează câmpurile marcate!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            panelMain.Controls.Add(btnSalveaza);

            // Buton Înapoi
            var btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 320)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuConturi();
            panelMain.Controls.Add(btnInapoi);
        }

        private void AfiseazaFormularStergereCont()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Ștergere Cont Bancar",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            if (utilizatorSelectat.Conturi == null || utilizatorSelectat.Conturi.Count == 0)
            {
                MessageBox.Show("Utilizatorul nu are conturi bancare!", "Informație", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AfiseazaMeniuConturi();
                return;
            }

            // ListBox conturi
            ListBox lstConturi = new ListBox
            {
                Location = new Point(20, 60),
                Size = new Size(400, 150)
            };

            foreach (var cont in utilizatorSelectat.Conturi)
                lstConturi.Items.Add($"{cont.NumeCont} | {cont.ID} | {cont.Moneda} | Sold: {cont.Sold}");

            panelMain.Controls.Add(lstConturi);

            // Label + TextBox PIN
            Label lblPin = new Label
            {
                Text = "Introdu PIN-ul contului selectat:",
                Location = new Point(20, 230),
                AutoSize = true
            };
            panelMain.Controls.Add(lblPin);

            TextBox txtPin = new TextBox
            {
                Location = new Point(20, 255),
                Width = 300,
                PasswordChar = '*'
            };
            panelMain.Controls.Add(txtPin);

            // Buton Ștergere
            Button btnSterge = new Button
            {
                Text = "Șterge Contul",
                Location = new Point(20, 300)
            };
            btnSterge.Click += (s, e) =>
            {
                bool valid = true;
                txtPin.BackColor = Color.White;

                if (lstConturi.SelectedIndex == -1)
                {
                    MessageBox.Show("Selectează un cont din listă!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPin.Text) || !txtPin.Text.All(char.IsDigit))
                {
                    txtPin.BackColor = Color.LightCoral;
                    valid = false;
                }

                var contSelectat = utilizatorSelectat.Conturi[lstConturi.SelectedIndex];

                if (valid && contSelectat.VerificaPin(txtPin.Text))
                {
                    managerConturi.StergeCont(utilizatorSelectat, conturi, contSelectat.ID);

                    MessageBox.Show("Cont șters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AfiseazaMeniuConturi();
                }
                else
                {
                    txtPin.BackColor = Color.LightCoral;
                    MessageBox.Show("PIN incorect sau câmp invalid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            panelMain.Controls.Add(btnSterge);

            // Buton Înapoi
            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 300)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuConturi();
            panelMain.Controls.Add(btnInapoi);
        }

        private void AfiseazaFormularAutentificareATM()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Autentificare ATM - Selectează un cont",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            if (utilizatorSelectat.Conturi == null || utilizatorSelectat.Conturi.Count == 0)
            {
                MessageBox.Show("Utilizatorul nu are conturi bancare!", "Informație", MessageBoxButtons.OK, MessageBoxIcon.Information);
                AfiseazaMeniuConturi();
                return;
            }

            // ListBox conturi
            ListBox lstConturi = new ListBox
            {
                Location = new Point(20, 60),
                Size = new Size(450, 150)
            };

            foreach (var cont in utilizatorSelectat.Conturi)
                lstConturi.Items.Add($"{cont.NumeCont} | ID: {cont.ID} | {cont.Moneda} | Sold: {cont.Sold}");

            panelMain.Controls.Add(lstConturi);

            // Label + TextBox PIN
            Label lblPin = new Label
            {
                Text = "Introdu PIN-ul contului:",
                Location = new Point(20, 230),
                AutoSize = true
            };
            panelMain.Controls.Add(lblPin);

            TextBox txtPin = new TextBox
            {
                Location = new Point(20, 255),
                Width = 300,
                PasswordChar = '*'
            };
            panelMain.Controls.Add(txtPin);

            // Buton "Accesează ATM"
            Button btnATM = new Button
            {
                Text = "Accesează ATM",
                Location = new Point(20, 300)
            };
            btnATM.Click += (s, e) =>
            {
                bool valid = true;
                txtPin.BackColor = Color.White;

                if (lstConturi.SelectedIndex == -1)
                {
                    MessageBox.Show("Selectează un cont din listă!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtPin.Text) || !txtPin.Text.All(char.IsDigit))
                {
                    txtPin.BackColor = Color.LightCoral;
                    valid = false;
                }

                var contSelectat = utilizatorSelectat.Conturi[lstConturi.SelectedIndex];

                if (valid && contSelectat.VerificaPin(txtPin.Text))
                {
                    contSelectatATM = contSelectat; // pentru a-l accesa în MeniuATM
                    AfiseazaMeniuATM();
                }
                else
                {
                    txtPin.BackColor = Color.LightCoral;
                    MessageBox.Show("PIN greșit sau câmp invalid!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            panelMain.Controls.Add(btnATM);

            // Buton Înapoi
            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 300)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuConturi();
            panelMain.Controls.Add(btnInapoi);
        }

        private void AfiseazaMeniuATM()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = $"ATM - {contSelectatATM.ID} ({contSelectatATM.NumeCont})",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            // Vizualizare sold
            Button btnSold = new Button
            {
                Text = $"1. Vizualizare Sold ({contSelectatATM.Moneda})",
                Location = new Point(20, 70),
                Size = new Size(250, 30)
            };
            btnSold.Click += (s, e) =>
            {
                MessageBox.Show($"Sold actual: {contSelectatATM.Sold} {contSelectatATM.Moneda}", "Sold", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };
            panelMain.Controls.Add(btnSold);

            // Depunere bani
            Button btnDepunere = new Button
            {
                Text = "2. Depunere Bani",
                Location = new Point(20, 110),
                Size = new Size(250, 30)
            };
            btnDepunere.Click += (s, e) => AfiseazaFormularDepunere();
            panelMain.Controls.Add(btnDepunere);

            // Retragere bani
            Button btnRetragere = new Button
            {
                Text = "3. Retragere Bani",
                Location = new Point(20, 150),
                Size = new Size(250, 30)
            };
            btnRetragere.Click += (s, e) => AfiseazaFormularRetragere();
            panelMain.Controls.Add(btnRetragere);

            // Transfer bani
            Button btnTransfer = new Button
            {
                Text = "4. Transfer Bani",
                Location = new Point(20, 190),
                Size = new Size(250, 30)
            };
            btnTransfer.Click += (s, e) => AfiseazaFormularTransfer();
            panelMain.Controls.Add(btnTransfer);

            // Schimb valutar
            Button btnSchimb = new Button
            {
                Text = "5. Schimb Valutar",
                Location = new Point(20, 230),
                Size = new Size(250, 30)
            };
            btnSchimb.Click += (s, e) => AfiseazaFormularSchimbValutar();
            panelMain.Controls.Add(btnSchimb);

            // Buton Închide sesiunea
            Button btnInapoi = new Button
            {
                Text = "6. Închide Sesiunea",
                Location = new Point(20, 280),
                Size = new Size(250, 30)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuConturi();
            panelMain.Controls.Add(btnInapoi);
        }

        private void AfiseazaFormularDepunere()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = $"Depunere bani în cont ({contSelectatATM.NumeCont} - {contSelectatATM.Moneda})",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            var lblSuma = new Label
            {
                Text = "Introduceți suma de depus:",
                Location = new Point(20, 70),
                AutoSize = true
            };
            panelMain.Controls.Add(lblSuma);

            TextBox txtSuma = new TextBox
            {
                Location = new Point(20, 95),
                Width = 200
            };
            panelMain.Controls.Add(txtSuma);

            Button btnDepune = new Button
            {
                Text = "Depune",
                Location = new Point(20, 140),
                AutoSize = true
            };
            btnDepune.Click += (s, e) =>
            {
                txtSuma.BackColor = Color.White;
                if (decimal.TryParse(txtSuma.Text, out decimal suma) && suma > 0)
                {
                    contSelectatATM.Depunere(suma);
                    managerConturi.SalveazaConturi(conturi);
                    MessageBox.Show($"Depunere reușită! Sold nou: {contSelectatATM.Sold} {contSelectatATM.Moneda}", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    AfiseazaMeniuATM(); // revenim în meniul ATM
                }
                else
                {
                    txtSuma.BackColor = Color.LightCoral;
                    MessageBox.Show("Introduceți o sumă validă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            panelMain.Controls.Add(btnDepune);

            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(120, 140),
                AutoSize = true
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuATM();
            panelMain.Controls.Add(btnInapoi);
        }

        private void AfiseazaFormularRetragere()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = $"Retragere bani din cont ({contSelectatATM.NumeCont} - {contSelectatATM.Moneda})",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            var lblSuma = new Label
            {
                Text = "Introduceți suma de retras:",
                Location = new Point(20, 70),
                AutoSize = true
            };
            panelMain.Controls.Add(lblSuma);

            TextBox txtSuma = new TextBox
            {
                Location = new Point(20, 95),
                Width = 200
            };
            panelMain.Controls.Add(txtSuma);

            Button btnRetrage = new Button
            {
                Text = "Retrage",
                Location = new Point(20, 140),
                AutoSize = true
            };
            btnRetrage.Click += (s, e) =>
            {
                txtSuma.BackColor = Color.White;
                if (decimal.TryParse(txtSuma.Text, out decimal suma) && suma > 0)
                {
                    if (contSelectatATM.Retragere(suma))
                    {
                        managerConturi.SalveazaConturi(conturi);
                        MessageBox.Show($"Retragere reușită! Sold nou: {contSelectatATM.Sold} {contSelectatATM.Moneda}", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AfiseazaMeniuATM();
                    }
                    else
                    {
                        MessageBox.Show("Fonduri insuficiente sau limită zilnică depășită!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    txtSuma.BackColor = Color.LightCoral;
                    MessageBox.Show("Introduceți o sumă validă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            panelMain.Controls.Add(btnRetrage);

            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(120, 140),
                AutoSize = true
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuATM();
            panelMain.Controls.Add(btnInapoi);
        }

        private void AfiseazaFormularTransfer()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Transfer Bani",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            // IBAN destinatar
            var lblDest = new Label { Text = "ID/IBAN destinatar:", Location = new Point(20, 70), AutoSize = true };
            TextBox txtDestinatar = new TextBox { Location = new Point(20, 95), Width = 300 };
            panelMain.Controls.Add(lblDest);
            panelMain.Controls.Add(txtDestinatar);

            // Sumă
            var lblSuma = new Label { Text = "Suma de transferat:", Location = new Point(20, 140), AutoSize = true };
            TextBox txtSuma = new TextBox { Location = new Point(20, 165), Width = 150 };
            panelMain.Controls.Add(lblSuma);
            panelMain.Controls.Add(txtSuma);

            // Buton transfer
            Button btnTransfer = new Button
            {
                Text = "Transferă",
                Location = new Point(20, 210),
                AutoSize = true
            };
            btnTransfer.Click += (s, e) =>
            {
                txtDestinatar.BackColor = Color.White;
                txtSuma.BackColor = Color.White;

                var contDest = conturi.FirstOrDefault(c => c.ID == txtDestinatar.Text.Trim());

                if (contDest == null)
                {
                    txtDestinatar.BackColor = Color.LightCoral;
                    MessageBox.Show("Cont destinatar inexistent!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtSuma.Text, out decimal suma) || suma <= 0)
                {
                    txtSuma.BackColor = Color.LightCoral;
                    MessageBox.Show("Introduceți o sumă validă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool succes = contSelectatATM.Transfer(contDest, suma);

                if (succes)
                {
                    managerConturi.SalveazaConturi(conturi);

                    if (contSelectatATM.Moneda != contDest.Moneda)
                    {
                        decimal curs = CursValutar.SchimbValutar(contSelectatATM.Moneda.ToString(), contDest.Moneda.ToString());
                        MessageBox.Show($"Moneda diferită! Conversie automată: {contSelectatATM.Moneda} → {contDest.Moneda} @ curs {curs}\n\nTransfer reușit!\nSold nou: {contSelectatATM.Sold} {contSelectatATM.Moneda}", "Transfer Finalizat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show($"Transfer reușit! Sold nou: {contSelectatATM.Sold} {contSelectatATM.Moneda}", "Transfer Finalizat", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    AfiseazaMeniuATM();
                }
                else
                {
                    MessageBox.Show("Fonduri insuficiente pentru transfer!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };
            panelMain.Controls.Add(btnTransfer);

            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(120, 210),
                AutoSize = true
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuATM();
            panelMain.Controls.Add(btnInapoi);
        }

        private void AfiseazaFormularSchimbValutar()
        {
            panelMain.Controls.Clear();

            var lblTitlu = new Label
            {
                Text = "Schimb Valutar între Conturi",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            panelMain.Controls.Add(lblTitlu);

            var alteConturi = utilizatorSelectat.Conturi.Where(c => c != contSelectatATM).ToList();

            if (alteConturi.Count == 0)
            {
                MessageBox.Show("Nu există alte conturi disponibile pentru schimb valutar.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                AfiseazaMeniuATM();
                return;
            }

            // Selectare cont destinatar
            var lblSelectie = new Label
            {
                Text = "Selectează contul destinatar:",
                Location = new Point(20, 60),
                AutoSize = true
            };
            panelMain.Controls.Add(lblSelectie);

            ComboBox cmbContDestinatie = new ComboBox
            {
                Location = new Point(20, 85),
                Width = 300,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            foreach (var c in alteConturi)
            {
                cmbContDestinatie.Items.Add($"{c.NumeCont} - {c.Moneda} - {c.ID}");
            }
            panelMain.Controls.Add(cmbContDestinatie);

            // Suma
            var lblSuma = new Label
            {
                Text = "Introduceți suma de schimbat:",
                Location = new Point(20, 130),
                AutoSize = true
            };
            TextBox txtSuma = new TextBox
            {
                Location = new Point(20, 155),
                Width = 150
            };
            panelMain.Controls.Add(lblSuma);
            panelMain.Controls.Add(txtSuma);

            // Buton schimb valutar
            Button btnSchimba = new Button
            {
                Text = "Execută Schimb",
                Location = new Point(20, 200),
                AutoSize = true
            };
            btnSchimba.Click += (s, e) =>
            {
                txtSuma.BackColor = Color.White;

                if (cmbContDestinatie.SelectedIndex == -1)
                {
                    MessageBox.Show("Selectați un cont destinatar!", "Avertisment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(txtSuma.Text, out decimal suma) || suma <= 0)
                {
                    txtSuma.BackColor = Color.LightCoral;
                    MessageBox.Show("Introduceți o sumă validă!", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var contDest = alteConturi[cmbContDestinatie.SelectedIndex];
                var curs = CursValutar.SchimbValutar(contSelectatATM.Moneda.ToString(), contDest.Moneda.ToString());

                var confirmare = MessageBox.Show(
                    $"Se va face conversia {suma} {contSelectatATM.Moneda} în {contDest.Moneda} (curs: {curs}). Continuăm?",
                    "Confirmare Schimb Valutar",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmare == DialogResult.Yes)
                {
                    bool rezultat = contSelectatATM.Transfer(contDest, suma);
                    if (rezultat)
                    {
                        managerConturi.SalveazaConturi(conturi);
                        MessageBox.Show("Schimb valutar efectuat cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        AfiseazaMeniuATM();
                    }
                    else
                    {
                        MessageBox.Show("Transfer eșuat. Verifică suma sau soldul disponibil.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            };
            panelMain.Controls.Add(btnSchimba);

            // Buton Înapoi
            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 200),
                AutoSize = true
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuATM();
            panelMain.Controls.Add(btnInapoi);
        }
    }
}
