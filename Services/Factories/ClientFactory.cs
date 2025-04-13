using Implementation.Client;
using Interfaces.Factories;
using Jet3UpInterfaces.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Implementation.Factories
{
    public class ClientFactory : IClientFactory
    {
        public IClient createClient(string address, int port, string name)
        {
            Jet3upTCPClient client = new Jet3upTCPClient();
            client.SetHost(address, port);
            client.SetName(name);
            return client;
        }
    }
}
