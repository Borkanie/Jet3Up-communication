using Implementation.Client;
using Interfaces.Factories;
using Jet3UpInterfaces.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Implementation.Factories
{
    public class ClientFactory : IClientFactory
    {

        private List<IClient> clients = new List<IClient>();

        public bool RemoveClient(IClient client)
        {
            try
            {
                if (client.IsConnected())
                {
                    client.StopCommand();
                    client.StopListening();
                    client.Disconect();
                }
                clients.Remove(client);
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
            if (isAdressNotInUse(endpoint) && !client.IsConnected())
            {
                client.SetHost(address, port);
                return true;
            }
            else
            {
                return false;
            }
        }

        public IClient? createClient(string address, int port, string name)
        {
            var endpoint = IPEndPoint.Parse(address);
            endpoint.Port = port;
            if (isAdressNotInUse(endpoint))
            {
                Jet3upTCPClient client = new Jet3upTCPClient();
                client.SetHost(address, port);
                client.SetName(name);
                clients.Add(client);
                return client;
            }
            else
            {
                return null; 
            }
        }

        public bool isAdressNotInUse(IPEndPoint endpoint)
        {
            return clients.Any((client) => client.GetAddress() == endpoint);
        }
    }
}
