using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class Shape
    {
        public static readonly Shape Player = new Shape(new[] {
            new Vector2(0, 0),
            new Vector2(0, 32),
            new Vector2((float)(Math.Sqrt(3) / 2 * 48), 16)
        }, new Vector2((float)(Math.Sqrt(3) / 6 * 48), 16));

        public static readonly Shape Bullet = new Shape(new[] {
            new Vector2(0, 2),
            new Vector2(24, 2)
        }, new Vector2(0, 2), false);

        public static readonly Shape Turret = new Shape(new[] {
            new Vector2(10, 8),
            new Vector2(10, 24),
            new Vector2(40, 18),
            new Vector2(40, 14)
        }, new Vector2((float)(Math.Sqrt(3) / 6 * 48), 16));

        private readonly List<Vector2> points = new List<Vector2>();
        public Vector2 Origin { get; }
        public float Radius { get; }
        public bool Closed { get; }

        public IReadOnlyCollection<Vector2> Points => points;

        public Shape(IEnumerable<Vector2> points, Vector2 origin, bool closed = true)
        {
            this.points.AddRange(points);
            Radius = (float)Math.Sqrt(points.Select(p => (p - origin).LengthSquared()).Min());
            Origin = origin;
            Closed = closed;
        }

        public static string ToString(Shape shape)
        {
            if (shape == Shape.Player) return "Player";
            else if (shape == Shape.Bullet) return "Bullet";
            else if (shape == Shape.Turret) return "Turret";
            else return "unknown";
        }

        public static Shape FromString(string name)
        {
            switch (name)
            {
                case "Player":
                    return Shape.Player;
                case "Bullet":
                    return Shape.Bullet;
                case "Turret":
                    return Shape.Turret;
                default:
                    return null;
            }
        }
    }
}
