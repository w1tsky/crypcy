using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using crypcy.shared;

public class Client
{
    UdpClient ClientUDP;
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
        SendMessageUDP(LocalClientInfo.Simplified(), serverEndpoint);
    }

    public void SendMessage(string message, IPEndPoint EP)
    {
        ClientUDP = new UdpClient();
        try
        {
            while(true)
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                ClientUDP.Send(data, data.Length, EP); // отправка
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            ClientUDP.Close();
        }
    }


    public void SendMessageUDP(IPeerItem Item, IPEndPoint EP)
    {
        Item.ID = LocalClientInfo.ID;

        ClientUDP = new UdpClient();

        string jsonStr = JsonSerializer.Serialize<PeerInfo>(LocalClientInfo);
        System.Console.WriteLine(jsonStr);
        byte[] data = PeerConverter.PeerToByteArray(LocalClientInfo);
        string str =JsonSerializer.Serialize<IPeerItem>(PeerConverter.ByteArrayToPeer(data)); 
        System.Console.WriteLine(str);

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