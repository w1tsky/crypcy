

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using crypcy.shared;

namespace crypcy.core
{
    public class Peer
    {
        public IPEndPoint ServerEndpoint { get; set; }
        private IPAddress InternetAccessAdapter;
        private TcpClient PeerTCP = new TcpClient();
        private UdpClient PeerUDP = new UdpClient();
        public PeerInfo LocalPeerInfo = new PeerInfo();

        private List<PeerInfo> Peers = new List<PeerInfo>();
        private List<Ack> AckResponces = new List<Ack>();

        private Thread ThreadTCPListen;
        private Thread ThreadUDPListen;


        public event EventHandler OnServerConnect;
        public event EventHandler OnServerDisconnect;

        public event EventHandler<string> OnResultsUpdate;
        public event EventHandler<MessageReceivedEventArgs> OnMessageReceived;
        public event EventHandler<PeerInfo> OnClientAdded;
        public event EventHandler<PeerInfo> OnClientUpdated;
        public event EventHandler<PeerInfo> OnClientRemoved;
        public event EventHandler<IPEndPoint> OnClientConnection;


        private bool _TCPListen = false;
        public bool TCPListen
        {
            get { return _TCPListen; }
            set
            {
                _TCPListen = value;
                if (value)
                    ListenTCP();
            }
        }

        private bool _UDPListen = false;
        public bool UDPListen
        {
            get { return _UDPListen; }
            set
            {
                _UDPListen = value;
                if (value)
                    ListenUDP();
            }
        }

        public Peer(IPEndPoint serverEndpoint)
        {
            ServerEndpoint = serverEndpoint;

            PeerUDP.AllowNatTraversal(true);
            PeerUDP.Client.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
            PeerUDP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            LocalPeerInfo.Name = System.Environment.MachineName;
            LocalPeerInfo.ConnectionType = ConnectionTypes.Unknown;
            LocalPeerInfo.ID = DateTime.Now.Ticks;

            var IPs = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            foreach (var IP in IPs)
                LocalPeerInfo.InternalAddresses.Add(IP);


            ListenTCP();
            ListenUDP();
        }

        private void ListenUDP()
        {
            ThreadUDPListen = new Thread(new ThreadStart(delegate
            {
                while (UDPListen)
                {
                    try
                    {
                        IPEndPoint EP = LocalPeerInfo.InternalEndpoint;

                        if (EP != null)
                        {
                            byte[] ReceivedBytes = PeerUDP.Receive(ref EP);
                            PeerItem Item = ReceivedBytes.ByteArrayToPeer(ReceivedBytes.Length);
                            ProcessItem(Item, EP);
                        }
                    }
                    catch (Exception e)
                    {
                        if (OnResultsUpdate != null)
                            OnResultsUpdate.Invoke(this, "Error on UDP Receive: " + e.Message);
                    }
                }
            }));

            ThreadUDPListen.IsBackground = true;

            if (UDPListen)
                ThreadUDPListen.Start();
        }

        private void ListenTCP()
        {
            ThreadTCPListen = new Thread(new ThreadStart(delegate
            {
                byte[] ReceivedBytes = new byte[4096];
                int BytesRead = 0;

                while (TCPListen)
                {
                    try
                    {
                        BytesRead = PeerTCP.GetStream().Read(ReceivedBytes, 0, ReceivedBytes.Length);

                        if (BytesRead == 0)
                            break;
                        else
                        {
                            PeerItem peerItem = ReceivedBytes.ByteArrayToPeer(BytesRead);
                            ProcessItem(peerItem);
                        }
                    }
                    catch (Exception e)
                    {
                        if (OnResultsUpdate != null)
                            OnResultsUpdate.Invoke(this, "Error on TCP Receive: " + e.Message);
                    }
                }
            }));

            ThreadTCPListen.IsBackground = true;

            if (TCPListen)
                ThreadTCPListen.Start();
        }
        public void ConnectOrDisconnect()
        {
            if (PeerTCP.Connected)
            {
                PeerTCP.Client.Disconnect(true);

                UDPListen = false;
                TCPListen = false;
                Peers.Clear();

                if (OnServerDisconnect != null)
                    OnServerDisconnect.Invoke(this, new EventArgs());

                if (OnResultsUpdate != null)
                    OnResultsUpdate.Invoke(this, "Disconnected.");
            }
            else
            {
                try
                {
                    InternetAccessAdapter = IPAddress.Parse("127.0.0.1");

                    if (OnResultsUpdate != null)
                        OnResultsUpdate.Invoke(this, "Adapter with Internet Access: " + InternetAccessAdapter);

                    PeerTCP = new TcpClient();
                    PeerTCP.Client.Connect(ServerEndpoint);

                    UDPListen = true;
                    TCPListen = true;

                    SendMessageUDP(LocalPeerInfo.Simplified(), ServerEndpoint);
                    LocalPeerInfo.InternalEndpoint = (IPEndPoint)PeerUDP.Client.LocalEndPoint;

                    Thread.Sleep(500);
                    SendMessageTCP(LocalPeerInfo.Simplified());

                    Thread KeepAlive = new Thread(new ThreadStart(delegate
                    {
                        while (PeerTCP.Connected)
                        {
                            Thread.Sleep(5000);
                            SendMessageTCP(new KeepAlive());
                        }
                    }));

                    KeepAlive.IsBackground = true;
                    KeepAlive.Start();

                    if (OnServerConnect != null)
                        OnServerConnect.Invoke(this, new EventArgs());

                }
                catch (Exception ex)
                {
                    if (OnResultsUpdate != null)
                        OnResultsUpdate.Invoke(this, "Error when connecting " + ex.Message);
                }
            }
        }

