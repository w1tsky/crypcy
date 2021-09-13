using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using witskyNet;

public class Client
{
    UdpClient clientUdp;
    public string Message { get; set; }
    public string RemoteAdress { get; set; }
    public int RemotePort { get; set; }
    public ClientInfo LocalClientInfo = new ClientInfo();
    public event EventHandler<string> OnResultsUpdate;

    public Client(IPEndPoint serverEndpoint)
    {

        LocalClientInfo.Name = System.Environment.MachineName;
        LocalClientInfo.ConnectionType = ConnectionTypes.Unknown;
        LocalClientInfo.ID = DateTime.Now.Ticks;

        var IPs = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);

        foreach (var IP in IPs)
            LocalClientInfo.InternalAddresses.Add(IP);

        // SendMessage(Message, RemoteAdress, RemotePort);
        SendMessageUDP(serverEndpoint);
    }

    public void SendMessage(string message, IPEndPoint EP)
    {
        clientUdp = new UdpClient();
        try
        {
            while(true)
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                clientUdp.Send(data, data.Length, EP); // отправка
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            clientUdp.Close();
        }
    }


    public void SendMessageUDP(IPEndPoint EP)
    {

        clientUdp = new UdpClient();
        
        byte[] data = JsonSerializer.SerializeToUtf8Bytes(LocalClientInfo);

        try
        {
            if (data != null)
                clientUdp.Send(data, data.Length, EP);
        }
        catch (Exception e)
        {
            if (OnResultsUpdate != null)
                OnResultsUpdate.Invoke(this, "Error on UDP Send: " + e.Message);
        }
    }
}