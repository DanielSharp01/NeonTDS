using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class Client
    {
        public List<byte> ReceiveQueue { get; } = new List<byte>();
        public MessageQueue SendQueue { get; } = new MessageQueue();

        public List<Message> ReceivedMessages { get; private set; } = null;

        public void ProcessMessages()
        {
            MemoryStream stream;
            lock (ReceiveQueue)
            {
                stream = new MemoryStream(ReceiveQueue.ToArray());
                ReceiveQueue.Clear();
            }
            ReceivedMessages = Message.FromPackedBytes(stream);
        }
    }

    public class NetworkServer : IDisposable
    {
        public const uint TickRate = 64;

        private UdpClient client;

        public MessageQueue SendQueue { get; } = new MessageQueue();

        public Dictionary<IPEndPoint, Client> Clients { get; } = new Dictionary<IPEndPoint, Client>();

        public NetworkServer(UdpClient client)
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
                        await ReceiveMessages();
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }

        public void SendMessages()
        {
            foreach (IPEndPoint clientEndpoint in Clients.Keys) {
                List<Message> messages = SendQueue.RetrieveMessages();
                messages.AddRange(Clients[clientEndpoint].SendQueue.RetrieveMessages());
                SendToEndpoint(messages, clientEndpoint);
            }
        }

        private void SendToEndpoint(List<Message> messages, IPEndPoint endpoint)
        {
            MemoryStream stream = new MemoryStream();
            Message.ToPackedBytes(stream, messages);
            var bytes = stream.GetBuffer();
            try
            {
                client.Send(bytes, bytes.Length, endpoint);
            }
            catch (Exception) { }
        }

        private async Task ReceiveMessages()
        {
            var received = await client.ReceiveAsync();
            if (!Clients.ContainsKey(received.RemoteEndPoint))
            {
                Connected(received.RemoteEndPoint, new Client());
            }
            lock (Clients[received.RemoteEndPoint].ReceiveQueue)
            {
                Clients[received.RemoteEndPoint].ReceiveQueue.AddRange(received.Buffer);
            }
        }

        private void Connected(IPEndPoint endpoint, Client client)
        {
            Clients.Add(endpoint, client);
        }

        public void Disconnected(IPEndPoint endpoint)
        {
            Clients.Remove(endpoint);
        }

        public void Dispose()
        {
            client.Close();
            client.Dispose();
        }
    }
}
