using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace ProgrammeInformatiqueWpf
{
    public class Log
    {
        public string ComputerName { get; set; }
        public string Date{ get; set; }
        public string UserName { get; set; }
        public List<string> ComputerParams { get; set; }
    }
}
