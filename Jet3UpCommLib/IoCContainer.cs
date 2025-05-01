// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.


using Jet3UpCommLib.Implementation.Factories;
using Jet3UpCommLib.Interfaces.Factories;
using Jet3UpCommLib.MockUp.Factories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;


namespace Jet3UpCommLib
{
    public class IoCContainer
    {
        private IHost host { get; }
        private IoCContainer()
        {
            host = StartIoCContainer();
        }
        private IHost StartIoCContainer()
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder();
#if DEBUG
            builder.Services.AddSingleton<IClientFactory, ClientFactory>();
            builder.Services.AddSingleton<IUserFactory, UserFactory>();
#else
            builder.Services.AddSingleton<IClientFactory, MockCLientFactory>();
            builder.Services.AddSingleton<IUserFactory, UserFactoryMockup>();

#endif
            IHost host = builder.Build();

            //host.Run();
            return host;

        }

        private static IoCContainer? instance;

        public static IHost Instance
        {
            get
            {
                if (instance == null)
                    instance = new IoCContainer();
                return instance.host;
            }
        }
    }
}
