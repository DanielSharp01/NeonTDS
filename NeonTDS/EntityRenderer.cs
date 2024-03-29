﻿using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;

namespace NeonTDS
{
    public class EntityRenderer
    {
        private readonly Dictionary<Shape, CanvasBitmap> sprites = new Dictionary<Shape, CanvasBitmap>();
        public IReadOnlyDictionary<Shape, CanvasBitmap> Sprites => sprites;

        public void SetupSprites(CanvasAnimatedControl canvas)
        {
            sprites.Add(Shape.Player, SpriteBuilder.RenderFromShape(canvas, 48, 32, Shape.Player));
            sprites.Add(Shape.Turret, SpriteBuilder.RenderFromShape(canvas, 48, 32, Shape.Turret));
            sprites.Add(Shape.Bullet, SpriteBuilder.RenderFromShape(canvas, 24, 4, Shape.Bullet));
			sprites.Add(Shape.PowerUp, SpriteBuilder.RenderFromShape(canvas, 48, 32, Shape.PowerUp));
			sprites.Add(Shape.SniperBullet, SpriteBuilder.RenderFromShape(canvas, 1000, 4, Shape.SniperBullet));

		}

		public void AddDynamicSprite(CanvasAnimatedControl canvas, Shape shape,float bitmapSize)
		{
			sprites.Add(shape, SpriteBuilder.RenderFromShape(canvas, bitmapSize * 2, bitmapSize * 2, shape));
		}

        private readonly Dictionary<uint, IDrawable> drawables = new Dictionary<uint, IDrawable>();

        public void CreateDrawable(Entity entity)
        {
            if (entity is Player player)
            {
                drawables.Add(entity.ID, new DrawablePlayer(player));
            }
            else
            {
                drawables.Add(entity.ID, new DrawableEntity(entity));
            }
        }

        public void DestroyDrawable(Entity entity)
        {
            drawables.Remove(entity.ID);
        }

        public void Draw(CanvasSpriteBatch sb, CanvasTimingInformation timing)
        {
            foreach (IDrawable drawable in drawables.Values)
            {
                drawable.Draw(this, sb, timing);
            }
        }
    }
}
