using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Threading;

namespace FBM_SWISS_KNIFE
{
    class ThreadRights_Test
    {
        public ThreadRights_Test(Util_Test util, List<User_Test> listeOfUsers)
        {
            this.util = util;
            this.listeOfUsers = listeOfUsers;
        }

        Util_Test util;
        List<User_Test> listeOfUsers = new List<User_Test>();

        public void ThreadRightsChecker()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                TestIfUserExistsInAD(listeOfUsers);
                Thread.CurrentThread.Abort();
            }
        }

        private void TestIfUserExistsInAD(List<User_Test> listOfUsers)
        {
            Directory.CreateDirectory(@"\\nas.unil.ch\FBM\SYS\TEST");

            int x = 0;

            foreach (User_Test user in listOfUsers)
            {
                util.Log("Try for User : " + user.Username, x);

                try
                {
                    DirectoryInfo info = new DirectoryInfo(@"\\nas.unil.ch\FBM\SYS\TEST");
                    DirectorySecurity dirSecurity = info.GetAccessControl();
                    dirSecurity.SetAccessRuleProtection(true, false);

                    dirSecurity.AddAccessRule(new FileSystemAccessRule(@"ad\" + user.Username, FileSystemRights.Write, AccessControlType.Allow));
                    Directory.SetAccessControl(@"\\nas.unil.ch\FBM\SYS\TEST", dirSecurity);
                    util.Log(user.Username + " exists", x);
                }
                catch
                {
                    util.Log(user.Username + " doesn't exist", x);
                    util.LogError(user.Departement + " : " + user.Username);
                }
                x++;
            }
        }
    }
}
