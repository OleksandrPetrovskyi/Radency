using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL.Models
{
    internal class RecordingFormat
    {
        public string City { get; set; }
        public List<UserServices> Services { get; set; }
        public string Total { get; set; }
    }
}
