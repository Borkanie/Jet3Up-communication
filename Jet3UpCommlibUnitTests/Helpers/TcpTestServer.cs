using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Jet3UpCommLib.Tests.Helpers
{
    internal class TcpTestServer : IDisposable
    {
        public record MessageEntry(string Message, DateTimeOffset Timestamp);

        private readonly Queue<MessageEntry> messages = new();
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
                    var entry = new MessageEntry(text, DateTimeOffset.UtcNow);
                    messages.Enqueue(entry);
                    LastReceived = text;
                    LastReceivedTcs?.TrySetResult(text);

                    var resp = respond(text);
                    if (!string.IsNullOrEmpty(resp))
                    {
                        var bytes = Encoding.ASCII.GetBytes(resp);
                        await stream.WriteAsync(bytes.AsMemory(0, bytes.Length), cts.Token).ConfigureAwait(false);
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex) { LastException = ex; }
        }

        public string respond(string message)
        {
            // Reply to counter requests (accepts either '^0?CC' or '^0=CC' variants)
            if (message.StartsWith("^0?CC"))
            {
                return "^0=CC" + messages.Count + "\t100\t3999";
            }
            return string.Empty;
        }

        public string? LastReceived { get; private set; }
        public Exception? LastException { get; private set; }

        private TaskCompletionSource<string>? LastReceivedTcs;

        public Queue<MessageEntry> GetMessages() => messages;
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
