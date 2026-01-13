using System;
using System.IO;
using System.Text;
using System.Linq;

namespace Hotel_la_reggia
{
    // CLASSE CLIENTE
    public class Cliente
    {
        private string _nome;
        private string _cognome;
        private string _email;
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
                _nome = value;
            }
        }

        public string Cognome
        {
            get { return _cognome; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Il cognome non può essere vuoto.");
                _cognome = value;
            }
        }

        public string Email
        {
            get { return _email; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("L'email non può essere vuota.");
                _email = value;
            }
        }

        public string Telefono
        {
            get { return _telefono; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Il telefono non può essere vuoto.");
                _telefono = value;
            }
        }

        public string NomeCompleto
        {
            get { return $"{Nome} {Cognome}"; }
        }

        public string ToJson()
        {
            return $"{{\"nome\":\"{Nome}\",\"cognome\":\"{Cognome}\",\"email\":\"{Email}\",\"telefono\":\"{Telefono}\"}}";
        }

        public static Cliente FromJson(string json)
        {
            json = json.Trim('{', '}');
            string[] parts = json.Split(',');

            string nome = ExtractValue(parts[0]);
            string cognome = ExtractValue(parts[1]);
            string email = ExtractValue(parts[2]);
            string telefono = ExtractValue(parts[3]);

            return new Cliente(nome, cognome, email, telefono);
        }

        private static string ExtractValue(string pair)
        {
            int colonIndex = pair.IndexOf(':');
            string value = pair.Substring(colonIndex + 1).Trim('"', ' ');
            return value;
        }
    }

    // CLASSE CAMERA
    public class Camera
    {
        private int _numeroCamera;
        private int _numeroPiano;
        private string _idCamera;
        private int _numeroLetti;

        public Camera(int numeroCamera, int numeroPiano, string idCamera, int numeroLetti)
        {
            _numeroCamera = numeroCamera;
            _numeroPiano = numeroPiano;
            _idCamera = idCamera;
            _numeroLetti = numeroLetti;
        }

        public int NumeroCamera { get { return _numeroCamera; } }
        public int NumeroPiano { get { return _numeroPiano; } }
        public string IdCamera { get { return _idCamera; } }
        public int NumeroLetti { get { return _numeroLetti; } }

        public bool IsDisponibile(DateTime inizio, DateTime fine, Prenotazione[] prenotazioni, int numPrenotazioni)
        {
            for (int i = 0; i < numPrenotazioni; i++)
            {
                if (prenotazioni[i].NumeroCamera == this.NumeroCamera)
                {
                    if (!(fine < prenotazioni[i].Inizio || inizio > prenotazioni[i].Fine))
                        return false;
                }
            }
            return true;
        }
    }

    // CLASSE PRENOTAZIONE
    public class Prenotazione
    {
        private static int _contatoreId = 1;
        private int _id;
        private int _numeroCamera;
        private DateTime _inizio;
        private DateTime _fine;
        private int _numeroPersone;
        private Cliente[] _clienti;
        private int _numeroClientiInseriti;
        private int _indiceIntestatario;

        public Prenotazione(int numeroCamera, DateTime inizio, DateTime fine, int numeroPersone)
        {
            _id = _contatoreId++;
            NumeroCamera = numeroCamera;
            Inizio = inizio;
            Fine = fine;
            NumeroPersone = numeroPersone;
            _clienti = new Cliente[numeroPersone];
            _numeroClientiInseriti = 0;
            _indiceIntestatario = -1;
        }

        public int Id { get { return _id; } }

        public int NumeroCamera
        {
            get { return _numeroCamera; }
            private set { _numeroCamera = value; }
        }

        public DateTime Inizio
        {
            get { return _inizio; }
            set
            {
                if (value.Date < DateTime.Today)
                    throw new ArgumentException("La data di inizio non può essere antecedente a oggi.");
                _inizio = value;
            }
        }

        public DateTime Fine
        {
            get { return _fine; }
            set
            {
                if (value.Date < _inizio.Date)
                    throw new ArgumentException("La data di fine non può essere antecedente alla data di inizio.");
                _fine = value;
            }
        }

        public int NumeroPersone
        {
            get { return _numeroPersone; }
            private set
            {
                if (value <= 0)
                    throw new ArgumentException("Il numero di persone deve essere maggiore di 0.");
                _numeroPersone = value;
            }
        }

        public Cliente[] Clienti { get { return _clienti; } }
        public int NumeroClientiInseriti { get { return _numeroClientiInseriti; } }
        public bool IsCompleta { get { return _numeroClientiInseriti == NumeroPersone; } }

        public Cliente Intestatario
        {
            get { return _indiceIntestatario >= 0 ? _clienti[_indiceIntestatario] : null; }
        }

        public void AggiungiCliente(Cliente cliente, bool isIntestatario = false)
        {
            if (_numeroClientiInseriti >= NumeroPersone)
                throw new InvalidOperationException($"Numero massimo di persone raggiunto ({NumeroPersone}).");

            _clienti[_numeroClientiInseriti] = cliente;

            if (_indiceIntestatario == -1 || isIntestatario)
                _indiceIntestatario = _numeroClientiInseriti;

            _numeroClientiInseriti++;
        }

        public string ToJson()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            sb.Append($"\"id\":{_id},");
            sb.Append($"\"numeroCamera\":{_numeroCamera},");
            sb.Append($"\"inizio\":\"{_inizio:yyyy-MM-dd}\",");
            sb.Append($"\"fine\":\"{_fine:yyyy-MM-dd}\",");
            sb.Append($"\"numeroPersone\":{_numeroPersone},");
            sb.Append($"\"indiceIntestatario\":{_indiceIntestatario},");
            sb.Append("\"clienti\":[");

            for (int i = 0; i < _numeroClientiInseriti; i++)
            {
                sb.Append(_clienti[i].ToJson());
                if (i < _numeroClientiInseriti - 1)
                    sb.Append(",");
            }

            sb.Append("]}");
            return sb.ToString();
        }
    }

    // CLASSE HOTEL
    public class Hotel
    {
        private Camera[] _camere;
        private int _numeroCamere;
        private Prenotazione[] _prenotazioni;
        private int _numeroPrenotazioni;
        private const int MAX_CAMERE = 100;
        private const int MAX_PRENOTAZIONI = 1000;
        private const string FILE_PRENOTAZIONI = "prenotazioni.json";

        public Hotel()
        {
            _camere = new Camera[MAX_CAMERE];
            _numeroCamere = 0;
            _prenotazioni = new Prenotazione[MAX_PRENOTAZIONI];
            _numeroPrenotazioni = 0;

            InizializzaCamere();
            CaricaPrenotazioni();
        }

        private void InizializzaCamere()
        {
            // Piano 1
            AggiungiCamera(new Camera(100, 1, "LRP1C100L4", 4));
            AggiungiCamera(new Camera(101, 1, "LRP1C101L5", 5));
            AggiungiCamera(new Camera(102, 1, "LRP1C102L7", 7));
            AggiungiCamera(new Camera(103, 1, "LRP1C103L6", 6));
            AggiungiCamera(new Camera(104, 1, "LRP1C104L3", 3));
            AggiungiCamera(new Camera(105, 1, "LRP1C105L2", 2));

            // Piano 2
            AggiungiCamera(new Camera(201, 2, "LRP2C201L4", 4));
            AggiungiCamera(new Camera(202, 2, "LRP2C202L3", 3));
            AggiungiCamera(new Camera(203, 2, "LRP2C203L5", 5));
            AggiungiCamera(new Camera(204, 2, "LRP2C204L5", 5));
            AggiungiCamera(new Camera(205, 2, "LRP2C205L4", 4));
        }

        public void AggiungiCamera(Camera camera)
        {
            if (_numeroCamere >= MAX_CAMERE)
                throw new InvalidOperationException("Numero massimo di camere raggiunto.");

            _camere[_numeroCamere] = camera;
            _numeroCamere++;
        }

        public Camera GetCamera(int numeroCamera)
        {
            for (int i = 0; i < _numeroCamere; i++)
            {
                if (_camere[i].NumeroCamera == numeroCamera)
                    return _camere[i];
            }
            return null;
        }

        public Camera[] GetCamereDisponibili(DateTime inizio, DateTime fine, int numeroPersone)
        {
            Camera[] temp = new Camera[_numeroCamere];
            int count = 0;

            for (int i = 0; i < _numeroCamere; i++)
            {
                if (_camere[i].NumeroLetti >= numeroPersone &&
                    _camere[i].IsDisponibile(inizio, fine, _prenotazioni, _numeroPrenotazioni))
                {
                    temp[count] = _camere[i];
                    count++;
                }
            }

            Camera[] risultato = new Camera[count];
            Array.Copy(temp, risultato, count);
            return risultato;
        }

        public Prenotazione IniziaPrenotazione(int numeroCamera, DateTime inizio, DateTime fine, int numeroPersone)
        {
            Camera camera = GetCamera(numeroCamera);
            if (camera == null)
                throw new InvalidOperationException($"Camera {numeroCamera} non trovata.");

            if (camera.NumeroLetti < numeroPersone)
                throw new InvalidOperationException($"La camera ha solo {camera.NumeroLetti} letti, richiesti {numeroPersone}.");

            if (!camera.IsDisponibile(inizio, fine, _prenotazioni, _numeroPrenotazioni))
                throw new InvalidOperationException($"Camera {numeroCamera} non disponibile nel periodo richiesto.");

            return new Prenotazione(numeroCamera, inizio, fine, numeroPersone);
        }

        public void AggiungiClienteAPrenotazione(Prenotazione prenotazione, string nome, string cognome, string email, string telefono, bool isIntestatario = false)
        {
            Cliente cliente = new Cliente(nome, cognome, email, telefono);
            prenotazione.AggiungiCliente(cliente, isIntestatario);
        }

        public void ConfermaPrenotazione(Prenotazione prenotazione)
        {
            if (!prenotazione.IsCompleta)
                throw new InvalidOperationException($"Devi inserire tutti i {prenotazione.NumeroPersone} clienti prima di confermare.");

            if (prenotazione.Intestatario == null)
                throw new InvalidOperationException("Devi specificare un intestatario per la prenotazione.");

            if (_numeroPrenotazioni >= MAX_PRENOTAZIONI)
                throw new InvalidOperationException("Numero massimo di prenotazioni raggiunto.");

            _prenotazioni[_numeroPrenotazioni] = prenotazione;
            _numeroPrenotazioni++;

            SalvaPrenotazioni();
        }

        public void EliminaPrenotazione(int idPrenotazione)
        {
            int indice = -1;
            for (int i = 0; i < _numeroPrenotazioni; i++)
            {
                if (_prenotazioni[i].Id == idPrenotazione)
                {
                    indice = i;
                    break;
                }
            }

            if (indice == -1)
                throw new InvalidOperationException($"Prenotazione {idPrenotazione} non trovata.");

            for (int i = indice; i < _numeroPrenotazioni - 1; i++)
            {
                _prenotazioni[i] = _prenotazioni[i + 1];
            }
            _prenotazioni[_numeroPrenotazioni - 1] = null;
            _numeroPrenotazioni--;

            SalvaPrenotazioni();
        }

        public Prenotazione[] GetPrenotazioni()
        {
            Prenotazione[] risultato = new Prenotazione[_numeroPrenotazioni];
            Array.Copy(_prenotazioni, risultato, _numeroPrenotazioni);
            return risultato;
        }

        // PERSISTENZA JSON
        private void SalvaPrenotazioni()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("{");
                sb.AppendLine("  \"prenotazioni\": [");

                for (int i = 0; i < _numeroPrenotazioni; i++)
                {
                    sb.Append("    ");
                    sb.Append(_prenotazioni[i].ToJson());
                    if (i < _numeroPrenotazioni - 1)
                        sb.AppendLine(",");
                    else
                        sb.AppendLine();
                }

                sb.AppendLine("  ]");
                sb.Append("}");

                File.WriteAllText(FILE_PRENOTAZIONI, sb.ToString());
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Errore nel salvataggio: {ex.Message}");
            }
        }

        private void CaricaPrenotazioni()
        {
            if (!File.Exists(FILE_PRENOTAZIONI))
                return;

            try
            {
                string json = File.ReadAllText(FILE_PRENOTAZIONI);
                // Per semplicità, non implemento il parsing completo del JSON
                // In un'applicazione reale si userebbe Newtonsoft.Json o System.Text.Json
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Errore nel caricamento: {ex.Message}");
            }
        }

        public string[] GetStatoCamere(DateTime inizio, DateTime fine)
        {
            string[] stati = new string[_numeroCamere];

            for (int i = 0; i < _numeroCamere; i++)
            {
                bool disponibile = _camere[i].IsDisponibile(inizio, fine, _prenotazioni, _numeroPrenotazioni);
                stati[i] = disponibile ? "Libera" : "Occupata";
            }

            return stati;
        }

        public Camera[] GetTutteLeCamere()
        {
            Camera[] risultato = new Camera[_numeroCamere];
            Array.Copy(_camere, risultato, _numeroCamere);
            return risultato;
        }
    }
}