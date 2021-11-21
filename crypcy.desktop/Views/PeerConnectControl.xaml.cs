using crypcy.core;
using crypcy.shared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public ObservableCollection<PeerInfo> PeersList { get; set; }


        public PeerConnectControl(Peer peer)
        {
            InitializeComponent();
            Peer = peer;
            PeersList = new ObservableCollection<PeerInfo>(Peer.Peers);

            lstPeers.ItemsSource = PeersList;

            Peer.OnClientAdded += Peer_OnClientAdded;
            Peer.OnClientUpdated += Peer_OnClientUpdated;
            Peer.OnClientRemoved += Peer_OnClientRemoved;

            Peer.OnClientConnection += Peer_OnClientConnection;
        }

        private void Peer_OnClientAdded(object sender, PeerInfo e)
        {
            Dispatcher.Invoke(delegate
            {
                PeersList.Add(e);
            });
        }

        private void Peer_OnClientUpdated(object sender, PeerInfo e)
        {
            Dispatcher.Invoke(delegate
            {
                PeersList.ToList().ForEach(peerInfo =>
                {
                    if (peerInfo.ID == e.ID)
                        peerInfo.Update(e);
                });

                PeersListRefresh();
            });
        }

        private void Peer_OnClientRemoved(object sender, PeerInfo e)
        {
            int i = -1;

            PeersList.ToList().ForEach(peerInfo =>
            {
                if (peerInfo.ID == e.ID)
                    i = PeersList.IndexOf(peerInfo);
            });


            Dispatcher.Invoke(delegate
            {
                if (i != -1)
                    PeersList.RemoveAt(i);

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
                }
            });
        }

        private void lstPeers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            if (lstPeers.SelectedItem != null)
            {
                PeerInfo peerInfo = (PeerInfo)lstPeers.SelectedItem;
                PeerChatControl peerChat = PeerChats.FirstOrDefault(p => p.RemoteEP.Equals(peerInfo.ExternalEndpoint));
                if (peerChat == null)
                {
                    peerChat = new PeerChatControl(Peer, peerInfo.Name, peerInfo.ExternalEndpoint, peerInfo.ID);
                    PeerChatFrame.Navigate(peerChat);
                }
                else
                {
                    PeerChatFrame.Navigate(peerChat);
                }
            }
        }

        private void btnConnectPeer_Click(object sender, RoutedEventArgs e)
        {
            
            if (lstPeers.SelectedItem != null)
            {
                PeerInfo peerInfo = (PeerInfo)lstPeers.SelectedItem;
                PeerChatControl peerChat = PeerChats.FirstOrDefault(p => p.RemoteEP.Equals(peerInfo.ExternalEndpoint));
                if (peerChat == null)
                {
                    peerChat = new PeerChatControl(Peer, peerInfo.Name, peerInfo.ExternalEndpoint, peerInfo.ID);
                    Peer.ConnectToPeer(peerInfo);
                    PeerChatFrame.Navigate(peerChat);
                }
                else
                {
                    PeerChatFrame.Navigate(peerChat);
                }


            }
        }

        public void PeersListRefresh()
        {
            if (lstPeers.SelectedItem != null)
            {
                PeersList.Clear();
                PeersList = new ObservableCollection<PeerInfo>(Peer.Peers);
                lstPeers.ItemsSource = PeersList;
            }
        }
    }
}
