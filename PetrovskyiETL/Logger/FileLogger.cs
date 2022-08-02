using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL.Logger
{
    internal class FileLogger
    {
        private readonly string _outputFolder;
        public string errors = string.Empty;
        public string PathToReadFiles { get; set; }
        public int LineNumber { get; set; }

        public FileLogger()
        {
            _outputFolder = ConfigurationManager.AppSettings.Get("Results");
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
            catch(InvalidOperationException)
            {
                return null;
            }
            catch (Exception ex)
            {
                errors += $"{ex}{Environment.NewLine}";
                return $"Exception: {ex}";
            }

            return message;
        }

        public void Record(string message)
        {
            if(!Directory.Exists($@"{_outputFolder}"))
                Directory.CreateDirectory($@"{_outputFolder}");

            var folder = DateTime.Now.Date.ToShortDateString().ToString().Replace('/', '.');

            using (var writer = new StreamWriter($@"{_outputFolder}\{folder}\output.json", true))
            {
                writer.WriteLineAsync(message);
            }
        }

        public void ImportantRecord(string message)
        {
            var folder = DateTime.Now.Date.ToShortDateString().ToString().Replace('/', '.');

            using (var writer = new StreamWriter($@"{_outputFolder}{folder}\meta.log", true))
            {
                writer.WriteLineAsync(message);
            }
        }
    }
}
