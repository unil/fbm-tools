using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Microsoft.VisualBasic;
using Microsoft.Win32;
using System.Threading;

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

            GetListeTraduction();
        }

        private Thread thread;
        public Logger logger;
        public List<Traduction> listeTraduction = new List<Traduction>();
        public List<string> listePath = new List<string>();
        public List<string> listeAvancement = new List<string>();

        private void Init()
        {
            logger = new Logger(this);
            listBoxAvancement.Items.Clear();
            listeAvancement = new List<string>();

            ThreadRenamer threadRenamer = new ThreadRenamer(this);
            thread = new Thread(new ThreadStart(threadRenamer.Work));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        public void GetListeTraduction()
        {
            listeTraduction = SerializeProvider.DeserializeTraduction();
        }

        public void SetListeTraduction()
        {
             SerializeProvider.SerializeTraduction(listeTraduction);
        }

        private void GetListePath(string path)
        {
            listePath = SerializeProvider.GetListOfPath(path);
        }

        private void FillListOfPath()
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
                GetListePath(openFile.FileName);
                FillListOfPath();
            }
        }

        private void btnLancer_Click(object sender, RoutedEventArgs e)
        {
            if(listBoxDossiers.Items.Count > 0)
            {
                Init();
            }
        }

        private void btnSauvegarder_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void btnAjouterTraduction_Click(object sender, RoutedEventArgs e)
        {
            AjouterTraductionWindow atw = new AjouterTraductionWindow(this);
            atw.ShowDialog();
        }

        private void listBoxDossiers_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string[] strFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                for (long i = 0; i <= strFiles.GetUpperBound(0); i++)
                {
                    listePath = SerializeProvider.GetListOfPath(strFiles[i]);
                    FillListOfPath();
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
