using System.Net.Sockets;
using System.Threading;

public class PeerClientTCP
{

    private TcpClient PeerTCP = new TcpClient();

    private bool _TCPListen = false;

    private Thread ThreadTCPListen;

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
                            string jsonStr = Encoding.UTF8.GetString(ReceivedBytes);
                            if (OnResultsUpdate != null)
                                OnResultsUpdate.Invoke(this, "TCP received: " + jsonStr);

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


}