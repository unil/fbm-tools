using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using Microsoft.Win32;

namespace UserExistTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        List<User> listeOfTestUsers = new List<User>();
        Thread thread;

        public void Log(string text, int index)
        {
            listBoxLog.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    listBoxLog.Items[index] = text;
                    listBoxLog.ScrollIntoView(listBoxLog.Items[index]);
                    labelCountTotal.Content = "Effectué : " + (index + 1) + "/" + listeOfTestUsers.Count;
                }));
        }

        public void LogError(string text)
        {
            listBoxError.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    listBoxError.Items.Add(text);
                    listBoxError.ScrollIntoView(listBoxError.Items[listBoxError.Items.Count - 1]);
                    labelCountError.Content = "En erreur : " + listBoxError.Items.Count;
                }));
        }

        private void GetUsersFromLists(string path)
        {
            string[] depSplit = path.Split('\\');
            string dep = depSplit[depSplit.Length - 1].Split('.')[0];
            listeOfTestUsers.AddRange(StreamWriterProvider.GetListOfUsers(dep, path));
            labelCountTotal.Content = "Total : " + listeOfTestUsers.Count;
        }

        private void FillListBoxTest()
        {
            foreach (User user in listeOfTestUsers)
            {
                listBoxLog.Items.Add(user.Username);
            }
        }

        private void btnParcourirTest_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            openFile.Multiselect = true;
            openFile.Filter = "Fichier texte|*.txt";
            openFile.Title = "Sélection du fichier de noms";
            openFile.ShowDialog();
            if (openFile.FileNames.Length > 0)
            {
                for (long i = 0; i < openFile.FileNames.Length; i++)
                {
                    GetUsersFromLists(openFile.FileNames[i]);
                }
                FillListBoxTest();
            }
        }

        private void btnTester_Click(object sender, RoutedEventArgs e)
        {
            listBoxError.Items.Clear();
            ThreadRights threadRights = new ThreadRights(this, listeOfTestUsers);
            thread = new Thread(new ThreadStart(threadRights.ThreadRightsChecker));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxError.Items.Count > 0)
            {
                try
                {
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.AddExtension = true;
                    saveFile.Filter = "Text documents (.txt)|*.txt";
                    saveFile.DefaultExt = ".txt";
                    saveFile.FileName = "RightsUtility_save";
                    bool? result = saveFile.ShowDialog();

                    if (result == true)
                    {
                        string contents = "";
                        foreach (string ligne in listBoxError.Items)
                        {
                            contents += ligne + "\r\n";
                        }

                        File.WriteAllText(saveFile.FileName, contents);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Il n'y a rien à sauvegarder", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void listBoxLogTest_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string[] strFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                for (long i = 0; i <= strFiles.GetUpperBound(0); i++)
                {
                    GetUsersFromLists(strFiles[i]);
                }
                FillListBoxTest();
            }
            catch { }
        }

        private void listBoxLogTest_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Copy;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (thread != null)
            {
                if (thread.ThreadState == ThreadState.Running || thread.ThreadState == ThreadState.Suspended)
                {
                    if (thread.ThreadState == ThreadState.Suspended)
                    {
                        thread.Resume();
                    }
                    thread.Abort();
                }
            }
        }
    }
}
