using System.Collections.Generic;
using System.Threading;
using System.Windows;
using FBM_SWISS_KNIFE.Common;
using FBM_SWISS_KNIFE.Testeur_dexistence;
using Microsoft.Win32;
using FBM_SWISS_KNIFE.Renommeur_de_dossier;

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

        // Testeur d'existence
        Utils utils;
        Thread _threadRights;
        List<User> listOfTestUsers = new List<User>();
        List<Thread> listOfThreads = new List<Thread>();

        // Renommeur de dossier
        Thread _threadRenamer;
        public Logger logger;
        public List<Traduction> listeTraduction = new List<Traduction>();
        public List<string> listePath = new List<string>();
        public List<string> listeAvancement = new List<string>();

        private void Init()
        {
            utils = new Utils(this);

            // Renommeur de dossier
            GetListOfTraductions();
            ShowListOfTraductions();
        }

        #region Renommeur de dossier

        public void GetListOfTraductions()
        {
            listeTraduction = SerializeProvider.DeserializeTraduction();
        }

        public void SetListOfTraductions()
        {
            SerializeProvider.SerializeTraduction(listeTraduction);
        }

        private void ShowListOfTraductions()
        {
            listBoxTraductionsRDossier.Items.Clear();
            foreach (Traduction trad in listeTraduction)
            {
                listBoxTraductionsRDossier.Items.Add("Traduction de : " + trad.Fr + " en " + trad.En);
            }
        }

        private void GetListOfPath(string path)
        {
            listePath = SerializeProvider.GetListOfPath(path);
        }

        private void ShowListOfPath()
        {
            listBoxDossiersRacinesRDossier.Items.Clear();
            foreach (string path in listePath)
            {
                listBoxDossiersRacinesRDossier.Items.Add(path);
            }
        }

        private void btnParcourirRDossier_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            openFile.Filter = "Fichier txt|*.txt";
            openFile.Title = "Sélection du fichier";
            openFile.ShowDialog();
            if (openFile.FileName.Length > 0)
            {
                GetListOfPath(openFile.FileName);
                ShowListOfPath();
            }
        }

        private void btnSauvegarderRDossier_Click(object sender, RoutedEventArgs e)
        {
            Saver.SaveInformations(this, 3);
        }

        private void btnLancerRDossier_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxDossiersRacinesRDossier.Items.Count > 0)
            {
                logger = new Logger(this);
                listBoxDossiersRacinesRDossier.Items.Clear();
                listeAvancement = new List<string>();

                ThreadRenamer threadRenamer = new ThreadRenamer(this);
                _threadRenamer = new Thread(new ThreadStart(threadRenamer.Work));
                _threadRenamer.SetApartmentState(ApartmentState.STA);
                _threadRenamer.Start();
            }
        }

        private void btnAjouterTraductionRDossier_Click(object sender, RoutedEventArgs e)
        {
            if (txtBoxValeurRDossier.Text != "" && txtBoxValeurTraduiteRDossier.Text != "")
            {
                listeTraduction.Add(new Traduction(txtBoxValeurRDossier.Text, txtBoxValeurTraduiteRDossier.Text));
                SetListOfTraductions();
                ShowListOfTraductions();
            }
        }

        private void listBoxDossiersRacinesRDossier_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string[] strFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                for (long i = 0; i < strFiles.Length; i++)
                {
                    listePath = SerializeProvider.GetListOfPath(strFiles[i]);
                    ShowListOfPath();
                }
            }
            catch { }
        }

        private void listBoxDossiersRacinesRDossier_DragEnter(object sender, DragEventArgs e)
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

        #region Testeur d'existence
        private void btnParcourirTExistence_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            openFile.Multiselect = true;
            openFile.Filter = "Fichier texte|*.txt";
            openFile.Title = "Sélection du fichier de noms";
            openFile.ShowDialog();
            if (openFile.FileNames.Length > 0)
            {
                utils.GetUsersFromLists(openFile.FileNames);
            }
        }

        private void btnTesterExistenceTExistence_Click(object sender, RoutedEventArgs e)
        {
            listBoxErrorTExistence.Items.Clear();
            ThreadRights threadRights = new ThreadRights(utils, utils.GetListOfTestUsers());
            _threadRights = new Thread(new ThreadStart(threadRights.Checker));
            _threadRights.SetApartmentState(ApartmentState.STA);
            _threadRights.Start();
            listOfThreads.Add(_threadRights);
        }

        private void btnEnregistrerTExistence_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxErrorTExistence.Items.Count > 0)
            {
                Saver.SaveInformations(this, 5);
            }
            else
            {
                MessageBox.Show("Il n'y a rien à sauvegarder", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void listBoxLogTExistence_Drop(object sender, DragEventArgs e)
        {
            try
            {
                utils.GetUsersFromLists((string[])e.Data.GetData(DataFormats.FileDrop));
            }
            catch { }
        }

        private void listBoxLogTExistence_DragEnter(object sender, DragEventArgs e)
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

        private void tabControlMain_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void checkBoxFullContentCDossier_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void checkBoxFullContentCDossier_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {

        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {

        }
    }
}
