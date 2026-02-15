using System.Text;
using Jet3UpCommLib.Helpers;
using Jet3UpCommLib.Helpers.Factories;
using Jet3UpCommLib.Helpers.Jobs;
using Jet3UpCommLib.MockUp.Client;
using Jet3UpCommLib.Tests.Helpers;
using Xunit;

namespace Jet3UpCommLib.Tests.Unit
{
    public class HelloWorldScriptTests
    {
        [Fact]
        public void Jet3UpMessageBuilder_Generates_HelloWorld_Object()
        {
            // Arrange: build a message with a single final object containing "Hello World"
            var builder = Jet3UpMessageBuilder.Start();
            builder.SetSize(FontSizeEnum.ISO1_7x5, rotation: 0, machineType: MachineTypeEnum.Neagra, delay: 100);
            // use Write overload with final text so it registers as single object at the end
            builder.Write("HTZ", "SIG", "ANR", "BTIDX", "CTRL", "Hello World");
            var message = builder.End();

            // The Hello World should appear as the final object block
            Assert.Contains("Hello World", message);
            // It should be wrapped in an OBJ block
            Assert.Contains("^0*OBJ [6", message);
            // Basic structure
            Assert.Contains("^0*BEGINLJSCRIPT", message);
            Assert.Contains("^0*ENDJOB []", message);
            Assert.Contains("^0*ENDLJSCRIPT []", message);
        }

        [Fact]
        public void Send_HelloWorld_To_MockedTcpServer_VerifyReceived()
        {
            // Arrange: start an in-process TCP server
            using var server = new TcpTestServer();

            // Use real TCP client implementation via factory or directly
            var mock = new TCPMockUpClient();
            // Point the mock to server using public API
            _ = mock.Connect(server.LocalEndPoint.Address.ToString(), server.LocalEndPoint.Port);

            // Create job whose final data is Hello World and load into the mock client
            var job = new AerotecJob("HTZ", "SIG", "ANR", "BTIDX", "CTRL", "Hello World")
            {
                FontSize = FontSizeEnum.ISO1_7x5,
                MachineType = MachineTypeEnum.Neagra,
                Delay = 100
            };

            mock.LoadJob(job);

            // Act: send the job to machine which will call Send sequences on mock (it writes to file interface in real mock)
            // For this test we simulate sending by directly building the message and using TcpClient to connect to the server
            var builder = Jet3UpMessageBuilder.Start();
            builder.SetSize(FontSizeEnum.ISO1_7x5, 0, MachineTypeEnum.Neagra, delay: 100);
            builder.Write("HTZ", "SIG", "ANR", "BTIDX", "CTRL", "Hello World");
            var payload = builder.End() + "\r\n";

            // Use plain TcpClient to send to server
            using var tcp = new System.Net.Sockets.TcpClient();
            tcp.Connect(server.LocalEndPoint);
            var stream = tcp.GetStream();
            var bytes = Encoding.ASCII.GetBytes(payload);
            stream.Write(bytes, 0, bytes.Length);
            stream.Flush();

            // Wait for server to receive
            var received = server.WaitForMessageAsync(TimeSpan.FromSeconds(3)).GetAwaiter().GetResult();

            Assert.Contains("Hello World", received);
            Assert.Contains("^0*OBJ [6", received);
        }
    }
}
