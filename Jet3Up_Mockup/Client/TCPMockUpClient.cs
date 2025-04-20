// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using Helpers;
using Helpers.Jobs;
using Jet3UpHelpers;
using Jet3UpHelpers.Resources;
using Jet3UpInterfaces.Client;
using Microsoft.VisualBasic;

namespace Mockup.Client
{
    /// <inheritdoc/>
    public class TCPMockUpClient : IClient
    {
        private string ip = "0.0.0.0";
        private int port = 3000;
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
        public bool Connect(string Ip, int port)
        {
            SetHost(Ip,port);
            return Connect();
        }

        /// <inheritdoc/>
        public void ContinueWriting()
        {
            fileInterface.Write("ContinueWriting method called");
            Send(IClient.GO);
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

            fileInterface.Write(text);
        }

        /// <inheritdoc/>
        public void StartWriting(int delay, FontSizeEnum size, int rotation, MachineTypeEnum machine,
            string HTZ, string signature, string ANR, string BTIDX, string controllerId, int expectedQuantity,
            int encoderResolution, string? anzahl)
        {
            Send(IClient.RC);

            var job = new AerotecJob(HTZ, signature, ANR, BTIDX, controllerId, anzahl);

            Send(job.getJobStartMessage());
            Send($"{IClient.CC}0" + Constants.vbTab + expectedQuantity.ToString() + Constants.vbTab + "3999");
            Send(IClient.EQ);
            Send(IClient.GO);
            fileInterface.StartReading(expectedQuantity);
        }

        /// <inheritdoc/>
        public void StopCommand()
        {
            fileInterface.StopReading();
            Send(IClient.jet3UpStopSequence);
        }

        /// <inheritdoc/>
        public void SetCount(int Expected, int current)
        {
            Send($"{IClient.CC} {current} {Constants.vbTab} {Expected} 3999");
        }

        /// <inheritdoc/>
        public void StopListening()
        {
           
        }

        /// <summary>
        /// only set's up local variables for later use.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public void SetHost(string address, int port)
        {
            ip = address;
            this.port = port;
        }

        // here port is used as timeout.
        public bool Connect()
        {
            fileInterface.Write("Connect method called with IP: " + ip + " and timeout: " + port);
            return true;
        }

        private string name;

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        public override bool Equals(object? obj)
        {
            if (obj is TCPMockUpClient)
            {
                return name == ((TCPMockUpClient)obj).GetName();
            }
            return false;
        }

        private Job job;

        public void Disconect()
        {
            
        }

        public void LoadJob(Job job)
        {
            this.job = job;
        }
    }
}
