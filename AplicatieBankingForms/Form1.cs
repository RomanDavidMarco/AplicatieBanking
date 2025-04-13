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

        //locatie fisiere
        static string locatieFisierSolutie = Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName;

        // Initializare manageri
        ManagerBanci managerBanci = new ManagerBanci(locatieFisierSolutie + "\\banci.txt");
        ManagerUtilizatori managerUtilizatori = new ManagerUtilizatori(locatieFisierSolutie + "\\utilizatori.txt");
        ManagerConturi managerConturi = new ManagerConturi(locatieFisierSolutie + "\\conturi.txt");

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

                if (valid)
                {
                    managerBanci.AddBanca(banci, txtNume.Text, txtInitiale.Text.ToUpper());
                    MessageBox.Show("Banca adăugată cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    AfiseazaMeniuBanci(); // revenim la meniul principal
                }
                else
                {
                    MessageBox.Show("Datele introduse nu sunt valide! Corectează câmpurile marcate.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // Label + TextBox pentru CNP
            Label lblCNP = new Label { Text = "CNP:", Location = new Point(20, 70), AutoSize = true };
            TextBox txtCNP = new TextBox { Location = new Point(150, 70), Width = 200 };
            panelMain.Controls.Add(lblCNP);
            panelMain.Controls.Add(txtCNP);

            // Label + TextBox pentru Nume
            Label lblNume = new Label { Text = "Nume:", Location = new Point(20, 110), AutoSize = true };
            TextBox txtNume = new TextBox { Location = new Point(150, 110), Width = 200 };
            panelMain.Controls.Add(lblNume);
            panelMain.Controls.Add(txtNume);

            // Label + TextBox pentru Prenume
            Label lblPrenume = new Label { Text = "Prenume:", Location = new Point(20, 150), AutoSize = true };
            TextBox txtPrenume = new TextBox { Location = new Point(150, 150), Width = 200 };
            panelMain.Controls.Add(lblPrenume);
            panelMain.Controls.Add(txtPrenume);

            // Label + TextBox pentru Parola
            Label lblParola = new Label { Text = "Parola:", Location = new Point(20, 190), AutoSize = true };
            TextBox txtParola = new TextBox { Location = new Point(150, 190), Width = 200, PasswordChar = '*' };
            panelMain.Controls.Add(lblParola);
            panelMain.Controls.Add(txtParola);

            // Buton de adaugare
            Button btnAdauga = new Button
            {
                Text = "Adaugă Utilizator",
                Location = new Point(20, 240)
            };
            panelMain.Controls.Add(btnAdauga);

            // Buton inapoi
            Button btnInapoi = new Button
            {
                Text = "Înapoi",
                Location = new Point(150, 240)
            };
            btnInapoi.Click += (s, e) => AfiseazaMeniuUtilizatori();
            panelMain.Controls.Add(btnInapoi);

            btnAdauga.Click += (s, e) =>
            {
                bool eroare = false;

                // Resetăm culorile în caz că au fost roșii
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

                if (eroare)
                {
                    MessageBox.Show("Datele introduse nu sunt valide! Corectează câmpurile marcate.", "Eroare", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Dacă totul e valid, adaugăm utilizatorul
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
                        bancaSelectata.Utilizatori.Remove(utilizatorDeSters);
                        utilizatori.Remove(utilizatorDeSters);
                        managerUtilizatori.SalveazaUtilizatori(utilizatori);

                        MessageBox.Show("Utilizator șters cu succes!", "Succes", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
