using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using witskyNet;

public class Server
{
    public UdpClient server;

    public int LocalPort { get; set; }
    IPEndPoint remoteIp = null;

    public Server(int localPort)
    {
        LocalPort = localPort;
        Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
        receiveThread.Start();
    }

    public void ReceiveMessage()
    {
        UdpClient server = new UdpClient(LocalPort); // UdpClient для получения данных
        try
        {
            while(true)
            {
                byte[] data = server.Receive(ref remoteIp); // получаем данные
                string message = Encoding.Unicode.GetString(data);
                Console.WriteLine("Собеседник: {0}", message);
            }
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            server.Close();
        }
    }
}