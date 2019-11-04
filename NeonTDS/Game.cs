using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections;
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

        public Game()
        {
            if (Instance == null) Instance = this;
            else throw new Exception("There can't be more than one game!");
        }

        private readonly FPSCounter fpsCounter = new FPSCounter();
        public InputManager InputManager { get; } = new InputManager();
        public Camera Camera { get; } = new Camera();

        readonly EntityManager EntityManager = new EntityManager();
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
        private readonly object gameStateQueueLock = new object();
        private Queue gameStateQueue = new Queue();

        public void Load()
        {
            SetupGameConnection();
            EntityManager.EntityCreated += EntityRenderer.CreateDrawable;
            EntityManager.EntityCreated += entity =>
            {
                if (entity is Player)
                {
                    entity.OnCollisionWith += other =>
                    {
                        if (other is Bullet bullet) new BulletImpactEffect(bullet.HitPosition, other.Direction, entity.Color).Spawn(EntityManager);
                    };
                }
            };

            EntityManager.EntityDestroyed += EntityRenderer.DestroyDrawable;
            EntityManager.EntityDestroyed += entity =>
            {
                if (entity is Player) new PlayerExplosionEffect(entity.Position, entity.Color).Spawn(EntityManager);
            };
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
                        lock (gameStateQueueLock)
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

            lock (gameStateQueueLock)
            {
                foreach (GameStateMessage gameState in gameStateQueue)
                {
                    HandleGameState(gameState);
                }
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
                if (LocalPlayer == null && gameState.PlayerEntityID == entity.ID)
                {
                    Camera.FollowedEntity = LocalPlayer = (Player)EntityManager.GetEntityById(gameState.PlayerEntityID);
                }
            };
        }

        public void Draw(CanvasDrawingSession drawingSession, CanvasTimingInformation timing)
        {
            fpsCounter.Draw(timing);
            using (var ds = bloomRendering.CreateDrawingSession())
            {
                ds.Clear(Colors.Black);
                ds.Transform = Camera.Transform;
                using (var spriteBatch = ds.CreateSpriteBatch())
                {
                    EntityRenderer.Draw(spriteBatch, timing);
                }
            }

            bloomRendering.DrawResult(drawingSession);
            drawingSession.DrawText(fpsCounter.FPS.ToString(), Vector2.Zero, Colors.LimeGreen);
            drawingSession.DrawText(DebugString, new Vector2(0, 32), Colors.DarkRed);
        }
    }
}
