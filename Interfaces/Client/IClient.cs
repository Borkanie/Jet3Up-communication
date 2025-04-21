// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.
using Helpers;
using Jet3UpHelpers;

namespace Jet3UpInterfaces.Client
{
    /// <summary>
    /// The service controlling the machine.
    /// </summary>
    public interface IClient
    {
        /// <summary>
        /// Reset counter command.
        /// </summary>
        protected const string RC = "^0!RC";
        
        /// <summary>
        /// Start job message that needs to be send to machine to start printing.
        /// </summary>
        protected const string GO = "^0!GO";

        /// <summary>
        /// Stop running job command.
        /// </summary>
        protected const string jet3UpStopSequence = "^0!ST";

        /// <summary>
        /// Message that needs to be send to machine at the end of a job script.
        /// </summary>
        protected const string EQ = "^0!EQ";
        
        /// <summary>
        /// Current counter message.
        /// </summary>
        protected const string CC = "^0=CC";

        public string Ip { get; set; }
        public int Port { get; set; }

        /// <summary>
        /// Safely closes current connection to the printer while ensuring the job stopped.
        /// </summary>
        public void Disconect();

        /// <summary>
        /// Establishes communication to the machine.
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool Connect(string Ip, int port);

        /// <summary>
        /// Sets up a new host.
        /// </summary>
        /// <param name="address">The adress where we want to connect to.</param>
        /// <param name="port">The port of the desired machine.</param>
        /// <returns></returns>
        public void SetHost(string address, int port);

        /// <summary>
        /// Establishes communication to the already set host.
        /// </summary>
        public bool Connect();

        /// <summary>
        /// Sends a message to the machine.
        /// </summary>
        /// <param name="text">The string to be send.</param>
        public void Send(string text);

        /// <summary>
        /// Loads a given job into the client.
        /// </summary>
        /// <param name="job"></param>
        public void LoadJob(Job job);

        /// <summary>
        /// Creates the standardized objects to send to the machine to print.
        /// </summary>
        /// <param name="delay"> The delay of the writing command in micrometers.</param>
        /// <param name="size">The desired <see cref="FontSizeEnum"/> of the writing.</param>
        /// <param name="rotation">The machine has two flags, 180 dgereees and mirrored.
        /// This are configured trough the ehternet interface using an angle of 0 90 180 or 270. 
        /// Any other values will result in unexpected behaviour.</param>
        /// <param name="machine">TThe <see cref="MachineTypeEnum"/> of the machine.</param>
        /// <param name="HTZ">A given number representative of the lot.</param>
        /// <param name="signature">PA from Premium Aerotec.</param>
        /// <param name="ANR">A given number preresentative of the lot.</param>
        /// <param name="BTIDX">Id of the command.</param>
        /// <param name="controllerId">id of the controller manning the machine.</param>
        /// <param name="expectedQuantity">The quantity that is needed to be printed.</param>
        /// <param name="anzahl">Final message string.
        /// If it's NOT NULL the message will be considered final message and standard size will be written for black machine.
        /// Client specified this configuration.</param>
        public void StartWriting(int delay, FontSizeEnum size, int rotation, MachineTypeEnum machine,            
            string HTZ, string signature, string ANR, string BTIDX, string controllerId, int expectedQuantity,
            int encoderResolution, string? anzahl);

        /// <summary>
        /// After each message requesting current counter a go message needs to be sent to the machine.
        /// </summary>
        public void ContinueWriting();

        /// <summary>
        /// Flag for the connection of the software to the machine.
        /// </summary>
        /// <returns></returns>
        public bool IsConnected();

        /// <summary>
        /// Interrupts all current executation by sending a stop signal to the machine.
        /// </summary>
        public void StopCommand();

        /// <summary>
        /// A message from the machine has been recieved and parsed.
        /// it will be forwarded to the service layer.
        /// </summary>
        public event EventHandler<Jet3UpMessageHendlerEventArgs> Jet3UpMessageHendler;

        /// <summary>
        /// An event that signals that the machine either became unresponsive or it forcefully closed or rejected a request.
        /// </summary>
        public event EventHandler<Jet3UpCommunicationInterruptedErrorEventArgs> Jet3UpCommunicationInterrupted;

        /// <summary>
        /// Configures the printed quantity in the machine.
        /// </summary>
        /// <param name="expected">The quantity that needs to be printed.</param>
        /// <param name="current">The quantity that has been actually printed.</param>
        public void SetCount(int expected, int current);

        public void StopListening();

        public void SetName(string name);

        public string GetName();

        public void StartJob();
    }
}
