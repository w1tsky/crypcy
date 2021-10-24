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
using crypcy.core;
using crypcy.shared;

namespace crypcy.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static IPEndPoint serverEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23555);
        static Peer peer = new Peer(serverEndpoint);

        public MainWindow()
        {
            InitializeComponent();

            peer.OnServerConnect += Client_OnServerConnect;
            peer.OnServerDisconnect += Client_OnServerDisconnect;
            peer.OnClientAdded += Client_OnClientAdded;
        }

        private void Client_OnServerDisconnect(object sender, EventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                btnConnect.Content = "Connect";
                lstPeers.Items.Clear();

                //for (int c = 0; c < ChatWindows.Count - 1; c++)
                //    ChatWindows[c].Close();
            });
        }

        private void Client_OnClientAdded(object sender, PeerInfo e)
        {
            Dispatcher.Invoke(delegate
            {
                lstPeers.Items.Add(e);
            });
        }

        private void Client_OnServerConnect(object sender, EventArgs e)
        {
            btnConnect.Content = "Disconnect from Server";
        }


        private void lstPeers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstPeers.SelectedItem != null)
            {
                PeerInfo peerInfo = (PeerInfo)lstPeers.SelectedItem;
                PeerInfoDetails.DataContext = peerInfo;
            }
        }

        private void btnConnect_Click(object sender, RoutedEventArgs e)
        {
            peer.ConnectOrDisconnect();
        }

        private void closeApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void minimizeApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Minimized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void MaximizeApp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                this.WindowState = WindowState.Maximized;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
