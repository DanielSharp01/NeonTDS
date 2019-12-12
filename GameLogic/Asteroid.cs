using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace NeonTDS
{
	public class Asteroid : Entity
	{
		public byte ShapeID { get; set; }
		public float RotationSpeed { get; set; }
		public float Radius { get; set; }
		public Asteroid(EntityManager entityManager, byte shapeID) : base(entityManager, Shape.PowerUp)
		{
            ShapeID = shapeID;
            Shape = generateShape(shapeID);
            Speed = 0;
            Direction = (float)(Math.PI * 2 * GameMath.Random.NextDouble());
            RotationSpeed = (float)(Math.PI / 8 * GameMath.Random.NextDouble() - Math.PI / 16);
            Position = FindSpawnPosition();
        }

        public Vector2 FindSpawnPosition()
        {
            Vector2 position;
            bool SpawnPositionIsWrong()
            {
                foreach (Asteroid other in entityManager.Entities.Where(e => e is Asteroid).Cast<Asteroid>())
                {
                    if (other == this) continue;
                    if ((position - other.Position).Length() < other.Radius + Radius)
                    {
                        return true;
                    }
                }

                return false;
            }

            do
            {
                position = new Vector2(GameMath.RandomFloat(-EntityManager.GameSize + Radius, EntityManager.GameSize - Radius),
                    GameMath.RandomFloat(-EntityManager.GameSize + Radius, EntityManager.GameSize - Radius));
            }
            while (SpawnPositionIsWrong());

            return position;
        }

        private Vector2[] generateAsteroidPoints(Random random, int n, float major, float minor)
        {
            double[] randAngles = new double[n];
            for (int i = 0; i < n; i++)
            {
                randAngles[i] = Math.PI * 2 * random.NextDouble();
            }
            Array.Sort(randAngles);

            Vector2[] ret = new Vector2[n];

            for (int i = 0; i < n; i++)
            {
                ret[i] = new Vector2(major + major * (float)Math.Cos(randAngles[i]), minor + minor * (float)Math.Sin(randAngles[i]));
            }
            return ret;
        }

        private Vector2 centerOfMass(Vector2[] points)
        {
            Vector2 sum = Vector2.Zero;
            foreach (Vector2 point in points)
            {
                sum += point;
            }

            return sum / points.Length;
        }

        private Shape generateShape(byte ShapeID)
		{
            Random random = new Random(ShapeID);
            float major = (float)(random.NextDouble() * 64 + 64);
            float minor = major * (0.75f + (float)random.NextDouble() * 0.5f);
            Radius = major;
            Vector2[] points = generateAsteroidPoints(random, (int)(major / 8), major, minor);
            Color = new Vector4((float)random.NextDouble() * 0.5f + 0.5f, (float)random.NextDouble() * 0.5f + 0.5f, (float)random.NextDouble() * 0.5f + 0.5f, 1);
            return new Shape(points, centerOfMass(points));
		}

		public override void Update(float elapsedTimeSeconds)
		{
			base.Update(elapsedTimeSeconds);
            Direction += RotationSpeed*elapsedTimeSeconds;
		}

		public override void CollidesWith(Entity other)
		{
            base.CollidesWith(other);
			if (!entityManager.ServerSide) return;
			if(other is Bullet)
			{
                entityManager.Destroy(other);
			}
		}
	}
}
