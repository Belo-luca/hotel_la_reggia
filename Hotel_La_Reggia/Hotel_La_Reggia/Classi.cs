using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hotel_La_Reggia
{
    // Classe Cliente
    public class Cliente
    {
        private string _nome;
        private string _email;
        private string _cognome;
        private string _telefono;

        public Cliente(string nome, string cognome, string email, string telefono)
        {
            Nome = nome;
            Cognome = cognome;
            Email = email;
            Telefono = telefono;
        }

        public string Nome
        {
            get { return _nome; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Il nome non può essere vuoto.");
                _nome = value.Trim();
            }
        }

        public string Telefono
        {
            get { return _telefono; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^\+?[0-9]{7,15}$"))
                    throw new ArgumentException("Il numero di telefono non è valido.");
                _telefono = value;
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || !Regex.IsMatch(value, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new ArgumentException("L'email non è valida.");
                _email = value.ToLower().Trim();
            }
        }

        public string Cognome
        {
            get { return _cognome; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Il cognome non può essere vuoto.");
                _cognome = value.Trim();
            }
        }

        public override string ToString()
        {
            return $"{Nome} {Cognome} - Email: {Email}, Tel: {Telefono}";
        }
    }

    // Classe Camera
    public class Camera
    {
        private int _idStanza;
        private int _numeroCamera;
        private int _numeroLetti;
        private decimal _prezzoPerNotte;
        private bool _occupata;

        public Camera(int idStanza, int numeroCamera, int numeroLetti, decimal prezzoPerNotte)
        {
            IdStanza = idStanza;
            NumeroCamera = numeroCamera;
            NumeroLetti = numeroLetti;
            PrezzoPerNotte = prezzoPerNotte;
            _occupata = false;
        }

        public int IdStanza
        {
            get { return _idStanza; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("L'ID stanza deve essere positivo.");
                _idStanza = value;
            }
        }

        public int NumeroCamera
        {
            get { return _numeroCamera; }
            set
            {
                if (value <= 0 || value > 10)
                    throw new ArgumentException("Il numero camera deve essere tra 1 e 10.");
                _numeroCamera = value;
            }
        }

        public int NumeroLetti
        {
            get { return _numeroLetti; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Il numero di letti deve essere positivo.");
                _numeroLetti = value;
            }
        }

        public decimal PrezzoPerNotte
        {
            get { return _prezzoPerNotte; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("Il prezzo non può essere negativo.");
                _prezzoPerNotte = value;
            }
        }

        public bool Occupata
        {
            get { return _occupata; }
            set { _occupata = value; }
        }

        public override string ToString()
        {
            string stato = Occupata ? "Occupata" : "Libera";
            return $"Camera {NumeroCamera} (ID: {IdStanza}) - {NumeroLetti} letti - €{PrezzoPerNotte}/notte - Stato: {stato}";
        }
    }

    // Classe Prenotazione
    public class Prenotazione
    {
        private static int _contatore = 1;
        private DateTime _dataInizio;
        private DateTime _dataFine;
        private Cliente[] _ospiti;
        private int _numeroOspiti;

        public Prenotazione(Cliente[] ospiti, int numeroOspiti, Camera camera, DateTime dataInizio, DateTime dataFine)
        {
            IdPrenotazione = _contatore++;
            _ospiti = new Cliente[numeroOspiti];
            Array.Copy(ospiti, _ospiti, numeroOspiti);
            _numeroOspiti = numeroOspiti;

            ClienteIntestatario = ospiti[0]; // Il primo è l'intestatario
            Camera = camera ?? throw new ArgumentNullException(nameof(camera));
            DataInizio = dataInizio;
            DataFine = dataFine;
            DataPrenotazione = DateTime.Now;
        }

        public int IdPrenotazione { get; private set; }
        public Cliente ClienteIntestatario { get; private set; }
        public Camera Camera { get; private set; }
        public DateTime DataPrenotazione { get; private set; }
        public int NumeroOspiti { get { return _numeroOspiti; } }

        public DateTime DataInizio
        {
            get { return _dataInizio; }
            set
            {
                if (value.Date < DateTime.Today)
                    throw new ArgumentException("La data di inizio non può essere antecedente a oggi.");
                _dataInizio = value;
            }
        }

        public DateTime DataFine
        {
            get { return _dataFine; }
            set
            {
                if (value.Date < _dataInizio.Date)
                    throw new ArgumentException("La data di fine non può essere antecedente alla data di inizio.");
                _dataFine = value;
            }
        }

        public int NumeroNotti
        {
            get { return (DataFine.Date - DataInizio.Date).Days; }
        }

        public decimal CalcolaCostoTotale()
        {
            return NumeroNotti * Camera.PrezzoPerNotte;
        }

        public Cliente[] GetOspiti()
        {
            Cliente[] copia = new Cliente[_numeroOspiti];
            Array.Copy(_ospiti, copia, _numeroOspiti);
            return copia;
        }

        public override string ToString()
        {
            string altriOspiti = _numeroOspiti > 1 ? $" + {_numeroOspiti - 1} ospiti" : "";
            return $"Prenotazione #{IdPrenotazione} - {ClienteIntestatario.Nome} {ClienteIntestatario.Cognome}{altriOspiti} - Camera {Camera.NumeroCamera} - Dal {DataInizio:dd/MM/yyyy} al {DataFine:dd/MM/yyyy} ({NumeroNotti} notti) - Totale: €{CalcolaCostoTotale():F2}";
        }
    }

    // Classe GestoreHotel - il cuore del sistema
    public class GestoreHotel
    {
        private Cliente[] _clienti;
        private Camera[] _camere;
        private Prenotazione[] _prenotazioni;
        private int _contatoreClienti;
        private int _contatorePrenotazioni;

        public GestoreHotel()
        {
            _clienti = new Cliente[100]; // Capacità massima 100 clienti
            _camere = new Camera[10]; // 10 camere fisse
            _prenotazioni = new Prenotazione[500]; // Capacità massima 500 prenotazioni
            _contatoreClienti = 0;
            _contatorePrenotazioni = 0;
        }

        // Gestione Clienti
        public void AggiungiCliente(Cliente cliente)
        {
            if (cliente == null)
                throw new ArgumentNullException(nameof(cliente));

            if (_contatoreClienti >= _clienti.Length)
                throw new InvalidOperationException("Capacità massima clienti raggiunta.");

            // Verifica email duplicata
            for (int i = 0; i < _contatoreClienti; i++)
            {
                if (_clienti[i].Email.Equals(cliente.Email, StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Esiste già un cliente con questa email.");
            }

            _clienti[_contatoreClienti] = cliente;
            _contatoreClienti++;
        }

        public Cliente TrovaCliente(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
                return null;

            criterio = criterio.ToLower().Trim();

            for (int i = 0; i < _contatoreClienti; i++)
            {
                if (_clienti[i].Nome.ToLower().Contains(criterio) ||
                    _clienti[i].Cognome.ToLower().Contains(criterio) ||
                    _clienti[i].Email.ToLower().Contains(criterio) ||
                    _clienti[i].Telefono.Contains(criterio))
                {
                    return _clienti[i];
                }
            }
            return null;
        }

        public Cliente[] TrovaTuttiClienti(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
                return new Cliente[0];

            criterio = criterio.ToLower().Trim();
            Cliente[] risultati = new Cliente[_contatoreClienti];
            int count = 0;

            for (int i = 0; i < _contatoreClienti; i++)
            {
                if (_clienti[i].Nome.ToLower().Contains(criterio) ||
                    _clienti[i].Cognome.ToLower().Contains(criterio) ||
                    _clienti[i].Email.ToLower().Contains(criterio) ||
                    _clienti[i].Telefono.Contains(criterio))
                {
                    risultati[count] = _clienti[i];
                    count++;
                }
            }

            Cliente[] risultatiFinali = new Cliente[count];
            Array.Copy(risultati, risultatiFinali, count);
            return risultatiFinali;
        }

        // Gestione Camere
        public void InizializzaCamereHotel()
        {
            if (_camere[0] != null)
                return; // Già inizializzate

            _camere[0] = new Camera(1, 1, 1, 50.00m);
            _camere[1] = new Camera(2, 2, 1, 50.00m);
            _camere[2] = new Camera(3, 3, 2, 70.00m);
            _camere[3] = new Camera(4, 4, 2, 70.00m);
            _camere[4] = new Camera(5, 5, 2, 70.00m);
            _camere[5] = new Camera(6, 6, 3, 90.00m);
            _camere[6] = new Camera(7, 7, 3, 90.00m);
            _camere[7] = new Camera(8, 8, 4, 110.00m);
            _camere[8] = new Camera(9, 9, 4, 110.00m);
            _camere[9] = new Camera(10, 10, 5, 150.00m);
        }

        public Camera TrovaCameraPerIdStanza(int idStanza)
        {
            for (int i = 0; i < _camere.Length; i++)
            {
                if (_camere[i] != null && _camere[i].IdStanza == idStanza)
                    return _camere[i];
            }
            return null;
        }

        public Camera TrovaCameraPerNumero(int numeroCamera)
        {
            for (int i = 0; i < _camere.Length; i++)
            {
                if (_camere[i] != null && _camere[i].NumeroCamera == numeroCamera)
                    return _camere[i];
            }
            return null;
        }

        public Camera[] TrovaCamereLibere()
        {
            Camera[] risultati = new Camera[10];
            int count = 0;

            for (int i = 0; i < _camere.Length; i++)
            {
                if (_camere[i] != null && !_camere[i].Occupata)
                {
                    risultati[count] = _camere[i];
                    count++;
                }
            }

            Camera[] risultatiFinali = new Camera[count];
            Array.Copy(risultati, risultatiFinali, count);
            return risultatiFinali;
        }

        public Camera[] TrovaCamereLiberePerLetti(int numeroLetti)
        {
            Camera[] risultati = new Camera[10];
            int count = 0;

            for (int i = 0; i < _camere.Length; i++)
            {
                if (_camere[i] != null && !_camere[i].Occupata && _camere[i].NumeroLetti >= numeroLetti)
                {
                    risultati[count] = _camere[i];
                    count++;
                }
            }

            Camera[] risultatiFinali = new Camera[count];
            Array.Copy(risultati, risultatiFinali, count);
            return risultatiFinali;
        }

        public Camera[] GetTutteCamere()
        {
            Camera[] risultati = new Camera[10];
            Array.Copy(_camere, risultati, 10);
            return risultati;
        }

        // Gestione Prenotazioni
        public Prenotazione CreaPrenotazione(Cliente[] ospiti, int numeroOspiti, Camera camera, DateTime dataInizio, DateTime dataFine)
        {
            if (ospiti == null || numeroOspiti == 0)
                throw new ArgumentException("Deve esserci almeno un ospite.");
            if (camera == null)
                throw new ArgumentNullException(nameof(camera));

            if (_contatorePrenotazioni >= _prenotazioni.Length)
                throw new InvalidOperationException("Capacità massima prenotazioni raggiunta.");

            if (camera.Occupata)
                throw new InvalidOperationException("La camera è già occupata.");

            // Verifica che il numero di ospiti non superi i letti disponibili
            if (numeroOspiti > camera.NumeroLetti)
                throw new InvalidOperationException($"La camera ha solo {camera.NumeroLetti} letti disponibili.");

            // Aggiungi tutti gli ospiti alla lista clienti (se non esistono già)
            for (int i = 0; i < numeroOspiti; i++)
            {
                bool esisteGia = false;
                for (int j = 0; j < _contatoreClienti; j++)
                {
                    if (_clienti[j].Email.Equals(ospiti[i].Email, StringComparison.OrdinalIgnoreCase))
                    {
                        esisteGia = true;
                        break;
                    }
                }

                if (!esisteGia)
                {
                    if (_contatoreClienti >= _clienti.Length)
                        throw new InvalidOperationException("Capacità massima clienti raggiunta.");

                    _clienti[_contatoreClienti] = ospiti[i];
                    _contatoreClienti++;
                }
            }

            var prenotazione = new Prenotazione(ospiti, numeroOspiti, camera, dataInizio, dataFine);
            _prenotazioni[_contatorePrenotazioni] = prenotazione;
            _contatorePrenotazioni++;

            camera.Occupata = true;

            return prenotazione;
        }

        public Prenotazione TrovaPrenotazione(int idPrenotazione)
        {
            for (int i = 0; i < _contatorePrenotazioni; i++)
            {
                if (_prenotazioni[i].IdPrenotazione == idPrenotazione)
                    return _prenotazioni[i];
            }
            return null;
        }

        public Prenotazione[] TrovaPrenotazioniCliente(string criterio)
        {
            if (string.IsNullOrWhiteSpace(criterio))
                return new Prenotazione[0];

            criterio = criterio.ToLower().Trim();
            Prenotazione[] risultati = new Prenotazione[_contatorePrenotazioni];
            int count = 0;

            for (int i = 0; i < _contatorePrenotazioni; i++)
            {
                Cliente c = _prenotazioni[i].ClienteIntestatario;
                if (c.Nome.ToLower().Contains(criterio) ||
                    c.Cognome.ToLower().Contains(criterio) ||
                    c.Email.ToLower().Contains(criterio) ||
                    c.Telefono.Contains(criterio))
                {
                    risultati[count] = _prenotazioni[i];
                    count++;
                }
            }

            Prenotazione[] risultatiFinali = new Prenotazione[count];
            Array.Copy(risultati, risultatiFinali, count);
            return risultatiFinali;
        }

        public Prenotazione[] TrovaPrenotazioniCamera(int numeroCamera)
        {
            Prenotazione[] risultati = new Prenotazione[_contatorePrenotazioni];
            int count = 0;

            for (int i = 0; i < _contatorePrenotazioni; i++)
            {
                if (_prenotazioni[i].Camera.NumeroCamera == numeroCamera)
                {
                    risultati[count] = _prenotazioni[i];
                    count++;
                }
            }

            Prenotazione[] risultatiFinali = new Prenotazione[count];
            Array.Copy(risultati, risultatiFinali, count);
            return risultatiFinali;
        }

        public bool EliminaPrenotazione(int idPrenotazione)
        {
            for (int i = 0; i < _contatorePrenotazioni; i++)
            {
                if (_prenotazioni[i].IdPrenotazione == idPrenotazione)
                {
                    // Libera la camera
                    _prenotazioni[i].Camera.Occupata = false;

                    // Sposta tutti gli elementi successivi indietro di una posizione
                    for (int j = i; j < _contatorePrenotazioni - 1; j++)
                    {
                        _prenotazioni[j] = _prenotazioni[j + 1];
                    }
                    _prenotazioni[_contatorePrenotazioni - 1] = null;
                    _contatorePrenotazioni--;
                    return true;
                }
            }
            return false;
        }

        public bool ModificaPrenotazione(int idPrenotazione, DateTime nuovaDataInizio, DateTime nuovaDataFine)
        {
            var prenotazione = TrovaPrenotazione(idPrenotazione);
            if (prenotazione == null)
                return false;

            prenotazione.DataInizio = nuovaDataInizio;
            prenotazione.DataFine = nuovaDataFine;

            return true;
        }

        // Statistiche
        public Prenotazione[] GetPrenotazioniAttive()
        {
            var oggi = DateTime.Today;
            Prenotazione[] risultati = new Prenotazione[_contatorePrenotazioni];
            int count = 0;

            for (int i = 0; i < _contatorePrenotazioni; i++)
            {
                if (_prenotazioni[i].DataInizio <= oggi && _prenotazioni[i].DataFine >= oggi)
                {
                    risultati[count] = _prenotazioni[i];
                    count++;
                }
            }

            Prenotazione[] risultatiFinali = new Prenotazione[count];
            Array.Copy(risultati, risultatiFinali, count);
            return risultatiFinali;
        }

        public Prenotazione[] GetPrenotazioniFuture()
        {
            Prenotazione[] risultati = new Prenotazione[_contatorePrenotazioni];
            int count = 0;

            for (int i = 0; i < _contatorePrenotazioni; i++)
            {
                if (_prenotazioni[i].DataInizio > DateTime.Today)
                {
                    risultati[count] = _prenotazioni[i];
                    count++;
                }
            }

            Prenotazione[] risultatiFinali = new Prenotazione[count];
            Array.Copy(risultati, risultatiFinali, count);
            return risultatiFinali;
        }

        public decimal CalcolaRicavoTotale()
        {
            decimal totale = 0;
            for (int i = 0; i < _contatorePrenotazioni; i++)
            {
                totale += _prenotazioni[i].CalcolaCostoTotale();
            }
            return totale;
        }

        public Cliente[] GetTuttiClienti()
        {
            Cliente[] risultati = new Cliente[_contatoreClienti];
            Array.Copy(_clienti, risultati, _contatoreClienti);
            return risultati;
        }

        public Prenotazione[] GetTuttePrenotazioni()
        {
            Prenotazione[] risultati = new Prenotazione[_contatorePrenotazioni];
            Array.Copy(_prenotazioni, risultati, _contatorePrenotazioni);
            return risultati;
        }

        public int GetNumeroClienti()
        {
            return _contatoreClienti;
        }

        public int GetNumeroPrenotazioni()
        {
            return _contatorePrenotazioni;
        }
    }
}