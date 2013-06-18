using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Security.AccessControl;

namespace UserExistTest
{
    class ThreadRights
    {
        public ThreadRights(MainWindow mw, List<User> listeOfUsers)
        {
            this.mw = mw;
            this.listeOfUsers = listeOfUsers;
        }

        MainWindow mw;
        List<User> listeOfUsers = new List<User>();

        public void ThreadRightsChecker()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                TestIfUserExistsInAD(listeOfUsers);
                Thread.CurrentThread.Abort();
            }
        }

        private void TestIfUserExistsInAD(List<User> listOfUsers)
        {
            Directory.CreateDirectory(@"\\nas.unil.ch\FBM\SYS\TEST");

            int x = 0;

            foreach (User user in listOfUsers)
            {
                mw.Log("Try for User : " + user.Username, x);

                try
                {
                    DirectoryInfo info = new DirectoryInfo(@"\\nas.unil.ch\FBM\SYS\TEST");
                    DirectorySecurity dirSecurity = info.GetAccessControl();
                    dirSecurity.SetAccessRuleProtection(true, false);

                    dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\" + user.Username, FileSystemRights.Write, AccessControlType.Allow));
                    Directory.SetAccessControl(@"\\nas.unil.ch\FBM\SYS\TEST", dirSecurity);
                    mw.Log(user.Username + " exists", x);
                }
                catch
                {
                    mw.Log(user.Username + " doesn't exist", x);
                    mw.LogError(user.Departement + " : " + user.Username);
                }
                x++;
            }
        }
    }
}
