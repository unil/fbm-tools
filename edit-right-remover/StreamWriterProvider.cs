using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace RightsUtility
{
    class StreamWriterProvider
    {
        public static List<string> GetAdGroupsAccepted(string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            string line;

            List<string> listeAdGroups = new List<string>();

            line = sr.ReadLine();

            while (line != null)
            {
                listeAdGroups.Add(line);
                line = sr.ReadLine();
            }
            sr.Close();

            return listeAdGroups;
        }
    }
}
