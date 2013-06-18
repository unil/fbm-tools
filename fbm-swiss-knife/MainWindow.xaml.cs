using System.Collections.Generic;
using System.Windows;
using Microsoft.Win32;
using System.Threading;

namespace FBM_SWISS_KNIFE
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Init();
        }

        // Classes principales
        Util_Test util_Test;

        //Classes fonctionnelles
        Thread threadRights;

        // LISTES
        List<User_Test> listOfTestUsers = new List<User_Test>();
        List<Thread> listOfThreads = new List<Thread>();

        private void Init()
        {
            util_Test = new Util_Test(this);
        }

        #region Testeur d'existence
        private void btnParcourir_Test_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            openFile.Multiselect = true;
            openFile.Filter = "Fichier texte|*.txt";
            openFile.Title = "Sélection du fichier de noms";
            openFile.ShowDialog();
            if (openFile.FileNames.Length > 0)
            {
                util_Test.GetUsersFromLists(openFile.FileNames);
            }
        }

        private void btnTesterExistence_Test_Click(object sender, RoutedEventArgs e)
        {
            listBoxError_Test.Items.Clear();
            ThreadRights_Test threadRights_Test = new ThreadRights_Test(util_Test, util_Test.GetListOfTestUsers());
            threadRights = new Thread(new ThreadStart(threadRights_Test.ThreadRightsChecker));
            threadRights.SetApartmentState(ApartmentState.STA);
            threadRights.Start();
            listOfThreads.Add(threadRights);
        }

        private void btnEnregistrer_Test_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxError_Test.Items.Count > 0)
            {
                Save.SaveInformations(this, 5);
            }
            else
            {
                MessageBox.Show("Il n'y a rien à sauvegarder", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void listBoxLog_Test_Drop(object sender, DragEventArgs e)
        {
            try
            {
                util_Test.GetUsersFromLists((string[])e.Data.GetData(DataFormats.FileDrop));
            }
            catch { }
        }

        private void listBoxLog_Test_DragEnter(object sender, DragEventArgs e)
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
        #endregion

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Thread thread in listOfThreads)
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
}
