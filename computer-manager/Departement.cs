using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
using System.Windows;
using System;
using System.Text;
using System.Diagnostics;

namespace ProgrammeInformatiqueWpf
{
    public class Departement
    {
        public string Nom { get; set; }
        public string Abreviation { get; set; }
        public string OU { get; set; }

        public void Serialize(List<Departement> listeDepartements, Configs config)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Departement>));
                using (StreamWriter stream = new StreamWriter(config.Path + "departements.xml"))
                {
                    xs.Serialize(stream, listeDepartements);
                }
            }
            catch { }
        }

        public List<Departement> Deserialize(Configs config)
        {
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(List<Departement>));
                using (StreamReader stream = new StreamReader(config.Path + "departements.xml"))
                {
                    return xs.Deserialize(stream) as List<Departement>;
                }
            }
            catch(IOException)
            {
                MessageBox.Show("Vous devez vous connecter à \\\\nas.unil.ch pour avoir accès aux ressources du programme", "Connexion nécessaire", MessageBoxButton.OK, MessageBoxImage.Information);
                Process.Start("explorer.exe", @"\\fbmnas");
                return new List<Departement>();
            }
        }
    }
}
