using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Diagnostics;

namespace ProgrammeInformatiqueWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            OS = Environment.OSVersion.Version.Major;

            config = new Configs();
            config.LDAP = "OU=FBM,OU=OLD-DOM,DC=ad,DC=unil,DC=ch";
            config.OU = "";
            config.AD = "ad.unil.ch";
            config.Path = basePath;

            GetConfigsFromSerialization();
            GetDepartementsFromSerialization();
        }

        private int OS;
        private string basePath = @"\\fbmnas\DATA\SYS\SCRIPTS\ADMIN\";
        private Departement departement = new Departement();
        public List<Departement> listeDepartements { get; set; }
        public Configs config { get; set; }
        private List<Log> listeLogs = new List<Log>();

        private void GetConfigsFromSerialization()
        {
            config = config.Deserialize(config);
        }

        private void GetDepartementsFromSerialization()
        {
            listeDepartements = departement.Deserialize(config);
            AfficherDepartements();
        }

        private void AfficherDepartements()
        {
            if (listeDepartements != null)
            {
                comboBoxDepartement.Items.Clear();
                foreach (Departement dep in listeDepartements)
                {
                    comboBoxDepartement.Items.Add(dep.Nom);
                }
            }
        }

        private Boolean Valider()
        {
            Boolean valide = true;
            lblDepartement.Foreground = Brushes.Black;
            lblPasswordAD.Foreground = Brushes.Black;
            lblUsernameAD.Foreground = Brushes.Black;
            lblComputerName.Foreground = Brushes.Black;
            groupBoxAction.Foreground = Brushes.Black;

            if (passwordBoxCompte.Password == "")
            {
                lblPasswordAD.Foreground = Brushes.Red;
                valide = false;
            }

            if (comboBoxDepartement.SelectedIndex < 0 && (bool)!(radioButtonSupprimer.IsChecked))
            {
                lblDepartement.Foreground = Brushes.Red;
                valide = false;
            }

            if (txtBoxUsername.Text == "")
            {
                lblUsernameAD.Foreground = Brushes.Red;
                valide = false;
            }

            if (radioButtonChangerNom.IsChecked == true)
            {
                if (txtBoxNom.Text == "")
                {
                    lblComputerName.Foreground = Brushes.Red;
                    valide = false;
                }
            }

            if (!(radioButtonAjouter.IsChecked == true || radioButtonSupprimer.IsChecked == true || radioButtonChangerNom.IsChecked == true))
            {
                groupBoxAction.Foreground = Brushes.Red;
                valide = false;
            }

            return valide;
        }

        private void comboBoxDepartement_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxDepartement.Items.Count < 1)
            {
                GetDepartementsFromSerialization();
            }

            try
            {
                var query = (from q in listeDepartements
                             where q.Nom == comboBoxDepartement.SelectedItem.ToString()
                             select q).Single();

                txtBoxPrefixe.Text = query.Abreviation;
                config.OU = "OU=" + query.OU + "," + config.LDAP;
            }
            catch { }
        }

        private void btnSupprimerDepartement_Click(object sender, RoutedEventArgs e)
        {
            var query = (from q in listeDepartements
                         where q.Nom == comboBoxDepartement.SelectedItem.ToString()
                         select q).Single();
            listeDepartements.Remove(query);

            departement.Serialize(listeDepartements, config);
            GetDepartementsFromSerialization();
            txtBoxPrefixe.Text = "";
        }

        private void radioButtonAjouter_Checked(object sender, RoutedEventArgs e)
        {
            departement.Deserialize(config);
            groupBoxDepartement.IsEnabled = true;
            groupBoxUtilisateur.IsEnabled = true;
            groupBoxOrdinateur.IsEnabled = false;
        }

        private void radioButtonSupprimer_Checked(object sender, RoutedEventArgs e)
        {
            groupBoxDepartement.IsEnabled = false;
            groupBoxUtilisateur.IsEnabled = true;
            groupBoxOrdinateur.IsEnabled = false;
        }

        private void radioButtonChangerNom_Checked(object sender, RoutedEventArgs e)
        {
            groupBoxDepartement.IsEnabled = true;
            groupBoxUtilisateur.IsEnabled = true;
            groupBoxOrdinateur.IsEnabled = true;
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            btnValider.IsEnabled = false;

            if (Valider())
            {
                int res = 0;
                String strMessage = "";
                if (radioButtonAjouter.IsChecked == true)
                {
                    if (!Utils.CheckComputerName(txtBoxPrefixe.Text))
                    {
                        radioButtonChangerNom.IsChecked = true;
                        groupBoxOrdinateur.IsEnabled = true;
                        groupBoxAction.IsEnabled = false;
                        res = -1;
                    }
                    else
                    {
                        res = Domain.Join(config.AD, config.OU, "ad\\" + txtBoxUsername.Text, passwordBoxCompte.Password);
                        strMessage = "Bienvenu dans le domaine " + config.AD + ".";

                        DateTime d = DateTime.Now;
                        Log log = new Log();
                        log.ComputerName = Environment.MachineName;
                        log.Date = d.Year + "-" + d.Month + "-" + d.Day + " " + d.Hour + ":" + d.Minute + ":" + d.Second;
                        log.UserName = txtBoxUsername.Text;
                        log.ComputerParams = new List<string>();
                        log.ComputerParams.AddRange(Utils.GetComputerInformations());
                        listeLogs.Clear();
                        listeLogs = log.Deserialize(config);
                        listeLogs.Add(log);
                        log.Serialize(config, listeLogs);
                    }
                }
                else if (radioButtonSupprimer.IsChecked == true)
                {
                    res = Domain.Remove(txtBoxUsername.Text, passwordBoxCompte.Password);
                    strMessage = "Vous vous êtes déconnecté du domaine avec succès.";
                }
                else if (radioButtonChangerNom.IsChecked == true)
                {
                    Domain.RenameMachineByDirectoryServices(Environment.MachineName,
                        txtBoxPrefixe.Text + txtBoxNom.Text,
                        "ad\\" + txtBoxUsername.Text,
                        passwordBoxCompte.Password);
                    strMessage = "Le nom de l'ordinateur a été changé avec succès.";

                }
                btnValider.IsEnabled = true;

                if (res == 0)
                {
                    txtBoxSortie.Text = strMessage;
                    DesactivateForm();
                }
                else
                {
                    txtBoxSortie.Text = Utils.GetErrorMessage(res);
                }
            }
            else
            {
                btnValider.IsEnabled = true;
            }
        }

        private void DesactivateForm()
        {
            groupBoxAction.IsEnabled = false;
            groupBoxUtilisateur.IsEnabled = false;
            groupBoxOrdinateur.IsEnabled = false;
            groupBoxInformations.IsEnabled = false;
            groupBoxDepartement.IsEnabled = false;
            btnValider.IsEnabled = false;
            txtBoxSortie.Text += Environment.NewLine + "Pour appliquer les changements, veuillez redémarrer Windows.";

            if (MessageBox.Show("Souhaitez-vous redémarrer maintenant ?", "Redémarrage nécessaire", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Process process = new Process();
                process.StartInfo.FileName = "shutdown.exe";
                process.StartInfo.Arguments = " -r -t 0";
                process.Start();
                process.Close();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (OS >= 6 && Utils.IsUacEnabled())
            {
                if (MessageBox.Show("Pour l'utilisation de ce programme, l'UAC (User Account Control)" + Environment.NewLine +
                          "doit être désactivé. Voulez-vous le désactiver maintenant?", "Programme informatique", MessageBoxButton.OKCancel, MessageBoxImage.Information) == MessageBoxResult.OK)
                {
                    MessageBox.Show(Utils.DisableUAC());
                    DesactivateForm();
                }
                else
                {
                    Environment.Exit(1);
                }
            }
        }

        private void passwordBoxCompte_GotFocus(object sender, RoutedEventArgs e)
        {
            passwordBoxCompte.Password = "";
        }
    }
}
