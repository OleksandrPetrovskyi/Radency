using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL.Logger
{
    internal class FileLogger : ILogger
    {
        public string PathToReadFiles { get; set; }
        public string PathToRecordingFiles { get; set; }

        public string Read(int lineNumber)
        {
            if (PathToReadFiles == null)
                return null;

            string message;

            try
            {
                message = File.ReadLines(PathToReadFiles).Skip(lineNumber).First();
            }
            catch (Exception ex)
            {
                return $"Exception: {ex}";
            }

            return message;
        }

        public void Log(string message)
        {
            throw new NotImplementedException();
        }

        public void MetaLog()
        {
            throw new NotImplementedException();
        }
    }
}
