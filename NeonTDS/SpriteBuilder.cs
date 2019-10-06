using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;

namespace Win2DEngine
{
    public class SpriteBuilder
    {
        public const float SCALE_FACTOR = 4;
        private CanvasRenderTarget renderTarget;
        private CanvasPathBuilder pathBuilder;
        private bool begun = false;
        private Vector2 origin;

        public SpriteBuilder(CanvasAnimatedControl canvas, float width, float height)
        {
            pathBuilder = new CanvasPathBuilder(canvas.Device);
            renderTarget = new CanvasRenderTarget(canvas, width * SCALE_FACTOR, height * SCALE_FACTOR);
            origin = Vector2.Zero;
        }

        public SpriteBuilder(CanvasAnimatedControl canvas, float width, float height, Vector2 origin)
        {
            pathBuilder = new CanvasPathBuilder(canvas.Device);
            renderTarget = new CanvasRenderTarget(canvas, width * SCALE_FACTOR, height * SCALE_FACTOR);
            this.origin = origin * SCALE_FACTOR;
        }

        public SpriteBuilder AddPoint(Vector2 point)
        {
            if (!begun)
            {
                pathBuilder.BeginFigure(point * SCALE_FACTOR);
                begun = true;
            }
            else pathBuilder.AddLine(point * SCALE_FACTOR);

            return this;
        }

        public SpriteBuilder AddPoints(params Vector2[] points)
        {
            foreach (Vector2 point in points)
            {
                AddPoint(point);
            }

            return this;
        }

        public Sprite BuildPath(bool closed = true)
        {
            pathBuilder.EndFigure(closed ? CanvasFigureLoop.Closed : CanvasFigureLoop.Open);
            using (var ds = renderTarget.CreateDrawingSession())
            {
                ds.DrawGeometry(CanvasGeometry.CreatePath(pathBuilder), Colors.White, 2 * SCALE_FACTOR);
            }
            return new Sprite(renderTarget) { Origin = origin };
        }
    }
}
