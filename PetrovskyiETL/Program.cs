using Microsoft.Extensions.DependencyInjection;
using PetrovskyiETL.Check;
using PetrovskyiETL.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var service = new ServiceCollection()
            .AddSingleton<ILogger, FileLogger>()
            .AddSingleton<ICheck, EtlCheck>()
            .AddSingleton<Startup>()
            .BuildServiceProvider();

            service.GetRequiredService<Startup>().Run();
        }
    }
}
