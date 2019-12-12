using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class Bullet : Entity
    {
        public const int Damage = 10;
        public Vector2 SpawnPosition { get; set; }
        public uint OwnerID { get; set; }

		public bool IsSniperBullet { get; set; }

        public Bullet(EntityManager entityManager, uint ownerID):
            base(entityManager, Shape.Bullet)
        {
            OwnerID = ownerID;
            IsSniperBullet = false
                ;
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