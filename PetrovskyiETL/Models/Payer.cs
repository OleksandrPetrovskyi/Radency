using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL.Models
{
    internal class Payer
    {
        public string Name { get; set; }
        public string Payment { get; set; }
        public string Date    { get; set; }
        public string AccountNumber { get; set; }
    }
}
