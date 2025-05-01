using Jet3UpCommLib.Interfaces.Client;
using Jet3UpCommLib.Interfaces.Factories;
using Jet3UpCommLib.MockUp.Client;
using System.Net;

namespace Jet3UpCommLib.MockUp.Factories
{
    public class MockCLientFactory : IClientFactory
    {
        public bool ChangeAdressToClient(IClient client, string address, int port)
        {
            throw new NotImplementedException();
        }

        public IClient CreateClient(string address, int port, string name)
        {
            var mock = new TCPMockUpClient();
            ((InternalClient)mock).SetHost(address, port);
            mock.SetName(name);
            return mock;
        }

        public IClient CreateClient()
        {
            return new TCPMockUpClient();
        }

        public bool isAdressInUse(IPEndPoint endpoint)
        {
            throw new NotImplementedException();
        }

        public bool Remove(IClient client)
        {
            throw new NotImplementedException();
        }
    }
}
