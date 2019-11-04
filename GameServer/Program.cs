using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Threading;
using System.Timers;

namespace NeonTDS
{
    class Program
    {
        readonly static object entityManagerLock = new object();
        readonly static object gameClientsLock = new object();
        readonly static EntityManager entityManager = new EntityManager();
        readonly static MultiNetworkClient networkClient = new MultiNetworkClient(new UdpClient(new IPEndPoint(IPAddress.Any, 32131)));
        readonly static Dictionary<IPEndPoint, GameClient> gameClients = new Dictionary<IPEndPoint, GameClient>();

        static void Main(string[] args)
        {
            ConnectThread();
            SetupGameLoop();
            while (Console.ReadLine() != "exit")
            { }
        }

        static void ConnectThread()
        {
            networkClient.OnMessageRecieved += message =>
            {
                if (message is ConnectMessage connectRequest)
                {
                    var distinguishedClient = new DistinguishedNetworkClient(networkClient, message.RemoteEndPoint);
                    if (gameClients.Values.Select(client => client.Name).Any(s => connectRequest.Name == s))
                    {
                        ColorLog(ConsoleColor.Red, "Rejected", " " + connectRequest.Name + " [name taken]");
                        distinguishedClient.SendMessage(new ConnectResponseMessage { Approved = false });
                        return;
                    }
                    
                    networkClient.AddClient(distinguishedClient);

                    ColorLog(ConsoleColor.Green, "Connected", " " + connectRequest.Name);
                    var newPlayer = new Player(entityManager, connectRequest.Name) { Color = new Vector4(1, 1, 1, 1) };
                    lock (entityManagerLock)
                    {
                        entityManager.Create(newPlayer);
                    }
                    lock (gameClientsLock)
                    {
                        gameClients.Add(message.RemoteEndPoint, new GameClient(distinguishedClient, newPlayer));
                    }
                    distinguishedClient.SendMessage(new ConnectResponseMessage { Approved = true, PlayerEntityID = newPlayer.ID });
                }
                else if (message is DisconnectMessage)
                {
                    if (!gameClients.ContainsKey(message.RemoteEndPoint)) return;

                    ColorLog(ConsoleColor.Red, "Disconnected", " " + gameClients[message.RemoteEndPoint].Name);
                    lock (gameClientsLock)
                    {
                        if (gameClients.ContainsKey(message.RemoteEndPoint))
                        {
                            lock (entityManagerLock)
                            {
                                entityManager.Destroy(gameClients[message.RemoteEndPoint].Player);
                            }
                        }
                        gameClients.Remove(message.RemoteEndPoint);
                    }
                    networkClient.RemoveClient(message.RemoteEndPoint);
                }
            };
            networkClient.Listen();
            ColorLog(ConsoleColor.Green, "Listening", " on port 32131");
        }

        static void SetupGameLoop()
        {
            var timer = new System.Timers.Timer(33);
            timer.Elapsed += (source, e) =>
            {
                lock (entityManagerLock)
                {
                    entityManager.Update(0.033f);
                    long checkTimestamp = DateTime.Now.Ticks;

                    lock (gameClientsLock)
                    {
                        foreach (GameClient client in gameClients.Values.ToList())
                        {
                            if (new TimeSpan(checkTimestamp - client.Client.LastMessageTimestasmp).TotalSeconds > 5)
                            {
                                ColorLog(ConsoleColor.Red, "Disconnected", " " + gameClients[client.Client.RemoteEndPoint].Name + " [timed out]");
                                entityManager.Destroy(client.Player);
                                gameClients.Remove(client.Client.RemoteEndPoint);
                                networkClient.RemoveClient(client.Client.RemoteEndPoint);
                                
                            }
                            else
                            {
                                client.SendGameStateMessage(new GameStateMessage() { PlayerEntityID = client.Player.ID, Entities = entityManager.Entities.ToArray() });
                            }
                        }
                    }
                }
            };
            timer.AutoReset = true;
            timer.Start();
        }

        static void ColorLog(ConsoleColor color, string colored, string normal)
        {
            Console.ForegroundColor = color;
            Console.Write(colored);
            Console.ResetColor();
            Console.WriteLine(normal);
        }
    }
}