using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace FBM_SWISS_KNIFE.Renommeur_de_dossier
{
    class SerializeProvider
    {
        public static List<string> GetListOfPath(string path)
        {
            try
            {
                List<string> listePath = new List<string>();
                StreamReader sr = new StreamReader(path, Encoding.UTF8);
                string line = sr.ReadLine();

                while (line != null)
                {
                    listePath.Add(line);
                    line = sr.ReadLine();
                }
                sr.Close();

                return listePath;
            }
            catch
            {
                return new List<string>();
            }
        }

        public static void SerializeTraduction(List<Traduction> listeTraduction)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Traduction>));
                using (StreamWriter stream = new StreamWriter(@".\folder_rename_traduction.xml"))
                {
                    xs.Serialize(stream, listeTraduction);
                }
            }
            catch { }
        }

        public static List<Traduction> DeserializeTraduction()
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Traduction>));
                using (StreamReader stream = new StreamReader(@".\folder_rename_traduction.xml"))
                {
                    return xs.Deserialize(stream) as List<Traduction>;
                }
            }
            catch
            {
                return new List<Traduction>();
            }
        }
    }
}
