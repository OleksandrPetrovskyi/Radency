using PetrovskyiETL.Check;
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
        private readonly ICheck _check;
        private readonly ILogger _logger;
        private readonly Regex CorrectNotation = new Regex(@"(?<firstName>\w+ ?\w+), ?(?<lastName>\w*), “(?<adress>\w+, \w+ \d+, \d)”,  (?<payment>\d+\.\d), (?<date>\d{4}-\d{2}-\d{2}), “?(?<account_number>\d{7})”?, (?<service>\w*)");

        public Transformer(ICheck check, ILogger logger)
        {
            _check = check;
            _logger = logger;
        }

        public List<RecordingFormat> Transform(string path)
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
                    var row = rowData[0].Groups;

                    var name = $"{row["firstName"].Value} {row["lastName"].Value}";
                    var payer = new Payer
                    {
                        Name = name,
                        Payment = row["payment"].Value,
                        Date = row["date"].Value,
                        AccountNumber = row["account_number"].Value
                    };

                    validLines.Add(new RecordingFormat
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
                    invalidLines++;
                }
            }

            return validLines;
        }
    }
}
