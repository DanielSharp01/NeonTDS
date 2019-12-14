using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;

namespace NeonTDS
{
    class Program
    {
        public const int MaxKeepAliveTicks = 100;

        readonly static EntityManager entityManager = new EntityManager(true);
        static NetworkServer networkServer;
        readonly static Dictionary<IPEndPoint, GameClient> gameClients = new Dictionary<IPEndPoint, GameClient>();
        static Config config;
        static string logContext;


        static void Main(string[] args)
        {
            using (StreamReader reader = new StreamReader("config.json"))
            {
                config = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
                networkServer = new NetworkServer(new UdpClient(new IPEndPoint(IPAddress.Any, config.Port)));
            }
            logContext = "main";
            ReceiveThread();
            SetupGameLoop();
            string input;
            while ((input = Console.ReadLine()) != "exit")
            {
                switch (input)
                {
                    case "entities":
                        logContext = "entities";
 
                        using (System.Timers.Timer timer = new System.Timers.Timer(300))
                        {
                            timer.Elapsed += (source, e) =>
                            {
                                Console.Clear();
                                foreach (Entity entity in entityManager.Entities)
                                {
                                    ColorLog("entities", ConsoleColor.Cyan, entity.ID.ToString(), " " + entity.GetType().ToString() + " (Position= " + entity.Position + ", Direction=" + entity.Direction + ", Speed=" + entity.Speed + ")");
                                }
                            };
                            timer.AutoReset = true;
                            timer.Start();
                            Console.ReadKey();
                            timer.Stop();
                        }

                        Console.Clear();
                        logContext = "main";
                        break;
                    case "players":
                        logContext = "players";
                        using (System.Timers.Timer timer = new System.Timers.Timer(300))
                        {
                            timer.Elapsed += (source, e) =>
                            {
                                Console.Clear();
                                foreach (KeyValuePair<IPEndPoint, GameClient> kvp in gameClients)
                                {
                                    ColorLog("players", ConsoleColor.Cyan, kvp.Value.Player.Name, "(Health=" + kvp.Value.Player.Health + ", Shield=" + kvp.Value.Player.Shield + ", PowerUp=" + Enum.GetName(typeof(PowerUpTypes), kvp.Value.Player.ActivePowerUp) +  ") from " + kvp.Key.ToString() + " w/ ping " + kvp.Value.Client.PingMS);
                                }
                            };
                            timer.AutoReset = true;
                            timer.Start();
                            Console.ReadKey();
                            timer.Stop();
                        }

                        Console.Clear();
                        logContext = "main";
                        break;
                }
            }
        }

        static void ReceiveThread()
        {
            networkServer.Listen();
            ColorLog("main", ConsoleColor.Green, "Listening", $" on port { config.Port }");
        }

        static EntityData GetEntityDataFor(Entity entity)
        {
            switch (entity)
            {
                case Player player:
                    return new PlayerData() {
                        Name = player.Name,
                        Color = player.PlayerColor,
                        Direction = player.Direction,
                        Speed = player.Speed,
                        Position = player.Position,
                        TurretDirection = player.TurretDirection,
                        Health = (byte)player.Health,
                        Shield = (byte)player.Shield,
                        ActivePowerUp = player.ActivePowerUp
                    };
                case Bullet bullet:
                    if (bullet.IsSniperBullet) return new RayData()
                    {
                        Direction = bullet.Direction,
                        Position = bullet.Position,
                        PlayerID = bullet.OwnerID
                    };
                    else return new BulletData()
                    {
                        Direction = bullet.Direction,
                        Speed = bullet.Speed,
                        Position = bullet.Position,
                        PlayerID = bullet.OwnerID
                    };
                case PowerUp powerUp:
                    return new PowerUpData()
                    {
                        PowerUpType = powerUp.Type,
                        Position = powerUp.Position
                    };
                case Asteroid asteroid:
                    return new AsteroidData()
                    {
                        ShapeID = asteroid.ShapeID,
                        Direction = asteroid.Direction,
                        Position = asteroid.Position,
                        RotationSpeed = asteroid.RotationSpeed
                    };
            }

            return null;
        }

