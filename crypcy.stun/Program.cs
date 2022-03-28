using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using crypcy.shared;
using static PeerTCP;

namespace crypcy.stun
{
    class Program
    {
        static int Port = 23555;
        static IPEndPoint UDPEndPoint = new IPEndPoint(IPAddress.Any, Port);
        static UdpClient UDP = new UdpClient(UDPEndPoint);

        static IPEndPoint TCPEndPoint = new IPEndPoint(IPAddress.Any, Port);
        static List<PeerInfo> Peers = new List<PeerInfo>();

        static void Main(string[] args)
        {
            Thread ThreadTCP = new Thread(new ThreadStart(TCPListen));
            Thread ThreadUDP = new Thread(new ThreadStart(UDPListen));

            ThreadUDP.Start();
            ThreadTCP.Start();

        e: Console.WriteLine("Type 'exit' to shutdown the server");

            if (Console.ReadLine().ToUpper() == "EXIT")
            {
                Console.WriteLine("Shutting down...");
                Environment.Exit(0);
            }
            else
            {
                goto e;
            }

        }

        static void UDPListen()
        {
            Console.WriteLine("UDP Listener Started");
            UDP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            while (true)
            {
                byte[] ReceivedBytes = null;

                try
                {
                    ReceivedBytes = UDP.Receive(ref UDPEndPoint);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("UDP Error: {0}", ex.Message);
                }

                if (ReceivedBytes != null)
                {

                    PeerItem peerItem = ReceivedBytes.ByteArrayToPeer(ReceivedBytes.Length);

                    string jsonStr = Encoding.UTF8.GetString(ReceivedBytes);
                    Console.WriteLine("UDP received {0}:", jsonStr);

                    ProcessItem(peerItem, ProtocolType.Udp, UDPEndPoint);
                }
            }
        }


        static void TCPListen()
        {
            PeerTCP newPeer = new PeerTCP(TCPEndPoint, false);

            Console.WriteLine("TCP Listener Started");

            newPeer.StartListenAsync();
            newPeer.OnPeerItemReceived += (sender, peer) => ProcessItem(peer.HadleTcpClient(), ProtocolType.Tcp, null, peer.PeerTcpClient);

        }


        static void Disconnect(TcpClient peerTCP)
        {
            PeerInfo peerInfo = Peers.FirstOrDefault(x => x.PeerTCP == peerTCP);

            if (peerInfo != null)
            {
                Peers.Remove(peerInfo);
                Console.WriteLine("Client Disconnected {0}", peerTCP.Client.RemoteEndPoint.ToString());
                peerTCP.Close();

                BroadcastTCP(new Notification(NotificationsTypes.Disconnected, peerInfo.ID));
            }
        }

        public static void ProcessItem(PeerItem peerItem, ProtocolType Protocol, IPEndPoint EP = null, TcpClient peerTCP = null)
        {
            if (peerItem.GetType() == typeof(PeerInfo))
            {
                PeerInfo peerInfo = Peers.FirstOrDefault(x => x.ID == ((PeerInfo)peerItem).ID);

                if (peerInfo == null)  // Add peer to Peers List
                {
                    peerInfo = (PeerInfo)peerItem;
                    Peers.Add(peerInfo);

                    // Check if Peer has EndPoint
                    if (EP != null)
                        Console.WriteLine("Client Added: UDP EP: {0}:{1}, Name: {2}", EP.Address, EP.Port, peerInfo.Name);
                    // Check if Peer has TcpClient initialized
                    else if (peerTCP != null)
                        Console.WriteLine("Client Added: TCP EP: {0}:{1}, Name: {2}", ((IPEndPoint)peerTCP.Client.RemoteEndPoint).Address, ((IPEndPoint)peerTCP.Client.RemoteEndPoint).Port, peerInfo.Name);
                }
                else  // If peer exist update infromation about this peer
                {
                    peerInfo.Update((PeerInfo)peerItem);
                    if (EP != null) // Check if Peer has EndPoint
                        Console.WriteLine("Client Updated: UDP EP: {0}:{1}, Name: {2}", EP.Address, EP.Port, peerInfo.Name);
                    else if (peerTCP != null) // Check if Peer has TcpClient initialized
                        Console.WriteLine("Client Updated: TCP EP: {0}:{1}, Name: {2}", ((IPEndPoint)peerTCP.Client.RemoteEndPoint).Address, ((IPEndPoint)peerTCP.Client.RemoteEndPoint).Port, peerInfo.Name);
                }

                if (EP != null)
                    peerInfo.ExternalEndpoint = EP;

                if (peerTCP != null)
                    peerInfo.PeerTCP = peerTCP;

                BroadcastTCP(peerInfo);

                if (!peerInfo.Initialized)
                {
                    if (peerInfo.ExternalEndpoint != null & Protocol == ProtocolType.Udp)
                        SendUDP(new Message("Server", peerInfo.Name, "UDP Communication Test"), peerInfo.ExternalEndpoint);

                    if (peerInfo.PeerTCP != null & Protocol == ProtocolType.Tcp)
                        SendTCP(new Message("Server", peerInfo.Name, "TCP Communication Test"), peerInfo.PeerTCP);

                    if (peerInfo.PeerTCP != null & peerInfo.ExternalEndpoint != null)
                    {
                        foreach (PeerInfo p in Peers)
                            SendUDP(peerInfo, peerInfo.ExternalEndpoint);

                        peerInfo.Initialized = true;
                    }
                }

            }

            else if (peerItem.GetType() == typeof(Message))
            {
                Console.WriteLine("Message from {0}:{1}: {2}", UDPEndPoint.Address, UDPEndPoint.Port, ((Message)peerItem).Content);
            }

            else if (peerItem.GetType() == typeof(Req))
            {
                Req r = (Req)peerItem;

                PeerInfo peer = Peers.FirstOrDefault(p => p.ID == r.RecipientID);

                if (peer != null)
                {
                    SendTCP(r, peer.PeerTCP);
                }
            }
        }


        static void SendTCP(PeerItem peerItem, TcpClient peerTCP)
        {
            if (peerTCP != null && peerTCP.Connected)
            {
                byte[] Data = peerItem.PeerToByteArray();

                NetworkStream NetStream = peerTCP.GetStream();
                NetStream.Write(Data, 0, Data.Length);
            }
        }

        static void SendUDP(PeerItem peerItem, IPEndPoint EP)
        {
            byte[] Bytes = peerItem.PeerToByteArray();
            UDP.Send(Bytes, Bytes.Length, UDPEndPoint);
        }

        static void BroadcastTCP(PeerItem peerItem)
        {
            foreach (PeerInfo peer in Peers.Where(x => x.PeerTCP != null))
                SendTCP(peerItem, peer.PeerTCP);
        }

        static void BroadcastUDP(PeerItem peerItem)
        {
            foreach (PeerInfo peer in Peers)
                SendUDP(peerItem, peer.ExternalEndpoint);
        }
    }
}
