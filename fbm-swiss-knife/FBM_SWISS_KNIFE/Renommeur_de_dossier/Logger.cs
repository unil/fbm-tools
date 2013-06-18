using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FBM_SWISS_KNIFE.Renommeur_de_dossier
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
                mw.listBoxAvancementRDossier.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    mw.listeAvancement.Add("[" + date + "] " + message);
                    mw.listBoxAvancementRDossier.Items.Add(message);
                    mw.listBoxAvancementRDossier.ScrollIntoView(mw.listBoxAvancementRDossier.Items[mw.listBoxAvancementRDossier.Items.Count - 1]);
                }));
            }
            catch { }
        }
    }
}
