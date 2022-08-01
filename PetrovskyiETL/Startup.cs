using Newtonsoft.Json;
using PetrovskyiETL.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PetrovskyiETL
{
    internal class Startup
    {
        private readonly ManualResetEvent _event = new ManualResetEvent(false);
        private readonly ILogger _logger;
        private readonly Transformer _transformer;
        private readonly string _inputFolder;

        private int _working;
        private int _parsedFiles = 0;
        private List<string> _invalidFiles = new List<string>();
        private int _parsedLines = 0;
        private int _foundErrors = 0;
        
        public Startup(ILogger logger, Transformer transformer)
        {
            _logger = logger;
            _transformer = transformer;

            _inputFolder = ConfigurationManager.AppSettings.Get("FolderWithFiles");
        }

        public void Run()
        {
            Thread thread = new Thread(Work);
            thread.IsBackground = true;
            thread.Start();

            while(true)
            {
                Console.WriteLine(@"Press 1 to pause ETL.
Press 2 to resume ETL.
Press 3 to restart ETL.");
                int.TryParse(Console.ReadLine(), out _working);

                if(thread.ThreadState == ThreadState.Stopped)
                    break;

                else if (_working == 1)
                {
                    _event.Reset();
                    Console.WriteLine("ETL suspended.");
                }

                else if (_working == 2)
                {
                    _event.Set();
                    Console.WriteLine("ETL resumed.");
                }

                else if (_working == 3)
                {
                    _parsedFiles = 0;
                    _invalidFiles = new List<string>();
                    _parsedLines = 0;
                    _foundErrors = 0;

                    thread = new Thread(Work);
                    thread.IsBackground = true;
                    Console.WriteLine("ETL restarted.");
                }
            }

            thread.Join();

            var outpt = $@"parsed_files: {_parsedFiles}
parsed_lines: {_parsedLines}
found_errors: {_foundErrors}
invalid_files: [";

            foreach (var file in _invalidFiles)
            {
                outpt += $"{file}, ";
            }

            outpt = outpt.Remove(outpt.Length - 2);
            outpt += ']';
            _logger.MetaLog(outpt);
        }

        private void Work()
        {
            var filesInFolder = Directory.EnumerateFiles($@"{_inputFolder}", "*", SearchOption.TopDirectoryOnly).ToArray();
            var files = filesInFolder.Where(file => file.EndsWith(".csv") || file.EndsWith(".txt")).ToArray();

            _parsedFiles += filesInFolder.Length;
            _invalidFiles = filesInFolder.Where(file => !file.EndsWith(".csv") && !file.EndsWith(".txt")).ToList();

            foreach (var file in files)
            {
                _event.WaitOne();
                Thread.Sleep(5000);
                var result = _transformer.Transform(file);

                if (result.parsedLines > 0)
                {
                    _parsedLines += result.parsedLines;
                    _foundErrors += result.foundErrors;

                    var serializedContent = JsonConvert.SerializeObject(result.recordings);
                }
            }
        }
    }
}
