using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Net.NetworkInformation;
using System.Windows;
using System.Security;

namespace ProgrammeInformatiqueWpf
{
    class Utils
    {
        public static bool IsUacEnabled()
        {
            try
            {
                bool res = true;
                RegistryKey rootLocalMachine = Registry.LocalMachine;
                RegistryKey registryKeySoftware = rootLocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System", true);
                string value = registryKeySoftware.GetValue("EnableLUA").ToString();

                if (value == "0")
                {
                    res = false;
                }
                return res;
            }
            catch(SecurityException)
            {
                MessageBox.Show("Vous devez exécuter ce programme avec les droits administrateurs afin de pouvoir effectuer les changements nécessaires.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
                return true;
            }
        }

        public static string DisableUAC()
        {
            return ProcessExplorer.Create("powershell.exe", "Set-ItemProperty HKLM:\\Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System -name EnableLUA -type DWord -value 0");
        }


        public static bool CheckComputerName(string departement)
        {
            bool res = false;
            string nom = Environment.MachineName;

            res = nom.Substring(0, departement.Length) == departement;

            return res;
        }

        public static List<string> GetComputerInformations()
        {
            List<string> param = new List<string>();
            param.Add("Is 64bits : " + Environment.Is64BitOperatingSystem);
            param.Add("Machine Name : " + Environment.MachineName);
            param.Add("System Version : " + Environment.OSVersion.VersionString);
            param.Add("Processor Count : " + Environment.ProcessorCount.ToString());
            param.Add("UserName : " + Environment.UserName);
            param.Add("Domain Name : " + Environment.UserDomainName.ToString());
            param.Add("Version Build : " + Environment.Version.Build.ToString());
            param.Add("Version Major : " + Environment.Version.Major.ToString());
            param.Add("Version Major Revision : " + Environment.Version.MajorRevision.ToString());
            param.Add("Version Minor : " + Environment.Version.Minor.ToString());
            param.Add("Version Minor Revision : " + Environment.Version.MinorRevision.ToString());
            param.Add("Version Revision : " + Environment.Version.Revision.ToString());

            NetworkInterface network = NetworkInterface.GetAllNetworkInterfaces()[0];
            string macAdress = BitConverter.ToString(network.GetPhysicalAddress().GetAddressBytes());
            param.Add("MacAdress : " + macAdress);

            // Retiré car trop de possibilités dans le tableau de récupération des ip
            //IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            //IPAddress ipAddressV4 = ipHostEntry.AddressList[2];
            //param.Add("IPv4 Adress : " + ipAddressV4.ToString());
            //IPAddress ipAddressV6 = ipHostEntry.AddressList[3];
            //param.Add("IPv6 Adress : " + ipAddressV6.ToString());

            return param;
        }

        public static string GetErrorMessage(int errorNb)
        {
            string strErrorDescription = "";
            switch (errorNb)
            {
                case -1:
                    strErrorDescription = Environment.NewLine +
                             "Le nom actuel de l'ordinateur ne correspond pas aux critères. Veuillez d'abord changer le nom." +
                             Environment.NewLine +
                             "Exemple: DPT23302";
                    break;
                case 5:
                    strErrorDescription = "Access is denied";
                    break;
                case 87:
                    strErrorDescription = "The parameter is incorrect";
                    break;
                case 110:
                    strErrorDescription = "The system cannot open the specified object";
                    break;
                case 1323:
                    strErrorDescription = "Unable to update the password";
                    break;
                case 1326:
                    strErrorDescription = "Logon failure: unknown username or bad password";
                    break;
                case 1355:
                    strErrorDescription = "The specified domain either does not exist or could not be contacted";
                    break;
                case 2224:
                    strErrorDescription = "The account already exists";
                    break;
                case 2691:
                    strErrorDescription = "The machine is already joined to the domain";
                    break;
                case 2692:
                    strErrorDescription = "The machine is not currently joined to a domain";
                    break;
                default:
                    strErrorDescription = "Erreur n° " + errorNb;
                    break;
            }
            return strErrorDescription;
        }
    }
}
