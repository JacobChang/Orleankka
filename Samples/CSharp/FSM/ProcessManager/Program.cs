using System;
using System.Reflection;
using System.Threading.Tasks;

using Orleans;
using Orleankka;
using Orleankka.Client;
using Orleankka.Cluster;

using Orleans.Hosting;
using Orleans.Core;

using static System.Console;

namespace Example
{
    using Microsoft.Extensions.DependencyInjection;

    public static class Program
    {   
        public static async Task Main()
        {
            WriteLine("Running example. Booting cluster might take some time ...\n");

            var host = await new SiloHostBuilder()
                .ConfigureServices(s => s.AddTransient(typeof(IStorage<CopierData>), new StateStorageBridge<CopierData>("copier-state")))
                .ConfigureApplicationParts(x => x
                    .AddApplicationPart(Assembly.GetExecutingAssembly())
                    .WithCodeGeneration())
                .UseOrleankka()
                .Start();

            var client = await host.Connect();
            await Run(client.ActorSystem());

            Write("\n\nPress any key to terminate ...");
            ReadKey(true);

            host.Dispose();
            Environment.Exit(0);
        }

        static async Task Run(IActorSystem system)
        {
        }
    }
}
