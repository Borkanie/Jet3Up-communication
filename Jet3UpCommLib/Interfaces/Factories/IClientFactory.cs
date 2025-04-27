using Jet3UpCommLib.Interfaces.Client;
using System.Net;

namespace Jet3UpCommLib.Interfaces.Factories
{
    public interface IClientFactory
    {
        /// <summary>
        /// Creates a new client cxonnected to amachine. If ip port combination is already in use returns null.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <param name="name"></param>
        /// <returns>The new object or null.</returns>
        public IClient? CreateClient(string address, int port, string name);

        /// <summary>
        /// Changes adress assiigned to a client.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public bool ChangeAdressToClient(IClient client, string address, int port);

        /// <summary>
        /// Checks whetear address is unique.
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public bool isAdressInUse(IPEndPoint endpoint);


        public bool Remove(IClient client);

        public IClient CreateClient();
    }

}
