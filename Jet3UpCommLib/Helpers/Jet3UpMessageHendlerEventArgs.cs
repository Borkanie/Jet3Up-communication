// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.
using Jet3UpCommLib.Helpers.Resources;

namespace Jet3UpCommLib.Helpers
{
    /// <summary>
    /// Message from the machine.
    /// </summary>
    public class Jet3UpMessageHendlerEventArgs : EventArgs
    {
        public Jet3UpMessageHendlerEventArgs(Jet3UpStatusMessageType type, string message)
        {
            Type = type;
            Message = message;
        }

        /// <summary>
        /// Text recieved trough ethernet.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Status of the message.
        /// </summary>
        public Jet3UpStatusMessageType Type { get; }
    }
}
