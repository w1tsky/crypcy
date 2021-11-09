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
using static crypcy.core.Peer;

namespace crypcy.desktop.Views
{
    /// <summary>
    /// Interaction logic for PeerChatControl.xaml
    /// </summary>
    public partial class PeerChatControl : UserControl
    {

        public static Peer Peer;

        public new string Name;
        public IPEndPoint RemoteEP;
        public long ID;

        public PeerChatControl(Peer peer, string name, IPEndPoint remoteEP, long id)
        {
            InitializeComponent();

            Peer = peer;
            Name = name;
            RemoteEP = remoteEP;
            ID = id;

            Peer.OnMessageReceived += Peer_OnMessageReceived;
        }

        private void SendMessage()
        {
            Message M = new Message(Peer.LocalPeerInfo.Name, Name, SendMessageBox.Text);
            Peer.SendMessageUDP(M, RemoteEP);
            MessagesBox.Text += Peer.LocalPeerInfo.Name + ": " + SendMessageBox.Text + '\n';
            SendMessageBox.Text = string.Empty;
            MessagesBox.CaretIndex = MessagesBox.Text.Length;
            MessagesBox.ScrollToEnd();
            SendMessageBox.Focus();
        }

        public void ReceiveMessage(Message M)
        {
            MessagesBox.Text += M.From + ": " + M.Content + '\n';
            MessagesBox.CaretIndex = MessagesBox.Text.Length;
            MessagesBox.ScrollToEnd();
            SendMessageBox.Focus();
        }

        private void ButtonSendMessage_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
        }

        private void Peer_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                ReceiveMessage(e.message);
            });
        }
    }
}
