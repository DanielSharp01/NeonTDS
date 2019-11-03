using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NeonTDS
{
    class GameServer : UdpBase
    {
        public GameServer()
        {
            client = new UdpClient(new IPEndPoint(IPAddress.Any, 32123));
        }

        public void Reply(string message, IPEndPoint endpoint)
        {
            var datagram = Encoding.ASCII.GetBytes(message);
            client.Send(datagram, datagram.Length, endpoint);
        }
    }
}