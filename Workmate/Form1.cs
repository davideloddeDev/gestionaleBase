using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;

namespace Workmate
{
    public partial class Form1 : Form
    {
        private int borderSize = 2;
        private bool magazzino = false;
        private bool prod = false;
        bool mostra_avviso = true;
        bool avv_mostrati = false;
        public float totalefatturato;
        bool ordinicaricati = false;
        bool prodotticaricati = false;
        bool codicicaricati = false;
        public Form1()
        {
            InitializeComponent();
            Checkdirs();
            this.Padding = new Padding(borderSize);
            this.BackColor = Color.FromArgb(29, 133, 181);
            carica_ordini();
            bar_pnl.Visible = false;
            ordini_data.Visible = false;
            magazzino_data.Visible = false;
            settings_pnl.Visible = false;
            prod_data.Visible = false;
            desktop_pnl.Visible = true;
        }
        private void closeform(object sender, FormClosingEventArgs e)
        {
            if (magazzino == true && var.ended == true)
            {
                carica_codici();
                var.ended = false;
                codicicaricati = true;
            }
            else if (prod == true && var.ended == true)
            {
                carica_prodotti();
                var.ended = false;
                prodotticaricati = true;
            }
            else if (var.ended == true)
            {
                carica_ordini();
                var.ended = false;
                ordinicaricati = true;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void bars_btn_Click(object sender, EventArgs e)
        {
            CollapseMenu();
        }

        private void CollapseMenu()
        {
            if (this.dock_pnl.Width > 200)
            {
                dock_pnl.Width = 100;
                bars_btn.Dock = DockStyle.Top;
                pictureBox1.Visible = false;
                foreach (Button menuButton in dock_pnl.Controls.OfType<Button>())
                {
                    menuButton.Text = "";
                    menuButton.ImageAlign = ContentAlignment.MiddleCenter;
                    menuButton.Padding = new Padding(0);
                }
            }
            else
            {
                dock_pnl.Width = 230;
                bars_btn.Dock = DockStyle.None;
                pictureBox1.Visible = true;
                foreach (Button menuButton in dock_pnl.Controls.OfType<Button>())
                {
                    menuButton.Text = "   " + menuButton.Tag.ToString();
                    menuButton.ImageAlign = ContentAlignment.MiddleLeft;
                    menuButton.Padding = new Padding(10, 0, 0, 0);
                }
            }
        }

        private void Checkdirs()
        {
            string root = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + @"\Workmate";
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }
            if (!Directory.Exists(root + @"\Magazzino"))
            {
                Directory.CreateDirectory(root + @"\Magazzino");
            }
            if (!Directory.Exists(root + @"\Ordini"))
            {
                Directory.CreateDirectory(root + @"\Ordini");
            }
            if (!Directory.Exists(root + @"\Prodotti"))
            {
                Directory.CreateDirectory(root + @"\Prodotti");
            }
        }

        private void carica_codici(string testoc = "", string colonnac = "", int search = 0)
        {
            MessageBox.Show("Caricamento codici in corso...");
            magazzino_data.Rows.Clear();
            string[] codici = var.carica_codici();
            for (int i = 0; i < codici.Length; i++)
            {
                XmlDocument xml_doc = new XmlDocument();
                xml_doc.Load(codici[i]);
                XmlNode codice = xml_doc.DocumentElement.SelectSingleNode("/codice/cod");
                XmlNode prezzo = xml_doc.DocumentElement.SelectSingleNode("/codice/prezzo");
                XmlNode quantita = xml_doc.DocumentElement.SelectSingleNode("/codice/quantit�");
                XmlNode quantitamin = xml_doc.DocumentElement.SelectSingleNode("/codice/quantit�min");
                XmlNode descrizione = xml_doc.DocumentElement.SelectSingleNode("/codice/descrizione");
                string[] riga = { codice.InnerText, prezzo.InnerText, quantita.InnerText, quantitamin.InnerText, descrizione.InnerText };
                //MessageBox.Show(codici[i].Replace(var.db+@"Magazzino\", ""));
                string contenuto = "";
                switch (colonnac)
                {
                    case "Codice":
                        contenuto = codice.InnerText;
                        break;
                    case "Prezzo":
                        contenuto = prezzo.InnerText;
                        break;
                    case "Quantit�":
                        contenuto = quantita.InnerText;
                        break;
                    case "Descrizione":
                        contenuto = descrizione.InnerText;
                        break;
                    default:
                        contenuto = "";
                        break;
                }
                contenuto = contenuto.ToLower();
                testoc = testoc.ToLower();
                if (contenuto.Contains(testoc))
                    magazzino_data.Rows.Add(riga);

                if (Int32.Parse(quantita.InnerText) < Int32.Parse(quantitamin.InnerText) && mostra_avviso == true && search == 0 && avv_mostrati == false)
                {
                    CustomDialog customdialog = new CustomDialog();
                    customdialog.label1.Text = codice.InnerText + " � sceso sotto la soglia minima";
                    customdialog.ShowDialog();
                    if (customdialog.DialogResult.Equals(DialogResult.Yes))
                    {
                        mostra_avviso = false;
                    }
                }
            }
        }

        private void carica_ordini(string testoc = "", string colonnac = "")
        {
            MessageBox.Show("Caricamento ordini in corso...");
            if (testoc == "")
                totalefatturato = 0;
            ordini_data.Rows.Clear();
            string[] ordini = var.carica_ordini();
            for (int i = 0; i < ordini.Length; i++)
            {
                XmlDocument xml_doc = new XmlDocument();
                xml_doc.Load(ordini[i]);
                XmlNode ordine = xml_doc.DocumentElement.SelectSingleNode("/ordine/ordine");
                XmlNode prezzo = xml_doc.DocumentElement.SelectSingleNode("/ordine/prezzo");
                XmlNode cliente = xml_doc.DocumentElement.SelectSingleNode("/ordine/cliente");
                XmlNode note = xml_doc.DocumentElement.SelectSingleNode("/ordine/note");
                string[] riga = { ordine.InnerText, prezzo.InnerText, cliente.InnerText, note.InnerText };

                string contenuto = "";
                switch (colonnac)
                {
                    case "Ordine":
                        contenuto = ordine.InnerText;
                        break;
                    case "Prezzo":
                        contenuto = prezzo.InnerText;
                        break;
                    case "Cliente":
                        contenuto = cliente.InnerText;
                        break;
                    case "Note":
                        contenuto = note.InnerText;
                        break;
                    default:
                        contenuto = "";
                        break;
                }
                contenuto = contenuto.ToLower();
                testoc = testoc.ToLower();
                if (contenuto.Contains(testoc))
                    ordini_data.Rows.Add(riga);
                if (testoc == "")
                    totalefatturato += float.Parse(prezzo.InnerText, CultureInfo.InvariantCulture.NumberFormat);
            }
        }

        private void carica_prodotti(string testoc = "", string colonnac = "", int search = 0)
        {
            MessageBox.Show("Caricamento prodotti in corso...");
            prod_data.Rows.Clear();
            string[] prodotti = var.carica_prodotti();
            for (int i = 0; i < prodotti.Length; i++)
            {
                XmlDocument xml_doc = new XmlDocument();
                xml_doc.Load(prodotti[i]);
                XmlNode prodotto = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/prodotto");
                XmlNode cod1 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod1");
                XmlNode cod2 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod2");
                XmlNode cod3 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod3");
                XmlNode cod4 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod4");
                XmlNode cod5 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod5");
                XmlNode cod6 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod6");
                XmlNode cod7 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod7");
                XmlNode cod8 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod8");
                XmlNode cod9 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod9");
                XmlNode cod10 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod10");
                XmlNode cod11 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod11");
                XmlNode cod12 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod12");
                XmlNode cod13 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod13");
                XmlNode cod14 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod14");
                XmlNode cod15 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/cod15");
                XmlNode qt1 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt1");
                XmlNode qt2 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt2");
                XmlNode qt3 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt3");
                XmlNode qt4 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt4");
                XmlNode qt5 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt5");
                XmlNode qt6 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt6");
                XmlNode qt7 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt7");
                XmlNode qt8 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt8");
                XmlNode qt9 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt9");
                XmlNode qt10 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt10");
                XmlNode qt11 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt11");
                XmlNode qt12 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt12");
                XmlNode qt13 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt13");
                XmlNode qt14 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt14");
                XmlNode qt15 = xml_doc.DocumentElement.SelectSingleNode("/Prodotto/qt15");

                string[] riga = { prodotto.InnerText, "", cod1.InnerText, cod2.InnerText, cod3.InnerText, cod4.InnerText, cod5.InnerText, cod6.InnerText, cod7.InnerText, cod8.InnerText, cod9.InnerText, cod10.InnerText, cod11.InnerText, cod12.InnerText, cod13.InnerText, cod14.InnerText, cod15.InnerText, qt1.InnerText, qt2.InnerText, qt3.InnerText, qt4.InnerText, qt5.InnerText, qt6.InnerText, qt7.InnerText, qt8.InnerText, qt9.InnerText, qt10.InnerText, qt11.InnerText, qt12.InnerText, qt13.InnerText, qt14.InnerText, qt15.InnerText };
                //MessageBox.Show(codici[i].Replace(var.db+@"Magazzino\", ""));
                string contenuto = "";
                switch (colonnac)
                {
                    case "Prodotto":
                        contenuto = prodotto.InnerText;
                        break;
                    default:
                        contenuto = "";
                        break;
                }
                contenuto = contenuto.ToLower();
                testoc = testoc.ToLower();
                if (contenuto.Contains(testoc))
                   prod_data.Rows.Add(riga);
            }
        }
        private void magazzino_btn_Click(object sender, EventArgs e)
        {
            nordini_pnl.Visible = false;
            totfat_pnl.Visible = false;
            totfat_pic.Visible = false;
            totord_pic.Visible = false;
            logo_pic.Visible = false;
            settings_pnl.Visible = false;
            desktop_pnl.Visible = true;
            bar_pnl.Visible = true;
            ordini_data.Visible = false;
            magazzino_data.Visible = true;
            prod_data.Visible = false;
            magazzino = true;
            prod = false;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Codice");
            comboBox1.Items.Add("Prezzo");
            comboBox1.Items.Add("Quantit�");
            comboBox1.Items.Add("Descrizione");
            comboBox1.SelectedIndex = 0;
            if (codicicaricati == false || mostra_avviso == true)
            {
                carica_codici();
                codicicaricati = true;
            }
            avv_mostrati = true;

        }

        private void ordini_btn_Click(object sender, EventArgs e)
        {
            nordini_pnl.Visible = false;
            totfat_pnl.Visible = false;
            totfat_pic.Visible = false;
            totord_pic.Visible = false;
            logo_pic.Visible = false;
            settings_pnl.Visible = false;
            desktop_pnl.Visible = true;
            bar_pnl.Visible = true;
            ordini_data.Visible = true;
            magazzino_data.Visible = false;
            prod_data.Visible = false;
            magazzino = false;
            prod = false;
            avv_mostrati = false;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Ordine");
            comboBox1.Items.Add("Prezzo");
            comboBox1.Items.Add("Cliente");
            comboBox1.Items.Add("Note");
            comboBox1.SelectedIndex = 0;
            if (ordinicaricati == false)
            {
                carica_ordini();
                ordinicaricati = true;
            }

        }
        private void prod_btn_Click(object sender, EventArgs e)
        {
            nordini_pnl.Visible = false;
            totfat_pnl.Visible = false;
            totfat_pic.Visible = false;
            totord_pic.Visible = false;
            logo_pic.Visible = false;
            prod_data.Visible = true;
            magazzino_data.Visible = false;
            ordini_data.Visible = false;
            settings_pnl.Visible = false;
            desktop_pnl.Visible = true;
            bar_pnl.Visible = true;
            magazzino = false;
            prod = true;
            avv_mostrati = false;
            comboBox1.Items.Clear();
            comboBox1.Items.Add("Prodotto");
            comboBox1.Items.Add("Descrizione");
            comboBox1.SelectedIndex = 0;
            if (prodotticaricati == false)
            {
                carica_prodotti();
                prodotticaricati = true;
            }
        }
        private void home_btn_Click(object sender, EventArgs e)
        {
            totfat_lbl.Text = totalefatturato.ToString("0.00") + " �";
            nordini_lbl.Text = var.cno().ToString();
            settings_pnl.Visible = false;
            desktop_pnl.Visible = true;
            bar_pnl.Visible = false;
            ordini_data.Visible = false;
            magazzino_data.Visible = false;
            prod_data.Visible = false;
            avv_mostrati = false;
            nordini_pnl.Visible = true;
            totfat_pnl.Visible = true;
            totfat_pic.Visible = true;
            totord_pic.Visible = true;
            logo_pic.Visible = true;
        }
        private void impostazioni_btn_Click(object sender, EventArgs e)
        {
            desktop_pnl.Visible = false;
            bar_pnl.Visible = false;
            settings_pnl.Visible = true;
            avv_mostrati = false;
        }

        private void plus_btn_Click_1(object sender, EventArgs e)
        {
            if (magazzino == true)
            {
                Aggiungi_Codice Nuovo_Codice = new Aggiungi_Codice();
                Nuovo_Codice.FormClosing += new FormClosingEventHandler(closeform);
                Nuovo_Codice.ShowDialog();
                if(Nuovo_Codice.DialogResult == DialogResult.Yes)
                    codicicaricati = false;
            }
            else if (prod == true)
            {
                Aggiungi_Prodotto Nuovo_Prodotto = new Aggiungi_Prodotto();
                Nuovo_Prodotto.FormClosing += new FormClosingEventHandler(closeform);
                Nuovo_Prodotto.ShowDialog();
                if(Nuovo_Prodotto.DialogResult == DialogResult.Yes)
                    prodotticaricati = false;
            }
            else
            {
                Aggiungi_Ordine Nuovo_Ordine = new Aggiungi_Ordine();
                Nuovo_Ordine.FormClosing += new FormClosingEventHandler(closeform);
                Nuovo_Ordine.ShowDialog();
                if(Nuovo_Ordine.DialogResult == DialogResult.Yes)
                    ordinicaricati = false;
            }
        }

        private void edit_btn_Click_1(object sender, EventArgs e)
        {
            if (magazzino == true)
            {
                Aggiungi_Codice Nuovo_Codice = new Aggiungi_Codice();
                Nuovo_Codice.FormClosing += new FormClosingEventHandler(closeform);
                Nuovo_Codice.varCod = magazzino_data.Rows[magazzino_data.CurrentCell.RowIndex].Cells[0].Value.ToString();
                Nuovo_Codice.varPrz = magazzino_data.Rows[magazzino_data.CurrentCell.RowIndex].Cells[1].Value.ToString();
                Nuovo_Codice.varQt = magazzino_data.Rows[magazzino_data.CurrentCell.RowIndex].Cells[2].Value.ToString();
                Nuovo_Codice.varQtmin = magazzino_data.Rows[magazzino_data.CurrentCell.RowIndex].Cells[3].Value.ToString();
                Nuovo_Codice.varDes = magazzino_data.Rows[magazzino_data.CurrentCell.RowIndex].Cells[4].Value.ToString();
                Nuovo_Codice.Modifica = 1;
                Nuovo_Codice.ShowDialog();
            }
            else if (prod == true)
            {
                Aggiungi_Prodotto Nuovo_Prodotto = new Aggiungi_Prodotto();
                Nuovo_Prodotto.FormClosing += new FormClosingEventHandler(closeform);
                Nuovo_Prodotto.varProdotto = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[0].Value.ToString();
                Nuovo_Prodotto.varDescrizione = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[1].Value.ToString();
                Nuovo_Prodotto.varCod1 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[2].Value.ToString();
                Nuovo_Prodotto.varCod2 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[3].Value.ToString();
                Nuovo_Prodotto.varCod3 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[4].Value.ToString();
                Nuovo_Prodotto.varCod4 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[5].Value.ToString();
                Nuovo_Prodotto.varCod5 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[6].Value.ToString();
                Nuovo_Prodotto.varCod6 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[7].Value.ToString();
                Nuovo_Prodotto.varCod7 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[8].Value.ToString();
                Nuovo_Prodotto.varCod8 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[9].Value.ToString();
                Nuovo_Prodotto.varCod9 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[10].Value.ToString();
                Nuovo_Prodotto.varCod10 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[11].Value.ToString();
                Nuovo_Prodotto.varCod11 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[12].Value.ToString();
                Nuovo_Prodotto.varCod12 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[13].Value.ToString();
                Nuovo_Prodotto.varCod13 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[14].Value.ToString();
                Nuovo_Prodotto.varCod14 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[15].Value.ToString();
                Nuovo_Prodotto.varCod15 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[16].Value.ToString();
                Nuovo_Prodotto.varQt1 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[17].Value.ToString();
                Nuovo_Prodotto.varQt2 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[18].Value.ToString();
                Nuovo_Prodotto.varQt3 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[19].Value.ToString();
                Nuovo_Prodotto.varQt4 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[20].Value.ToString();
                Nuovo_Prodotto.varQt5 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[21].Value.ToString();
                Nuovo_Prodotto.varQt6 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[22].Value.ToString();
                Nuovo_Prodotto.varQt7 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[23].Value.ToString();
                Nuovo_Prodotto.varQt8 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[24].Value.ToString();
                Nuovo_Prodotto.varQt9 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[25].Value.ToString();
                Nuovo_Prodotto.varQt10 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[26].Value.ToString();
                Nuovo_Prodotto.varQt11 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[27].Value.ToString();
                Nuovo_Prodotto.varQt12 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[28].Value.ToString();
                Nuovo_Prodotto.varQt13 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[29].Value.ToString();
                Nuovo_Prodotto.varQt14 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[30].Value.ToString();
                Nuovo_Prodotto.varQt15 = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[31].Value.ToString();
                Nuovo_Prodotto.Modifica = 1;

                Nuovo_Prodotto.ShowDialog();
            }
            else
            {
                Aggiungi_Ordine Nuovo_Ordine = new Aggiungi_Ordine();
                Nuovo_Ordine.FormClosing += new FormClosingEventHandler(closeform);
                Nuovo_Ordine.varOrdine = ordini_data.Rows[ordini_data.CurrentCell.RowIndex].Cells[0].Value.ToString();
                Nuovo_Ordine.varPrz = ordini_data.Rows[ordini_data.CurrentCell.RowIndex].Cells[1].Value.ToString();
                Nuovo_Ordine.varCliente = ordini_data.Rows[ordini_data.CurrentCell.RowIndex].Cells[2].Value.ToString();
                Nuovo_Ordine.varNote = ordini_data.Rows[ordini_data.CurrentCell.RowIndex].Cells[3].Value.ToString();
                Nuovo_Ordine.Modifica = 1;

                Nuovo_Ordine.ShowDialog();
            }
        }

        private void del_btn_Click(object sender, EventArgs e)
        {
            if (magazzino == true)
            {
                string codice = magazzino_data.Rows[magazzino_data.CurrentCell.RowIndex].Cells[0].Value.ToString();
                DialogResult Scelta = MessageBox.Show("Sei sicuro di voler eliminare " + codice, "Eliminazione codice", MessageBoxButtons.YesNo);
                if (Scelta == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(var.db + @"Magazzino\" + codice + ".xml");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, " Impossibile eliminare il codice");
                    }
                    carica_codici();
                }
            }else if (prod == true)
            {
                string prodotto = prod_data.Rows[prod_data.CurrentCell.RowIndex].Cells[0].Value.ToString();
                DialogResult Scelta = MessageBox.Show("Sei sicuro di voler eliminare " + prodotto, "Eliminazione Prodotto", MessageBoxButtons.YesNo);
                if (Scelta == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(var.db + @"Prodotti\" + prodotto + ".xml");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, " Impossibile eliminare il prodotto");
                    }
                    carica_prodotti();
                }
            }
            else
            {
                string ordine = ordini_data.Rows[ordini_data.CurrentCell.RowIndex].Cells[0].Value.ToString();
                DialogResult Scelta = MessageBox.Show("Sei sicuro di voler eliminare " + ordine, "Eliminazione ordine", MessageBoxButtons.YesNo);
                if (Scelta == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(var.db + @"Ordini\" + ordine + ".xml");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, " Impossibile eliminare l'ordine");
                    }
                    carica_ordini();
                }
            }
        }

        private void srch_btn_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length > 0)
            {
                if (magazzino == true)
                {
                    carica_codici(textBox1.Text, comboBox1.Text, 1);
                }
                else
                {
                    carica_ordini(textBox1.Text, comboBox1.Text);
                }
            }
            else
            {
                if (magazzino == true)
                    carica_codici();
                else
                    carica_ordini();
            }
        }

