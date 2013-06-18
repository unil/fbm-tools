using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;

namespace RightsUtility
{
    class ThreadFolders
    {
        public ThreadFolders(MainWindow mw, List<string> listeAdGroupsAccepted, string cible)
        {
            this.mw = mw;
            this.listeAdGroupsAccepted = listeAdGroupsAccepted;
            this.cible = cible;
        }

        MainWindow mw;
        List<string> listeAdGroupsAccepted = new List<string>();
        string cible;
        double done, total = 0;

        public void ThreadMettre()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                mw.Log("Adding deny rights to " + cible + " and sub-directories");
                mw.Log("[" + GetDate() + "] Calculating the number of elements");
                mw.Log("0 elements listed");
                GetNumberOfItem(cible);
                mw.Log("[" + GetDate() + "] Calculation finished");
                mw.Log("[" + GetDate() + "] Initiation of the right's modification operation");
                mw.Log("Loading... This may take a while...");
                Recursive(cible);
                mw.Log("[" + GetDate() + "] Right's modification operation finished");
                mw.IsFinished();
                Thread.CurrentThread.Abort();
            }
        }

        public void ThreadRetirer()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                mw.Log("Removing deny rights to " + cible + " and sub-directories");
                mw.Log("[" + GetDate() + "] Calculating the number of elements");
                mw.Log("0 elements listed");
                GetNumberOfItem(cible);
                mw.Log("[" + GetDate() + "] Calculation finished");
                mw.Log("[" + GetDate() + "] Initiation of the right's modification operation");
                mw.Log("Loading... This may take a while...");
                RecursiveClean(cible);
                mw.Log("[" + GetDate() + "] Right's modification operation finished");
                mw.IsFinished();
                Thread.CurrentThread.Abort();
            }
        }

        private string GetDate()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void GetNumberOfItem(string path)
        {
            total++;
            total += Directory.GetFiles(path).Count();
            mw.Count(total + " elements listed");

            foreach (string folder in Directory.GetDirectories(path))
            {
                GetNumberOfItem(folder);
            }
        }

        private void Recursive(string sourceItem)
        {
            ChangeItemRightsOnGroup(sourceItem);
            foreach (string file in Directory.GetFiles(sourceItem))
            {
                ChangeItemRightsOnFile(file);
            }

            foreach (string folder in Directory.GetDirectories(sourceItem))
            {
                Recursive(folder);
            }
        }

        private void RecursiveClean(string sourceItem)
        {
            ClearItemRightsOnGroup(sourceItem);
            foreach (string file in Directory.GetFiles(sourceItem))
            {
                CleanItemRightsOnFile(file);
            }

            foreach (string folder in Directory.GetDirectories(sourceItem))
            {
                RecursiveClean(folder);
            }
        }

        private void Incremente()
        {
            done++;
            mw.Progress(done, total);
        }

        private void ChangeItemRightsOnFile(string path)
        {
            Incremente();
            FileSecurity fs = File.GetAccessControl(path);
            foreach (FileSystemAccessRule rule in fs.GetAccessRules(true, true, typeof(NTAccount)))
            {
                try
                {
                    string group = rule.IdentityReference.Value;
                    string[] groupsSplit = group.Split('\\');

                    bool change = true;
                    foreach (string right in listeAdGroupsAccepted)
                    {
                        if (groupsSplit[1] == right || groupsSplit[1] == "Système")
                        {
                            change = false;
                            break;
                        }
                    }

                    if (change)
                    {
                        fs.AddAccessRule(new FileSystemAccessRule(@group, FileSystemRights.CreateFiles |
                                                                          FileSystemRights.CreateDirectories |
                                                                          FileSystemRights.WriteAttributes |
                                                                          FileSystemRights.WriteExtendedAttributes |
                                                                          FileSystemRights.Delete |
                                                                          FileSystemRights.DeleteSubdirectoriesAndFiles,
                                                                          InheritanceFlags.None,
                                                                          PropagationFlags.NoPropagateInherit,
                                                                          AccessControlType.Deny));
                        File.SetAccessControl(path, fs);
                        mw.LogAdvance("Modify right for [" + group + "] on File " + path);
                    }
                }
                catch
                {
                    mw.LogAdvance("Error on File " + path);
                }
            }
        }

        private void ChangeItemRightsOnGroup(string path)
        {
            Incremente();
            DirectorySecurity fs = Directory.GetAccessControl(path);
            foreach (FileSystemAccessRule rule in fs.GetAccessRules(true, true, typeof(NTAccount)))
            {
                try
                {
                    string group = rule.IdentityReference.Value;
                    string[] groupsSplit = group.Split('\\');

                    bool change = true;
                    foreach (string right in listeAdGroupsAccepted)
                    {
                        if (groupsSplit[1] == right || groupsSplit[1] == "Système")
                        {
                            change = false;
                            break;
                        }
                    }

                    if (change)
                    {
                        fs.AddAccessRule(new FileSystemAccessRule(@group, FileSystemRights.CreateFiles |
                                                                          FileSystemRights.CreateDirectories |
                                                                          FileSystemRights.WriteAttributes |
                                                                          FileSystemRights.WriteExtendedAttributes |
                                                                          FileSystemRights.Delete |
                                                                          FileSystemRights.DeleteSubdirectoriesAndFiles,
                                                                          InheritanceFlags.None,
                                                                          PropagationFlags.NoPropagateInherit,
                                                                          AccessControlType.Deny));
                        Directory.SetAccessControl(path, fs);
                        mw.LogAdvance("Modify right for [" + group + "] on Directory " + path);
                    }
                }
                catch 
                {
                    mw.LogAdvance("Error on Directory " + path);
                }
            }
        }

        private void CleanItemRightsOnFile(string path)
        {
            Incremente();
            FileSecurity fs = File.GetAccessControl(path);
            foreach (FileSystemAccessRule rule in  fs.GetAccessRules(true, true, typeof(NTAccount)))
            {
                if (rule.AccessControlType == AccessControlType.Deny)
                {
                    fs.RemoveAccessRule((FileSystemAccessRule)rule);
                    File.SetAccessControl(path, fs);
                    mw.LogAdvance("Modify right for [" + rule.IdentityReference.Value + "] on File " + path);
                }
            }
        }

        private void ClearItemRightsOnGroup(string path)
        {
            Incremente();
            DirectorySecurity fs = Directory.GetAccessControl(path);
            foreach (FileSystemAccessRule rule in fs.GetAccessRules(true, true, typeof(NTAccount)))
            {
                if (rule.AccessControlType == AccessControlType.Deny)
                {
                    fs.RemoveAccessRule((FileSystemAccessRule)rule);
                    Directory.SetAccessControl(path, fs);
                    mw.LogAdvance("Modify right for [" + rule.IdentityReference.Value + "] on Directory " + path);
                }
            }
        }
    }
}
