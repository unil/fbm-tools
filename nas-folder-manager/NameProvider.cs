using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Folder_Creator
{
    class NameProvider
    {
        public static List<Folder> GetListOfFolders(string file)
        {
            List<Folder> listeFolders = new List<Folder>();
            try
            {
                StreamReader sr = new StreamReader(file, Encoding.UTF8);
                string line = sr.ReadLine();

                while (line != null)
                {
                    Folder folder = new Folder();
                    folder.Name = line;
                    folder.CreateContent = true;
                    folder.State = "En attente";
                    folder.StatePerso =   "Personnel : En attente";
                    folder.StatePrivate = "Private : En attente";
                    folder.StatePublic =  "Public : En attente";

                    listeFolders.Add(folder);
                    line = sr.ReadLine();
                }
                sr.Close();

                return listeFolders;
            }
            catch
            {
                return new List<Folder>();
            }
        }
    }
}
