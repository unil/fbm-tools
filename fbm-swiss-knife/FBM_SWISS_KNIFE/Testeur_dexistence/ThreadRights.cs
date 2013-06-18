using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using System.Threading;

namespace FBM_SWISS_KNIFE.Testeur_dexistence
{
    class ThreadRights
    {
        public ThreadRights(Utils util, List<User> listOfUsers)
        {
            this.util = util;
            this.listOfUsers = listOfUsers;
        }

        Utils util;
        List<User> listOfUsers = new List<User>();

        public void Checker()
        {
            while (Thread.CurrentThread.IsAlive)
            {
                Directory.CreateDirectory(@"\\nas.unil.ch\FBM\SYS\TEST");

                int x = 0;

                foreach (User user in listOfUsers)
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
                Thread.CurrentThread.Abort();
            }
        }
    }
}
