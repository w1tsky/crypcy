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
using crypcy.desktop.Views;
using crypcy.shared;

namespace crypcy.desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Peer Peer { get; set; }
        

        public MainWindow(IPEndPoint serverEndpoint)
        {
            InitializeComponent();
            Peer = new Peer(serverEndpoint);
            Peer.ConnectOrDisconnect();

            ConnectionStatus.Text = "Connected";
        }

        private void btnChat_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PeerConnectControl(Peer));
        }

        private void btnPeerList_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ConnectServerControl(Peer));
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
