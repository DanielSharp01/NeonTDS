using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NeonTDS {
	public class ShieldPU : PowerUp {
		public ShieldPU(EntityManager entityManager, Shape shape) : base(entityManager, shape, PowerUpTypes.Shield) {
			Color = new Vector4(0, 0, 1, 1);
		}
	}
}
