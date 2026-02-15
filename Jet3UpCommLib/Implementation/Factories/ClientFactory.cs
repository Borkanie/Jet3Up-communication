using Jet3UpCommLib.Implementation.Client;
using Jet3UpCommLib.Interfaces.Client;
using Jet3UpCommLib.Interfaces.Factories;
using System.Net;
using System.Xml.Linq;

namespace Jet3UpCommLib.Implementation.Factories
{
    public class ClientFactory : IClientFactory
    {

        private List<IClient> clients = new();

        public bool Remove(IClient client)
        {
            try
            {
                if (client.CheckConnection())
                {
                    client.SendStopCommand();
                    client.StopListening();
                    client.Disconect();
                }
                _ = clients.Remove(client);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        public bool ChangeAdressToClient(IClient client, string address, int port)
        {
            var endpoint = IPEndPoint.Parse(address);
            endpoint.Port = port;
            if (!isAdressInUse(endpoint) && !client.CheckConnection())
            {
                ((InternalClient)client).SetHost(address, port);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IClient? CreateClient(string address, int port, string name)
        {
            var endpoint = IPEndPoint.Parse(address);
            endpoint.Port = port;

            if (!isAdressInUse(endpoint))
            {
                Jet3upTCPClient client = new Jet3upTCPClient(address, port);
                client.SetName(name);
                clients.Add(client);
                return client;
            }
            else
            {
                return null;
            }
        }

        public bool isAdressInUse(IPEndPoint endpoint)
        {
            foreach (var client in clients)
            {
                if (client.GetAddress().Address.ToString() == endpoint.Address.ToString() && client.GetAddress().Port == endpoint.Port)
                    return true;
            }
            return false;

        }

        public IClient CreateClient()
        {
            var adress = GetNextAvailableAddress();
            Jet3upTCPClient client = new Jet3upTCPClient(adress.Item1, adress.Item2);
            client.SetName("Imprimanta");
            clients.Add(client);
            return client;
        }

        private Tuple<string, int> GetNextAvailableAddress(string baseIp = "127.0.0.1", int startingPort = 5000, int maxPort = 10000)
        {
            for (int port = startingPort; port <= maxPort; port++)
            {
                var endpoint = IPEndPoint.Parse(baseIp);
                endpoint.Port = port;
                if (!isAdressInUse(endpoint))
                {
                    return new Tuple<string, int>(baseIp, port);
                }
            }

            throw new InvalidOperationException("No available IP/port combinations found in the specified range.");
        }
    }
}
