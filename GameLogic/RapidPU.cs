﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NeonTDS {
	public class RapidPU : PowerUp {
        public static readonly Vector4 PUColor = new Vector4(1, 1, 0, 1);
        public RapidPU(EntityManager entityManager, Shape shape) : base(entityManager, shape, PowerUpTypes.RapidFire) {
			Color = PUColor;
		}
	}
}
