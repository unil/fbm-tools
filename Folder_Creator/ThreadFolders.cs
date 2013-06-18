using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Windows;

namespace Folder_Creator
{
    class ThreadFolders
    {
        public ThreadFolders(MainWindow mw, List<Folder> listeFolders, DirectoryInfo destinationFolder, string departement)
        {
            this.mw = mw;
            this.listeFolders = listeFolders;
            this.destinationFolder = destinationFolder;
            this.departement = departement;
        }

        private MainWindow mw;
        private List<Folder> listeFolders;
        private List<string> listeFoldersErrors = new List<string>();
        private DirectoryInfo destinationFolder;
        private string departement;
        private bool error = false;

        public void ThreadJob()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                mw.Log("### Début de l'opération ###");
                int compteur = 0; // compteur pour avoir l'index de l'objet
                foreach (Folder folder in listeFolders)
                {
                    error = false;
                    mw.MiseAJour(compteur, "En cours", 1);
                    string newFolder = destinationFolder.FullName + @"\" + folder.Name;

                    try
                    {
                        // Création du dossier de la personne, porte son nom. Pas de droits spécifiques si on créé les dossiers PRIVATE et PUBLIC sinon les droits sont hérités.
                        CreatePerso(newFolder, folder);
                        mw.MiseAJour(compteur, "Terminé", 2);
                    }
                    catch(Exception ex)
                    {
                        // On log l'erreur dans la partie log du programme et on affiche l'erreur dans la liste principale
                        mw.Log("[" + folder.Name + "] " + ex.Message);
                        mw.MiseAJour(compteur, "Erreur", 2);
                        listeFoldersErrors.Add(newFolder);
                        error = true;
                    }

                    // Si on veut le contenu du dossier pour cet utilisateur (PRIVATE, PUBLIC) on entre...
                    if (folder.CreateContent)
                    {
                        try
                        {
                            // Créé le dossier PRIVATE
                            CreatePrivate(newFolder + @"\PRIVATE", folder);
                            mw.MiseAJour(compteur, "Terminé", 3);
                        }
                        catch(Exception ex)
                        {
                            mw.Log("[" + folder.Name + "] " + ex.Message);
                            mw.MiseAJour(compteur, "Erreur", 3);
                            listeFoldersErrors.Add(newFolder);
                            error = true;
                        }

                        try
                        {
                            // Créé le dossier PUBLIC
                            CreatePublic(newFolder + @"\PUBLIC", folder);
                            mw.MiseAJour(compteur, "Terminé", 4);
                        }
                        catch(Exception ex)
                        {
                            mw.Log("[" + folder.Name + "] " + ex.Message);
                            mw.MiseAJour(compteur, "Erreur", 4);
                            error = true;
                        }
                    }
                    else 
                    {
                        // ... sinon on affiche l'état désactivé dans la liste principale
                        mw.MiseAJour(compteur, "Désactivé", 3);
                        mw.MiseAJour(compteur, "Désactivé", 4);
                    }

                    // Message de fin de création
                    string message = "Terminé";
                    if (error)
                    {
                        // Si on a eu une erreur pour le dossier utilisateur ou un sous-dossier, on le note
                        message += " avec erreur";
                    }

                    mw.MiseAJour(compteur, message, 1);

                    compteur++;
                }

                mw.SetFoldersOnError(listeFoldersErrors);
                mw.Log("### Opération terminée  ###");
                mw.Log(listeFolders.Count + " dossiers créés, " + listeFoldersErrors.Count + " erreurs durant la création.");
                Thread.CurrentThread.Abort();
            }
        }

        private void CreatePerso(string persoFolder, Folder folder)
        {
            // Create the new personnal directory
            Directory.CreateDirectory(persoFolder);

            DirectoryInfo info = new DirectoryInfo(persoFolder);
            DirectorySecurity dirSecurity = info.GetAccessControl();
            dirSecurity.SetAccessRuleProtection(false, true);

            // Get the directory for checkings
            if (!folder.CreateContent)
            {
                AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));
                foreach (AuthorizationRule rule in rules)
                {
                    if (rule is FileSystemAccessRule)
                    {
                        dirSecurity.RemoveAccessRule((FileSystemAccessRule)rule);
                    }
                }

                //Create the new rules for the directory
                dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\" + folder.Name, FileSystemRights.FullControl & ~FileSystemRights.DeleteSubdirectoriesAndFiles & ~FileSystemRights.TakeOwnership & ~FileSystemRights.ChangePermissions, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\fbm-" + departement + "-admin-acl-g", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\Domain Admins", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                Directory.SetAccessControl(persoFolder, dirSecurity);
            }
        }

        private void CreatePrivate(string privateFolder, Folder folder)
        {
            // Create the new private directory
            Directory.CreateDirectory(privateFolder);

            // Get the directory for checkings
            DirectoryInfo info = new DirectoryInfo(privateFolder);
            DirectorySecurity dirSecurity = info.GetAccessControl();
            dirSecurity.SetAccessRuleProtection(true, false);

            // Get the rules for the directory and suppress them
            AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));
            foreach (AuthorizationRule rule in rules)
            {
                if (rule is FileSystemAccessRule)
                {
                    dirSecurity.RemoveAccessRule((FileSystemAccessRule)rule);
                }
            }

            //Create the new rules for the directory
            dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\" + folder.Name, FileSystemRights.FullControl & ~FileSystemRights.DeleteSubdirectoriesAndFiles & ~FileSystemRights.TakeOwnership & ~FileSystemRights.ChangePermissions, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\fbm-" + departement + "-admin-acl-g", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\Domain Admins", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            Directory.SetAccessControl(privateFolder, dirSecurity);
        }

        private void CreatePublic(string publicFolder, Folder folder)
        {
            // Create the new public directory
            Directory.CreateDirectory(publicFolder);

            // Get the directory for checkings
            DirectoryInfo info = new DirectoryInfo(publicFolder);
            DirectorySecurity dirSecurity = info.GetAccessControl();
            dirSecurity.SetAccessRuleProtection(false, false);

            // Get the rules for the directory and suppress them
            AuthorizationRuleCollection rules = dirSecurity.GetAccessRules(true, true, typeof(NTAccount));
            foreach (AuthorizationRule rule in rules)
            {
                if (rule is FileSystemAccessRule)
                {
                    dirSecurity.RemoveAccessRule((FileSystemAccessRule)rule);
                }
            }

            //Create the new rules for the directory
            dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\fbm-" + departement + "-g", FileSystemRights.FullControl & ~FileSystemRights.DeleteSubdirectoriesAndFiles & ~FileSystemRights.TakeOwnership & ~FileSystemRights.ChangePermissions, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\fbm-" + departement + "-admin-acl-g", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\Domain Admins", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            Directory.SetAccessControl(publicFolder, dirSecurity);
        }
    }
}
