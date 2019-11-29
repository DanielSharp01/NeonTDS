using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class NetworkClient : IDisposable
    {
        private UdpClient client;

        public List<byte> ReceiveQueue { get; } = new List<byte>();

        public List<Message> ReceivedMessages { get; private set; } = null;
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
                        await ReceiveMessages();
                    }
                    catch (Exception)
                    {

                    }
                }
            });
        }

        public void SendMessage(Message message)
        {
            MemoryStream stream = new MemoryStream();
            Message.ToPackedBytes(stream, new List<Message>() { message });
            var bytes = stream.GetBuffer();
            try
            {
                client.Send(bytes, bytes.Length);
            }
            catch (Exception) { }
        }

        public void SendMessages()
        {
            MemoryStream stream = new MemoryStream();
            Message.ToPackedBytes(stream, SendQueue.RetrieveMessages());
            var bytes = stream.GetBuffer();
            try
            {
                client.Send(bytes, bytes.Length);
            }
            catch (Exception) { }
        }

        private async Task ReceiveMessages()
        {
            var received = await client.ReceiveAsync();
            lock (ReceiveQueue)
            {
                ReceiveQueue.AddRange(received.Buffer);
            }
        }

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

        public void Dispose()
        {
            client.Close();
            client.Dispose();
        }
    }
}
