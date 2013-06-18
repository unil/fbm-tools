﻿using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Folder_Rename_Wpf
{
    class ThreadRenamer
    {
        public ThreadRenamer(MainWindow mw)
        {
            this.mw = mw;
        }

        private MainWindow mw;
        private int renamedFolderCount = 0;
        private int totalFolderCount = 0;
        private List<DirectoryInfo> listeDirectory;

        public void Work()
        {
            while (Thread.CurrentThread.IsAlive)
            {            
                mw.logger.Log(mw.listePath.Count + " folder(s) in the list");
                foreach (string path in mw.listePath)
                {
                    try
                    {
                        mw.logger.Log("\nWorking with the folder " + path + " and its subdirectories");
                        mw.logger.Log("##############################################");
                        if (Directory.Exists(path))
                        {
                            DirectoryInfo directory = new DirectoryInfo(path);

                            switch (directory.Name.ToUpper())
                            {
                                case "USERS":
                                    GetFoldersFromUsers(directory);
                                    break;
                                case "GROUPS":
                                    GetFoldersFromGroups(directory);
                                    break;
                            }

                            for (int x = listeDirectory.Count - 1; x >= 0; x--)
                            {
                                RenameFolder(listeDirectory[x]);
                            }
                        }
                        else
                        {
                            mw.logger.Log("The specified directory doesn't exist");
                        }
                    }
                    catch { }
                }
                mw.logger.Log("\n############# OPERATION FINISHED #############\n");
                mw.logger.Log(totalFolderCount + " folders listed, " + renamedFolderCount + " actually renamed");

                Thread.CurrentThread.Abort();
            }
        }

        private void GetFoldersFromUsers(DirectoryInfo mainDirectory)
        {
            listeDirectory = new List<DirectoryInfo>();
            foreach (DirectoryInfo directory in mainDirectory.GetDirectories())
            {               
                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    try
                    {
                        AddDirectory(dir);
                    }
                    catch
                    {
                        mw.logger.Log("Error while adding [" + directory.FullName + "], or a subdirectory, to the list");
                    }
                }                
            }
        }

        private void GetFoldersFromGroups(DirectoryInfo mainDirectory)
        {
            listeDirectory = new List<DirectoryInfo>();
            foreach (DirectoryInfo directory in mainDirectory.GetDirectories())
            {
                try
                {
                    AddDirectory(directory);
                }                    
                catch
                {
                    mw.logger.Log("Error while adding [" + directory.FullName + "], or a subdirectory, to the list");
                }

                foreach (DirectoryInfo dir in directory.GetDirectories())
                {
                    try
                    {
                        AddDirectory(dir);
                    }
                    catch
                    {
                        mw.logger.Log("Error while adding [" + directory.FullName + "], or a subdirectory, to the list");
                    }

                    foreach (DirectoryInfo d in dir.GetDirectories())
                    {
                        try
                        {
                            AddDirectory(d);
                        }
                        catch
                        {
                            mw.logger.Log("Error while adding [" + directory.FullName + "], or a subdirectory, to the list");
                        }
                    }
                }
            }
        }

        private void AddDirectory(DirectoryInfo directory)
        {
            listeDirectory.Add(directory);
            totalFolderCount++;
            mw.logger.Log("Add [" + directory.FullName + "] to the list");
        }

        private void RenameFolder(DirectoryInfo directory)
        {
            try
            {
                string newName = "";

                foreach (Traduction trad in mw.listeTraduction)
                {
                    if (trad.Fr == directory.Name.ToUpper())
                    {
                        newName = trad.En;
                        break;
                    }
                }

                if (newName != "")
                {
                    Directory.Move(directory.FullName, directory.Parent.FullName + "\\" + newName);
                    mw.logger.Log("Rename [" + directory.FullName + "] to [" + directory.Parent.FullName + "\\" + newName + "]");
                    renamedFolderCount++;
                }
            }
            catch
            {
                mw.logger.Log("Error while renaming [" + directory.FullName + "]");
            }
        }    
    }
}
