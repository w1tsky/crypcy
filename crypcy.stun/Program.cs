using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using crypcy.shared;

namespace crypcy.stun
{
    class Program
    {

        static int Port = 50;

        static IPEndPoint UDPEndPoint = new IPEndPoint(IPAddress.Any, Port);
        static UdpClient UDP = new UdpClient(UDPEndPoint);

        static List<PeerInfo> Peers = new List<PeerInfo>();

        static void Main(string[] args)
        {
            Thread ThreadUDP = new Thread(new ThreadStart(UDPListen));

            ThreadUDP.Start();

            e:  Console.WriteLine("Type 'exit' to shutdown the server");

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
                    IPeer Item = ReceivedBytes.ByteArrayToPeer();
                    ProcessItem(Item, ProtocolType.Udp, UDPEndPoint);
                }
            }
        }

        static void ProcessItem(IPeer Item, ProtocolType Protocol, IPEndPoint EP = null) //, TcpClient tcp = null)
        {
            if (Item.GetType() == typeof(PeerInfo))
            {
                PeerInfo peer = Peers.FirstOrDefault(x => x.ID == ((PeerInfo)Item).ID);

                if (peer == null)
                {
                    peer = (PeerInfo)Item;
                    Peers.Add(peer);

                    if (EP != null)
                        Console.WriteLine("Client Added: UDP EP: {0}:{1}, Name: {2}", EP.Address, EP.Port, peer.Name);
                   // else if (tcp != null)
                   //     Console.WriteLine("Client Added: TCP EP: {0}:{1}, Name: {2}", ((IPEndPoint)tcp.Client.RemoteEndPoint).Address, ((IPEndPoint)tcp.Client.RemoteEndPoint).Port, peer.Name);
                }
                else
                {
                    peer.Update((PeerInfo)Item);

                    if (EP != null)
                        Console.WriteLine("Client Updated: UDP EP: {0}:{1}, Name: {2}", EP.Address, EP.Port, peer.Name);
                  //  else if (tcp != null)
                  //      Console.WriteLine("Client Updated: TCP EP: {0}:{1}, Name: {2}", ((IPEndPoint)tcp.Client.RemoteEndPoint).Address, ((IPEndPoint)tcp.Client.RemoteEndPoint).Port, peer.Name);
                }

                if (EP != null)
                    peer.ExternalEndpoint = EP;

                //if (tcp != null)
                //     peer.tcp = tcp;

                //BroadcastTCP(peer);

                if (!peer.Initialized)
                {
                    if (peer.ExternalEndpoint != null & Protocol == ProtocolType.Udp)
                        SendUDP(new Message("Server", peer.Name, "UDP Communication Test"), peer.ExternalEndpoint);

                //    if (peer.Client != null & Protocol == ProtocolType.Tcp)
                //        SendTCP(new Message("Server", peer.Name, "TCP Communication Test"), peer.Client);

                    // if (peer.tcp != null & peer.ExternalEndpoint != null)
                    // {
                    //     foreach (PeerInfo p in Peers)                                          
                    //         SendUDP(p, peer.ExternalEndpoint);                       

                    //     peer.Initialized = true;
                    // }
                }
            }


            // else if (Item.GetType() == typeof(Message))
            // {
            //     Console.WriteLine("Message from {0}:{1}: {2}", UDPEndPoint.Address, UDPEndPoint.Port, ((Message)Item).Content);
            // }           
            // else if (Item.GetType() == typeof(Req))
            // {
            //     Req R = (Req)Item;

            //     PeerInfo peer = Peers.FirstOrDefault(x => x.ID == R.RecipientID);

            //     if (peer != null)
            //         SendTCP(R, peer.tcp);
            // }            
        }


        // static void SendTCP(IPeer Item, TcpClient tcp)
        // {
        //     if (tcp != null && tcp.Connected)
        //     {
        //         byte[] Data = Item.PeerToByteArray();

        //         NetworkStream NetStream = tcp.GetStream();
        //         NetStream.Write(Data, 0, Data.Length);            
        //     }
        // }

        static void SendUDP(IPeer Item, IPEndPoint EP)
        {
            byte[] Bytes = Item.PeerToByteArray();
            UDP.Send(Bytes, Bytes.Length, UDPEndPoint);
        }




        // static void BroadcastTCP(IPeer Item)
        // {
        //     foreach (PeerInfo peer in Peers.Where(x => x.tcp != null))
        //         SendTCP(Item, peer.tcp);
        // }

        static void BroadcastUDP(IPeer Item)
        {
            foreach (PeerInfo peer in Peers)
                SendUDP(Item, peer.ExternalEndpoint);
        }       
    }
}
