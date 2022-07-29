using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL.Models
{
    internal class UserServices
    {
        public string Name { get; set; }
        public List<Payer> Payers { get; set; } = new List<Payer>();
        public string Total { get; set; }
    }
}
