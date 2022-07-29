using PetrovskyiETL.Logger;
using PetrovskyiETL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetrovskyiETL.Check
{
    internal class EtlCheck : ICheck
    {
        private readonly ILogger _logger;
        private readonly Regex CorrectNotation = new Regex(@"(?<firstName>\w+ ?\w+), ?(?<lastName>\w*), “(?<adress>\w+, \w+ \d+, \d)”,  (?<payment>\d+\.\d), (?<date>\d{4}-\d{2}-\d{2}), “?(?<account_number>\d{7})”?, (?<service>\w*)");
        
        public EtlCheck(ILogger logger)
        {
            _logger = logger;
        }

        public List<RecordingFormat> CheckFileLines(string path)
        {
            if (path == null)
                throw new Exception("Path is null.");

            var validLines = new List<RecordingFormat>();
            var invalidLines = 0;
            _logger.PathToReadFiles = path;

            for (int i = 0; ; i++)
            {
                var line = _logger.Read(i);

                if (line == null || line.Contains("Exception"))
                {
                    break;
                }
                else if (CorrectNotation.IsMatch(line))
                {
                    var rowData = CorrectNotation.Matches(line);

                    foreach (Match matches in rowData)
                    {
                        var row = matches.Groups;
                        var name = $"{row["firstName"].Value} {row["lastName"].Value}";
                        
                        validLines.Add(new RecordingFormat
                        {
                            City = row["adress"].Value,

                            Services = new List<UserServices>()
                            {
                                {
                                    new UserServices 
                                    {
                                        Name = name,
                                        Payers = new List<Payer>()
                                        {
                                            {
                                                new Payer()
                                                {
                                                Name = name,
                                                Payment = row["payment"].Value,
                                                Date = row["date"].Value,
                                                AccountNumber = row["account_number"].Value
                                                }
                                            }
                                        },
                                        Total = row["payment"].Value
                                    }
                                }
                            },

                            Total = row["payment"].Value
                        }) ;
                    }
                }
                else
                {
                    invalidLines++;
                }
            }

            return validLines;
        }

        public List<string> FileSearch()
        {
            var filesInFolder = Directory.EnumerateFiles(@"C:\Users\Олександр\Desktop\Template", "*", SearchOption.TopDirectoryOnly).ToArray();
            var files = new List<string>();

            foreach (var file in filesInFolder)
            {
                if (file.EndsWith(".csv") || file.EndsWith(".txt"))
                    files.Add(file);
            }

            return files;
        }

    }
}
