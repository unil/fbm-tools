using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBM_SWISS_KNIFE.Testeur_dexistence
{
    class Utils
    {
        public Utils(MainWindow mw)
        {
            this.mw = mw;
        }

        MainWindow mw;
        List<User> listOfTestUsers = new List<User>();

        public void Log(string text, int index)
        {
            mw.listBoxLogTExistence.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    mw.listBoxLogTExistence.Items[index] = text;
                    mw.listBoxLogTExistence.ScrollIntoView(mw.listBoxLogTExistence.Items[index]);
                    mw.labelCountTotalTExistence.Content = "Effectué : " + (index + 1) + "/" + listOfTestUsers.Count;
                }));
        }

        public void LogError(string text)
        {
            mw.listBoxErrorTExistence.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    mw.listBoxErrorTExistence.Items.Add(text);
                    mw.listBoxErrorTExistence.ScrollIntoView(mw.listBoxErrorTExistence.Items[mw.listBoxErrorTExistence.Items.Count - 1]);
                    mw.labelCountErrorTExistence.Content = "En erreur : " + mw.listBoxErrorTExistence.Items.Count;
                }));
        }

        public List<User> GetListOfTestUsers()
        {
            return listOfTestUsers;
        }

        public void GetUsersFromLists(string[] paths)
        {
            foreach (string path in paths)
            {
                string[] depSplit = path.Split('\\');
                string dep = depSplit[depSplit.Length - 1].Split('.')[0];
                listOfTestUsers.AddRange(StreamProvider.GetListOfUsers(dep, path));
                mw.labelCountTotalTExistence.Content = "Total : " + listOfTestUsers.Count;
            }
            FillListBoxTest();
        }

        private void FillListBoxTest()
        {
            foreach (User user in listOfTestUsers)
            {
                mw.listBoxLogTExistence.Items.Add(user.Username);
            }
        }
    }
}
