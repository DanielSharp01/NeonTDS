using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NeonTDS {
	public class SniperPU : PowerUp {
		public SniperPU(EntityManager entityManager, Shape shape) : base(entityManager, shape) {
			Color = new Vector4(0, 1, 1, 1);
			Position = new Vector2((float)new Random().NextDouble() * EntityManager.GameSize, (float)new Random().NextDouble() * EntityManager.GameSize);
		}


	
	}
	
}
