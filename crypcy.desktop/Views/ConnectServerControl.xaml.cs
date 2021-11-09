using crypcy.core;
using crypcy.shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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

namespace crypcy.desktop.Views
{
    /// <summary>
    /// Interaction logic for ConnectServerControl.xaml
    /// </summary>
    public partial class ConnectServerControl : UserControl
    {
   
        public ConnectServerControl(Peer peer)
        {
            InitializeComponent();

            lstPeers.ItemsSource = peer.Peers;

        }

        private void lstPeers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPeers.SelectedItem != null)
            {
                PeerInfo peerInfo = (PeerInfo)lstPeers.SelectedItem;
                PeerInfoDetails.DataContext = peerInfo;
            }
        }

    }
}
