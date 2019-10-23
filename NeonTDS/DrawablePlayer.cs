using System.Numerics;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;

namespace NeonTDS
{
    public class DrawablePlayer : IDrawable
    {
        private readonly Player player;
        public DrawablePlayer(Player player)
        {
            this.player = player;
        }

        public void Draw(EntityRenderer renderer, CanvasSpriteBatch sb, CanvasTimingInformation timing)
        {
            sb.Draw(renderer.Sprites[player.Shape], Matrix3x2.CreateTranslation(-player.Shape.Origin *
                SpriteBuilder.SCALE_FACTOR) *
                Matrix3x2.CreateRotation(player.Direction) *
                Matrix3x2.CreateScale(1f / SpriteBuilder.SCALE_FACTOR) *
                Matrix3x2.CreateTranslation(player.Position), player.Color);
            sb.Draw(renderer.Sprites[Shape.Turret], Matrix3x2.CreateTranslation(-player.Shape.Origin *
                SpriteBuilder.SCALE_FACTOR) *
                Matrix3x2.CreateRotation(player.TurretDirection) *
                Matrix3x2.CreateScale(1f / SpriteBuilder.SCALE_FACTOR) *
                Matrix3x2.CreateTranslation(player.Position), player.Color);
        }
    }
}
