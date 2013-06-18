using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Folder_Rename_Wpf
{
    public class Logger
    {
        public Logger(MainWindow mw)
        {
            this.mw = mw;
        }

        MainWindow mw;

        public void Log(string message)
        {
            try
            {
                mw.listBoxAvancement.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {

                    string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    mw.listeAvancement.Add("[" + date + "] " + message);
                    mw.listBoxAvancement.Items.Add(message);
                    mw.listBoxAvancement.ScrollIntoView(mw.listBoxAvancement.Items[mw.listBoxAvancement.Items.Count - 1]);
                }));
            }
            catch { }
        }
    }
}
