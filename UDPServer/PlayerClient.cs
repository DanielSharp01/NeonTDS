using NeonTDS;
using System.Text;

class PlayerClient : UdpBase
{
    public PlayerClient() { }

    Player player;
    string playerEntityID;

    public static PlayerClient ConnectTo(string hostname, int port)
    {
        var connection = new PlayerClient();
        connection.client.Connect(hostname, port);
        return connection;
    }
    
    public void Send(string message)
    {
        var datagram = Encoding.ASCII.GetBytes(message);
        client.Send(datagram, datagram.Length);
    }

}
