// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.

using Jet3UpHelpers;
using Jet3UpHelpers.Factories;
using Jet3UpHelpers.Resources;
using Jet3UpInterfaces.Client;
using Microsoft.VisualBasic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Implementation.Client
{
    /// <summary>
    /// This implementation needs a machine to connect to in order to work. 
    /// <inheritdoc cref="IClient"/>
    /// </summary>
    public class Jet3upTCPClient : IClient
    {
        private string name = "Printer";
        private string ip = "0.0.0.0";
        private int port = 3000;
        private int expectedQuantity = 0;
        private TcpClient client;
        private NetworkStream tcpClientStream;
        // CancellationTokenSource to allow task cancellation
        private CancellationTokenSource? cancellationTokenSource;
        public event EventHandler<Jet3UpMessageHendlerEventArgs> Jet3UpMessageHendler;
        public event EventHandler<Jet3UpCommunicationInterruptedErrorEventArgs> Jet3UpCommunicationInterrupted;

        /// <inheritdoc/>
        public bool Connect(string Ip, int port)
        {
            SetHost(Ip, port);
            return Connect();
        }

        /// <inheritdoc/>
        public void ContinueWriting()
        {
            Send("^0!GO");
        }

        /// <inheritdoc/>
        public bool IsConnected()
        {
            return client != null && client.Connected;
        }

        /// <inheritdoc/>
        public void Send(string text)
        {

            if (IsConnected())
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
        public void StartWriting(int delay, FontSizeEnum size, int rotation, MachineTypeEnum machine,
            string HTZ, string signature, string ANR, string BTIDX, string controllerId, int expectedQuantity, int encoderResolution, string? anzahl)
        {
            this.expectedQuantity = expectedQuantity;
            string message;
            Send("^0!RC");

            var jet3upMessageBuilder = Jet3UpMessageBuilder.Start().Create();

            if (anzahl == null)
            {

                message = jet3upMessageBuilder.SetSize(size, rotation, machine, delay, encoderResolution: encoderResolution).Write(HTZ, signature, ANR, BTIDX, controllerId).End();
            }
            else
            {
                message = jet3upMessageBuilder.SetSize(FontSizeEnum.ISO1_7x5, rotation, MachineTypeEnum.Neagra, delay, encoderResolution: encoderResolution).Write(HTZ, signature, ANR, BTIDX, controllerId, anzahl).End();
            }
            Thread.Sleep(500);
            Send(message);
            Send("^0=CC0" + Constants.vbTab + expectedQuantity.ToString() + Constants.vbTab + "3999");
            Send("^0!EQ");
            Thread.Sleep(500);
            Send("^0!GO");
            if (anzahl == null)
                StartListening();
        }

        /// <inheritdoc/>
        public void StopListening()
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = null;
        }

        /// <inheritdoc/>
        public void StopCommand()
        {
            StopListening();
            Send("^0!ST");
        }

        /// <inheritdoc/>
        public void StartListening()
        {
            if (IsConnected())
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
            byte[] buffer = new byte[15 + NumberOfDigitsInInt(expectedQuantity)]; // Adjust the buffer size as needed
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
                            ContinueWriting();
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
        private int NumberOfDigitsInInt(int expectedQuantity)
        {
            int result = 0;
            while (expectedQuantity > 0)
            {
                expectedQuantity = expectedQuantity / 10;
                result++;
            }
            return result;
        }

        /// <inheritdoc/>
        private int AskForCurrentIndex(ref byte[] buffer)
        {
            int bytesRead;
            string getCurrentIndex = "^0?CC";
            Send(getCurrentIndex);
            lock (client)
            {
                bytesRead = tcpClientStream.Read(buffer, 0, buffer.Length);
            }

            return bytesRead;
        }

        /// <inheritdoc/>
        public void SetCount(int Expected, int current)
        {
            Send($"^0=CC{current} {Constants.vbTab} {Expected} 3999");
        }

        public void SetHost(string address, int port)
        {
            if (IsConnected())
            {
                StopCommand();
            }
            ip = address;
            this.port = port;
        }

        public bool Connect()
        {
            client = new TcpClient(ip, port);
            tcpClientStream = client.GetStream();
            tcpClientStream.ReadTimeout = 2000;
            return tcpClientStream.CanRead && tcpClientStream.CanWrite;
        }

        public void SetName(string name)
        {
            this.name = name;
        }

        public string GetName() => name;

        public override bool Equals(object? obj)
        {
            if(obj is Jet3upTCPClient)
            {
                return name == ((Jet3upTCPClient)obj).GetName();
            }
            return false;
        }
    }
}
