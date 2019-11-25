using NeonTDS;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
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

        public MessageQueue RecieveQueue { get; } = new MessageQueue();
        public MessageQueue SendQueue { get; } = new MessageQueue();

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
                        await RecieveMessage();
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }

        public void SendMessages(IPEndPoint remoteEndPoint = null)
        {
            MemoryStream stream = new MemoryStream();
            MessageUtils.StreamFromMessageQueue(stream, SendQueue);
            var bytes = stream.GetBuffer();
            try
            {
                if (remoteEndPoint != null) client.Send(, bytes.Length, remoteEndPoint);
                else
                    client.Send(bytes, bytes.Length);
            }
            catch (Exception) { }
        }

        public async Task RecieveMessage()
        {
            var recieved = await client.ReceiveAsync();
            MemoryStream stream = new MemoryStream(recieved.Buffer);
            MessageUtils.StreamToMessageQueue(stream, RecieveQueue);
        }

        public void Dispose()
        {
            client.Close();
            client.Dispose();
        }
    }

    public class DistinguishedNetworkClient
    {
        private NetworkClient mainClient;
        public IPEndPoint RemoteEndPoint { get; }

        public DistinguishedNetworkClient(NetworkClient mainClient, IPEndPoint remoteEndPoint)
        {
            this.mainClient = mainClient;
            RemoteEndPoint = remoteEndPoint;
        }

        public void SendMessages()
        {
            mainClient.SendMessages(RemoteEndPoint);
        }
    }

}
