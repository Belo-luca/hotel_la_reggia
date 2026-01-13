using System;
using System.Windows.Forms;

namespace Hotel_la_reggia
{
    public partial class Prenotazioni : Form
    {
        private Hotel hotel;
        private Prenotazione prenotazioneCorrente;

        public Prenotazioni()
        {
            InitializeComponent();
            hotel = new Hotel();
            dateTimePicker1.MinDate = DateTime.Today;
            dateTimePicker2.MinDate = DateTime.Today;
            AggiornaStatoCamere();
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            AggiornaStatoCamere();
        }

        private void AggiornaStatoCamere()
        {
            DateTime inizio = dateTimePicker1.Value.Date;
            DateTime fine = dateTimePicker2.Value.Date;
            int numeroPersone = (int)numericUpDown1.Value;

            Camera[] camere = hotel.GetTutteLeCamere();
            string[] stati = hotel.GetStatoCamere(inizio, fine);

            // Aggiorna le label di disponibilità
            // label6, label12, label18, label19, label24, label25, label30, label31, label36, label37, label13
            Label[] labelDisponibilita = { label13, label19, label6, label12, label18, label24, label25, label30, label31, label36, label37 };

            for (int i = 0; i < labelDisponibilita.Length && i < camere.Length; i++)
            {
                bool disponibile = camere[i].NumeroLetti >= numeroPersone && stati[i] == "Libera";
                labelDisponibilita[i].Text = disponibile ? "Libera" : "Occupata";
                labelDisponibilita[i].ForeColor = disponibile ? System.Drawing.Color.Green : System.Drawing.Color.Red;
            }
        }

        // Quando l'utente clicca su una camera (puoi aggiungere eventi Click alle righe)
        private void SelezionaCamera(int numeroCamera)
        {
            try
            {
                DateTime inizio = dateTimePicker1.Value.Date;
                DateTime fine = dateTimePicker2.Value.Date;
                int numeroPersone = (int)numericUpDown1.Value;

                prenotazioneCorrente = hotel.IniziaPrenotazione(numeroCamera, inizio, fine, numeroPersone);

                // Apri il form per inserire i clienti
                Prenot_clienti formClienti = new Prenot_clienti(hotel, prenotazioneCorrente);
                formClienti.ShowDialog();

                AggiornaStatoCamere();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}