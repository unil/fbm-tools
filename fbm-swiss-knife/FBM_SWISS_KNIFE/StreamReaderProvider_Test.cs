using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FBM_SWISS_KNIFE
{
    class StreamReaderProvider_Test
    {
        public static List<User_Test> GetListOfUsers(string dep, string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            string line;

            List<User_Test> listeUsers = new List<User_Test>();

            line = sr.ReadLine();

            while (line != null)
            {
                listeUsers.Add(new User_Test(dep, line));
                line = sr.ReadLine();
            }
            sr.Close();

            return listeUsers;
        }
    }
}
