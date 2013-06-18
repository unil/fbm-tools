using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UserExistTest
{
    class StreamWriterProvider
    {
        public static List<User> GetListOfUsers(string dep, string path)
        {
            StreamReader sr = new StreamReader(path, Encoding.UTF8);
            string line;

            List<User> listeUsers = new List<User>();

            line = sr.ReadLine();

            while (line != null)
            {
                listeUsers.Add(new User(dep, line));
                line = sr.ReadLine();
            }
            sr.Close();

            return listeUsers;
        }
    }
}
