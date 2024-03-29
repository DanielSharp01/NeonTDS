﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;

namespace NeonTDS
{
    public class DrawableEntity: IDrawable
    {
        private readonly Entity entity;
        public DrawableEntity(Entity entity)
        {
            this.entity = entity;
        }

        public void Draw(EntityRenderer renderer, CanvasSpriteBatch sb, CanvasTimingInformation timing)
        {
            sb.Draw(renderer.Sprites[entity.Shape], Matrix3x2.CreateTranslation(-entity.Shape.Origin *
                SpriteBuilder.SCALE_FACTOR) *
                Matrix3x2.CreateRotation(entity.Direction) *
                Matrix3x2.CreateScale(1f / SpriteBuilder.SCALE_FACTOR) *
                Matrix3x2.CreateTranslation(entity.Position), entity.Color);
        }
    }
}
