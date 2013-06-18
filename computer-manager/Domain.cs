using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management;
using System.DirectoryServices;

namespace ProgrammeInformatiqueWpf
{
    class Domain
    {
        public static int Join(String domain, String ou, String username, String password)
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

        public static int Remove(String username, String password)
        {
            int resultat = 0;
            String[] args = new String[4];
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

        public static int RenameComputer(String name)
        {
            int resultat = 0;
            String[] args = new String[1];
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
        public static int RenameMachineByDirectoryServices(String oldname, String newname, String administrator, String administratorPassword)
        {
            //PAS TERMINE
            int res = 0;

            try
            {
                DirectoryEntry machineNode = null;
                machineNode = new DirectoryEntry("WinNT://" + oldname);
                machineNode.Username = administrator;
                machineNode.Password = administratorPassword;
                machineNode.AuthenticationType = AuthenticationTypes.Secure;
                machineNode.Rename("CN=" + newname);
                machineNode.CommitChanges();
            }
            catch (Exception e)
            {
                String msg = e.Message;
                String stacktrace = e.StackTrace;
            }

            return res;
        }
    }
}
