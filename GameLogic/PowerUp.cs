using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace NeonTDS {
    public class PowerUp : Entity {

        public PowerUpTypes Type { get; }

		public PowerUp(EntityManager entityManager, Shape shape, PowerUpTypes type) : base(entityManager, shape) {
            Type = type;
            Position = new Vector2((float)(new Random().NextDouble() - 0.5) * EntityManager.GameSize * 2, (float)(new Random().NextDouble() - 0.5) * EntityManager.GameSize * 2);
        }

        public override void CollidesWith(Entity other) {
			base.CollidesWith(other);
			
		}

		public override void OnCreate() {
			base.OnCreate();
		}

		public override void OnDestroy() {
			base.OnDestroy();
		}

		public override void Update(float elapsedTimeSeconds) {
			base.Update(elapsedTimeSeconds);
			
		}
	}
}
