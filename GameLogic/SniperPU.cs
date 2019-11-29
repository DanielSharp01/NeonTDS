using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NeonTDS {
	public class SniperPU : PowerUp {
        public static readonly Vector4 PUColor = new Vector4(0, 1, 1, 1);
        public SniperPU(EntityManager entityManager, Shape shape) : base(entityManager, shape, PowerUpTypes.RayGun) {
			Color = PUColor;
			
		}
	}
	
}
