using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI;
using Windows.System;

namespace Win2DEngine
{
    public class Player : GameObject
    {
        public Player()
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
        }
    }
}
