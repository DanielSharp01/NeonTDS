using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class Bullet : Entity
    {
        public Vector2 SpawnPosition { get; private set; }
        public bool Harmless = false;
        public Player Owner { get; }

        public Bullet(EntityManager entityManager, Player owner):
            base(entityManager, Shape.Bullet)
        {
            Owner = owner;
        }

        public override void OnCreate()
        {
            SpawnPosition = Position;
        }

        public override void Update(float elapsedTimeSeconds) {

			base.Update(elapsedTimeSeconds);
            if (!Harmless)
            {
                foreach (Entity entity in entityManager.GetCollidableEntities(this))
                {
                    if (entity == Owner) continue;

                    Vector2? hitPosition = CollisionAlgorithms.TestBulletHit(this, entity);

                    if (hitPosition != null)
                    {
                        entity.CollidesWith(this);
                        entityManager.Destroy(this);

                        for (int i = 0; i < 16 + Game.Instance.Random.Next(16); i++)
                        {
                            entityManager.Create(new Bullet(entityManager, (Player)entity) { Harmless = true, Position = hitPosition.Value, Speed = 1000 + 500 * (float)Game.Instance.Random.NextDouble(), Direction = (Game.Instance.Random.NextDouble() > 0.25 ? 0 : 1) * (float)Math.PI + Direction - (float)Math.PI / 3 * ((float)Game.Instance.Random.NextDouble() - 0.5f) });  ;
                        }
                        return;
                    }
                }
            }

            if (Harmless)
            {
                Speed -= 50;
                if (Speed < 0)
                {
                    entityManager.Destroy(this);
                }
            }
        }
	}
}