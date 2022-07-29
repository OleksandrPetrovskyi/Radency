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
        string PathToRecordingFiles { get; set; }

        string Read(int lineNumber);
        void Log(string message);
        void MetaLog();
    }
}
