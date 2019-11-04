using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NeonTDS {
	public class RapidPU : PowerUp {
		public RapidPU(EntityManager entityManager, Shape shape) : base(entityManager, shape) {
			Color = new Vector4(1, 1, 0, 1);

		}
	}
}
