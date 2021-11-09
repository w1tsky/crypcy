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
    /// Interaction logic for PeerChatControl.xaml
    /// </summary>
    public partial class PeerConnectControl : UserControl
    {

        static Peer Peer;

        List<PeerChatControl> PeerChats = new List<PeerChatControl>();

        public PeerConnectControl(Peer peer)
        {
            InitializeComponent();
            Peer = peer;
            lstPeers.ItemsSource = Peer.Peers;

            Peer.OnClientAdded += Peer_OnClientAdded;
            Peer.OnClientUpdated += Peer_OnClientUpdated;
            Peer.OnClientRemoved += Peer_OnClientRemoved;

            Peer.OnClientConnection += Peer_OnClientConnection;
        }

        private void Peer_OnClientAdded(object sender, PeerInfo e)
        {
            Dispatcher.Invoke(delegate
            {
                lstPeers.Items.Add(e);
            });
        }

        private void Peer_OnClientUpdated(object sender, PeerInfo e)
        {
            Dispatcher.Invoke(delegate
            {
                foreach (PeerInfo peerInfo in lstPeers.Items)
                    if (peerInfo.ID == e.ID)
                        peerInfo.Update(e);

                PeersListRefresh();
            });
        }

        private void Peer_OnClientRemoved(object sender, PeerInfo e)
        {
            int i = -1;

            foreach (PeerInfo peerInfo in lstPeers.Items)
                if (peerInfo.ID == e.ID)
                    i = lstPeers.Items.IndexOf(peerInfo);


            Dispatcher.Invoke(delegate
            {
                if (i != -1)
                    lstPeers.Items.RemoveAt(i);

                PeersListRefresh();
            });
        }

        private void Peer_OnClientConnection(object sender, IPEndPoint ipEndPoint)
        {
            Dispatcher.Invoke(delegate
            {
                PeerChatControl peerChat = PeerChats.FirstOrDefault(p => p.RemoteEP.Equals(ipEndPoint));

                if (peerChat == null)
                {
                    peerChat = new PeerChatControl(Peer, ((PeerInfo)sender).Name, ipEndPoint, ((PeerInfo)sender).ID);

                    PeerChats.Add(peerChat);
                    PeerChatFrame.Navigate(peerChat);

                }
                else
                {
                    peerChat.Focus();
                    peerChat.BringIntoView();
                }
            });
        }

        private void lstPeers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPeers.SelectedItem != null)
            {
                PeerInfo peerInfo = (PeerInfo)lstPeers.SelectedItem;
            }
        }

        private void btnConnectPeer_Click(object sender, RoutedEventArgs e)
        {
            if (lstPeers.SelectedItem != null)
            {
                PeerInfo peerInfo = (PeerInfo)lstPeers.SelectedItem;
                Peer.ConnectToPeer(peerInfo);
            }
        }

        public void PeersListRefresh()
        {
            if (lstPeers.SelectedItem != null)
            {
                lstPeers.ItemsSource = Peer.Peers;
                PeerInfo peerInfo = (PeerInfo)lstPeers.SelectedItem;
            }
        }
    }
}
