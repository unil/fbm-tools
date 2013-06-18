using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Log_Reader
{
    public class DeployScript
    {
        public DeployScript(string MachineName, string Date, string Text)
        {
            this.MachineName = MachineName;
            this.Date = Date;
            this.Text = Text;
        }

        public string MachineName { get; set; }
        public string Date { get; set; }
        public string Text {get; set; }
    }
}
