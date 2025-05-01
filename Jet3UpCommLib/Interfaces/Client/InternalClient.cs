namespace Jet3UpCommLib.Interfaces.Client
{
    internal interface InternalClient : IClient
    {
        /// <summary>
        /// Sets up a new host.
        /// </summary>
        /// <param name="address">The adress where we want to connect to.</param>
        /// <param name="port">The port of the desired machine.</param>
        /// <returns></returns>
        protected internal void SetHost(string address, int port);

    }
}
