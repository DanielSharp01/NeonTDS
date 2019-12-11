using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NeonTDS
{
	public class Asteroid : Entity
	{
		public byte ShapeID { get; set; }
		public float RotationSpeed { get; set; }
		public float Radius { get; set; }
		public Asteroid(EntityManager entityManager, byte ShapeID) : base(entityManager, Shape.PowerUp)
		{
			Shape = generateShape(ShapeID);
			Color = new Vector4(1, 1, 1, 1);
			Speed = 0;
			
		}

		private Shape generateShape(byte ShapeID)
		{
			int seed = ShapeID / 255;

			Radius = 50 + seed * 300;
			
			List<Vector2> pointlist = new List<Vector2>();
			
			for(float phi = 0; phi < 2*Math.PI; phi+= (float)(seed*Math.PI))
			{
				pointlist.Add(new Vector2(Radius * (float)Math.Cos(phi), Radius * (float)Math.Sin(phi)));
			}
			
   
			return new Shape(pointlist.ToArray(),new Vector2(0,0));
		}

		public override void Update(float elapsedTimeSeconds)
		{
			base.Update(elapsedTimeSeconds);
			Direction += RotationSpeed*elapsedTimeSeconds;
		}

		public override void CollidesWith(Entity other)
		{
			if (!entityManager.ServerSide) return;
			if(other is Player p)
			{
				p.InflictDamage(255);
			}
			if(other is Asteroid) { }
			else entityManager.Destroy(other);
		}
	}
}
