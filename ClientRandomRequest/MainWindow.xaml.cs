using SmartDrawerWpfApp.WcfServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ClientRandomRequest
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> TagList = new List<string>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private async  void Button_Click(object sender, RoutedEventArgs e)
        {
            string IP = txtIP.Text;
            string serial = txtSerial.Text;

            JsonInventory[] inventories = await  ProcessSelectionFromServer.GetLastScan(serial);
            if (inventories != null)
            {
                TagList.Clear();
                foreach (JsonInventory inv in inventories)
                {
                    inv.present_tags.ForEach(uid => TagList.Add(uid));
                }
            }
            int nbTag = TagList.Count;
            txtScanInfo.Text = DateTime.Now.ToShortDateString() + " : " + nbTag + " Tag(s) UID recover from last scan";
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            int nbTagInRequest = int.Parse(txtNbTag.Text);
            List<string> ListTagToPull = new List<string>();

            if (TagList.Count > nbTagInRequest)
            {
                Random rd = new Random(DateTime.Now.Millisecond);
                for (int loop = 0; loop < nbTagInRequest; loop++ )
                {
                    int rg = rd.Next(0, TagList.Count);
                    if (!ListTagToPull.Contains(TagList[rg]))
                        ListTagToPull.Add(TagList[rg]);
                }
            }

            string user = "christophe";
            string description = "Random Selection at " + DateTime.Now.ToShortTimeString();
            bool x = await ProcessSelectionFromServer.PostRequest(user, description, ListTagToPull);

        }

        private void txtNbTag_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
