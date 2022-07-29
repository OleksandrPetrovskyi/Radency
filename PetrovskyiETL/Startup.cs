using Newtonsoft.Json;
using PetrovskyiETL.Check;
using PetrovskyiETL.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PetrovskyiETL
{
    internal class Startup
    {
        private readonly ILogger _logger;
        private readonly ICheck _check;
        private readonly Transformer _transformer;
        public Startup(ILogger logger, ICheck check, Transformer transformer)
        {
            _logger = logger;
            _check = check;
            _transformer = transformer;
        }

        public void Run()
        {
            while (true)
            {
                var files = _check.FileSearch();

                var TTTTT = _transformer.Transform("C:\\Users\\Олександр\\Desktop\\Template\\New Text Document.txt");

                foreach (var t in TTTTT)
                {
                    var serializedContent = JsonConvert.SerializeObject(t);
                }


            }
        }
    }
}
