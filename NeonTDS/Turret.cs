using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.System;

namespace Win2DEngine
{
   public class Turret : GameObject
    {
        private float tDirection;

        public float TDirection { get; private set; }

        public Turret()
        {
            Speed = 400;
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
            if (Speed < 100) Speed = 100;
            if (Speed > 700) Speed = 700;

            base.Update(timing);
           
            tDirection = 2*(float)Math.PI*(float)Math.Atan((float)((Game.Instance.InputManager.MousePosition.Y)) / (float)(Game.Instance.InputManager.MousePosition.X));
    }

        public override void Draw(CanvasSpriteBatch sb, CanvasTimingInformation timing)
        {

            Matrix3x2 matrix = Matrix3x2.CreateTranslation(-Sprite.Origin) * Matrix3x2.CreateRotation(tDirection) * Matrix3x2.CreateTranslation(Sprite.Origin) * Matrix3x2.CreateScale(1 / SpriteBuilder.SCALE_FACTOR) * Matrix3x2.CreateTranslation(Position);
            sb.Draw(Sprite.Bitmap, matrix, Color);
        }

    }
}
