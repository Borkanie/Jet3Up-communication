// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using Jet3UpCommLib.Helpers;
using Jet3UpCommLib.Helpers.Jobs;
using Jet3UpCommLib.Helpers.Resources;
using Jet3UpCommLib.Interfaces.Client;
using Microsoft.VisualBasic;
using System.Net;

namespace Jet3UpCommLib.MockUp.Client
{
    /// <inheritdoc/>
    public class TCPMockUpClient : InternalClient
    {
        protected IPEndPoint target = IPEndPoint.Parse("127.0.0.1:3000");
        private FileInterface fileInterface;
        public event EventHandler<Jet3UpMessageHendlerEventArgs>? Jet3UpMessageHendler;
        public event EventHandler<Jet3UpCommunicationInterruptedErrorEventArgs>? Jet3UpCommunicationInterrupted;

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
            ((InternalClient)this).SetHost(Ip,port);
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
        void InternalClient.SetHost(string address, int port)
        {
            target = IPEndPoint.Parse(address + ":" + port);
        }

        // here port is used as timeout.
        public bool Connect()
        {
            fileInterface.Write("Connect method called with IP: " + target.Address + " and timeout: " + target.Port);
            return true;
        }

        private string name = "";

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName()
        {
            return name;
        }

        private Job? job;

        public void Disconect()
        {
            
        }

        public void LoadJob(Job job)
        {
            this.job = job;
        }

        public IPEndPoint GetAddress()
        {
            return target;
        }

        public void SendJobToMachine()
        {
            if (job == null)
                return;
            Send(IClient.RC);
            Thread.Sleep(500);
            Send(job!.getJobStartMessage());
            Send($"{IClient.CC}0" + Constants.vbTab + 100.ToString() + Constants.vbTab + "3999");
            Send(IClient.EQ);
            Thread.Sleep(500);
            Send(IClient.GO);
        }
    }
}
