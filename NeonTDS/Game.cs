using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Windows.Foundation;
using Windows.UI;

namespace Win2DEngine
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
        public void Load()
        {
            // Only data can go here, intialize sprites in CreateResources
        }

        private List<GameObject> gameObjects = new List<GameObject>();
        private Player player;
        private Turret turret;


        private readonly BloomRendering bloomRendering = new BloomRendering
        {
            BloomIntensity = 200,
            BloomThreshold = 0,
            BloomBlur = 16
        };

        public void CreateResources(CanvasAnimatedControl canvas)
        {
            bloomRendering.CanvasLoaded(canvas);
            bloomRendering.SetupParameters();

            Random random = new Random();
            Camera.Size = canvas.Size.ToVector2();
            Camera.Center = Vector2.Zero;

            SpriteBuilder spriteBuilder = new SpriteBuilder(canvas, 48, 32, new Vector2((float)(Math.Sqrt(3) / 6 * 48), 16));
            Sprite sprite = spriteBuilder.AddPoints(new Vector2(0, 0), new Vector2(0, 32), new Vector2((float)(Math.Sqrt(3) / 2 * 48), 16)).BuildPath(true);

            SpriteBuilder turretSpriteBuilder = new SpriteBuilder(canvas, 48, 32, new Vector2((float)(Math.Sqrt(3) / 6 * 48), 16));
            Sprite turretSprite = turretSpriteBuilder.AddPoints(new Vector2(10, 8), new Vector2(10, 24), new Vector2(40, 18), new Vector2(40, 14)).BuildPath(true); //milan

            SpriteBuilder bulletSpriteBuilder = new SpriteBuilder(canvas, 48, 32, new Vector2((float)(Math.Sqrt(3) / 6 * 48), 16));
            Sprite bulletSprite = bulletSpriteBuilder.AddPoints(new Vector2(0, 0), new Vector2(4, 0)).BuildPath(true); //milan

            Vector2[] generateAsteroidPoints(int n, float major, float minor)
            {
                double[] randAngles = new double[n];
                for (int i = 0; i < n; i++)
                {
                    randAngles[i] = Math.PI * 2 * random.NextDouble();
                }
                Array.Sort(randAngles);

                Vector2[] ret = new Vector2[n];

                for (int i = 0; i < n; i++)
                {
                    ret[i] = new Vector2(major + major * (float)Math.Cos(randAngles[i]), minor + minor * (float)Math.Sin(randAngles[i]));
                }
                return ret;
            }

            Vector2 centerOfMass(Vector2[] points)
            {
                Vector2 sum = Vector2.Zero;
                foreach (Vector2 point in points)
                {
                    sum += point;
                }

                return sum / points.Length;
            }

            for (int i = 0; i < 100; i++)
            {
                float major = (float)(random.NextDouble() * 64 + 64);
                float minor = major * (0.75f + (float)random.NextDouble() * 0.5f);
                GameObject obj = new GameObject();
                Vector2[] points = generateAsteroidPoints((int)(major / 8), major, minor);
                spriteBuilder = new SpriteBuilder(canvas, 2*major, 2*minor, centerOfMass(points));
                obj.Sprite = spriteBuilder.AddPoints(points).BuildPath(true);
                obj.Position = new Vector2((float)random.NextDouble() * 1920, (float)random.NextDouble() * 1080);
                obj.Direction = (float)(random.NextDouble() * Math.PI * 2);
                obj.Speed = (float)random.NextDouble() * 50;
                obj.Color = new Vector4((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1);
                gameObjects.Add(obj);
            }

            player = new Player();
            player.Sprite = sprite;
            player.Color = new Vector4(1, 0, 0, 1);
            player.Direction = 0;
            gameObjects.Add(player);

            turret = new Turret();
            turret.Sprite = turretSprite;
            turret.Color = new Vector4(0, 1, 0, 1);
            turret.Direction = 0;
            gameObjects.Add(turret);

            Bullet bullet = new Bullet();
            bullet.Sprite = bulletSprite;
            bullet.Color = new Vector4(1, 0, 1, 1);
            bullet.Direction = (float)Math.PI/2 ;
            gameObjects.Add(bullet);

            Bullet bullet2 = new Bullet();
            bullet2.Sprite = bulletSprite;
            bullet2.Color = new Vector4(1, 0, 1, 1);
            bullet2.Direction = 0;
            gameObjects.Add(bullet2);
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
            foreach (GameObject obj in gameObjects) obj.Update(timing);
            Camera.Update(player);
            InputManager.AfterUpdate();
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
                    foreach (GameObject obj in gameObjects) obj.Draw(spriteBatch, timing);
                }
            }

            bloomRendering.DrawResult(drawingSession);
            drawingSession.DrawText(fpsCounter.FPS.ToString(), Vector2.Zero, Colors.LimeGreen);
        }

       
    }
}
