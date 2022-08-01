using Microsoft.Extensions.DependencyInjection;
using PetrovskyiETL.Logger;

namespace PetrovskyiETL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var service = new ServiceCollection()
            .AddSingleton<FileLogger>()
            .AddSingleton<Transformer>()
            .AddSingleton<Startup>()
            .AddSingleton<ConsoleLogger>()
            .BuildServiceProvider();

            service.GetRequiredService<Startup>().Run();
        }
    }
}
