using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.System;
using Windows.UI;

namespace NeonTDS
{
    public class Game
    {
        public static Game Instance { get; private set; }
        public static Config Config { get; private set; }
        public Game()
        {
            using (StreamReader reader = new StreamReader("config.json"))
            {
                Config = JsonConvert.DeserializeObject<Config>(reader.ReadToEnd());
            }
            if (Instance == null) Instance = this;
            else throw new Exception("There can't be more than one game!");
        }

        private readonly FPSCounter fpsCounter = new FPSCounter();
        public InputManager InputManager { get; } = new InputManager();
        public Camera Camera { get; } = new Camera();

        readonly EntityManager EntityManager = new EntityManager(false);
        readonly EntityRenderer EntityRenderer = new EntityRenderer();
        Player LocalPlayer;
        public string DebugString { get; set; } = "";

        private readonly BloomRendering bloomRendering = new BloomRendering
        {
            BloomIntensity = 200,
            BloomThreshold = 0,
            BloomBlur = 16
        };

        private GameServer gameServer;
        private readonly Queue gameStateQueue = new Queue();

        public void Load()
        {
            SetupGameConnection();
            EntityManager.EntityCreated += EntityRenderer.CreateDrawable;
            EntityManager.EntityDestroyed += EntityRenderer.DestroyDrawable;
        }

        private void SetupGameConnection()
        {
            gameServer = new GameServer();
            gameServer.ConnectResponseRecieved += response =>
            {
                if (response.Approved)
                {
                    gameServer.GameStateMessageRecieved += gameState =>
                    {
                        lock (gameStateQueue)
                        {
                            gameStateQueue.Enqueue(gameState);
                        }
                    };
                }
            };
            gameServer.SendConnect(Guid.NewGuid().ToString());
        }

		public void CreateResources(CanvasAnimatedControl canvas)
        {
            bloomRendering.CanvasLoaded(canvas);
            bloomRendering.SetupParameters();

            
            Camera.Size = canvas.Size.ToVector2();
            Camera.Center = Vector2.Zero;

            EntityRenderer.SetupSprites(canvas);
        }

        public void Exit()
        {
            gameServer.Disconnect();
        }

        public void SizeChanged(CanvasAnimatedControl canvas, Size newSize, Size previousSize)
        {
            Camera.Size = newSize.ToVector2();
            bloomRendering.Resize(canvas, newSize);
        }

        public void Update(CanvasTimingInformation timing)
        {
            fpsCounter.Update(timing);
            InputManager.Update();
            Matrix3x2.Invert(Camera.Transform, out Matrix3x2 inverse);
            if (LocalPlayer != null) HandlePlayerInput();
            DebugString = LocalPlayer?.Position.ToString();
            lock (gameStateQueue)
            {
                foreach (GameStateMessage gameState in gameStateQueue)
                {
                    HandleGameState(gameState);
                }
                gameStateQueue.Clear();
            }

            EntityManager.Update((float)timing.ElapsedTime.TotalSeconds);
            Camera.Update();
            InputManager.AfterUpdate();
        }

        private void HandlePlayerInput()
        {
            var diff = InputManager.CurrentState.MousePosition - Camera.Size / 2;
            LocalPlayer.TurretDirection = (float)Math.Atan2(diff.Y, diff.X);

            var turnState = TurnState.None;
            if (InputManager.IsKey(VirtualKey.A, PressState.Down))
            {
                turnState = TurnState.Left;
            }
            if (InputManager.IsKey(VirtualKey.D, PressState.Down))
            {
                if (turnState == TurnState.None) turnState = TurnState.Right;
                else turnState = TurnState.None;
            }
            LocalPlayer.TurnState = turnState;

            var speedState = SpeedState.None;
            if (InputManager.IsKey(VirtualKey.W, PressState.Down))
            {
                speedState = SpeedState.SpeedUp;
            }
            if (InputManager.IsKey(VirtualKey.S, PressState.Down))
            {
                if (speedState == SpeedState.None) speedState = SpeedState.SlowDown;
                else speedState = SpeedState.None;
            }
            LocalPlayer.SpeedState = speedState;

            LocalPlayer.Firing = InputManager.IsMouseButton(MouseButton.Left, PressState.Down);

            gameServer.SendInput(new InputMessage { Firing = LocalPlayer.Firing, SpeedState = LocalPlayer.SpeedState, TurnState = LocalPlayer.TurnState, TurretDirection = LocalPlayer.TurretDirection });
        }

