using System;
using System.Collections.Generic;
using System.Text;

namespace NeonTDS {
	public class PowerUp : Entity {
		public PowerUp(EntityManager entityManager, Shape shape) : base(entityManager, shape) {
		}

        public override void PostSerialize(EntityManager entityManager)
        {
            base.PostSerialize(entityManager);
            Shape = Shape.PowerUp;
            CalculateBoundingRadius();
        }

        public override void CollidesWith(Entity other) {
			base.CollidesWith(other);
			if (other is Player ) {
				entityManager.Destroy(this);
			}
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
