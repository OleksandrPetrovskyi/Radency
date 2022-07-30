using Newtonsoft.Json;
using PetrovskyiETL.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL
{
    internal class Startup
    {
        private readonly ILogger _logger;
        private readonly Transformer _transformer;
        private readonly string _inputFolder;
        private readonly string _outputFolder;

        public Startup(ILogger logger, Transformer transformer)
        {
            _logger = logger;
            _transformer = transformer;

            _inputFolder = ConfigurationManager.AppSettings.Get("FolderWithFiles");

            var folder = DateTime.Now.Date.ToShortDateString().ToString().Replace('/', '.');
            _outputFolder = $@"{ConfigurationManager.AppSettings.Get("Results")}\{folder}";
        }

        public void Run()
        {
            var parsedFiles = 0;
            var invalidFiles = new List<string>();
            var parsedLines = 0;
            var foundErrors = 0;

            var filesInFolder = Directory.EnumerateFiles($@"{_inputFolder}", "*", SearchOption.TopDirectoryOnly).ToArray();
            var files = filesInFolder.Where(file => file.EndsWith(".csv") || file.EndsWith(".txt")).ToArray();
            
            parsedFiles += filesInFolder.Length;
            invalidFiles = filesInFolder.Where(file => !file.EndsWith(".csv") && !file.EndsWith(".txt")).ToList();
            
            foreach (var file in files)
            {
                var result = _transformer.Transform(file);

                if (result.parsedLines == 0)
                    continue;

                parsedLines += result.parsedLines;
                foundErrors += result.foundErrors;

                foreach (var t in result.recordings)
                {
                    var serializedContent = JsonConvert.SerializeObject(t);
                    _logger.Log(serializedContent);
                }
            }
            var outpt = $@"parsed_files: {parsedFiles}
parsed_lines: {parsedLines}
found_errors: {foundErrors}
invalid_files: [";
            foreach (var file in invalidFiles)
            {
                outpt += $"{file}, ";
            }
            outpt = outpt.Remove(outpt.Length - 2);
            outpt += ']';
            _logger.MetaLog(outpt);
        }
    }
}
