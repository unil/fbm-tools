using System;
using System.DirectoryServices;
using System.Management;
using System.Threading;

namespace ProgrammeInformatiqueWpf
{
    class Domain
    {
        public static int Join(string domain, string ou, string username, string password)
        {
            int resultat = 0;
            Object[] args = new Object[5];
            ManagementObjectCollection comps;
            ManagementClass clsComps = new ManagementClass("Win32_ComputerSystem");

            comps = clsComps.GetInstances();

            foreach (ManagementObject comp in comps)
            {
                args[0] = domain;
                args[1] = password;
                args[2] = username;
                args[3] = ou;
                args[4] = 3;
                resultat = Convert.ToInt32(comp.InvokeMethod("JoinDomainOrWorkgroup", args));
            }
            return resultat;
        }

        public static int Remove(string username, string password)
        {
            ProcessExplorer.Create("powershell.exe", @".\SCRIPTS\delete_from_ad_and_set_workgroup.ps1");
            Thread.Sleep(6); // Attente de 6 secondes pour laisser le script s'exécuter

            int resultat = 0;
            string[] args = new String[4];
            ManagementObjectCollection comps;
            ManagementClass clsComps = new ManagementClass("Win32_ComputerSystem");

            comps = clsComps.GetInstances();

            foreach (ManagementObject comp in comps)
            {
                args[0] = password;
                args[1] = username;
                args[2] = "0";

                resultat = Convert.ToInt32(comp.InvokeMethod("UnjoinDomainOrWorkgroup", args));
            }

            return resultat;
        }

        public static int RenameComputer(string name)
        {
            int resultat = 0;
            string[] args = new String[1];
            ManagementObjectCollection comps;
            ManagementClass clsComps = new ManagementClass("Win32_ComputerSystem");

            comps = clsComps.GetInstances();
            args[0] = name;

            foreach (ManagementObject comp in comps)
            {
                resultat = Convert.ToInt32(comp.InvokeMethod("Rename", args));
            }

            return resultat;
        }

        //PAS TERMINE
        //public static int RenameMachineByDirectoryServices(String oldname, String newname, String administrator, String administratorPassword)
        //{
        //    int resultat = 0;

        //    try
        //    {
        //        DirectoryEntry machineNode = null;
        //        machineNode = new DirectoryEntry("WinNT://" + oldname);
        //        machineNode.Username = administrator;
        //        machineNode.Password = administratorPassword;
        //        machineNode.AuthenticationType = AuthenticationTypes.Secure;
        //        machineNode.Rename("CN=" + newname);
        //        machineNode.CommitChanges();
        //    }
        //    catch
        //    {
        //        resultat = 1;
        //    }

        //    return resultat;
        //}
    }
}
