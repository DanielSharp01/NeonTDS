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
        [JsonIgnore]
        public Vector2 HitPosition { get; private set; }

        public Bullet(EntityManager entityManager, Player owner):
            base(entityManager, Shape.Bullet)
        {
            OwnerID = owner?.ID;
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
                    HitPosition = hitPosition.Value;
                    entity.CollidesWith(this);
                    CollidesWith(entity);
                    entityManager.Destroy(this);
                }
            }
        }
	}
}