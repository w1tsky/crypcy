using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using crypcy.shared;

public class Client
{
    UdpClient ClientUDP= new UdpClient();
    public string Message { get; set; }
    public string RemoteAdress { get; set; }
    public int RemotePort { get; set; }
    public PeerInfo LocalClientInfo = new PeerInfo();
    public event EventHandler<string> OnResultsUpdate;

    public Client(IPEndPoint serverEndpoint)
    {

        // ClientUDP.AllowNatTraversal(true);
        // ClientUDP.Client.SetIPProtectionLevel(IPProtectionLevel.Unrestricted);
        // ClientUDP.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        LocalClientInfo.Name = System.Environment.MachineName;
        LocalClientInfo.ConnectionType = ConnectionTypes.Unknown;
        LocalClientInfo.ID = DateTime.Now.Ticks;

        var IPs = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);

        foreach (var IP in IPs)
            LocalClientInfo.InternalAddresses.Add(IP);

        // SendMessage(Message, RemoteAdress, RemotePort);
        SendMessageUDP( serverEndpoint);
        
    }

    public void SendMessageUDP(IPEndPoint EP)
    {
        
        // tem.ID = LocalClientInfo.ID;
        byte[] data = JsonSerializer.SerializeToUtf8Bytes<PeerInfo>(LocalClientInfo);

        string jsonStr = Encoding.UTF8.GetString(data);

        System.Console.WriteLine(jsonStr);
        // JsonSerializer.Deserialize<PeerInfo>(jsonStr);


        try
        {
            if (data != null)
                ClientUDP.Send(data, data.Length, EP);
        }
        catch (Exception e)
        {
            if (OnResultsUpdate != null)
                OnResultsUpdate.Invoke(this, "Error on UDP Send: " + e.Message);
        }
    }
}