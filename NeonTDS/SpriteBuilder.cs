using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using NeonTDS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace NeonTDS
{
    public class SpriteBuilder
    {
        public const int SCALE_FACTOR = 4;
        public CanvasBitmap Bitmap { get; private set; }

        public static CanvasBitmap RenderFromShape(CanvasAnimatedControl canvas, float width, float height, Shape shape, float thickness = 2, bool filled = false)
        {
            var renderTarget = new CanvasRenderTarget(canvas, width * SCALE_FACTOR, height * SCALE_FACTOR);
            var pathBuilder = new CanvasPathBuilder(canvas.Device);
            pathBuilder.BeginFigure(shape.Points.First() * SCALE_FACTOR);
            foreach (Vector2 point in shape.Points.Skip(1))
            {
                pathBuilder.AddLine(point * SCALE_FACTOR);
            }

            pathBuilder.EndFigure(shape.Closed ? CanvasFigureLoop.Closed : CanvasFigureLoop.Open);
            using (var ds = renderTarget.CreateDrawingSession())
            {
                if (filled) ds.FillGeometry(CanvasGeometry.CreatePath(pathBuilder), Colors.White);
                else ds.DrawGeometry(CanvasGeometry.CreatePath(pathBuilder), Colors.White, thickness * SCALE_FACTOR);
            }
            return renderTarget;
        }
    }
}
