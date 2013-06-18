using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using Microsoft.Win32;

namespace Folder_Rename_Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GetListOfTraductions();
            ShowListOfTraductions();
        }

        private Thread thread;
        public Logger logger;
        public List<Traduction> listeTraduction = new List<Traduction>();
        public List<string> listePath = new List<string>();
        public List<string> listeAvancement = new List<string>();

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
            listBoxTraductions.Items.Clear();
            foreach (Traduction trad in listeTraduction)
            {
                listBoxTraductions.Items.Add("Traduction de : " + trad.Fr + " en " + trad.En);
            }
        }

        private void GetListOfPath(string path)
        {
            listePath = SerializeProvider.GetListOfPath(path);
        }

        private void ShowListOfPath()
        {
            listBoxDossiers.Items.Clear();
            foreach (string path in listePath)
            {
                listBoxDossiers.Items.Add(path);
            }
        }

        void Save()
        {
            try
            {
                string date = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                SaveFileDialog saveFile = new SaveFileDialog();
                saveFile.AddExtension = true;
                saveFile.Filter = "Text documents (.txt)|*.txt";
                saveFile.DefaultExt = ".txt";
                saveFile.FileName = "Folder_Rename_Save " + date;
                bool? result = saveFile.ShowDialog();
                
                if (result == true)
                {
                    string contents = "";
                    foreach (string ligne in listeAvancement)
                    {
                        contents += ligne + "\r\n";
                    }

                    File.WriteAllText(saveFile.FileName, contents);
                }
            }
            catch { }
        }

        private void btnParcourir_Click(object sender, RoutedEventArgs e)
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

        private void btnLancer_Click(object sender, RoutedEventArgs e)
        {
            if(listBoxDossiers.Items.Count > 0)
            {
                logger = new Logger(this);
                listBoxAvancement.Items.Clear();
                listeAvancement = new List<string>();

                ThreadRenamer threadRenamer = new ThreadRenamer(this);
                thread = new Thread(new ThreadStart(threadRenamer.Work));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void btnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void listBoxDossiers_Drop(object sender, DragEventArgs e)
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

        private void listBoxDossiers_DragEnter(object sender, DragEventArgs e)
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

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if (txtBoxValeur.Text != "" && txtBoxValeurTraduite.Text != "")
            {
                listeTraduction.Add(new Traduction(txtBoxValeur.Text, txtBoxValeurTraduite.Text));
                SetListOfTraductions();
                ShowListOfTraductions();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (thread != null)
            {
                if (thread.ThreadState == ThreadState.Running)
                {
                    thread.Abort();
                }
            }
        }
    }
}
