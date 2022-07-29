using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL.Logger
{
    internal interface ILogger
    {
        string PathToReadFiles { get; set; }
        int LineNumber { get; set; }
        string Read();
        void Log(string message);
        void MetaLog(string message);
    }
}
