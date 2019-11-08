using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class Bullet : Entity
    {
        public Vector2 SpawnPosition { get; set; }
        public string OwnerID { get; set; }

		public bool IsSniperBullet { get; set; }

		public int Damage { get; set; }

        public Bullet(EntityManager entityManager, Player owner):
            base(entityManager, Shape.Bullet)
        {
            OwnerID = owner?.ID;
            IsSniperBullet = false;
        }

        public override void PostSerialize(EntityManager entityManager)
        {
            base.PostSerialize(entityManager);
            Shape = Shape.Bullet;
            CalculateBoundingRadius();
        }

        public override void Update(float elapsedTimeSeconds) {

			base.Update(elapsedTimeSeconds);
            foreach (Entity entity in entityManager.GetCollidableEntities(this))
            {
                if (entity.ID == OwnerID) continue;

                Vector2? hitPosition = CollisionAlgorithms.TestBulletHit(this, entity, elapsedTimeSeconds);

                if (hitPosition != null)
                {
                    entity.CollidesWith(this);
                    CollidesWith(entity);
                    new BulletImpactEffect(hitPosition.Value, Direction, entity.Color).Spawn(entityManager);
					if(!IsSniperBullet) entityManager.Destroy(this);
                }
            }
			//if(IsSniperBullet) entityManager.Destroy(this);
        }
	}
}