using System;
using System.Windows.Forms;

namespace Hotel_la_reggia
{
    public partial class Prenot_clienti : Form
    {
        private Hotel hotel;
        private Prenotazione prenotazione;
        private int clienteCorrente;

        public Prenot_clienti(Hotel hotel, Prenotazione prenotazione)
        {
            InitializeComponent();
            this.hotel = hotel;
            this.prenotazione = prenotazione;
            this.clienteCorrente = 1;

            AggiornaInterfaccia();
        }

        private void AggiornaInterfaccia()
        {
            label1.Text = $"Cliente {clienteCorrente} di {prenotazione.NumeroPersone}";

            // Mostra il pulsante "Concludi" solo all'ultimo cliente
            if (clienteCorrente == prenotazione.NumeroPersone)
            {
                button1.Visible = false;
                button2.Visible = true;
            }
            else
            {
                button1.Visible = true;
                button2.Visible = false;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Passa alla prossima persona
            try
            {
                bool isIntestatario = (clienteCorrente == 1);

                hotel.AggiungiClienteAPrenotazione(
                    prenotazione,
                    textBox2.Text,  // nome
                    textBox3.Text,  // cognome
                    textBox4.Text,  // email
                    textBox1.Text,  // telefono
                    isIntestatario
                );

                clienteCorrente++;

                // Pulisci i campi
                textBox1.Clear();
                textBox2.Clear();
                textBox3.Clear();
                textBox4.Clear();

                AggiornaInterfaccia();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Concludi la prenotazione
            try
            {
                bool isIntestatario = (clienteCorrente == 1);

                hotel.AggiungiClienteAPrenotazione(
                    prenotazione,
                    textBox2.Text,
                    textBox3.Text,
                    textBox4.Text,
                    textBox1.Text,
                    isIntestatario
                );

                hotel.ConfermaPrenotazione(prenotazione);

                MessageBox.Show("Prenotazione confermata con successo!", "Successo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}