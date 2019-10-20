using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Windows.System;

namespace Win2DEngine
{
    public class Player : GameObject
    {
        public Turret Turret { get; set; }

        public int MinSpeed { get; set; } = 100;
        public int MaxSpeed { get; set; } = 700;

        public Sprite TurretSprite { get; set; }
        public float TurretDirection { get; set; }

        private float fireTimer = 0;
        private int fireRate = 250;
        public int FireRate
        {
            get { return fireRate; }
            set
            {
                if (value < 500) fireRate = value;
            }
        }


        public float Damage { get; set; }

        public int Shield { get; set; }

        public Player()
        {
            Speed = 400;
            Health = 100;
            Shield = 0;
            Sprite = Game.Instance.PlayerSprite;
            TurretSprite = Game.Instance.TurretSprite;
            fireTimer = 60f / FireRate;
        }

        public override void Update(CanvasTimingInformation timing)
        {
            if (Game.Instance.InputManager.IsKey(VirtualKey.A, PressState.Down))
            {
                Direction -= (float)(Math.PI * timing.ElapsedTime.TotalSeconds);
            }
            if (Game.Instance.InputManager.IsKey(VirtualKey.D, PressState.Down))
            {
                Direction += (float)(Math.PI * timing.ElapsedTime.TotalSeconds);
            }
            

            if (Game.Instance.InputManager.IsKey(VirtualKey.W, PressState.Down))
            {
                Speed += 300 * (float)timing.ElapsedTime.TotalSeconds;
            }
            if (Game.Instance.InputManager.IsKey(VirtualKey.S, PressState.Down))
            {
                Speed -= 300 * (float)timing.ElapsedTime.TotalSeconds;
            }

            // Clamp values
            if (Speed < MinSpeed) Speed = MinSpeed;
            if (Speed > MaxSpeed) Speed = MaxSpeed;
            base.Update(timing);

            var diff = Game.Instance.InputManager.CurrentState.MousePosition - Game.Instance.Camera.Size / 2;
            TurretDirection = (float)Math.Atan2(diff.Y, diff.X);
            fireTimer += (float)timing.ElapsedTime.TotalSeconds;
            Game.Instance.DebugString = $"{fireTimer}s";

            if (fireTimer >= (60f / FireRate) && Game.Instance.InputManager.IsMouseButton(MouseButton.Left, PressState.Down))
            {
                Game.Instance.CreateObject(new Bullet() { Direction = TurretDirection, Position = Position, Speed = 2000, Color = Color });
                fireTimer = 0;
            }
        }

        public override void Draw(CanvasSpriteBatch sb, CanvasTimingInformation timing)
        {
            Matrix3x2 matrix = Matrix3x2.CreateTranslation(-Sprite.Origin) * Matrix3x2.CreateRotation(Direction) * Matrix3x2.CreateScale(1 / SpriteBuilder.SCALE_FACTOR) * Matrix3x2.CreateTranslation(Position);
            sb.Draw(Sprite.Bitmap, matrix, Color);
            Matrix3x2 turretMatrix = Matrix3x2.CreateTranslation(-TurretSprite.Origin) * Matrix3x2.CreateRotation(TurretDirection) * Matrix3x2.CreateScale(1 / SpriteBuilder.SCALE_FACTOR) * Matrix3x2.CreateTranslation(Position);
            sb.Draw(TurretSprite.Bitmap, turretMatrix, Color);
        }
    }
}
