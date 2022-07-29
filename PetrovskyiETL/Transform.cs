using PetrovskyiETL.Check;
using PetrovskyiETL.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetrovskyiETL
{
    internal class Transform
    {
        private readonly ICheck _check;
        private readonly ILogger _logger;

        public Transform(ICheck check, ILogger logger)
        {
            _check = check;
            _logger = logger;
        }

        void transform(string path)
        {
           /* _logger.PathToReadFiles = path;

            for (int i = 0; ; i++)
            {
                GroupCollection rowData;
                var line = _logger.Read(i);

                if (line != null && !line.Contains("Exception"))
                    rowData = _check.CheckFileLines(line);

                var name = $"{rowData["firstName"].Value} {rowData["lastName"].Value}";

                validLines.City.Add(rowData["address"].Value);

                validLines.Services.Add(
                    new UserServices(
                    name,
                    new Payer(name, rowData["payment"].Value, rowData["date"].Value, rowData["account_number"].Value),
                    rowData["payment"].Value));

                validLines.Total.Add(rowData["payment"].Value);


                *//*foreach (Match matches in rowData)
                {
                    var row = matches.Groups;
                    var name = $"{row["firstName"].Value} {row["lastName"].Value}";

                    validLines.City.Add(row["address"].Value);

                    validLines.Services.Add(
                        new UserServices(
                        name,
                        new Payer(name, row["payment"].Value, row["date"].Value, row["account_number"].Value),
                        row["payment"].Value));

                    validLines.Total.Add(row["payment"].Value);
                }*/
            //}

        }
    }
}
