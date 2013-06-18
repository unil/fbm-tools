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

        public void Serialize(Configs config, List<Log> listeLogs)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Log>));
                using (StreamWriter stream = new StreamWriter(config.Path + "log.xml"))
                {
                    xs.Serialize(stream, listeLogs);
                }
            }
            catch
            { }
        }

        public List<Log> Deserialize(Configs config)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Log>));
                using (StreamReader stream = new StreamReader(GetPath(config) + "log.xml"))
                {
                    return xs.Deserialize(stream) as List<Log>;
                }
            }
            catch
            {
                return new List<Log>();
            }
        }

        private string GetPath(Configs config)
        {
            string path = "\\\\fbmnas\\DATA\\SYS\\SCRIPTS\\ADMIN\\";
            if (config.Path != path)
            {
                path = config.Path;
            }
            return path;
        }
    }
}
