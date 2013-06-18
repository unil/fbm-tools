using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBM_SWISS_KNIFE
{
    public class Util_Test
    {
        public Util_Test(MainWindow mw)
        {
            this.mw = mw;
        }

        MainWindow mw;
        List<User_Test> listOfTestUsers = new List<User_Test>();

        public void Log(string text, int index)
        {
            mw.listBoxLog_Test.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    mw.listBoxLog_Test.Items[index] = text;
                    mw.listBoxLog_Test.ScrollIntoView(mw.listBoxLog_Test.Items[index]);
                    mw.labelCountTotal_Test.Content = "Effectué : " + (index + 1) + "/" + listOfTestUsers.Count;
                }));
        }

        public void LogError(string text)
        {
            mw.listBoxError_Test.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    mw.listBoxError_Test.Items.Add(text);
                    mw.listBoxError_Test.ScrollIntoView(mw.listBoxError_Test.Items[mw.listBoxError_Test.Items.Count - 1]);
                    mw.labelCountError_Test.Content = "En erreur : " + mw.listBoxError_Test.Items.Count;
                }));
        }

        public List<User_Test> GetListOfTestUsers()
        {
            return listOfTestUsers;
        }

        public void GetUsersFromLists(string[] paths)
        {
            foreach (string path in paths)
            {
                string[] depSplit = path.Split('\\');
                string dep = depSplit[depSplit.Length - 1].Split('.')[0];
                listOfTestUsers.AddRange(StreamReaderProvider_Test.GetListOfUsers(dep, path));
                mw.labelCountTotal_Test.Content = "Total : " + listOfTestUsers.Count;
            }
            FillListBoxTest();
        }

        private void FillListBoxTest()
        {
            foreach (User_Test user in listOfTestUsers)
            {
                mw.listBoxLog_Test.Items.Add(user.Username);
            }
        }
    }
}
