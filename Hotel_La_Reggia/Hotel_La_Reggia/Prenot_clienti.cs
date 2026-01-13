using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hotel_La_Reggia
{
    public partial class Prenot_clienti : Form
    {
            private GestoreHotel gestore;

            // Variabili per Step 1
            private DateTime dataInizio;
            private DateTime dataFine;
            private int numeroPersone;
            private Camera cameraSelezionata;

            // Variabili per Step 2
            private Cliente[] ospiti;
            private int indiceOspiteCorrente = 0;

            public Prenot_clienti(GestoreHotel gestoreHotel)
            {
                InitializeComponent();
                gestore = gestoreHotel;
            }

        private void CalcolaNotti()
            {
                if (dataInizio < dataFine)
                {
                    int notti = (dataFine.Date - dataInizio.Date).Days;
                }
            }

            private void BtnSelezionaCamera_Click(object sender, EventArgs e)
            {
                // Ottieni le date e numero persone dai controlli
                // dataInizio = dtpCheckIn.Value;
                // dataFine = dtpCheckOut.Value;
                // numeroPersone = (int)numPersone.Value;

                // Trova camere disponibili con abbastanza letti
                Camera[] camereLibere = gestore.TrovaCamereLiberePerLetti(numeroPersone);

                if (camereLibere.Length == 0)
                {
                    MessageBox.Show("Nessuna camera disponibile per il numero di persone selezionato.",
                                  "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // QUI: Mostra le camere disponibili all'utente (ListBox, ComboBox, altro Form, ecc.)
                // L'utente seleziona una camera e la salvi in cameraSelezionata

                // Esempio: prendi la prima disponibile (DA SOSTITUIRE CON LA TUA LOGICA)
                cameraSelezionata = camereLibere[0];

                // Aggiorna l'UI per mostrare quale camera è stata selezionata
                // lblCameraSelezionata.Text = $"Camera {cameraSelezionata.NumeroCamera}...";
            }

            private void BtnAvanti_Click(object sender, EventArgs e)
            {
                // Ottieni i valori dai controlli (DateTimePicker, NumericUpDown)
                // dataInizio = dtpCheckIn.Value;
                // dataFine = dtpCheckOut.Value;
                // numeroPersone = (int)numPersone.Value;

                // Validazione date
                if (dataFine <= dataInizio)
                {
                    MessageBox.Show("La data di check-out deve essere successiva al check-in!",
                                  "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (dataInizio < DateTime.Today)
                {
                    MessageBox.Show("La data di check-in non può essere nel passato!",
                                  "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (cameraSelezionata == null)
                {
                    MessageBox.Show("Seleziona una camera prima di proseguire!",
                                  "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Verifica che la camera abbia abbastanza letti
                if (numeroPersone > cameraSelezionata.NumeroLetti)
                {
                    MessageBox.Show($"La camera selezionata ha solo {cameraSelezionata.NumeroLetti} letti!",
                                  "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Inizializza array ospiti
                ospiti = new Cliente[numeroPersone];
                indiceOspiteCorrente = 0;

                // Passa allo step 2 (nascondere panel1, mostrare panel2)
                MostraStep2();
            }

            // ===== METODI STEP 2 =====

            private void BtnProssimoOspite_Click(object sender, EventArgs e)
            {
                // Ottieni i valori dalle TextBox
                string nome = ""; // txtNome.Text
                string cognome = ""; // txtCognome.Text
                string email = ""; // txtEmail.Text
                string telefono = ""; // txtTelefono.Text

                // Validazione campi vuoti
                if (string.IsNullOrWhiteSpace(nome))
                {
                    MessageBox.Show("Inserisci il nome!", "Attenzione",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(cognome))
                {
                    MessageBox.Show("Inserisci il cognome!", "Attenzione",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show("Inserisci l'email!", "Attenzione",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(telefono))
                {
                    MessageBox.Show("Inserisci il telefono!", "Attenzione",
                                  MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    // Crea cliente (questo farà anche la validazione di email e telefono)
                    Cliente cliente = new Cliente(nome, cognome, email, telefono);

                    // Salva nell'array
                    ospiti[indiceOspiteCorrente] = cliente;
                    indiceOspiteCorrente++;

                    // Verifica se ci sono altri ospiti da inserire
                    if (indiceOspiteCorrente < numeroPersone)
                    {
                        // Pulisci i campi per il prossimo ospite
                        // txtNome.Clear();
                        // txtCognome.Clear();
                        // txtEmail.Clear();
                        // txtTelefono.Clear();

                        // Aggiorna la label che mostra quale ospite si sta inserendo
                        AggiornaLabelOspite();

                        // Se è l'ultimo ospite, cambia il testo del bottone
                        if (indiceOspiteCorrente == numeroPersone - 1)
                        {
                            // btnProssimoOspite.Text = "Concludi Prenotazione";
                        }
                    }
                    else
                    {
                        // Tutti gli ospiti sono stati inseriti, crea la prenotazione finale
                        CreaPrenotazioneFinale();
                    }
                }
                catch (ArgumentException ex)
                {
                    // Errore di validazione (email o telefono non validi)
                    MessageBox.Show($"Errore nei dati inseriti: {ex.Message}", "Errore",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void AggiornaLabelOspite()
            {
                // Aggiorna la label per mostrare quale ospite si sta inserendo
                string ruolo = indiceOspiteCorrente == 0 ? " (Intestatario)" : "";
                // lblOspiteNumero.Text = $"OSPITE {indiceOspiteCorrente + 1}/{numeroPersone}{ruolo}";
            }

            private void CreaPrenotazioneFinale()
            {
                try
                {
                    // Crea la prenotazione con tutti gli ospiti
                    Prenotazione prenotazione = gestore.CreaPrenotazione(
                        ospiti, numeroPersone, cameraSelezionata, dataInizio, dataFine);

                    // Crea messaggio con lista ospiti
                    string messaggioOspiti = "";
                    for (int i = 0; i < numeroPersone; i++)
                    {
                        messaggioOspiti += $"\n  {i + 1}. {ospiti[i].Nome} {ospiti[i].Cognome}";
                    }

                    // Mostra riepilogo finale
                    MessageBox.Show(
                        $"✅ PRENOTAZIONE CONFERMATA!\n\n" +
                        $"ID Prenotazione: #{prenotazione.IdPrenotazione}\n" +
                        $"Camera: {cameraSelezionata.NumeroCamera} ({cameraSelezionata.NumeroLetti} letti)\n" +
                        $"Check-in: {dataInizio:dd/MM/yyyy}\n" +
                        $"Check-out: {dataFine:dd/MM/yyyy}\n" +
                        $"Numero notti: {prenotazione.NumeroNotti}\n" +
                        $"Ospiti:{messaggioOspiti}\n\n" +
                        $"TOTALE: €{prenotazione.CalcolaCostoTotale():F2}",
                        "Prenotazione Confermata",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );

                    // Chiudi il form
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Errore durante la creazione della prenotazione: {ex.Message}",
                                  "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            private void BtnIndietro_Click(object sender, EventArgs e)
            {
                // Torna allo step 1 (con conferma)
                if (MessageBox.Show("Tornare indietro? I dati degli ospiti inseriti andranno persi.",
                                  "Conferma", MessageBoxButtons.YesNo,
                                  MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    // Reset variabili
                    ospiti = null;
                    indiceOspiteCorrente = 0;

                    MostraStep1();
                }
            }

            // ===== METODI DI NAVIGAZIONE =====

            private void MostraStep1()
            {
                // Nasconde panel step 2, mostra panel step 1
                // panelStep1.Visible = true;
                // panelStep2.Visible = false;
            }

            private void MostraStep2()
            {
                // Nasconde panel step 1, mostra panel step 2
                // panelStep1.Visible = false;
                // panelStep2.Visible = true;

                // Imposta la label del primo ospite
                AggiornaLabelOspite();

                // Imposta il testo del bottone
                // btnProssimoOspite.Text = numeroPersone > 1 ? "Prossimo Ospite →" : "Concludi Prenotazione";
            }
        }
    }