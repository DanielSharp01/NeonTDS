using NeonTDS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class NetworkClient: IDisposable
    {
        private static readonly JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        private UdpClient client;

        public NetworkClient(UdpClient client)
        {
            this.client = client;
        }

        public void Listen()
        {
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    try
                    {
                        OnMessageRecieved?.Invoke(await RecieveMessage());
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }

        public void SendMessage(Message message, IPEndPoint remoteEndPoint = null)
        {
            var bytes = Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(message, serializerSettings));
            try
            {
                if (remoteEndPoint != null) client.Send(bytes, bytes.Length, remoteEndPoint);
                else
                    client.Send(bytes, bytes.Length);
            }
            catch (Exception) { }
        }

        public async Task<Message> RecieveMessage()
        {
            var recieved = await client.ReceiveAsync();
            var message = JsonConvert.DeserializeObject<Message>(Encoding.ASCII.GetString(recieved.Buffer, 0, recieved.Buffer.Length), serializerSettings);
            message.RemoteEndPoint = recieved.RemoteEndPoint;
            return message;
        }

        public void Dispose()
        {
            OnMessageRecieved = null;
            client.Close();
            client.Dispose();
        }

        public event Action<Message> OnMessageRecieved;
    }

    public class MultiNetworkClient: NetworkClient
    {
        private readonly Dictionary<IPEndPoint, DistinguishedNetworkClient> clients = new Dictionary<IPEndPoint, DistinguishedNetworkClient>();
        public MultiNetworkClient(UdpClient client)
            : base(client)
        {
            OnMessageRecieved += message =>
            {
                if (clients.TryGetValue(message.RemoteEndPoint, out DistinguishedNetworkClient netClient))
                {
                    netClient.Recieve(message);
                }
            };
        }

        public void AddClient(DistinguishedNetworkClient client)
        {
            clients.Add(client.RemoteEndPoint, client);
        }

        public void RemoveClient(IPEndPoint endPoint)
        {
            if (clients.TryGetValue(endPoint, out DistinguishedNetworkClient netClient)) netClient.Dispose();
            clients.Remove(endPoint);
        }
    }

    public class DistinguishedNetworkClient: IDisposable
    {
        private NetworkClient mainClient;
        public IPEndPoint RemoteEndPoint { get; }

        public long LastMessageTimestasmp { get; private set; }

        public DistinguishedNetworkClient(NetworkClient mainClient, IPEndPoint remoteEndPoint)
        {
            LastMessageTimestasmp = DateTime.Now.Ticks;
            this.mainClient = mainClient;
            RemoteEndPoint = remoteEndPoint;
        }

        public void SendMessage(Message message)
        {
            mainClient.SendMessage(message, RemoteEndPoint);
        }

        public void Recieve(Message message)
        {
            OnMessageRecieved?.Invoke(message);
            LastMessageTimestasmp = DateTime.Now.Ticks;
        }

        public event Action<Message> OnMessageRecieved;

        public void Dispose()
        {
            OnMessageRecieved = null;
        }
    }

}