        private void HandleGameState(GameStateMessage gameState)
        {
            EntityManager.DiffEntities(gameState.Entities);
            EntityManager.EntityCreated += entity =>
            {
                if (gameState.PlayerEntityID == entity.ID)
                {
                    Camera.FollowedEntity = LocalPlayer = (Player)EntityManager.GetEntityById(gameState.PlayerEntityID);
                }
            };
        }

        public void Draw(CanvasDrawingSession drawingSession, CanvasTimingInformation timing)
        {
            float pulseTime(float period)
            {
                float modTime = (float)(timing.TotalTime.TotalMilliseconds % period);
                if (modTime < period / 2)
                {
                    return modTime / (period / 2);
                }
                else
                {
                    return 1 - (modTime - period / 2) / (period / 2);
                }
            }

            fpsCounter.Draw(timing);
            using (var ds = bloomRendering.CreateDrawingSession())
            {
                ds.Clear(Colors.Black);
                ds.Transform = Camera.Transform;
                for (float x = -EntityManager.GameSize + 100; x < EntityManager.GameSize; x += 100)
                {
                    ds.DrawLine(new Vector2(x, -EntityManager.GameSize), new Vector2(x, EntityManager.GameSize), Color.FromArgb(255, 20, 0, 0), 2);
                }

                for (float y = -EntityManager.GameSize + 100; y < EntityManager.GameSize; y += 100)
                {
                    ds.DrawLine(new Vector2(-EntityManager.GameSize, y), new Vector2(EntityManager.GameSize, y), Color.FromArgb(255, 20, 0, 0), 2);
                }

                using (var spriteBatch = ds.CreateSpriteBatch())
                {
                    EntityRenderer.Draw(spriteBatch, timing);
                }


                float pulse = pulseTime(1000);

                ds.DrawLine(new Vector2(-EntityManager.GameSize, -EntityManager.GameSize), new Vector2(-EntityManager.GameSize, EntityManager.GameSize), Colors.OrangeRed, 3 + pulse * 2);
                ds.DrawLine(new Vector2(EntityManager.GameSize, -EntityManager.GameSize), new Vector2(EntityManager.GameSize, EntityManager.GameSize), Colors.OrangeRed, 3 + pulse * 2);
                ds.DrawLine(new Vector2(-EntityManager.GameSize, EntityManager.GameSize), new Vector2(EntityManager.GameSize, EntityManager.GameSize), Colors.OrangeRed, 3 + pulse * 2);
                ds.DrawLine(new Vector2(-EntityManager.GameSize, -EntityManager.GameSize), new Vector2(EntityManager.GameSize, -EntityManager.GameSize), Colors.OrangeRed, 3 + pulse * 2);
            }

            bloomRendering.DrawResult(drawingSession);
            foreach (var player in EntityManager.Entities.Where(e => e is Player).Cast<Player>())
            {
                var playerColor = Color.FromArgb((byte)(player.Color.W * 255), (byte)(player.Color.X * 255), (byte)(player.Color.Y * 255), (byte)(player.Color.Z * 255));
                var playerPosition = Vector2.Transform(player.Position, Camera.Transform);
                var textLayout = new CanvasTextLayout(drawingSession, player.Name, new CanvasTextFormat { FontSize = 12 }, 256.0f, 32.0f)
                {
                    WordWrapping = CanvasWordWrapping.NoWrap
                };
                var textSize = textLayout.LayoutBounds;
                drawingSession.DrawTextLayout(textLayout, playerPosition - new Vector2((float)(textSize.Width / 2), (float)(textSize.Height / 2 + 64)), playerColor);
				drawingSession.DrawRectangle(new Rect(playerPosition.X - 32, playerPosition.Y - 48, 64f, 8), playerColor);
                if (player.Health > 0) drawingSession.FillRectangle(new Rect(playerPosition.X - 32, playerPosition.Y - 48, 64f * player.Health / 100f, 8), playerColor);
                if (player.Shield > 0) drawingSession.FillRectangle(new Rect(playerPosition.X - 32, playerPosition.Y - 48, 64f * player.Shield / 100f, 8), Colors.Aquamarine);
            }

            drawingSession.DrawText(fpsCounter.FPS.ToString(), Vector2.Zero, Colors.LimeGreen);
            drawingSession.DrawText(DebugString ?? "", new Vector2(0, 32), Colors.DarkRed);
        }
    }
}
