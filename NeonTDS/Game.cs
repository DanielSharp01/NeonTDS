using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
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
        

        EntityManager EntityManager = new EntityManager();
        EntityRenderer EntityRenderer = new EntityRenderer();
        Player LocalPlayer;
        public Random Random { get; } = new Random();
        public string DebugString { get; set; } = "";

        private readonly BloomRendering bloomRendering = new BloomRendering
        {
            BloomIntensity = 200,
            BloomThreshold = 0,
            BloomBlur = 16
        };

        public void Load()
        {
            EntityManager.EntityCreated += EntityRenderer.CreateDrawable;
            EntityManager.EntityDestroyed += EntityRenderer.DestroyDrawable;
            Camera.FollowedEntity = LocalPlayer = (Player)EntityManager.Create(new Player(EntityManager) { Color = new Vector4(0, 1, 0, 1), MinSpeed = 0, MaxSpeed = 700 } );
            EntityManager.Create(new Player(EntityManager) { Color = new Vector4(1, 0, 0, 1), MinSpeed = 0, Direction = 0.4f, MaxSpeed = 700, Speed = 50, Position = new Vector2(100, 0) });
        }

        public void CreateResources(CanvasAnimatedControl canvas)
        {
            bloomRendering.CanvasLoaded(canvas);
            bloomRendering.SetupParameters();

            
            Camera.Size = canvas.Size.ToVector2();
            Camera.Center = Vector2.Zero;

            EntityRenderer.SetupSprites(canvas);
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
            DebugString = Vector2.Transform(InputManager.MousePosition, inverse).ToString();
            HandlePlayerInput();

            EntityManager.Update((float)timing.ElapsedTime.TotalSeconds);
            Camera.Update();
            InputManager.AfterUpdate();
        }

        public void HandlePlayerInput()
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
