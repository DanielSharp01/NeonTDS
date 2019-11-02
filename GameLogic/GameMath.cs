using System;
using System.Numerics;

namespace NeonTDS
{
    public class GameMath
    {
        public static Random Random { get; } = new Random();

        public const float PI = (float)Math.PI;

        public static float RandomFloat(float min = 0, float max = 1)
        {
            return (float)(min + Random.NextDouble() * (max - min));
        }

        public static Vector4 RandomColor(float? alpha = null)
        {
            return new Vector4(RandomFloat(0, 1), RandomFloat(0, 1), RandomFloat(0, 1), alpha ?? RandomFloat(0, 1));
        }

        public static float RandomInt(int min, int max)
        {
            return Random.Next(min, max + 1);
        }

        public static Vector2 Vector2FromAngle(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static float PerpDot(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }

        public static float ToDegrees(float radians)
        {
            return (float)(radians / Math.PI * 180);
        }

        public static float ToRadians(float degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }
    }
}
