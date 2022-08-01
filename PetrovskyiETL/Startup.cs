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

namespace PetrovskyiETL
{
    internal class Startup
    {
        private readonly ManualResetEvent _event = new ManualResetEvent(true);
        private readonly FileLogger _fileLogger;
        private readonly ConsoleLogger _consoleLogger;
        private readonly Transformer _transformer;
        private readonly string _inputFolder;
        private string errors = string.Empty;
        private int _working;

        private int _parsedFiles = 0;
        private List<string> _invalidFiles = new List<string>();
        private int _parsedLines = 0;
        private int _foundErrors = 0;
        
        public Startup(FileLogger fileLogger, ConsoleLogger consoleLogger, Transformer transformer)
        {
            _fileLogger = fileLogger;
            _consoleLogger = consoleLogger;
            _transformer = transformer;

            _inputFolder = ConfigurationManager.AppSettings.Get("FolderWithFiles");
        }

        public void Run()
        {
            var EtlThread = new Thread(Etl);
            EtlThread.IsBackground = true;
            EtlThread.Start();
            
            var MetalogThread = new Thread(RecordInMetalog);
            MetalogThread.IsBackground = true;
            MetalogThread.Start();

            while(true)
            {
                _consoleLogger.DisplayMessage(@"Press 1 to pause ETL.
Press 2 to resume ETL.
Press 3 to restart ETL.");
                int.TryParse(_consoleLogger.InputMessage(), out _working);

                if(EtlThread.ThreadState == ThreadState.Stopped)
                    break;

                else if (_working == 1)
                {
                    _event.Reset();
                    _consoleLogger.DisplayMessage("ETL suspended.");
                }

                else if (_working == 2)
                {
                    _event.Set();
                    _consoleLogger.DisplayMessage("ETL resumed.");
                }

                else if (_working == 3)
                {
                    _parsedFiles = 0;
                    _invalidFiles = new List<string>();
                    _parsedLines = 0;
                    _foundErrors = 0;

                    EtlThread = new Thread(Etl);
                    EtlThread.IsBackground = true;
                    _consoleLogger.DisplayMessage("ETL restarted.");
                }
            }

            errors += _fileLogger.errors;
            if (errors.Length > 0)
                _consoleLogger.DisplayMessage(errors);
            else
                _consoleLogger.DisplayMessage("There were no runtime errors.");
        }

        private void Etl()
        {
            var filesInFolder = Directory.EnumerateFiles($@"{_inputFolder}", "*", SearchOption.TopDirectoryOnly).ToArray();
            var files = filesInFolder.Where(file => file.EndsWith(".csv") || file.EndsWith(".txt")).ToArray();

            _parsedFiles += filesInFolder.Length;
            _invalidFiles = filesInFolder.Where(file => !file.EndsWith(".csv") && !file.EndsWith(".txt")).ToList();

            foreach (var file in files)
            {
                _event.WaitOne();
                var result = _transformer.Transform(file);

                if (result.recordings.Count > 0)
                {
                    _parsedLines += result.parsedLines;
                    _foundErrors += result.foundErrors;

                    var serializedContent = JsonConvert.SerializeObject(result.recordings);
                    _fileLogger.Record(serializedContent);
                }
            }
        }

        private void RecordInMetalog()
        {
            while (true)
            {
                if (DateTime.Now.ToShortTimeString() == "11:58 PM")
                {
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
                    _fileLogger.ImportantRecord(outpt);
                }
            }
        }

    }
}
