using Interfaces.Factories;
using Jet3UpInterfaces.Client;
using Mockup.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Mockup.Factories
{
    public class MockCLientFactory : IClientFactory
    {
        public IClient createClient(string address, int port, string name)
        {
            var mock = new TCPMockUpClient();
            mock.SetHost(address, port);
            mock.SetName(name);
            return mock;
        }

        bool IClientFactory.ChangeAdressToClient(IClient client, string address, int port)
        {
            try
            {
                client.Disconect();
                client.SetHost(address, port);

            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
            return true;
        }

        IClient? IClientFactory.createClient(string address, int port, string name)
        {
            var client = new TCPMockUpClient();
            client.Connect(address, port);
            client.SetName(name);
            return client;
        }

        bool IClientFactory.isAdressNotInUse(IPEndPoint endpoint)
        {
            return true;
        }

        bool IClientFactory.RemoveClient(IClient client)
        {
            return true;
        }
    }
}
