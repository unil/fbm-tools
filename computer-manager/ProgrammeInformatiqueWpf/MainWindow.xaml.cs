using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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

            if (Utils.AreFilesAvalaible())
            {
                GetListOfDepartementsAndConfiguration();
            }
            else
            {
                txtBoxSortie.Text = "Vous devez exécuter le programme dans son dossier afin que les fichiers de configurations nécessaires soient disponibles.";
                DisableForm();
            }
        }

        private int OS;
        private List<Departement> listeDepartements = new List<Departement>();
        private Configuration configuration = new Configuration();
        private List<Log> listeLogs = new List<Log>();

        private void GetListOfDepartementsAndConfiguration()
        {
            configuration = SerializeProvider.DeserializeConfiguration(@".\CONFIGURATION\configuration.xml");
            listeDepartements = SerializeProvider.DeserializeDepartements(@".\CONFIGURATION\departements.xml");
            OS = Utils.GetOsVersion();

            foreach (Departement dep in listeDepartements)
            {
                comboBoxDepartement.Items.Add(dep.Nom);
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

            if (passwordBoxCompte.Password == "" && (bool)!radioButtonChangerNom.IsChecked)
            {
                lblPasswordAD.Foreground = Brushes.Red;
                valide = false;
            }

            if (comboBoxDepartement.SelectedIndex < 0 && (bool)!(radioButtonSupprimer.IsChecked))
            {
                lblDepartement.Foreground = Brushes.Red;
                valide = false;
            }

            if (txtBoxUsername.Text == "" && (bool)!radioButtonChangerNom.IsChecked)
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
            try
            {
                var query = (from q in listeDepartements
                             where q.Nom == comboBoxDepartement.SelectedItem.ToString()
                             select q).Single();

                txtBoxPrefixe.Text = query.Abreviation;
                configuration.OU = "OU=" + query.OU + "," + configuration.LDAP;
            }
            catch { }
        }

        private void radioButtonAjouter_Checked(object sender, RoutedEventArgs e)
        {
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
            groupBoxUtilisateur.IsEnabled = false;
            groupBoxOrdinateur.IsEnabled = true;
        }

        private void btnValider_Click(object sender, RoutedEventArgs e)
        {
            btnValider.IsEnabled = false;

            if (Valider())
            {
                int res = 0;
                string strMessage = "";
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
                        res = Domain.Join(configuration.AD, configuration.OU, "ad\\" + txtBoxUsername.Text, passwordBoxCompte.Password);
                        strMessage = "Bienvenu dans le domaine " + configuration.AD + ".";

                        Log log = new Log();
                        log.ComputerName = Environment.MachineName;
                        log.Date = DateTime.Now.Year + "-" + DateTime.Now.Month + "-" + DateTime.Now.Day + " " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second;
                        log.UserName = txtBoxUsername.Text;
                        log.ComputerParams = new List<string>();
                        log.ComputerParams.AddRange(Utils.GetComputerInformations());

                        listeLogs = SerializeProvider.DeserializeLog(@".\log.xml");
                        listeLogs.Add(log);
                        SerializeProvider.SerializeLog(@".\log.xml", listeLogs);
                    }
                }
                else if (radioButtonSupprimer.IsChecked == true)
                {
                    //res = Domain.Remove(txtBoxUsername.Text, passwordBoxCompte.Password);
                    res = Domain.Remove(txtBoxUsername.Text, passwordBoxCompte.Password);
                    strMessage = "Vous vous êtes déconnecté du domaine avec succès.";
                }
                else if (radioButtonChangerNom.IsChecked == true)
                {
                    //res = Domain.RenameMachineByDirectoryServices(Environment.MachineName, txtBoxPrefixe.Text + txtBoxNom.Text, "ad\\" + txtBoxUsername.Text, passwordBoxCompte.Password);
                    res = Domain.RenameComputer(txtBoxPrefixe.Text + txtBoxNom.Text);
                    strMessage = "Le nom de l'ordinateur a été changé avec succès.";
                }
                btnValider.IsEnabled = true;

                if (res == 0)
                {
                    txtBoxSortie.Text = strMessage;
                    DisableForm();
                    RebootComputer();
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

        private void DisableForm()
        {
            groupBoxAction.IsEnabled = false;
            groupBoxUtilisateur.IsEnabled = false;
            groupBoxOrdinateur.IsEnabled = false;
            groupBoxInformations.IsEnabled = false;
            groupBoxDepartement.IsEnabled = false;
            btnValider.IsEnabled = false;            
        }

        private void RebootComputer()
        {
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
                    DisableForm();
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
