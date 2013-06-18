using System.Windows;

namespace Log_Reader
{
    /// <summary>
    /// Interaction logic for AffichageWindow.xaml
    /// </summary>
    public partial class AffichageWindow : Window
    {
        public AffichageWindow(Affichage affichage)
        {
            InitializeComponent();

            labelMachineName.Content += affichage.MachineName;
            labelUser.Content += affichage.User;
            labelScript.Content += affichage.Script;
            labelDate.Content += affichage.Date;
            txtBoxMessage.Text += affichage.Message;
        }

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
