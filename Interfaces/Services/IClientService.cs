// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.
using Jet3UpHelpers;

namespace Jet3UpInterfaces.Services
{
    /// <summary>
    /// The service controlling the machine.
    /// </summary>
    public interface IClientService
    {
        /// <summary>
        /// Establishes communication to the machine.
        /// </summary>
        /// <param name="Ip"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool Connect(string Ip, int timeout);

        /// <summary>
        /// Sends a message to the machine.
        /// </summary>
        /// <param name="text">The string to be send.</param>
        public void Send(string text);

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
    }
}