        private void ordini_data_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    ordini_data.CurrentCell = ordini_data.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    ordini_data.Rows[e.RowIndex].Selected = true;
                    ordini_data.Focus();
                    ordini_data.ContextMenuStrip = contextMenuStrip1;
                    Point posizioneContext = new Point(MousePosition.X, MousePosition.Y);
                    ordini_data.ContextMenuStrip.Show(posizioneContext);
                    ordini_data.ContextMenuStrip = null;
                }
                else
                    ordini_data.ContextMenuStrip = null;
            }
        }

        private void magazzino_data_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    magazzino_data.CurrentCell = magazzino_data.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    magazzino_data.Rows[e.RowIndex].Selected = true;
                    magazzino_data.Focus();
                    magazzino_data.ContextMenuStrip = contextMenuStrip1;
                    Point posizioneContext = new Point(MousePosition.X, MousePosition.Y);
                    magazzino_data.ContextMenuStrip.Show(posizioneContext);
                    magazzino_data.ContextMenuStrip = null;
                }
                else
                    magazzino_data.ContextMenuStrip = null;
            }
        }

        private void modificaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            edit_btn_Click_1(sender, e);
        }

        private void aggiungiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            plus_btn_Click_1(sender, e);
        }

        private void eliminaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            del_btn_Click(sender, e);
        }

        private void erase_btn_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            comboBox1.SelectedIndex = 0;
            if (magazzino == true)
                carica_codici();
            else
                carica_ordini();
        }
    }
}