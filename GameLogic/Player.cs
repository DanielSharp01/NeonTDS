using System;
using System.Numerics;

namespace NeonTDS
{
    public class Player : Entity
    {
        public int Health { get; set; }
        public float MinSpeed { get; set; }
        public float MaxSpeed { get; set; }
        public int Shield { get; set; }

        private float fireTimer;
        private int fireRate;
        public int FireRate {
            get => fireRate;
            set
            {
                fireRate = value;
                fireTimer = 60.0f / fireRate;
            }
        }
        public bool Firing { get; set; }

        public WeaponType WeaponType { get; set; }
        public TurnState TurnState { get; set; }
        public SpeedState SpeedState { get; set; }

        public float TurretDirection { get; set; }
        

        public Matrix3x2 TurretTransformation => Matrix3x2.CreateTranslation(-Shape.Origin) *
            Matrix3x2.CreateRotation(TurretDirection) *
            Matrix3x2.CreateTranslation(Position);

        public Player(EntityManager entityManager)
            : base(entityManager, Shape.Player)
        {
            FireRate = 150;
        }
		
        public override void Update(float elapsedTimeSeconds)
        {
            if (TurnState == TurnState.Left)
            {
                Direction -= (float)(Math.PI * elapsedTimeSeconds);
            }
            else if(TurnState == TurnState.Right)
            {
                Direction += (float)(Math.PI * elapsedTimeSeconds);
            }


            if (SpeedState == SpeedState.SpeedUp)
            {
                Speed += 300 * elapsedTimeSeconds;
            }
            else if (SpeedState == SpeedState.SlowDown)
            {
                Speed -= 300 * elapsedTimeSeconds;
            }

            if (Speed < MinSpeed) Speed = MinSpeed;
            if (Speed > MaxSpeed) Speed = MaxSpeed;
            base.Update(elapsedTimeSeconds);

            fireTimer += elapsedTimeSeconds;
            if (fireTimer >= (60f / FireRate) && Firing)
            {
                fireTimer = 0;
                entityManager.Create(new Bullet(entityManager, this) { Position = Position, Speed = 2000, Direction = TurretDirection });
            }

            foreach (Entity entity in entityManager.GetCollidableEntities(this))
            {
                if (entity != this && CollisionAlgorithms.TestClosedShapes(this, entity))
                {
                    entity.CollidesWith(this);
                    CollidesWith(entity);
                }
            }
        }

        public void DamagePlayer(int damage)
        {
            int shieldDamage = Math.Min(damage, Shield);
            Shield -= shieldDamage;
            Health -= damage - shieldDamage;
            if (Health < 0) entityManager.Destroy(this);
        }

        public override void CollidesWith(Entity other)
        {
            base.CollidesWith(other);
            if (other is Bullet)
            {
                DamagePlayer(5);
            }
            else if (other is Player)
            {
                Health = 0;
                Shield = 0;
                entityManager.Destroy(this);
            }
        }
    }
}