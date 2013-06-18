using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ProgrammeInformatiqueWpf
{
    class SerializeProvider
    {
        // Utile seulement pour créer le fichier XML de configuration
        //public static void SerializeConfiguration(string path)
        //{
        //    Configuration conf = new Configuration();
        //    conf.AD = "ad.unil.ch";
        //    conf.LDAP = "ad.unil.ch";
        //    conf.OU = "";

        //    try
        //    {
        //        XmlSerializer xs = new XmlSerializer(typeof(Configuration));
        //        using (StreamWriter stream = new StreamWriter(path + "configuration.xml"))
        //        {
        //            xs.Serialize(stream, conf);
        //        }
        //    }
        //    catch
        //    { }
        //}

        public static Configuration DeserializeConfiguration(string path)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(Configuration));
                using (StreamReader stream = new StreamReader(path))
                {
                    return xs.Deserialize(stream) as Configuration;
                }
            }
            catch
            {
                return new Configuration(); ;
            }
        }

        // Utile seulement pour créer le fichier XML des départements
        //public static void SerializeDepartements(string path, List<Departement> listeDepartements)
        //{
        //    Departement departement = new Departement();
        //    departement.Nom = "Décanat";
        //    departement.OU = "";
        //    departement.Abreviation = "DEC";

        //    try
        //    {
        //        XmlSerializer xs = new XmlSerializer(typeof(List<Departement>));
        //        using (StreamWriter stream = new StreamWriter(path + "departements.xml"))
        //        {
        //            xs.Serialize(stream, listeDepartements);
        //        }
        //    }
        //    catch { }
        //}

        public static List<Departement> DeserializeDepartements(string path)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Departement>));
                using (StreamReader stream = new StreamReader(path))
                {
                    return xs.Deserialize(stream) as List<Departement>;
                }
            }
            catch
            {
                return new List<Departement>();
            }
        }

        public static void SerializeLog(string path, List<Log> listeLogs)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Log>));
                using (StreamWriter stream = new StreamWriter(path))
                {
                    xs.Serialize(stream, listeLogs);
                }
            }
            catch
            { }
        }

        public static List<Log> DeserializeLog(string path)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Log>));
                using (StreamReader stream = new StreamReader(path))
                {
                    return xs.Deserialize(stream) as List<Log>;
                }
            }
            catch
            {
                return new List<Log>();
            }
        }
    }
}
