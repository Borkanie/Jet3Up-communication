using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jet3UpCommLib.Tests.Helpers
{
    internal class TcpTestServer : IDisposable
    {
        private readonly TcpListener listener;
        private readonly CancellationTokenSource cts = new();
        private Task? acceptTask;
        private TcpClient? connectedClient;

        public IPEndPoint LocalEndPoint { get; }

        public TcpTestServer()
        {
            listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            LocalEndPoint = (IPEndPoint)listener.LocalEndpoint;
            acceptTask = Task.Run(AcceptLoopAsync);
        }

        private async Task AcceptLoopAsync()
        {
            try
            {
                connectedClient = await listener.AcceptTcpClientAsync(cts.Token).ConfigureAwait(false);
                using var stream = connectedClient.GetStream();
                var buffer = new byte[8192];
                while (!cts.IsCancellationRequested)
                {
                    var read = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cts.Token).ConfigureAwait(false);
                    if (read == 0) break;
                    var text = Encoding.ASCII.GetString(buffer, 0, read);
                    LastReceived = text;
                    LastReceivedTcs?.TrySetResult(text);

                    if (Responder != null)
                    {
                        var resp = Responder(text);
                        if (!string.IsNullOrEmpty(resp))
                        {
                            var bytes = Encoding.ASCII.GetBytes(resp);
                            await stream.WriteAsync(bytes.AsMemory(0, bytes.Length), cts.Token).ConfigureAwait(false);
                        }
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { LastException = ex; }
        }

        public Func<string, string?>? Responder { get; set; }
        public string? LastReceived { get; private set; }
        public Exception? LastException { get; private set; }

        private TaskCompletionSource<string>? LastReceivedTcs;
        public Task<string> WaitForMessageAsync(TimeSpan timeout)
        {
            LastReceivedTcs = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            var ct = new CancellationTokenSource(timeout);
            ct.Token.Register(() => LastReceivedTcs.TrySetCanceled());
            return LastReceivedTcs.Task;
        }

        public void Dispose()
        {
            cts.Cancel();
            try { acceptTask?.Wait(500); } catch { }
            connectedClient?.Close();
            listener.Stop();
            cts.Dispose();
        }
    }
}
