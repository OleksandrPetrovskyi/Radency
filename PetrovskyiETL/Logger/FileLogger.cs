using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL.Logger
{
    internal class FileLogger : ILogger
    {
        private readonly string _outputFolder;

        public string PathToReadFiles { get; set; }
        public int LineNumber { get; set; }

        public FileLogger()
        {
            var folder = DateTime.Now.Date.ToShortDateString().ToString().Replace('/', '.');
            _outputFolder = $@"{ConfigurationManager.AppSettings.Get("Results")}\{folder}";
        }

        public string Read()
        {
            if (PathToReadFiles == null)
                return null;

            string message;

            try
            {
                message = File.ReadLines(PathToReadFiles).Skip(LineNumber).First();
            }
            catch (Exception ex)
            {
                return $"Exception: {ex}";
            }

            return message;
        }

        public void Log(string message)
        {
            if(!Directory.Exists($@"{_outputFolder}"))
                Directory.CreateDirectory($@"{_outputFolder}");

            using (var writer = new StreamWriter($@"{_outputFolder}\output.json", true))
            {
                writer.WriteLineAsync(message);
            }
        }

        public void MetaLog(string message)
        {
            using (var writer = new StreamWriter($@"{_outputFolder}\meta.log", true))
            {
                writer.WriteLineAsync(message);
            }
        }
    }
}