        public void SendMessageTCP(PeerItem peerItem)
        {
            if (PeerTCP.Connected)
            {
                byte[] data = peerItem.PeerToByteArray();
                try
                {
                    string jsonStr = Encoding.UTF8.GetString(data);   
                    System.Console.WriteLine($"TCP Sending: {jsonStr}");

                    NetworkStream NetStream = PeerTCP.GetStream();
                    NetStream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    if (OnResultsUpdate != null)
                        OnResultsUpdate.Invoke(this, "Error on TCP Send: " + e.Message);
                }
            }
        }

        public void SendMessageUDP(PeerItem peerItem, IPEndPoint EP)
        {
            peerItem.ID = LocalPeerInfo.ID;

            byte[] data = peerItem.PeerToByteArray();

            try
            {
                if (data != null)
                {
                    string jsonStr = Encoding.UTF8.GetString(data);
                    System.Console.WriteLine($"UDP Sending: {jsonStr}");

                    PeerUDP.Send(data, data.Length, EP);
                }

            }

            catch (Exception e)
            {
                if (OnResultsUpdate != null)
                    OnResultsUpdate.Invoke(this, "Error on UDP Send: " + e.Message);
            }
        }

        private void ProcessItem(PeerItem peerItem, IPEndPoint EP = null)
        {
            if (peerItem.GetType() == typeof(Message))
            {
                PeerInfo peerInfo = Peers.FirstOrDefault(x => x.ID == peerItem.ID);
                Message m = (Message)peerItem;

                if (m.ID == 0)
                    if (OnResultsUpdate != null)
                        OnResultsUpdate.Invoke(this, m.From + ": " + m.Content);

                if (m.ID != 0 & EP != null & peerInfo != null)
                    if (OnMessageReceived != null)
                        OnMessageReceived.Invoke(EP, new MessageReceivedEventArgs(peerInfo, m, EP));
            }
            else if (peerItem.GetType() == typeof(PeerInfo))
            {
                PeerInfo peerInfo = Peers.FirstOrDefault(x => x.ID == peerItem.ID);
                if (peerInfo == null)
                {
                    Peers.Add((PeerInfo)peerItem);

                    if (OnClientAdded != null)
                        OnClientAdded.Invoke(this, (PeerInfo)peerItem);
                }
                else
                {
                    peerInfo.Update((PeerInfo)peerItem);

                    if (OnClientUpdated != null)
                        OnClientUpdated.Invoke(this, (PeerInfo)peerItem);
                }
            }
            else if (peerItem.GetType() == typeof(Notification))
            {
                Notification N = (Notification)peerItem;

                if (N.Type == NotificationsTypes.Disconnected)
                {
                    PeerInfo peerInfo = Peers.FirstOrDefault(x => x.ID == long.Parse(N.Tag.ToString()));

                    if (peerInfo != null)
                    {
                        if (OnClientRemoved != null)
                            OnClientRemoved.Invoke(this, peerInfo);

                        Peers.Remove(peerInfo);
                    }
                }
                else if (N.Type == NotificationsTypes.ServerShutdown)
                {
                    if (OnResultsUpdate != null)
                        OnResultsUpdate.Invoke(this, "Server shutting down.");

                    ConnectOrDisconnect();
                }
            }
            else if (peerItem.GetType() == typeof(Req))
            {
                Req R = (Req)peerItem;

                PeerInfo peerInfo = Peers.FirstOrDefault(x => x.ID == R.ID);

                if (peerInfo != null)
                {
                    if (OnResultsUpdate != null)
                        OnResultsUpdate.Invoke(this, "Received Connection Request from: " + peerInfo.ToString());

                    IPEndPoint ResponsiveEP = FindReachableEndpoint(peerInfo);

                    if (ResponsiveEP != null)
                    {
                        if (OnResultsUpdate != null)
                            OnResultsUpdate.Invoke(this, "Connection Successfull to: " + ResponsiveEP.ToString());

                        if (OnClientConnection != null)
                            OnClientConnection.Invoke(peerInfo, ResponsiveEP);

                        if (OnClientUpdated != null)
                            OnClientUpdated.Invoke(this, peerInfo);
                    }
                }
            }
            else if (peerItem.GetType() == typeof(Ack))
            {
                Ack A = (Ack)peerItem;

                if (A.Responce)
                    AckResponces.Add(A);
                else
                {
                    PeerInfo peerInfo = Peers.FirstOrDefault(x => x.ID == A.ID);

                    if (peerInfo.ExternalEndpoint.Address.Equals(EP.Address) & peerInfo.ExternalEndpoint.Port != EP.Port)
                    {
                        if (OnResultsUpdate != null)
                            OnResultsUpdate.Invoke(this, "Received Ack on Different Port (" + EP.Port + "). Updating ...");

                        peerInfo.ExternalEndpoint.Port = EP.Port;

                        if (OnClientUpdated != null)
                            OnClientUpdated.Invoke(this, peerInfo);
                    }

                    List<string> IPs = new List<string>();
                    peerInfo.InternalAddresses.ForEach(new Action<IPAddress>(delegate (IPAddress IP) { IPs.Add(IP.ToString()); }));

                    if (!peerInfo.ExternalEndpoint.Address.Equals(EP.Address) & !IPs.Contains(EP.Address.ToString()))
                    {
                        if (OnResultsUpdate != null)
                            OnResultsUpdate.Invoke(this, "Received Ack on New Address (" + EP.Address + "). Updating ...");

                        peerInfo.InternalAddresses.Add(EP.Address);
                    }

                    A.Responce = true;
                    A.RecipientID = LocalPeerInfo.ID;
                    SendMessageUDP(A, EP);
                }
            }
        }

