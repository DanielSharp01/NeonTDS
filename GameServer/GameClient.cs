using NeonTDS;
using System;
using System.Net;
using System.Text;

namespace NeonTDS
{
    public class GameClient
    {
        public DistinguishedNetworkClient Client { get; }
        public Player Player { get; }

        public string Name => Player.Name;

        public GameClient(DistinguishedNetworkClient client, Player player)
        {
            Client = client;
            Player = player;
            client.OnMessageRecieved += message =>
            {
                if (message is InputMessage inputMessage) HandlePlayerInput(inputMessage);
            };
        }

        public void HandlePlayerInput(InputMessage inputMessage)
        {
            Player.TurretDirection = inputMessage.TurretDirection;
            Player.TurnState = inputMessage.TurnState;
            Player.SpeedState = inputMessage.SpeedState;
            Player.Firing = inputMessage.Firing;
        }

        public void SendGameStateMessage(GameStateMessage message)
        {
            Client.SendMessage(message);
        }
    }
}
