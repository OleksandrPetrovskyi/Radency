using PetrovskyiETL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PetrovskyiETL.Check
{
    internal interface ICheck
    {
        List<string> FileSearch();
        List<RecordingFormat> CheckFileLines(string path);
    }
}
