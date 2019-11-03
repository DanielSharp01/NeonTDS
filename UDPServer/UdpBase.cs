using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

abstract class UdpBase
{
    protected UdpClient client;

    protected UdpBase()
    {
        client = new UdpClient();
    }

    public async Task<Received> Receive()
    {
        var result = await client.ReceiveAsync();
        return new Received()
        {
            Message = Encoding.ASCII.GetString(result.Buffer, 0, result.Buffer.Length),
            Sender = result.RemoteEndPoint
        };
    }
}
