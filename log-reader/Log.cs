using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Log_Reader
{
    public class Log
    {
        public Log(string MachineName, string Date, string ScriptName, string UserName, string Text)
        {
            this.MachineName = MachineName;
            this.Date = Date;
            this.ScriptName = ScriptName;
            this.UserName = UserName;
            this.Text = Text;
        }

        public string MachineName { get; set; }
        public string Date { get; set; }
        public string ScriptName { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
    }
}
