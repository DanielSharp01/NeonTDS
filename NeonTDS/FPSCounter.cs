using Microsoft.Graphics.Canvas.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win2DEngine
{
    public class FPSCounter
    {
        private bool firstGuess = true;
        public int FPS { get; private set; } = 0;
        private int frames = 0;
        private double elapsedTime = 0;

        public void Update(CanvasTimingInformation timing)
        {
            elapsedTime += timing.ElapsedTime.TotalSeconds;
            if (elapsedTime > 1)
            {
                elapsedTime -= 1;
                FPS = frames;
                frames = 0;
            }
        }

        public void Draw(CanvasTimingInformation timing)
        {
            frames++;
            // First guess to avoid 0
            if (firstGuess && timing.TotalTime.TotalSeconds > 0)
            {
                firstGuess = false;
                FPS = (int)(1.0 / timing.TotalTime.TotalSeconds);
            }
        }
    }
}
