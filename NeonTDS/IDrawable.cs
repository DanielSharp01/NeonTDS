using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeonTDS
{
    public interface IDrawable
    {
        void Draw(EntityRenderer renderer, CanvasSpriteBatch sb, CanvasTimingInformation timing);
    }
}
