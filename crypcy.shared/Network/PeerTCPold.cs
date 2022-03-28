using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace crypcy.shared
{
    public class PeerTCPold
    {
        const int BUFFER_SIZE = 4096;
        private TcpListener TCP;

        private IPEndPoint EndPoint;
        private bool ReuseEndPoint;

        public event EventHandler<PeerItem> OnPeerItemAdded;

        public PeerTCPold(IPEndPoint tcpEndpoint, bool reuseEndPoint)
        {
            EndPoint = tcpEndpoint;
            ReuseEndPoint = reuseEndPoint;

            TCP = new TcpListener(tcpEndpoint);

            TCP.Server.ReceiveBufferSize = BUFFER_SIZE;
            TCP.Server.SendBufferSize = BUFFER_SIZE;

            if (ReuseEndPoint == true)
            {
                TCP.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            }
        }

// IPeerItemHandler peerItemHandler
        public void StartListen()
        {
            TCP.Start();

            while (true)
            {
                try
                {
                    TcpClient newPeerHandler = TCP.AcceptTcpClient();

                    Action<object> ProcessPeerTCPold = new Action<object>(delegate (object _peer)
                    {
                        TcpClient peerTCPold = (TcpClient)_peer;

                        peerTCPold.ReceiveBufferSize = BUFFER_SIZE;
                        peerTCPold.SendBufferSize = BUFFER_SIZE;

                        if (ReuseEndPoint == true)
                        {
                            peerTCPold.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                            peerTCPold.Client.Bind(EndPoint);
                        }

                        peerTCPold.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

                        PeerItem peerItem = HadleTcpClient(peerTCPold);
                        OnPeerItemAdded.Invoke(this, peerItem);
                    });

                    Thread ThreadProcessData = new Thread(new ParameterizedThreadStart(ProcessPeerTCPold));
                    ThreadProcessData.Start(newPeerHandler);

                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex);
                }

            }
        }

        public void StopListen()
        {
            TCP.Stop();
        }

        public void Disconnect(TcpClient tcpClient)
        {
            tcpClient.Close();
        }

        // Implement Method as parameter delegate or what>??
        // Run of Start Listen should return some work e.g. ProcessItem work to serve PeerItems

        public PeerItem HadleTcpClient(TcpClient tcpClient)
        {
            using NetworkStream ns = tcpClient.GetStream();

            PeerHeader peerHeader = ReadMessageHeader(tcpClient);

            PeerItem item = ReadPeerItem(peerHeader, tcpClient);

            return item;
        }

        public PeerHeader ReadMessageHeader(TcpClient tcpClient)
        {

            NetworkStream ns = tcpClient.GetStream();

            byte[] peerHeaderBuffer = new byte[4];

            ns.Read(peerHeaderBuffer, 0, peerHeaderBuffer.Length);

            uint peerHeaderLength = BitConverter.ToUInt32(peerHeaderBuffer);

            return new PeerHeader
            {
                Length = peerHeaderLength
            };
        }

        public void WritePeerItem(PeerItem peerItem, TcpClient tcpClient)
        {

            NetworkStream ns = tcpClient.GetStream();

            // Write 4 bytes which represnts peerItem lenght into stream

            byte[] peerHeaderBuffer = new byte[4];

            PeerHeader peerHeader = new PeerHeader { Length = (uint)peerItem.PeerToByteArray().Length };

            peerHeaderBuffer = BitConverter.GetBytes(peerHeader.Length);

            ns.Write(peerHeaderBuffer, 0, peerHeaderBuffer.Length);

            // write peerItem into stream with certain amout of bytes

            byte[] itemBuffer;

            itemBuffer = peerItem.PeerToByteArray();

            ns.Write(itemBuffer, 0, peerHeaderBuffer.Length);

        }

        public PeerItem ReadPeerItem(PeerHeader peerHeader, TcpClient tcpClient)
        {
            NetworkStream ns = tcpClient.GetStream();

            byte[] itemBuffer = new byte[BUFFER_SIZE];

            ns.Read(itemBuffer, 0, (int)peerHeader.Length);

            PeerItem item = itemBuffer.ByteArrayToPeer(itemBuffer.Length);

            return item;

        }
    }
}