        public static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip;
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private IPEndPoint FindReachableEndpoint(PeerInfo peerInfo)
        {
            if (OnResultsUpdate != null)
                OnResultsUpdate.Invoke(this, "Attempting to Connect via LAN");

            for (int ip = 0; ip < peerInfo.InternalAddresses.Count; ip++)
            {
                if (!PeerTCP.Connected)
                    break;

                IPAddress IP = peerInfo.InternalAddresses[ip];

                IPEndPoint EP = new IPEndPoint(IP, peerInfo.InternalEndpoint.Port);

                for (int i = 1; i < 4; i++)
                {
                    if (!PeerTCP.Connected)
                        break;

                    if (OnResultsUpdate != null)
                        OnResultsUpdate.Invoke(this, "Sending Ack to " + EP.ToString() + ". Attempt " + i + " of 3");

                    SendMessageUDP(new Ack(LocalPeerInfo.ID), EP);
                    Thread.Sleep(200);

                    Ack Responce = AckResponces.FirstOrDefault(a => a.RecipientID == peerInfo.ID);

                    if (Responce != null)
                    {
                        if (OnResultsUpdate != null)
                            OnResultsUpdate.Invoke(this, "Received Ack Responce from " + EP.ToString());

                        peerInfo.ConnectionType = ConnectionTypes.LAN;

                        AckResponces.Remove(Responce);

                        return EP;
                    }
                }
            }

            if (peerInfo.ExternalEndpoint != null)
            {
                if (OnResultsUpdate != null)
                    OnResultsUpdate.Invoke(this, "Attempting to Connect via Internet");

                for (int i = 1; i < 100; i++)
                {
                    if (!PeerTCP.Connected)
                        break;

                    if (OnResultsUpdate != null)
                        OnResultsUpdate.Invoke(this, "Sending Ack to " + peerInfo.ExternalEndpoint + ". Attempt " + i + " of 99");

                    SendMessageUDP(new Ack(LocalPeerInfo.ID), peerInfo.ExternalEndpoint);
                    Thread.Sleep(300);

                    Ack Responce = AckResponces.FirstOrDefault(a => a.RecipientID == peerInfo.ID);

                    if (Responce != null)
                    {
                        if (OnResultsUpdate != null)
                            OnResultsUpdate.Invoke(this, "Received Ack New from " + peerInfo.ExternalEndpoint.ToString());

                        peerInfo.ConnectionType = ConnectionTypes.WAN;

                        AckResponces.Remove(Responce);

                        return peerInfo.ExternalEndpoint;
                    }
                }

                if (OnResultsUpdate != null)
                    OnResultsUpdate.Invoke(this, "Connection to " + peerInfo.Name + " failed");
            }
            else
            {
                if (OnResultsUpdate != null)
                    OnResultsUpdate.Invoke(this, "Client's External EndPoint is Unknown");
            }

            return null;
        }

        public class MessageReceivedEventArgs : EventArgs
        {
            public Message message { get; set; }
            public PeerInfo peerInfo { get; set; }
            public IPEndPoint EstablishedEP { get; set; }

            public MessageReceivedEventArgs(PeerInfo _peerInfo, Message _message, IPEndPoint _establishedEP)
            {
                peerInfo = _peerInfo;
                message = _message;
                EstablishedEP = _establishedEP;
            }
        }



    }
}