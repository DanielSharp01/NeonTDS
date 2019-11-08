using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class GameServer
    {
        private NetworkClient networkClient;

        public GameServer()
        {
            networkClient = new NetworkClient(new UdpClient(Game.Config.ServerIP, Game.Config.ServerPort));
            networkClient.OnMessageRecieved += message =>
            {
                if (message is GameStateMessage gameStateMsg) GameStateMessageRecieved?.Invoke(gameStateMsg);
                else if (message is ConnectResponseMessage connectResponse) ConnectResponseRecieved?.Invoke(connectResponse);
            };
            networkClient.Listen();
        }
 
        public void SendConnect(string name)
        {
            networkClient.SendMessage(new ConnectMessage { Name = name });
        }

        public void SendInput(InputMessage message)
        {
            networkClient.SendMessage(message);
        }

        public event Action<GameStateMessage> GameStateMessageRecieved;
        public event Action<ConnectResponseMessage> ConnectResponseRecieved;

        public void Disconnect()
        {
            networkClient.SendMessage(new DisconnectMessage());
        }
    }
}
