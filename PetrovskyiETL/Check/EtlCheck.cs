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
