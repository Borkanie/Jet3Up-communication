using Jet3UpCommLib.Helpers.Jobs;
using Jet3UpCommLib.Implementation.Factories;
using Jet3UpCommLib.Tests.Helpers;
using Microsoft.VisualBasic;

namespace Jet3UpCommlibUnitTests.Unit
{
    public class Jet3upTCPClientTests : IDisposable
    {
        private readonly TcpTestServer server;

        public Jet3upTCPClientTests()
        {
            // Setup: start mock TCP server for each test
            server = new TcpTestServer();
        }

        public void Dispose()
        {
            server.Dispose();
        }

        [Fact]
        public void Connect_NoJob_NoMessagesSent()
        {
            var factory = new ClientFactory();
            var client = factory.CreateClient(server.LocalEndPoint.Address.ToString(), server.LocalEndPoint.Port, "test");

            // Connect but do not start any job
            var connected = client.Connect();
            Assert.True(connected);

            // Wait a short time to allow any accidental sends
            System.Threading.Thread.Sleep(300);

            // Server should not have received any data
            Assert.Null(server.LastReceived);

            client.Disconect();
        }


        [Fact]
        public void StartWriting_WithJob_SendsExpectedMessages()
        {
            var factory = new ClientFactory();
            var client = factory.CreateClient(server.LocalEndPoint.Address.ToString(), server.LocalEndPoint.Port, "test");

            // Connect to the in-process server
            var connected = client.Connect();
            Assert.True(connected);

            // Start a job which will send sequence of messages
            // Synchonous
            var job = new AerotecJob("HTZ", "SIG", "ANR", "BTIDX", "CTRL");

            client.LoadJob(job);
            client.DeployJobOnMachineAndStartWriting();

            var messages = server.GetMessages();

            client.Disconect();
            Assert.Equal(5, messages.Count);
            // Expect reset command, begin script and end markers and CC quantity message
            Assert.Contains("^0!RC", messages.Dequeue().Message);
            var jobMessage = "^0*BEGINLJSCRIPT [()]\r\n^0*JLPAR [ 60 1 0 3 1000 0 0 0 00:00 0 30000 0 0 1000]\r\n^0*BEGINJOB [ 0 () ]\r\n^0*JOBPAR [ 0 0 0 220 0 0 0 1 1 0 -1 () 1 1 55000 0 9 0 1 0 100 0 1 0]\r\n^0*OBJ [1 0 11 0 (ISO1_7x5)  (HTZ ) 1 0 0 0 0 1 0 0 0 0 0 0 ()  () 0 0 ()]\r\n^0*OBJ [2 92 11 0 (ISO1_7x5)  (SIG) 1 0 0 0 0 1 0 0 0 0 0 0 ()  () 0 0 ()]\r\n^0*OBJ [3 0 0 0 (ISO1_7x5)  (ANR) 1 0 0 0 0 1 0 0 0 0 0 0 ()  () 0 0 ()]\r\n^0*OBJ [4 60 0 0 (ISO1_7x5)  (BTIDX ) 1 0 0 0 0 1 0 0 0 0 0 0 ()  () 0 0 ()]\r\n^0*OBJ [5 80 0 0 (ISO1_7x5)  (CTRL) 1 0 0 0 0 1 0 0 0 0 0 0 ()  () 0 0 ()]\r\n^0*ENDJOB []\r\n^0*ENDLJSCRIPT []\r\n\r\n";
            Assert.Equal(jobMessage, messages.Dequeue().Message);            
            Assert.Contains("^0=CC", messages.Dequeue().Message);
            Assert.Contains("EQ", messages.Dequeue().Message);
            Assert.Contains("GO", messages.Dequeue().Message);
        }

        [Fact]
        public void SendSetCount_WithJob_SendsExpectedMessages()
        {
            var factory = new ClientFactory();
            var client = factory.CreateClient(server.LocalEndPoint.Address.ToString(), server.LocalEndPoint.Port, "test");

            // Connect to the in-process server
            var connected = client.Connect();
            Assert.True(connected);
            var job = new AerotecJob("HTZ", "SIG", "ANR", "BTIDX", "CTRL");
            client.LoadJob(job);
            client.SendSetCountCommand(100, 10);

            var messages = server.GetMessages();

            client.Disconect();
            Assert.Single(messages);
            Assert.Equal($"^0=CC 10 {Constants.vbTab} 100 3999\r\n", messages.Dequeue().Message);
        }

        [Fact]
        public void SendStop_WithJob_SendsExpectedMessages()
        {
            var factory = new ClientFactory();
            var client = factory.CreateClient(server.LocalEndPoint.Address.ToString(), server.LocalEndPoint.Port, "test");

            // Connect to the in-process server
            var connected = client.Connect();
            Assert.True(connected);

            var job = new AerotecJob("HTZ", "SIG", "ANR", "BTIDX", "CTRL");
            client.LoadJob(job);
            client.SendStopCommand();

            var messages = server.GetMessages();

            client.Disconect();
            Assert.Single(messages);
            Assert.Equal("^0!ST\r\n", messages.Dequeue().Message);
        }


        [Fact]
        public void SendStop_WithouthJob_SendsExpectedMessages()
        {
            var factory = new ClientFactory();
            var client = factory.CreateClient(server.LocalEndPoint.Address.ToString(), server.LocalEndPoint.Port, "test");

            // Connect to the in-process server
            var connected = client.Connect();
            Assert.True(connected);

            client.SendStopCommand();

            var messages = server.GetMessages();

            client.Disconect();
            Assert.Single(messages);
            Assert.Equal("^0!ST\r\n", messages.Dequeue().Message);
        }


        [Fact]
        public void StartWriting_WithJob_CheckForCOunterPingsAndCorrectOutputs()
        {
            var factory = new ClientFactory();
            var client = factory.CreateClient(server.LocalEndPoint.Address.ToString(), server.LocalEndPoint.Port, "test");

            // Connect to the in-process server
            var connected = client.Connect();
            Assert.True(connected);

            // Start a job which will send sequence of messages
            // Synchonous
            var job = new AerotecJob("HTZ", "SIG", "ANR", "BTIDX", "CTRL");

            client.LoadJob(job);
            client.DeployJobOnMachineAndStartWriting();

            // Allow the clinet to ping a few times
            Thread.Sleep(3500);

            var messages = server.GetMessages();
            client.Disconect();

            Assert.True(messages.Count > 7);
            for (int i = 0; i <= 5; i++)
            {
                _ = messages.Dequeue().Message; // Skip initial messages
            }

            // Expect 5 pings
            while(messages.Count > 0)
            {
                Assert.Contains("^0?CC", messages.Dequeue().Message);
            }
        }
    }
}
