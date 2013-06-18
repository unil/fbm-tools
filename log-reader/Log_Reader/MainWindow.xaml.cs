using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System;


/// Log_Reader créé par Quentin Buache (qb) 15.03.2013
/// Programme servant à récupérer le contenu de fichiers de log (.txt) et à les afficher
/// Ne fonctionne qu'avec des logs formatés comme suit:
/// - Fichiers de logs d'utilisation normales    : [MACHINE_NAME] [DATE] [SCRIPT_NAME] [USERNAME] TEXT
/// - Fichiers de logs de déploiement PowerShell : [MACHINE_NAME] [DATE] [BOOL]
/// - Fichiers de logs de déploiement de scripts : [MACHINE_NAME] [DATE] TEXT
/// Logiciel livré tel quel, car fait hors des heures de travail
/// 
/// FBM SI
/// /////////////////////////////////////////////////////////////////////////////////////////////////



namespace Log_Reader
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            comboBoxDepartement.Items.Add(" -- Département --");
            comboBoxDepartement.SelectedIndex = 0;
            comboBoxScript.Items.Add("      -- Script --");
            comboBoxScript.SelectedIndex = 0;
            comboBoxUtilisateur.Items.Add("   -- Utilisateur --");
            comboBoxUtilisateur.SelectedIndex = 0;
            comboBoxDate.Items.Add("         -- Date --");
            comboBoxDate.SelectedIndex = 0;
            ////////////////////////////////////////////////////
            comboBoxDepartementDeployPS.Items.Add(" -- Département --");
            comboBoxDepartementDeployPS.SelectedIndex = 0;
            ////////////////////////////////////////////////////
            comboBoxDepartementDeployScript.Items.Add(" -- Département --");
            comboBoxDepartementDeployScript.SelectedIndex = 0;
            comboBoxDateDeployScript.Items.Add("         -- Date --");
            comboBoxDateDeployScript.SelectedIndex = 0;

            radioButtonTous.IsChecked = true;
        }

        private List<Log> listeLogs = new List<Log>();
        private List<DeployPs> listeDeployPs = new List<DeployPs>();
        private List<DeployScript> listeDeployScripts = new List<DeployScript>();
        private SearchParam searchParamLogs = new SearchParam("*", "*", "*", "*", "*", "*");
        private SearchParam searchParamDeployPs = new SearchParam("*", "*", true);
        private SearchParam searchParamDeployScripts = new SearchParam("*", "*", "*", "*");
        private string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        #region Log
        private void Show(List<Log> listeLogs)
        {
            listBoxAffichageLog.Items.Clear();
            foreach (Log file in listeLogs)
            {
                listBoxAffichageLog.Items.Add("[" + file.MachineName + "] [" + file.Date + "] [" + file.ScriptName + "] [" + file.UserName + "]" + file.Text);
            }
            labelCount.Content = listeLogs.Count + " affichés";
        }

        public List<Log> GetLogRowsFromFiles(FileInfo fichier)
        {
            List<Log> listeLog = new List<Log>();
            try
            {
                StreamReader sr = new StreamReader(fichier.FullName, Encoding.UTF8);
                string line;

                line = sr.ReadLine();

                while (line != null)
                {
                    try
                    {
                        string[] splitLine = line.Split(']');
                        splitLine[0] = splitLine[0].Replace('[', ' ').Trim();
                        splitLine[1] = splitLine[1].Replace('[', ' ').Trim();
                        splitLine[2] = splitLine[2].Replace('[', ' ').Trim();
                        splitLine[3] = splitLine[3].Replace('[', ' ').Trim();

                        listeLog.Add(new Log(splitLine[0], splitLine[1], splitLine[2], splitLine[3], splitLine[4]));
                    }
                    catch
                    { }
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch
            { }
            return listeLog;
        }

        private void btnAfficher_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openBrowser = new System.Windows.Forms.FolderBrowserDialog();
            openBrowser.Description = "Sélectionnez le dossier de log";
            openBrowser.SelectedPath = baseDirectory;
            openBrowser.ShowDialog();
            if (openBrowser.SelectedPath.Length > 0)
            {
                if (Directory.Exists(openBrowser.SelectedPath))
                {
                    listeLogs = new List<Log>();
                    FileInfo[] files = new DirectoryInfo(openBrowser.SelectedPath).GetFiles("*.txt");

                    foreach (FileInfo file in files)
                    {
                        listeLogs.AddRange(GetLogRowsFromFiles(file));
                    }

                    Show(ShownLog());
                    FillMainLogComboBoxes();
                }
            }
        }

        private string GetNameFromFullName(string name)
        {
            string retour = "";
            int x = 0;

            while (x < name.Count() - 1)
            {
                int temp = 0;
                if (int.TryParse(name[x].ToString(), out temp))
                {
                    break;
                }
                else
                {
                    retour += name[x];
                }
                x++;
            }

            return retour;
        }

        private void FillMainLogComboBoxes()
        {
            foreach (Log log in listeLogs)
            {
                if (!Exists(log.MachineName, 1))
                {
                    comboBoxDepartement.Items.Add(GetNameFromFullName(log.MachineName));
                }

                if (!Exists(log.ScriptName, 2))
                {
                    comboBoxScript.Items.Add(log.ScriptName);
                }

                if (!Exists(log.UserName, 3))
                {
                    comboBoxUtilisateur.Items.Add(log.UserName);
                }

                if (!Exists(log.Date, 4))
                {
                    comboBoxDate.Items.Add(log.Date.Split(' ')[0]);
                }
            }
        }

        private bool Exists(string valeur, int cas)
        {
            bool exists = false;

            switch (cas)
            {
                case 1:
                    foreach (object obj in comboBoxDepartement.Items)
                    {
                        if (GetNameFromFullName(valeur) == obj.ToString())
                        {
                            exists = true;
                            break;
                        }
                    }
                    break;
                case 2:
                    foreach (object obj in comboBoxScript.Items)
                    {
                        if (valeur == obj.ToString())
                        {
                            exists = true;
                            break;
                        }
                    }
                    break;
                case 3:
                    foreach (object obj in comboBoxUtilisateur.Items)
                    {
                        if (valeur == obj.ToString())
                        {
                            exists = true;
                            break;
                        }
                    }
                    break;
                case 4:
                    foreach (object obj in comboBoxDate.Items)
                    {
                        if (valeur.Split(' ')[0] == obj.ToString())
                        {
                            exists = true;
                            break;
                        }
                    }
                    break;
            }
            return exists;
        }

        private List<Log> ShownLog()
        {
            var query = listeLogs;
            if(searchParamLogs.Dep != "*")
            {
                query = (from q in query
                         where GetNameFromFullName(q.MachineName) == searchParamLogs.Dep
                         select q).ToList();
            }

            if (searchParamLogs.Ord != "*")
            {
                query = (from q in query
                         where q.MachineName == searchParamLogs.Ord
                         select q).ToList();
            }

            if (searchParamLogs.Dat != "*")
            {
                query = (from q in query
                         where q.Date.Split(' ')[0] == searchParamLogs.Dat
                         select q).ToList();
            }

            if (searchParamLogs.Scr != "*")
            {
                query = (from q in query
                         where q.ScriptName == searchParamLogs.Scr
                         select q).ToList();
            }

            if (searchParamLogs.Use != "*")
            {
                query = (from q in query
                         where q.UserName == searchParamLogs.Use
                         select q).ToList();
            }

            if (searchParamLogs.Heu != "*")
            {
                query = (from q in query
                         where q.Date.Split(' ')[1] == searchParamLogs.Heu
                         select q).ToList();
            }

            return (List<Log>)query;
        }

        private void FillComputerCombobox()
        {
            comboBoxOrdinateur.Items.Clear();
            comboBoxOrdinateur.Items.Add("   -- Ordinateur --");
            comboBoxOrdinateur.SelectedIndex = 0;

            foreach (Log log in listeLogs)
            {
                bool exists = false;
                if (GetNameFromFullName(log.MachineName) == searchParamLogs.Dep)
                {
                    foreach (object obj in comboBoxOrdinateur.Items)
                    {
                        if (log.MachineName == obj.ToString())
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        comboBoxOrdinateur.Items.Add(log.MachineName);
                    }
                }
            }
        }

        private void FillHeureCombobox()
        {
            comboBoxHeure.Items.Clear();
            comboBoxHeure.Items.Add("      -- Heure --");
            comboBoxHeure.SelectedIndex = 0;

            foreach (Log log in listeLogs)
            {
                bool exists = false;
                if (log.Date.Split(' ')[0] == searchParamLogs.Dat)
                {
                    foreach (object obj in comboBoxHeure.Items)
                    {
                        if (log.Date.Split(' ')[1] == obj.ToString())
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        comboBoxHeure.Items.Add(log.Date.Split(' ')[1]);
                    }
                }
            }
        }

        private void comboBoxDepartement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxDepartement.SelectedIndex < 1)
            {
                comboBoxDepartement.SelectedIndex = 0;
                searchParamLogs.Dep = "*";
                comboBoxOrdinateur.IsEnabled = false;
            }
            else
            {
                searchParamLogs.Dep = comboBoxDepartement.SelectedItem.ToString();
                comboBoxOrdinateur.IsEnabled = true;
            }
            Show(ShownLog());
            FillComputerCombobox();
        }

        private void comboBoxScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxScript.SelectedIndex < 1)
            {
                comboBoxScript.SelectedIndex = 0;
                searchParamLogs.Scr = "*";
            }
            else
            {
                searchParamLogs.Scr = comboBoxScript.SelectedItem.ToString();
            }
            Show(ShownLog());
        }

        private void comboBoxUtilisateur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxUtilisateur.SelectedIndex < 1)
            {
                comboBoxUtilisateur.SelectedIndex = 0;
                searchParamLogs.Use = "*";
            }
            else
            {
                searchParamLogs.Use = comboBoxUtilisateur.SelectedItem.ToString();
            }
            Show(ShownLog());
        }

        private void comboBoxDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxDate.SelectedIndex < 1)
            {
                comboBoxDate.SelectedIndex = 0;
                searchParamLogs.Dat = "*";
                comboBoxHeure.IsEnabled = false;
            }
            else
            {
                searchParamLogs.Dat = comboBoxDate.SelectedItem.ToString();
                comboBoxHeure.IsEnabled = true;
            }
            FillHeureCombobox();
            Show(ShownLog());
        }

        private void comboBoxOrdinateur_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxOrdinateur.SelectedIndex < 1)
            {
                comboBoxOrdinateur.SelectedIndex = 0;
                searchParamLogs.Ord = "*";
            }
            else
            {
                searchParamLogs.Ord = comboBoxOrdinateur.SelectedItem.ToString();
            }
            Show(ShownLog());
        }

        private void comboBoxHeure_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxHeure.SelectedIndex < 1)
            {
                comboBoxHeure.SelectedIndex = 0;
                searchParamLogs.Heu = "*";
            }
            else
            {
                searchParamLogs.Heu = comboBoxHeure.SelectedItem.ToString();
            }
            Show(ShownLog());
        }

        private void btnRAZ_Click(object sender, RoutedEventArgs e)
        {
            comboBoxDepartement.SelectedIndex = 0;
            comboBoxScript.SelectedIndex = 0;
            comboBoxUtilisateur.SelectedIndex = 0;
            comboBoxDate.SelectedIndex = 0;
        }

        private void OpenAffichageLog()
        {
            if (listBoxAffichageLog.SelectedItem != null)
            {
                string[] splitLine = listBoxAffichageLog.SelectedItem.ToString().Split(']');
                splitLine[0] = splitLine[0].Replace('[', ' ').Trim();
                splitLine[1] = splitLine[1].Replace('[', ' ').Trim();
                splitLine[2] = splitLine[2].Replace('[', ' ').Trim();
                splitLine[3] = splitLine[3].Replace('[', ' ').Trim();

                Affichage affichage = new Affichage();
                affichage.MachineName = splitLine[0];
                affichage.Date = splitLine[1];
                affichage.Script = splitLine[2];
                affichage.User = splitLine[3];
                affichage.Message = splitLine[4].Trim();

                AffichageWindow aw = new AffichageWindow(affichage);
                aw.ShowDialog();
            }
        }

        private void listBoxAffichageLog_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenAffichageLog();
        }

        private void listBoxAffichageLog_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                OpenAffichageLog();
            }
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Deploy PowerShell
        private void btnAfficherDeployPs_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openBrowser = new System.Windows.Forms.FolderBrowserDialog();
            openBrowser.Description = "Sélectionnez le dossier de log de déploiement PowerShell";
            openBrowser.SelectedPath = baseDirectory + @"DEPLOY\POWERSHELL";
            openBrowser.ShowDialog();
            if (openBrowser.SelectedPath.Length > 0)
            {
                if (Directory.Exists(openBrowser.SelectedPath))
                {
                    listeDeployPs = new List<DeployPs>();
                    FileInfo[] files = new DirectoryInfo(openBrowser.SelectedPath).GetFiles("*.txt");

                    foreach (FileInfo file in files)
                    {
                        listeDeployPs.AddRange(GetDeployPsRowsFromFiles(file));
                    }

                    Show(ShownDeployPs());
                    FillMainDeployPsComboBoxes();
                }
            }
        }

        private void FillMainDeployPsComboBoxes()
        {
            foreach (DeployPs deployPs in listeDeployPs)
            {
                if (!Exists(deployPs.MachineName))
                {
                    comboBoxDepartementDeployPS.Items.Add(GetNameFromFullName(deployPs.MachineName));
                }
            }
        }

        public List<DeployPs> GetDeployPsRowsFromFiles(FileInfo fichier)
        {
            List<DeployPs> listeLog = new List<DeployPs>();
            try
            {
                StreamReader sr = new StreamReader(fichier.FullName, Encoding.UTF8);
                string line;

                line = sr.ReadLine();

                DeployPs deploy = new DeployPs();
                while (line != null)
                {
                    bool etat = false;
                    string[] splitLine = line.Split(']');
                    splitLine[0] = splitLine[0].Replace('[', ' ').Trim();
                    splitLine[1] = splitLine[1].Replace('[', ' ').Trim();
                    splitLine[2] = splitLine[2].Replace('[', ' ').Trim();

                    if (splitLine[2].ToString().ToUpper() == "TRUE" || splitLine[2].ToString().ToUpper() == "VRAI")
                    {
                        etat = true;
                    }
                    else
                    {
                        etat = false;
                    }

                    deploy.MachineName = splitLine[0];
                    deploy.Date = splitLine[1];
                    deploy.Etat = etat;
                    
                    line = sr.ReadLine();
                }
                listeLog.Add(deploy);
                sr.Close();
            }
            catch
            { }
            return listeLog;
        }

        private void Show(List<DeployPs> listeDeployPs)
        {
            listBoxAffichageDeployPs.Items.Clear();
            foreach (DeployPs file in listeDeployPs)
            {
                listBoxAffichageDeployPs.Items.Add("[" + file.MachineName + "] [" + file.Date + "] [" + file.Etat + "]");
            }
            labelCountDeployPs.Content = listeDeployPs.Count + " affichés";
        }

        private bool Exists(string valeur)
        {
            bool exists = false;

            foreach (object obj in comboBoxDepartementDeployPS.Items)
            {
                if (GetNameFromFullName(valeur) == obj.ToString())
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }

        private List<DeployPs> ShownDeployPs()
        {
            var query = listeDeployPs;
            if (searchParamDeployPs.Dep != "*")
            {
                query = (from q in query
                         where GetNameFromFullName(q.MachineName) == searchParamDeployPs.Dep
                         select q).ToList();
            }

            if (searchParamDeployPs.Ord != "*")
            {
                query = (from q in query
                         where q.MachineName == searchParamDeployPs.Ord
                         select q).ToList();
            }

            if (searchParamDeployPs.Res != null)
            {
                query = (from q in query
                         where q.Etat == searchParamDeployPs.Res
                         select q).ToList();
            }

            return (List<DeployPs>)query;
        }

        private void FillComputerDeployPsCombobox()
        {
            comboBoxOrdinateurDeployPS.Items.Clear();
            comboBoxOrdinateurDeployPS.Items.Add("   -- Ordinateur --");
            comboBoxOrdinateurDeployPS.SelectedIndex = 0;

            foreach (DeployPs deployPs in listeDeployPs)
            {
                bool exists = false;
                if (GetNameFromFullName(deployPs.MachineName) == searchParamDeployPs.Dep)
                {
                    foreach (object obj in comboBoxOrdinateurDeployPS.Items)
                    {
                        if (deployPs.MachineName == obj.ToString())
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        comboBoxOrdinateurDeployPS.Items.Add(deployPs.MachineName);
                    }
                }
            }
        }

        private void comboBoxDepartementDeployPS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxDepartementDeployPS.SelectedIndex < 1)
            {
                comboBoxDepartementDeployPS.SelectedIndex = 0;
                searchParamDeployPs.Dep = "*";
                comboBoxOrdinateurDeployPS.IsEnabled = false;
            }
            else
            {
                searchParamDeployPs.Dep = comboBoxDepartementDeployPS.SelectedItem.ToString();
                comboBoxOrdinateurDeployPS.IsEnabled = true;
            }
            FillComputerDeployPsCombobox();
            Show(ShownDeployPs());
        }

        private void comboBoxOrdinateurDeployPS_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxOrdinateurDeployPS.SelectedIndex < 1)
            {
                comboBoxOrdinateurDeployPS.SelectedIndex = 0;
                searchParamDeployPs.Ord = "*";
            }
            else
            {
                searchParamDeployPs.Ord = comboBoxOrdinateurDeployPS.SelectedItem.ToString();
            }
            Show(ShownDeployPs());
        }

        private void btnRAZDeployPs_Click(object sender, RoutedEventArgs e)
        {
            comboBoxDepartementDeployPS.SelectedIndex = 0;
            comboBoxOrdinateurDeployPS.SelectedItem = 0;
            radioButtonTous.IsChecked = true;
        }

        private void OpenAffichageDeployPs()
        {
            if (listBoxAffichageDeployPs.SelectedItem != null)
            {
                string[] splitLine = listBoxAffichageDeployPs.SelectedItem.ToString().Split(']');
                splitLine[0] = splitLine[0].Replace('[', ' ').Trim();
                splitLine[1] = splitLine[1].Replace('[', ' ').Trim();
                splitLine[2] = splitLine[2].Replace('[', ' ').Trim();

                Affichage affichage = new Affichage();
                affichage.MachineName = splitLine[0];
                affichage.Date = splitLine[1];
                affichage.Message = splitLine[2];
                affichage.User = "All";
                affichage.Script = "-";

                AffichageWindow aw = new AffichageWindow(affichage);
                aw.ShowDialog();
            }
        }

        private void listBoxAffichageDeployPs_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenAffichageDeployPs();
        }

        private void listBoxAffichageDeployPs_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                OpenAffichageDeployPs();
            }
        }

        private void radioButtonTrue_Checked(object sender, RoutedEventArgs e)
        {
            searchParamDeployPs.Res = true;
            Show(ShownDeployPs());
        }

        private void radioButtonTous_Checked(object sender, RoutedEventArgs e)
        {
            searchParamDeployPs.Res = null;
            Show(ShownDeployPs());
        }

        private void radioButtonFalse_Checked(object sender, RoutedEventArgs e)
        {
            searchParamDeployPs.Res = false;
            Show(ShownDeployPs());
        }
        #endregion 

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Deploy Scripts
        private void btnAfficherDeployScript_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog openBrowser = new System.Windows.Forms.FolderBrowserDialog();
            openBrowser.Description = "Sélectionnez le dossier de log de déploiement des scripts";
            openBrowser.SelectedPath = baseDirectory + @"DEPLOY\NEW_SCRIPTS";
            openBrowser.ShowDialog();
            if (openBrowser.SelectedPath.Length > 0)
            {
                if (Directory.Exists(openBrowser.SelectedPath))
                {
                    listeDeployScripts = new List<DeployScript>();
                    FileInfo[] files = new DirectoryInfo(openBrowser.SelectedPath).GetFiles("*.txt");

                    foreach (FileInfo file in files)
                    {
                        listeDeployScripts.AddRange(GetLogScriptsFromFiles(file));
                    }

                    Show(ShownDeployScript());
                    FillMainDeployScriptComboBoxes();
                }
            }
        }

        private void FillMainDeployScriptComboBoxes()
        {
            foreach (DeployScript deployScript in listeDeployScripts)
            {
                if (!Exists(deployScript.MachineName, 1, 0))
                {
                    comboBoxDepartementDeployScript.Items.Add(GetNameFromFullName(deployScript.MachineName));
                }

                if (!Exists(deployScript.Date, 2, 0))
                {
                    comboBoxDateDeployScript.Items.Add(deployScript.Date.Split(' ')[0]);
                }
            }
        }

        public List<DeployScript> GetLogScriptsFromFiles(FileInfo fichier)
        {
            List<DeployScript> listeLog = new List<DeployScript>();
            try
            {
                StreamReader sr = new StreamReader(fichier.FullName, Encoding.UTF8);
                string line;

                line = sr.ReadLine();

                while (line != null)
                {
                    string[] splitLine = line.Split(']');
                    splitLine[0] = splitLine[0].Replace('[', ' ').Trim();
                    splitLine[1] = splitLine[1].Replace('[', ' ').Trim();
                    splitLine[2] = splitLine[2].Replace('[', ' ').Trim();

                    listeLog.Add(new DeployScript(splitLine[0], splitLine[1], splitLine[2]));
                    line = sr.ReadLine();
                }
                sr.Close();
            }
            catch
            { }
            return listeLog;
        }

        private void Show(List<DeployScript> listeDeployScript)
        {
            listBoxAffichageDeployScript.Items.Clear();
            foreach (DeployScript file in listeDeployScript)
            {
                listBoxAffichageDeployScript.Items.Add("[" + file.MachineName + "] [" + file.Date + "] [" + file.Text + "]");
            }
            labelCountDeployScript.Content = listeDeployScript.Count + " affichés";
        }

        private bool Exists(string valeur, int cas, int useless)
        {
            bool exists = false;

            switch (cas)
            {
                case 1:
                    foreach (object obj in comboBoxDepartementDeployScript.Items)
                    {
                        if (GetNameFromFullName(valeur) == obj.ToString())
                        {
                            exists = true;
                            break;
                        }
                    }
                    break;
                case 2:
                    foreach (object obj in comboBoxDateDeployScript.Items)
                    {
                        if (valeur.Split(' ')[0] == obj.ToString())
                        {
                            exists = true;
                            break;
                        }
                    }
                    break;
            }

            return exists;
        }

        private List<DeployScript> ShownDeployScript()
        {
            var query = listeDeployScripts;
            if (searchParamDeployScripts.Dep != "*")
            {
                query = (from q in query
                         where GetNameFromFullName(q.MachineName) == searchParamDeployScripts.Dep
                         select q).ToList();
            }

            if (searchParamDeployScripts.Ord != "*")
            {
                query = (from q in query
                         where q.MachineName == searchParamDeployScripts.Ord
                         select q).ToList();
            }

            if (searchParamDeployScripts.Dat != "*")
            {
                query = (from q in query
                         where q.Date.Split(' ')[0] == searchParamDeployScripts.Dat
                         select q).ToList();
            }

            if (searchParamDeployScripts.Heu != "*")
            {
                query = (from q in query
                         where q.Date.Split(' ')[1] == searchParamDeployScripts.Heu
                         select q).ToList();
            }

            return (List<DeployScript>)query;
        }

        private void FillComputerDeployScriptsCombobox()
        {
            comboBoxOrdinateurDeployScript.Items.Clear();
            comboBoxOrdinateurDeployScript.Items.Add("   -- Ordinateur --");
            comboBoxOrdinateurDeployScript.SelectedIndex = 0;

            foreach (DeployScript deployScript in listeDeployScripts)
            {
                bool exists = false;
                if (GetNameFromFullName(deployScript.MachineName) == searchParamDeployScripts.Dep)
                {
                    foreach (object obj in comboBoxOrdinateurDeployScript.Items)
                    {
                        if (deployScript.MachineName == obj.ToString())
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        comboBoxOrdinateurDeployScript.Items.Add(deployScript.MachineName);
                    }
                }
            }
        }

        private void FillHeureDeployScriptsCombobox()
        {
            comboBoxHeureDeployScript.Items.Clear();
            comboBoxHeureDeployScript.Items.Add("      -- Heure --");
            comboBoxHeureDeployScript.SelectedIndex = 0;

            foreach (DeployScript deployScript in listeDeployScripts)
            {
                bool exists = false;
                if (deployScript.Date.Split(' ')[0] == searchParamDeployScripts.Dat)
                {
                    foreach (object obj in comboBoxHeureDeployScript.Items)
                    {
                        if (deployScript.Date.Split(' ')[1] == obj.ToString())
                        {
                            exists = true;
                        }
                    }

                    if (!exists)
                    {
                        comboBoxHeureDeployScript.Items.Add(deployScript.Date.Split(' ')[1]);
                    }
                }
            }
        }

        private void comboBoxDepartementDeployScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxDepartementDeployScript.SelectedIndex < 1)
            {
                comboBoxDepartementDeployScript.SelectedIndex = 0;
                searchParamDeployScripts.Dep = "*";
                comboBoxOrdinateurDeployScript.IsEnabled = false;
            }
            else
            {
                searchParamDeployScripts.Dep = comboBoxDepartementDeployScript.SelectedItem.ToString();
                comboBoxOrdinateurDeployScript.IsEnabled = true;
            }
            FillComputerDeployScriptsCombobox();
            Show(ShownDeployScript());
        }


        private void comboBoxOrdinateurDeployScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxOrdinateurDeployScript.SelectedIndex < 1)
            {
                comboBoxOrdinateurDeployScript.SelectedIndex = 0;
                searchParamDeployScripts.Ord = "*";
            }
            else
            {
                searchParamDeployScripts.Ord = comboBoxOrdinateurDeployScript.SelectedItem.ToString();
            }
            Show(ShownDeployScript());
        }

        private void comboBoxDateDeployScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxDateDeployScript.SelectedIndex < 1)
            {
                comboBoxDateDeployScript.SelectedIndex = 0;
                searchParamDeployScripts.Dat = "*";
                comboBoxHeureDeployScript.IsEnabled = false;
            }
            else
            {
                searchParamDeployScripts.Dat = comboBoxDateDeployScript.SelectedItem.ToString();
                comboBoxHeureDeployScript.IsEnabled = true;
            }
            FillHeureDeployScriptsCombobox();
            Show(ShownDeployScript());
        }

        private void comboBoxHeureDeployScript_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxHeureDeployScript.SelectedIndex < 1)
            {
                comboBoxHeureDeployScript.SelectedIndex = 0;
                searchParamDeployScripts.Heu = "*";
            }
            else
            {
                searchParamDeployScripts.Heu = comboBoxHeureDeployScript.SelectedItem.ToString();
            }
            Show(ShownDeployScript());
        }

        private void btnRAZDeployScript_Click(object sender, RoutedEventArgs e)
        {
            comboBoxDepartementDeployScript.SelectedIndex = 0;
            comboBoxDateDeployScript.SelectedIndex = 0;
        }

        private void OpenAffichageDeployScript()
        {
            if (listBoxAffichageDeployScript.SelectedItem != null)
            {
                string[] splitLine = listBoxAffichageDeployScript.SelectedItem.ToString().Split(']');
                splitLine[0] = splitLine[0].Replace('[', ' ').Trim();
                splitLine[1] = splitLine[1].Replace('[', ' ').Trim();
                splitLine[2] = splitLine[2].Replace('[', ' ').Trim();


                Affichage affichage = new Affichage();
                affichage.MachineName = splitLine[0];
                affichage.Date = splitLine[1];
                affichage.Message = splitLine[2];
                affichage.User = "-";
                affichage.Script = "-";

                AffichageWindow aw = new AffichageWindow(affichage);
                aw.ShowDialog();
            }
        }

        private void listBoxAffichageDeployScript_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            OpenAffichageDeployScript();
        }

        private void listBoxAffichageDeployScript_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                OpenAffichageDeployScript();
            }
        }
        #endregion
    }
}
