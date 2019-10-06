using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2DEngine
{
    public class Sprite
    {
        public CanvasBitmap Bitmap { get; private set; }
        public Vector2 Origin { get; set; }

        public Sprite(CanvasBitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }
}
