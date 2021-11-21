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
using System.Windows.Shapes;

namespace crypcy.desktop.Views
{
    /// <summary>
    /// Interaction logic for ConnectToServerWindow.xaml
    /// </summary>
    public partial class ConnectToServerWindow : Window
    {
        private string peerName;
        private IPAddress ipAddress;
        private IPEndPoint serverEndpoint;

        public ConnectToServerWindow()
        {
            InitializeComponent();
        }

        public IPEndPoint ServerEndpoint
        {
            get { return serverEndpoint; }
        }

        public string PeerName
        {
            get { return peerName; }
        }


        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            if(AddressBox.Text != "" && PortBox.Text != "") 
            {
                if (!IPAddress.TryParse(AddressBox.Text, out ipAddress))
                {
                    ipAddress = Dns.GetHostEntry(AddressBox.Text).AddressList[0];
                }

                serverEndpoint = new IPEndPoint(address: ipAddress, port: int.Parse(PortBox.Text));
                peerName = PeerNameBox.Text;
            }
            else
            {
                MessageBox.Show("Inncorect host or port");
            }

            
            MainWindow mw = new MainWindow(ServerEndpoint, PeerName);
            mw.Show();
            this.Close();
        }
    }
}
