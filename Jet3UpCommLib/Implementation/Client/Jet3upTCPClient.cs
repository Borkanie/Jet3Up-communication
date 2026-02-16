// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using Jet3UpCommLib.Helpers;
using Jet3UpCommLib.Helpers.Resources;
using Jet3UpCommLib.Interfaces.Client;
using Microsoft.VisualBasic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Jet3UpCommLib.Implementation.Client
{
    /// <summary>
    /// This implementation needs a machine to connect to in order to work. 
    /// <inheritdoc cref="IClient"/>
    /// </summary>
    internal class Jet3upTCPClient : InternalClient, IDisposable
    {
        private IPEndPoint target = IPEndPoint.Parse("0.0.0.0:8000");
        private string name = "Printer";
        private int expectedQuantity = 0;
        private TcpClient? client;
        private Stream? tcpClientStream;
        // CancellationTokenSource to allow task cancellation
        private CancellationTokenSource? cancellationTokenSource;
        public event EventHandler<Jet3UpMessageHendlerEventArgs>? Jet3UpMessageHendler;
        public event EventHandler<Jet3UpCommunicationInterruptedErrorEventArgs>? Jet3UpCommunicationInterrupted;
        private Job? job;

        public Jet3upTCPClient()
        {
            
        }

        public Jet3upTCPClient(IPEndPoint endPoint)
        {
            target = endPoint;
        }

        public Jet3upTCPClient(string Ip, int port)
        {
          target = new IPEndPoint(IPAddress.Parse(Ip),port);
        }

        /// <inheritdoc/>
        public bool Connect(string Ip, int port)
        {

            ((InternalClient)this).SetHost(Ip, port);
            return Connect();
        }

        /// <inheritdoc/>
        public void SendContinueWritingCommand()
        {
            Send(IClient.GO);
        }

        /// <inheritdoc/>
        public bool CheckConnection()
        {
            return client != null && client.Connected;
        }

        /// <inheritdoc/>
        public void Send(string text)
        {
            if (client == null)
                return;
            if (CheckConnection())
            {
                byte[] SENDBYTES = Encoding.ASCII.GetBytes(text + Constants.vbCrLf);
                try
                {
                    lock (client)
                    {
                        _ = client.Client.Send(SENDBYTES);
                    }
                }
                catch (Exception ex)
                {
                    Jet3UpCommunicationInterrupted?.Invoke(this, new Jet3UpCommunicationInterruptedErrorEventArgs(ex, true));
                }

            }
        }

        /// <inheritdoc/>
        public void StopListening()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = null;
        }

        /// <inheritdoc/>
        public void SendStopCommand()
        {
            StopListening();
            Send(IClient.jet3UpStopSequence);
        }

        /// <inheritdoc/>
        public void StartListening()
        {
            if (CheckConnection())
            {
                if (cancellationTokenSource == null)
                {
                    // Create a CancellationTokenSource for task cancellation
                    cancellationTokenSource = new CancellationTokenSource();
                    _ = Task.Run(() => ListenForResponses(cancellationTokenSource.Token));
                }
            }
        }

        /// <inheritdoc/>
        private void ListenForResponses(CancellationToken cancellationToken)
        {
            Thread.Sleep(2000);
            byte[] buffer = new byte[15 + Commons.NumberOfDigitsInInt(expectedQuantity)]; // Adjust the buffer size as needed
            try
            {
                while (true)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        return;
                    }
                    Thread.Sleep(250);

                    int bytesRead = AskForCurrentIndex(ref buffer);
                    if (bytesRead > 0)
                    {
                        string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                        // Process the response here
                        // You can raise an event or do whatever is necessary with the response data
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        int val = int.Parse(response.Split('C')[2].Split('\t')[0]);
                        if (val < expectedQuantity)
                        {
                            SendContinueWritingCommand();
                        }
                        Jet3UpMessageHendler?.Invoke(this, new Jet3UpMessageHendlerEventArgs(Jet3UpStatusMessageType.Marked, response.Split('C')[2].Split('\t')[0]));

                    }

                }
            }
            catch (Exception ex)
            {
                Jet3UpCommunicationInterrupted?.Invoke(this, new Jet3UpCommunicationInterruptedErrorEventArgs(ex, false));
            }
        }

        /// <inheritdoc/>
        private int AskForCurrentIndex(ref byte[] buffer)
        {
            int bytesRead;
            Send(IClient.CC_GET);
            if (client == null || tcpClientStream == null)
                return -1;
            lock (client)
            {
                bytesRead = tcpClientStream.Read(buffer, 0, buffer.Length);
            }

            return bytesRead;
        }

        /// <inheritdoc/>
        public void SendSetCountCommand(int Expected, int current)
        {
            Send($"{IClient.CC_SET} {current} {Constants.vbTab} {Expected} 3999");
        }

        public bool Connect()
        {
            client = new TcpClient(target.Address.ToString(), target.Port);
            tcpClientStream = client.GetStream();
            tcpClientStream.ReadTimeout = 2000;
            return tcpClientStream.CanRead && tcpClientStream.CanWrite;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName() => name;

        /* inheritdoc */
        public void Disconect()
        {
            if (client == null || tcpClientStream == null)
                return;
            tcpClientStream.Close();
            client.Close();
        }

        /* inheritdoc */
        public void LoadJob(Job job)
        {
            this.job = job;

        }

        /* inheritdoc */
        public void DeployJobOnMachineAndStartWriting()
        {
            if (job == null)
                return;
            Send(IClient.RC);
            Thread.Sleep(500);
            Send(job!.getJobConfigurationMessage());
            Thread.Sleep(250);
            SendSetCountCommand(expectedQuantity, 0);
            Thread.Sleep(250);
            Send(IClient.EQ);
            Thread.Sleep(500);
            SendContinueWritingCommand();
            StartListening();
        }

        /* inheritdoc */
        public IPEndPoint GetAddress()
        {
            return target;
        }

        /* inheritdoc */
        void InternalClient.SetHost(string address, int port)
        {
            if (CheckConnection())
            {
                SendStopCommand();
            }
            target = IPEndPoint.Parse(address + ":" + port.ToString());
        }

        public void Dispose()
        {
            tcpClientStream?.Dispose();
            client?.Close();
        }
    }
}
