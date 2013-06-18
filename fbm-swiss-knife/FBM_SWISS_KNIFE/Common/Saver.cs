using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace FBM_SWISS_KNIFE.Common
{
    class Saver
    {
        public static void SaveInformations(MainWindow mw, int cas)
        {
            try
            {
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.AddExtension = true;
                saveFile.Filter = "Text documents (.txt)|*.txt";
                saveFile.DefaultExt = ".txt";
                bool? result = saveFile.ShowDialog();

                if (result == true)
                {
                    File.WriteAllText(saveFile.FileName, GetInformationToSave(cas, mw));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static string GetInformationToSave(int cas, MainWindow mw)
        {
            ListBox listBox = new ListBox();

            switch (cas)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    listBox = mw.listBoxAvancementRDossier;
                    break;
                case 4:
                    break;
                case 5:
                    listBox = mw.listBoxErrorTExistence;
                    break;
            }

            string contents = "";
            foreach (string ligne in listBox.Items)
            {
                contents += ligne + "\r\n";
            }

            return contents;
        }
    }
}
