using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows;

namespace ProgrammeInformatiqueWpf
{
    public class Configs
    {
        public string AD { get; set; }
        public string LDAP { get; set; }
        public string OU { get; set; }
        public string Path { get; set; }

        public void Serialize(Configs config)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Configs));
                using (StreamWriter stream = new StreamWriter(GetPath(config) + "configs.xml"))
                {
                    xs.Serialize(stream, config);
                }
            }
            catch
            { }
        }

        public Configs Deserialize(Configs config)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Configs));
                using (StreamReader stream = new StreamReader(GetPath(config) + "configs.xml"))
                {
                    return xs.Deserialize(stream) as Configs;
                }
            }
            catch
            {
                return config;
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
