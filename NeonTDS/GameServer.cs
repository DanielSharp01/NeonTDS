using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class GameServer
    {
        private NetworkClient networkClient;

        public List<Message> ReceivedMessages => networkClient.ReceivedMessages;

        public GameServer()
        {
            networkClient = new NetworkClient(new UdpClient(Game.Config.ServerIP, Game.Config.ServerPort));
            networkClient.Listen();
        }
 
        public void ProcessMessages()
        {
            networkClient.ProcessMessages();
        }

        public void QueueSend(Message message)
        {
            networkClient.SendQueue.Enqueue(message);
        }

        public void SendAll()
        {
            networkClient.SendMessages();
        }

        public void SendConnectRequest(string name, byte color)
        {
            networkClient.SendMessage(new ConnectMessage() { Name = name, Color = color });
        }

        public void Disconnect()
        {
            networkClient.SendMessage(new Message(MessageTypes.Disconnect));
        }
    }
}
