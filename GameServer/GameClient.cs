namespace NeonTDS
{
    public class GameClient
    {
        public Client Client { get; }
        public Player Player { get; }

        public string Name => Player.Name;

        public int KeepAliveTicks { get; set; } = 0;

        public GameClient(Client client, Player player)
        {
            Client = client;
            Player = player;
        }
    }
}
