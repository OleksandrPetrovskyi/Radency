using PetrovskyiETL.Logger;
using PetrovskyiETL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetrovskyiETL
{
    internal class Transformer
    {
        public string errors = string.Empty;
        private readonly FileLogger _fileLogger;
        private readonly Regex CorrectNotation = new Regex(@"(?<firstName>\w+), *?(?<lastName>\w+), *“(?<adress>\w+, \w+ \d+, \d)”,  *(?<payment>\d+\.\d), *(?<date>\d{4}-\d{2}-\d{2}), *“?(?<account_number>\d{7})”?, *(?<service>\w*)");

        public Transformer(FileLogger fileLogger)
        {
            _fileLogger = fileLogger;
        }

        public (List<RecordingFormat> recordings, int parsedLines, int foundErrors) Transform(string path)
        {
            _fileLogger.PathToReadFiles = path ?? throw new Exception("Path is null.");

            var lineNumber = 0;
            var recordings = new List<RecordingFormat>();
            var parsedLines = 0;
            var foundErrors = 0;
            
            if (path.EndsWith(".csv"))
                lineNumber = 1;

            for(; ; lineNumber++ )
            {
                _fileLogger.LineNumber = lineNumber;
                var line = _fileLogger.Read();

                if (line == null)
                {
                    break;
                }

                else if (CorrectNotation.IsMatch(line))
                {
                    parsedLines++;
                    var row = CorrectNotation.Matches(line)[0].Groups;

                    if(row["firstName"].Value.Length == 0 && row["lastName"].Value.Length == 0)
                    {
                        foundErrors++;
                        continue;
                    }

                    var name = $"{row["firstName"].Value} {row["lastName"].Value}";
                    var payer = new Payer
                    {
                        Name = name,
                        Payment = row["payment"].Value,
                        Date = row["date"].Value,
                        AccountNumber = row["account_number"].Value
                    };

                    recordings.Add(new RecordingFormat
                    {
                        City = row["adress"].Value,

                        Services = new List<UserServices>()
                        {
                            new UserServices
                            {
                                Name = name,
                                Payers = { payer },
                                Total = row["payment"].Value
                            }
                        },

                        Total = row["payment"].Value
                    });
                }

                else
                {
                    errors += $"Invalid record in {path} on line {lineNumber}.{Environment.NewLine}";
                    parsedLines++;
                    foundErrors++;
                }
            }

            return (recordings, parsedLines, foundErrors);
        }
    }
}