        static void SetupGameLoop()
        {
            entityManager.EntityCreated += (e) =>
            {
                networkServer.SendQueue.Enqueue(new EntityCreateMessage() { EntityID = e.ID, EntityData = GetEntityDataFor(e), Tick = e.CreationTick.Value });

            };
            entityManager.EntityDestroyed += (e) =>
            {
                networkServer.SendQueue.Enqueue(new EntityDestroyMessage() { EntityID = e.ID, Tick = e.DestructionTick.Value });
            };
            entityManager.PlayerHealthChanged += (p, remainingHealth, remainingShield) =>
            {
                networkServer.SendQueue.Enqueue(new HealthMessage() { PlayerID = p.ID, Health = remainingHealth, Shield = remainingShield });
            };
            entityManager.PlayerActivePowerUpChanged += (p, powerUpType) =>
            {
                networkServer.SendQueue.Enqueue(new PlayerPoweredUpMessage() { PlayerID = p.ID, PowerUpType = powerUpType });
            };
            entityManager.PlayerRespawned += (p) =>
            {
                networkServer.SendQueue.Enqueue(new PlayerRespawnedMessage() { PlayerID = p.ID, Position = p.Position, Direction = p.Direction, Speed = p.Speed, TurretDirection = p.TurretDirection });
            };
            var timer = new System.Timers.Timer(1000.0 / NetworkServer.TickRate);
            timer.Elapsed += (source, e) =>
            {
                List<IPEndPoint> disconnectedPlayers = new List<IPEndPoint>();
                foreach (KeyValuePair<IPEndPoint, Client> kvp in networkServer.Clients)
                {
                    kvp.Value.ProcessMessages();
                    if (kvp.Value.ReceivedMessages.Count == 0) {
                        if (gameClients.ContainsKey(kvp.Key))
                        {
                            gameClients[kvp.Key].KeepAliveTicks++;
                            if (gameClients[kvp.Key].KeepAliveTicks > MaxKeepAliveTicks)
                            {
                                ColorLog("main", ConsoleColor.Red, "Disconnected", " " + gameClients[kvp.Key].Player.Name + " [timed out] from " + kvp.Key.ToString());
                                entityManager.Destroy(gameClients[kvp.Key].Player, true);
                                networkServer.Disconnected(kvp.Key);
                                disconnectedPlayers.Add(kvp.Key);
                            }
                        }
                    }
                    else if (gameClients.ContainsKey(kvp.Key))
                    {
                        gameClients[kvp.Key].KeepAliveTicks = 0;
                    }


                    foreach (Message message in kvp.Value.ReceivedMessages)
                    {
                        if (!gameClients.ContainsKey(kvp.Key) && message is ConnectMessage connectMessage)
                        {
                            bool approved = IsNameAvailable(connectMessage.Name);
                            if (approved)
                            {
                                Player player = (Player)entityManager.Create(new Player(entityManager, connectMessage.Name, connectMessage.Color), true);
                                ColorLog("main", ConsoleColor.Green, "Connected", " " + connectMessage.Name + " from " + kvp.Key.ToString());
                                kvp.Value.SendQueue.Enqueue(new ConnectResponseMessage() { Approved = true, PlayerID = player.ID });
                                gameClients.Add(kvp.Key, new GameClient(kvp.Value, player));

                                foreach (Entity entity in entityManager.Entities)
                                {
                                    if (entity.ID != player.ID)
                                    {
                                        networkServer.SendQueue.Enqueue(new EntityCreateMessage() { EntityID = entity.ID, EntityData = GetEntityDataFor(entity), Tick = entity.CreationTick.Value });
                                    }
                                }

                                // TODO: Asteroid shape messages
                            }
                            else
                            {
                                ColorLog("main", ConsoleColor.Red, "Rejected", " " + connectMessage.Name + " [name taken] from " + kvp.Key.ToString());
                                networkServer.Disconnected(kvp.Key);
                                disconnectedPlayers.Add(kvp.Key);
                            }
                        }
                        if (gameClients.ContainsKey(kvp.Key) && message.Type == MessageTypes.Disconnect)
                        {
                            ColorLog("main", ConsoleColor.Red, "Disconnected", " " + gameClients[kvp.Key].Player.Name + " from " + kvp.Key.ToString());
                            entityManager.Destroy(gameClients[kvp.Key].Player, true);
                            networkServer.Disconnected(kvp.Key);
                            disconnectedPlayers.Add(kvp.Key);
                        }
                    }

                    foreach (IPEndPoint endpoint in disconnectedPlayers)
                    {
                        gameClients.Remove(endpoint);
                    }

                    uint? lastProcessedMessage = null;
                    foreach (Message message in kvp.Value.ReceivedMessages)
                    {
                        if (gameClients.ContainsKey(kvp.Key) && message is PlayerInputMessage inputMessage)
                        {
                            if (lastProcessedMessage == null || lastProcessedMessage < inputMessage.SequenceNumber)
                            {
                                lastProcessedMessage = inputMessage.SequenceNumber;
                            }
                            gameClients[kvp.Key].Player.Firing = inputMessage.Firing;
                            gameClients[kvp.Key].Player.TurnState = inputMessage.TurnState;
                            gameClients[kvp.Key].Player.SpeedState = inputMessage.SpeedState;
                            gameClients[kvp.Key].Player.TurretDirection = inputMessage.TurretDirection;
                        }
                    }

                    if (lastProcessedMessage != null)
                    {
                        gameClients[kvp.Key].Client.SendQueue.Enqueue(new InputAckMessage() { SequenceNumber = lastProcessedMessage.Value });
                    }
                }
                entityManager.Update(1.0f / NetworkServer.TickRate); 
                entityManager.Tick();

                foreach (Entity entity in entityManager.Entities)
                {
                    if (entity is Player player)
                    {
                        networkServer.SendQueue.Enqueue(new PlayerStateMessage() { PlayerID = player.ID, Position = player.Position, Direction = player.Direction, Speed = player.Speed, TurretDirection = player.TurretDirection });
                    }
                }

                networkServer.SendMessages();
            };
            timer.AutoReset = true;
            timer.Start();
        }

        public static void ColorLog(string context, ConsoleColor color, string colored, string normal)
        {
            if (context != logContext) return;
            Console.ForegroundColor = color;
            Console.Write(colored);
            Console.ResetColor();
            Console.WriteLine(normal);
        }

        static bool IsNameAvailable(string name)
        {
            foreach (GameClient gameClient in gameClients.Values)
            {
                if (gameClient.Player.Name == name) return false;
            }

            return true;
        }
    }
}