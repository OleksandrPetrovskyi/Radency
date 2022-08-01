using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL.Logger
{
    internal class ConsoleLogger
    {
        public string InputMessage()
        {
            return Console.ReadLine();
        }

        public void DisplayMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
