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
            peer.ConnectOrDisconnect();
            peer.OnServerConnect += Peer_OnServerConnect;
            peer.OnServerDisconnect += Peer_OnServerDisconnect;
            peer.OnClientAdded += Peer_OnClientAdded;
        }


        private void Peer_OnServerConnect(object sender, EventArgs e)
        {
            ((MainWindow)App.Current.MainWindow).ConnectionStatus.Text = "Connected";
        }

        private void Peer_OnServerDisconnect(object sender, EventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                //((MainWindow)App.Current.MainWindow).ConnectionStatus.Text = "Disconnected";
                lstPeers.Items.Clear();

                //for (int c = 0; c < ChatWindows.Count - 1; c++)
                //    ChatWindows[c].Close();
            });
        }

        private void Peer_OnClientAdded(object sender, PeerInfo e)
        {
            Dispatcher.Invoke(delegate
            {
                lstPeers.Items.Add(e);
            });
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
