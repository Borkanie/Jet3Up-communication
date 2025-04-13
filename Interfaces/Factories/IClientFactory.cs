using Jet3UpInterfaces.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Interfaces.Factories
{
    public interface IClientFactory
    {
        public IClient createClient(string address, int port, string name);
    }
}
