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
        private readonly ILogger _logger;
        private readonly Regex CorrectNotation = new Regex(@"(?<firstName>\w+ ?\w+), ?(?<lastName>\w*), “(?<adress>\w+, \w+ \d+, \d)”,  (?<payment>\d+\.\d), (?<date>\d{4}-\d{2}-\d{2}), “?(?<account_number>\d{7})”?, (?<service>\w*)");

        public Transformer(ILogger logger)
        {
            _logger = logger;
        }

        public (List<RecordingFormat> recordings, int parsedLines, int foundErrors) Transform(string path)
        {
            _logger.PathToReadFiles = path ?? throw new Exception("Path is null.");

            var recordings = new List<RecordingFormat>();
            var parsedLines = 0;
            var foundErrors = 0;

            for (int i = 0; ; i++)
            {
                _logger.LineNumber = i;
                var line = _logger.Read();

                if (line.Contains("Exception"))
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
                    parsedLines++;
                    foundErrors++;
                }
            }

            return (recordings, parsedLines, foundErrors);
        }
    }
}
