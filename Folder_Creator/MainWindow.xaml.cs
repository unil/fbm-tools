﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace Folder_Creator
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

        private List<Folder> listeFolders = new List<Folder>();
        private List<string> listeFoldersErrors = new List<string>();
        private Thread thread;
        private DirectoryInfo destinationFolder;

        private void Init()
        {
            foreach (string departement in StreamProvider.GetListOfDepartements())
            {
                comboBoxDepartement.Items.Add(departement);
            }

            textBlockNb.Text = "0 dossiers";
            textBlockPath.Text = "Destination : Non définie";
            textBlockNbRecup.Text = "0 dossiers";
            textBlockPathRecup.Text = "Cible : Non définie";            
        }

        public void MiseAJour(int index, string text, int cas)
        {
            listViewFolders.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    switch (cas)
                    {
                        case 1:
                            listeFolders[index].State = text;
                            break;
                        case 2:
                            listeFolders[index].StatePerso = "Personnel : " + text;
                            break;
                        case 3:
                            listeFolders[index].StatePublic = "Public : " + text;
                            break;
                        case 4:
                            listeFolders[index].StatePrivate = "Private : " + text;
                            break;
                    }
                    listViewFolders.Items.Refresh();
                }));
        }

        public void Log(string text)
        {
            listBoxLogs.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    listBoxLogs.Items.Add("[" + date + "] " + text);
                }));
        }

        public void SetFoldersOnError(List<string> folders)
        {
            listBoxLogs.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    listeFoldersErrors = folders;
                    EnableDeleteFolderButton();
                }));
        }

        private void FillListView(string path)
        {
            listeFolders = StreamProvider.GetListOfFolders(path);
            listViewFolders.ItemsSource = listeFolders;
            textBlockNb.Text = listViewFolders.Items.Count + " dossiers";
        }

        private void SuppressFoldersOnError()
        {
            foreach (string persoFolder in listeFoldersErrors)
            {
                Directory.Delete(persoFolder, true);
            }

            btnSupprimerErreurs.IsEnabled = false;
            Log("Les dossiers en erreurs ont été supprimés.");
            listeFoldersErrors = new List<string>();
        }

        private void EnableDeleteFolderButton()
        {
            if (listeFoldersErrors.Count > 1)
            {
                btnSupprimerErreurs.IsEnabled = true;
            }
            else
            {
                btnSupprimerErreurs.IsEnabled = false;
            }
        }

        private void btnParcourir_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            openFile.Filter = "Fichier texte|*.txt";
            openFile.Title = "Sélection du fichier de noms";
            openFile.ShowDialog();
            if (openFile.FileName.Length > 0)
            {
                FillListView(openFile.FileName);
            }
        }

        private void btnDestination_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openBrowser = new System.Windows.Forms.FolderBrowserDialog();
            openBrowser.Description = "Sélection du dossier de destination";
            openBrowser.ShowDialog();
            if (openBrowser.SelectedPath.Length > 0)
            {
                destinationFolder = new DirectoryInfo(openBrowser.SelectedPath);
                textBlockPath.Text = "Destination : " + openBrowser.SelectedPath;
            }
        }
        
        private void btnCreer_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxDepartement.SelectedItem != null && listeFolders.Count > 0 && destinationFolder != null)
            {
                listBoxLogs.Items.Clear();

                ThreadFolders threadFolders = new ThreadFolders(this, listeFolders, destinationFolder, comboBoxDepartement.SelectedItem.ToString());
                thread = new Thread(new ThreadStart(threadFolders.ThreadJob));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void checkBoxFullContent_Checked(object sender, RoutedEventArgs e)
        {
            foreach (Folder folder in listeFolders)
            {
                folder.CreateContent = true;
            }
            listViewFolders.Items.Refresh();
        }

        private void checkBoxFullContent_Unchecked(object sender, RoutedEventArgs e)
        {
            foreach (Folder folder in listeFolders)
            {
                folder.CreateContent = false;
            }
            listViewFolders.Items.Refresh();
        }

        private void btnEnregistrer_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxLogs.Items.Count > 0)
            {
                try
                {
                    string date = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.AddExtension = true;
                    saveFile.Filter = "Text documents (.txt)|*.txt";
                    saveFile.DefaultExt = ".txt";
                    saveFile.FileName = "Folder_Creator_Save " + date;
                    bool? result = saveFile.ShowDialog();

                    if (result == true)
                    {
                        string contents = "";
                        foreach (string ligne in listBoxLogs.Items)
                        {
                            contents += ligne + "\r\n";
                        }

                        File.WriteAllText(saveFile.FileName, contents);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message, "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Il n'y a rien à sauvegarder", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            Expander ex = (Expander)e.Source;
            ex.Header = "Masquer";
        }

        private void Expander_Collapsed(object sender, RoutedEventArgs e)
        {
            Expander ex = (Expander)e.Source;
            ex.Header = "Afficher";
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

        private void listViewFolders_Drop(object sender, DragEventArgs e)
        {
            try
            {
                string[] strFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

                for (long i = 0; i <= strFiles.GetUpperBound(0); i++)
                {
                    FillListView(strFiles[i]);
                }
            }
            catch { }
        }

        private void listViewFolders_DragEnter(object sender, DragEventArgs e)
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

        private void btnParcourirRecup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog openBrowser = new System.Windows.Forms.FolderBrowserDialog();
                openBrowser.Description = "Sélection du dossier";
                openBrowser.ShowDialog();
                if (openBrowser.SelectedPath.Length > 0)
                {
                    listBoxRecup.Items.Clear();
                    DirectoryInfo dir = new DirectoryInfo(openBrowser.SelectedPath);
                    foreach (DirectoryInfo info in dir.GetDirectories())
                    {
                        listBoxRecup.Items.Add(info.Name);
                    }

                    listBoxRecup.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription());
                    textBlockNbRecup.Text = listBoxRecup.Items.Count + " dossiers";
                    textBlockPathRecup.Text = "Cible : " + openBrowser.SelectedPath;
                }
            }
            catch { }
        }

        private void btnEnregistrerRecup_Click(object sender, RoutedEventArgs e)
        {
            if (listBoxRecup.Items.Count > 0)
            {
                try
                {
                    string[] pathRecupSplit = textBlockPathRecup.Text.Split('\\');
                    SaveFileDialog saveFile = new SaveFileDialog();
                    saveFile.AddExtension = true;
                    saveFile.Filter = "Text documents (.txt)|*.txt";
                    saveFile.DefaultExt = ".txt";
                    saveFile.FileName = pathRecupSplit[pathRecupSplit.Length - 2];
                    bool? result = saveFile.ShowDialog();

                    if (result == true)
                    {
                        string contents = "";
                        foreach (string ligne in listBoxRecup.Items)
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

        private void btnSupprimerErreurs_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Des erreurs se sont produites durant la création, des dossiers ont été créés sans avoir été finalisés (droits). Souhaitez-vous supprimer les dossiers correspondants ?", "Erreur durant la création", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                SuppressFoldersOnError();
            }
        }

        private void tabControlMain_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tabControlMain.SelectedItem == tabItemLog)
            {
                EnableDeleteFolderButton();
            }
        }
    }
}
