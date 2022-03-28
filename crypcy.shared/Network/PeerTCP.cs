using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using crypcy.shared;

public class PeerTCP : IDisposable
{

    const int BUFFER_SIZE = 4096;
    private readonly TcpListener TCP;
    private IPEndPoint EndPoint;
    private bool ReuseEndPoint;
    private bool isListening;
    private CancellationTokenSource _tokenSource;
    private CancellationToken _token;

    public event EventHandler<PeerItemEventArgs> OnPeerItemReceived;

    public PeerTCP(IPEndPoint tcpEndpoint, bool reuseEndPoint)
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

    public class PeerItemEventArgs : EventArgs
    {
        public TcpClient PeerTcpClient { get; private set; }

        public PeerItemEventArgs(TcpClient tcpClient)
        {
            System.Console.WriteLine("Client added");
            PeerTcpClient = tcpClient;
        }

        public PeerItem HadleTcpClient()
        {
            using NetworkStream ns = PeerTcpClient.GetStream();

            PeerHeader peerHeader = ReadMessageHeader();

            PeerItem item = ReadPeerItem(peerHeader);

            return item;
        }

        public PeerHeader ReadMessageHeader()
        {

            NetworkStream ns = PeerTcpClient.GetStream();

            byte[] peerHeaderBuffer = new byte[4];

            ns.Read(peerHeaderBuffer, 0, peerHeaderBuffer.Length);

            uint peerHeaderLength = BitConverter.ToUInt32(peerHeaderBuffer);

            return new PeerHeader
            {
                Length = peerHeaderLength
            };
        }

        public void WritePeerItem(PeerItem peerItem)
        {

            NetworkStream ns = PeerTcpClient.GetStream();

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

        public PeerItem ReadPeerItem(PeerHeader peerHeader)
        {
            NetworkStream ns = PeerTcpClient.GetStream();

            byte[] itemBuffer = new byte[BUFFER_SIZE];

            ns.Read(itemBuffer, 0, (int)peerHeader.Length);

            PeerItem item = itemBuffer.ByteArrayToPeer(itemBuffer.Length);

            return item;

        }
    }
    public bool Listening => isListening;

    public async Task StartListenAsync(CancellationToken? token = null)
    {
        _tokenSource = CancellationTokenSource.CreateLinkedTokenSource(token ?? new CancellationToken());
        _token = _tokenSource.Token;
        TCP.Start();
        isListening = true;

        try
        {
            while (!_token.IsCancellationRequested)
            {
                await Task.Run(async () =>
                {
                    var newPeerTask = TCP.AcceptTcpClientAsync();
                    var newPeerClient = await newPeerTask;
                    OnPeerItemReceived?.Invoke(this, new PeerItemEventArgs(newPeerClient));
                }, _token);
            }
        }
        finally
        {
            TCP.Stop();
            isListening = false;
        }
    }

    public void Stop()
    {
        _tokenSource?.Cancel();
    }

    public void Dispose()
    {
        Stop();
    }
}