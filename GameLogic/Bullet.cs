using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class Bullet : Entity
    {
        public Vector2 SpawnPosition { get; private set; }
        public Player Owner { get; }
        public Vector2 HitPosition { get; private set; }

		public bool IsSniperBullet { get; set; }

		public int Damage { get; set; }

        public Bullet(EntityManager entityManager, Player owner):
            base(entityManager, Shape.Bullet)
        {
            Owner = owner;
            Color = Owner?.Color ?? new Vector4(1, 1, 1, 1);
			IsSniperBullet = false;
			
        }

        public override void Update(float elapsedTimeSeconds) {

			base.Update(elapsedTimeSeconds);
            foreach (Entity entity in entityManager.GetCollidableEntities(this))
            {
                if (entity == Owner) continue;

                Vector2? hitPosition = CollisionAlgorithms.TestBulletHit(this, entity, elapsedTimeSeconds);

                if (hitPosition != null)
                {
                    HitPosition = hitPosition.Value;
                    entity.CollidesWith(this);
                    CollidesWith(entity);
					if(!IsSniperBullet) entityManager.Destroy(this);

				}
            }
			//if(IsSniperBullet) entityManager.Destroy(this);
        }
	}
}