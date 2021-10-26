using crypcy.core;
using crypcy.shared;
using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Interaction logic for PeerChatControl.xaml
    /// </summary>
    public partial class PeerChatControl : UserControl
    {
        public PeerChatControl(Peer peer)
        {
            InitializeComponent();
            peer.ConnectOrDisconnect();
            peer.OnClientAdded += Peer_OnClientAdded;
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
            }
        }
    }
}
