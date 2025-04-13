using Interfaces.Factories;
using Jet3UpInterfaces.Client;
using Mockup.Client;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
