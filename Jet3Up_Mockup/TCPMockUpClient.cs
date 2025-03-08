// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using Jet3UpHelpers;
using Jet3UpHelpers.Factories;
using Jet3UpHelpers.Resources;
using Jet3UpInterfaces.Services;
using Microsoft.VisualBasic;

namespace Jet3Up.Services.Mockup
{
    /// <inheritdoc/>
    public class TCPMockUpClient : IClientService
    {
        private TCPMockUpClient tcpMockUpClient;
        private FileInterface fileInterface;

        public event EventHandler<Jet3UpMessageHendlerEventArgs> Jet3UpMessageHendler;
        public event EventHandler<Jet3UpCommunicationInterruptedErrorEventArgs> Jet3UpCommunicationInterrupted;

        /// <inheritdoc/>
        public TCPMockUpClient()
        {
            fileInterface = new FileInterface();
            fileInterface.TextReaderEvent += FileInterface_TextReaderEvent;
        }

        /// <inheritdoc/>
        private void FileInterface_TextReaderEvent(object? sender, ReadMessageEventArg e)
        {

            if (e.Text.Contains("error"))
            {
                Jet3UpMessageHendler?.Invoke(this, new Jet3UpMessageHendlerEventArgs(Jet3UpStatusMessageType.Error, "error"));
                return;
            }
            if (int.Parse(e.Text) > -1)
            {
                Jet3UpMessageHendler?.Invoke(this, new Jet3UpMessageHendlerEventArgs(Jet3UpStatusMessageType.Marked, e.Text));
                return;
            }
            Jet3UpMessageHendler?.Invoke(this, new Jet3UpMessageHendlerEventArgs(Jet3UpStatusMessageType.Done, "done"));
        }

        /// <inheritdoc/>
        public bool Connect(string Ip, int timeout)
        {
            if (tcpMockUpClient == null)
                tcpMockUpClient = new TCPMockUpClient();
            fileInterface.Write("Connect method called with IP: " + Ip + " and timeout: " + timeout);
            return true;
        }

        /// <inheritdoc/>
        public void ContinueWriting()
        {
            fileInterface.Write("ContinueWriting method called");
            Send("^0!GO");
        }

        /// <inheritdoc/>
        public bool IsConnected()
        {
            fileInterface.Write("IsConnected method called");
            return true;
        }

        /// <inheritdoc/>
        public void Send(string text)
        {

            fileInterface.Write("Send method called with text: " + text);
        }

        /// <inheritdoc/>
        public void StartWriting(int delay, FontSizeEnum size, int rotation, MachineTypeEnum machine, 
            string HTZ, string signature, string ANR, string BTIDX, string controllerId, int expectedQuantity,
            int encoderResolution, string? anzahl)
        {
            string message;
            Send("^0!RC");
            if (anzahl != null)
            {
                message = Jet3UpMessageBuilder.Start().Create().SetSize(size, rotation, machine, encoderResolution: encoderResolution).Write(HTZ, signature, ANR, BTIDX, controllerId, anzahl).End();
            }
            else
            {
                message = Jet3UpMessageBuilder.Start().Create().SetSize(size, rotation, machine, encoderResolution: encoderResolution).Write(HTZ, signature, ANR, BTIDX, controllerId).End();
            }

            Send(message);
            Send("^0=CC0" + Constants.vbTab + expectedQuantity.ToString() + Constants.vbTab + "3999");
            Send("^0!GO");
            fileInterface.StartReading(expectedQuantity);
        }

        /// <inheritdoc/>
        public void StopCommand()
        {
            fileInterface.StopReading();
            fileInterface.Write("StopCommand method called");
            Send("^0!ST");
        }

        /// <inheritdoc/>
        public void SetCount(int Expected, int current)
        {

        }

        /// <inheritdoc/>
        public void StopListening()
        {
            throw new NotImplementedException();
        }
    }
}
