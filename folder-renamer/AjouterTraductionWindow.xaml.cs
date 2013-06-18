using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Folder_Rename_Wpf
{
    /// <summary>
    /// Interaction logic for AjouterTraductionWindow.xaml
    /// </summary>
    public partial class AjouterTraductionWindow : Window
    {
        public AjouterTraductionWindow(MainWindow mw)
        {
            InitializeComponent();
            this.mw = mw;
            FillTraductions();
        }

        private MainWindow mw;

        private void FillTraductions()
        {
            listBoxTraductions.Items.Clear();
            foreach (Traduction trad in mw.listeTraduction)
            {
                listBoxTraductions.Items.Add("Traduction de : " + trad.Fr + " en " + trad.En);
            }
        }

        private void Refresh()
        {
            txtBoxValeur.Focus();
            txtBoxValeur.Text = "";
            txtBoxValeurTraduite.Text = "";
        }

        private void btnAjouter_Click(object sender, RoutedEventArgs e)
        {
            if(txtBoxValeur.Text != "" && txtBoxValeurTraduite.Text != "")
            {
                Traduction trad = new Traduction(txtBoxValeur.Text, txtBoxValeurTraduite.Text);
                mw.listeTraduction.Add(trad);
                mw.SetListeTraduction();

                FillTraductions();
                Refresh();
            }
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            FillTraductions();
        }

        private void btnFermer_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
