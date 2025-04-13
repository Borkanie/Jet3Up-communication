// Copyrigth (c) S.C.SoftLab S.R.L.
// All Rigths reserved.


using Aerotec.Data.Model;
using Jet3UpInterfaces.Factories;
using Jet3UpInterfaces.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Mockup.Client;
using Mockup.Factories;
using Implementation.Factories;
using Implementation.Client;

namespace IoC
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
#if !DEBUG
            builder.Services.AddSingleton<IClient, TCPClient>();
            builder.Services.AddSingleton<IUserFactory, UserFactory>();
#else
            builder.Services.AddSingleton<IClient>(new TCPMockUpClient());
            builder.Services.AddSingleton<IUserFactory>(new UserFactoryMockup());

#endif
            IHost host = builder.Build();

            //host.Run();
            return host;

        }

        private static IoCContainer instance;

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
