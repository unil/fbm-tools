using System.Collections.Generic;
using System.Threading;
using System.Windows;
using Microsoft.Win32;
using System;
using System.IO;
using System.Security.AccessControl;

namespace RightsUtility
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            btnArreter.IsEnabled = false;
            btnPause.IsEnabled = false;
        }

        List<string> listeAdGroupsAccepted = new List<string>();
        string cible = "";
        Thread thread;

        public void Log(string text)
        {
            listBoxLog.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    listBoxLog.Items.Add(text);
                }));
        }

        public void LogAdvance(string text)
        {
            listBoxLog.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    listBoxLog.Items[5] = text;
                }));
        }

        public void Count(string text)
        {
            listBoxLog.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    listBoxLog.Items[2] = text;
                }));
        }

        public void Progress(double done, double total)
        {
            progressBarItems.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    double pour = (done / total) * 100;
                    progressBarItems.Value = pour;
                    labelPourc.Content = Math.Round(pour, 2) + "% - " + done + "/" + total;
                }));
        }

        public void IsFinished()
        {
            btnParcourir.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(
                delegate()
                {
                    btnParcourir.IsEnabled = true;
                    btnCible.IsEnabled = true;
                    btnMettre.IsEnabled = true;
                    btnRetirer.IsEnabled = true;
                    btnArreter.IsEnabled = false;
                    btnPause.IsEnabled = false;
                }));
        }

        private void btnParcourir_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.DefaultExt = ".txt";
            openFile.Filter = "Fichier texte|*.txt";
            openFile.Title = "Sélection du fichier des groupes AD non-modifiés";
            openFile.ShowDialog();
            if (openFile.FileName.Length > 0)
            {
                listeAdGroupsAccepted = StreamWriterProvider.GetAdGroupsAccepted(openFile.FileName);
                Nb.Text = "Groupes non-modifiés : " + listeAdGroupsAccepted.Count.ToString();
            }
        }

        private void btnCible_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openBrowser = new System.Windows.Forms.FolderBrowserDialog();
            openBrowser.Description = "Sélection du dossier cible";
            openBrowser.SelectedPath = @"\\fbmnas.unil.ch";
            openBrowser.ShowDialog();
            if (openBrowser.SelectedPath.Length > 0)
            {
                cible = openBrowser.SelectedPath;
                Cible.Text = "Cible : " + cible;
            }
        }

        private void btnMettre_Click(object sender, RoutedEventArgs e)
        {
            if (listeAdGroupsAccepted.Count > 0 && cible != "")
            {
                listBoxLog.Items.Clear();
                progressBarItems.Value = 0;
                ButtonsLock();

                ThreadFolders threadFolders = new ThreadFolders(this, listeAdGroupsAccepted, cible);
                thread = new Thread(new ThreadStart(threadFolders.ThreadMettre));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void btnRetirer_Click(object sender, RoutedEventArgs e)
        {
            if (cible != "")
            {
                listBoxLog.Items.Clear();
                progressBarItems.Value = 0;
                ButtonsLock();

                ThreadFolders threadFolders = new ThreadFolders(this, listeAdGroupsAccepted, cible);
                thread = new Thread(new ThreadStart(threadFolders.ThreadRetirer));
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void ButtonsLock()
        {
            btnParcourir.IsEnabled = false;
            btnCible.IsEnabled = false;
            btnMettre.IsEnabled = false;
            btnRetirer.IsEnabled = false;
            btnArreter.IsEnabled = true;
            btnPause.IsEnabled = true;
        }

        private void AbortThread()
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
                    btnPause.Content = "Pause";
                    IsFinished();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            AbortThread();
        }

        private void btnArreter_Click(object sender, RoutedEventArgs e)
        {
            AbortThread();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            if (thread != null)
            {
                if (thread.ThreadState == ThreadState.Running)
                {
                    thread.Suspend();
                    btnPause.Content = "Reprendre";
                }
                else if (thread.ThreadState == ThreadState.Suspended)
                {
                    thread.Resume();
                    btnPause.Content = "Pause";
                }
            }
        }
    }
}
