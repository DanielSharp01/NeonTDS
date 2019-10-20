using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using System;
using System.Numerics;

namespace Win2DEngine
{
    public class GameObject
    {
        public Vector4 Color { get; set; }
        public Sprite Sprite { get; set; }
        public Vector2 Position { get; set; }

        public float Speed { get; set; }
        public int Health { get; set; }

        public float Direction { get; set; }

        public virtual void Update(CanvasTimingInformation timing)
        {
            Position += new Vector2((float)Math.Cos(Direction), (float)Math.Sin(Direction)) * Speed * (float)timing.ElapsedTime.TotalSeconds;
        }

        public virtual void Draw(CanvasSpriteBatch sb, CanvasTimingInformation timing)
        {
            Matrix3x2 matrix = Matrix3x2.CreateTranslation(-Sprite.Origin) * Matrix3x2.CreateRotation(Direction) * Matrix3x2.CreateScale(1 / SpriteBuilder.SCALE_FACTOR) * Matrix3x2.CreateTranslation(Position);
            sb.Draw(Sprite.Bitmap, matrix, Color);
        }
    }
}
