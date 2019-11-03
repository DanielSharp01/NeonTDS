using System.Net;
using System.Net.Sockets;
using System.Text;

class GameServer : UdpBase
{
    private IPEndPoint listenOn;

    public GameServer() : this(new IPEndPoint(IPAddress.Any, 32123))
    {
    }

    public GameServer(IPEndPoint endpoint)
    {
        listenOn = endpoint;
        client = new UdpClient(listenOn);
    }

    public void Reply(string message, IPEndPoint endpoint)
    {
        var datagram = Encoding.ASCII.GetBytes(message);
        client.Send(datagram, datagram.Length, endpoint);
    }

}
