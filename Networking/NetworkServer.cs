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
        private double? pingRequestTimestamp = null;
        public double PingMS { get; private set; }
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

        public void SendingPing()
        {
            if (pingRequestTimestamp.HasValue) return;
            pingRequestTimestamp = DateTime.Now.TimeOfDay.TotalMilliseconds;
        }

        public void ReceivedPong()
        {
            if (pingRequestTimestamp == null) return;
            PingMS = DateTime.Now.TimeOfDay.TotalMilliseconds - pingRequestTimestamp.Value;
            pingRequestTimestamp = null;
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
            List<Message> sendQueue = SendQueue.RetrieveMessages();
            foreach (IPEndPoint clientEndpoint in Clients.Keys) {
                List<Message> messages = new List<Message>();
                messages.AddRange(Clients[clientEndpoint].SendQueue.RetrieveMessages());
                messages.AddRange(sendQueue);
                SendToEndpoint(messages, clientEndpoint);
            }
        }

        public void SendToEndpoint(List<Message> messages, IPEndPoint endpoint)
        {
            MemoryStream stream = new MemoryStream();
            Message.ToPackedBytes(stream, messages);
            var bytes = stream.ToArray();
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
            else if (received.Buffer.Length > 0 && received.Buffer[0] == (byte)MessageTypes.Ping)
            {
                Clients[received.RemoteEndPoint].ReceivedPong();
                Clients[received.RemoteEndPoint].SendingPing();
                SendToEndpoint(new List<Message>() { new Message(MessageTypes.Ping) }, received.RemoteEndPoint);
            }

            lock (Clients[received.RemoteEndPoint].ReceiveQueue)
            {
                Clients[received.RemoteEndPoint].ReceiveQueue.AddRange(received.Buffer);
            }
        }

        private void Connected(IPEndPoint endpoint, Client client)
        {
            Clients.Add(endpoint, client);
            client.SendingPing();
            SendToEndpoint(new List<Message>() { new Message(MessageTypes.Ping) }, endpoint);
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
