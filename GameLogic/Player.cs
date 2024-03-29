﻿using Newtonsoft.Json;
using System;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class Player : Entity
    {
        public const int DefaultFireRate = 200;
        public const int RapidFireRate = 400;
        public bool LocalPlayer { get; set; }
        public byte PlayerColor { get; set; }
        public string Name { get; set; }
        public int Health { get; set; } = 100; 
        public float MinSpeed { get; } = 100;
        public float MaxSpeed { get; } = 700;
        public int Shield { get; set; }

        public float FireTimer { get; set; }
        private int fireRate;

        public int FireRate {
            get => fireRate;
            set
            {
                if (fireRate != value)
                {
                    fireRate = value;
                    FireTimer = 60.0f / fireRate;
                }
            }
        }

        public bool Firing { get; set; }

        private PowerUpTypes activePowerUp = PowerUpTypes.None;
		public PowerUpTypes ActivePowerUp
        {
            get => activePowerUp;
            set {
                activePowerUp = value;
                switch (activePowerUp)
                {
                    case PowerUpTypes.None:
                        Color = new Vector4(1, 1, 1, 1); // TODO: Use byte based player color
                        fireRate = DefaultFireRate;
                        rapidTimer = 5;
                        break;
                    case PowerUpTypes.RapidFire:
                        Color = RapidPU.PUColor;
                        fireRate = RapidFireRate;
                        rapidTimer = 5;
                        break;
                    case PowerUpTypes.RayGun:
                        Color = SniperPU.PUColor;
                        fireRate = DefaultFireRate;
                        rapidTimer = 5;
                        break;
                }
                if (entityManager.ServerSide) entityManager.InvokePlayerActivePowerUpChanged(this, activePowerUp);
            }
        }

		private float rapidTimer = 5;

        public WeaponType WeaponType { get; set; }
        public TurnState TurnState { get; set; }
        public SpeedState SpeedState { get; set; }

        public float TurretDirection { get; set; }

        [JsonIgnore]
        public Matrix3x2 TurretTransformation => Matrix3x2.CreateTranslation(-Shape.Origin) *
            Matrix3x2.CreateRotation(TurretDirection) *
            Matrix3x2.CreateTranslation(Position);

        public Player(EntityManager entityManager, string name, byte playerColor)
            : base(entityManager, Shape.Player)
        {
            FireRate = DefaultFireRate;
            Name = name;
            PlayerColor = playerColor;
            if (entityManager.ServerSide) Position = FindSpawnPosition();
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

            FireTimer += elapsedTimeSeconds;
            if (FireTimer >= (60f / FireRate) && Firing)
            {
                FireTimer = 0;

                if (ActivePowerUp == PowerUpTypes.RayGun)
                {
                    if (entityManager.ServerSide)
                    {
                        entityManager.Create(new Bullet(entityManager, ID) { Position = Position, Speed = 4000, Direction = TurretDirection, IsSniperBullet = true, Shape = Shape.SniperBullet, Color = new Vector4(0, 1, 1, 1) });
                    }
                    ActivePowerUp = PowerUpTypes.None;
                }
                else if (entityManager.ServerSide)
                {
                    entityManager.Create(new Bullet(entityManager, ID) { Position = Position, Speed = 4000, Direction = TurretDirection, Color = Color });
                }
			}

            foreach (Entity entity in entityManager.GetCollidableEntities(this))
            {
                if (entity != this && CollisionAlgorithms.TestClosedShapes(this, entity))
                {
                    entity.CollidesWith(this);
                    CollidesWith(entity);
                }
            }
			
            
			if (ActivePowerUp == PowerUpTypes.RapidFire) {
				rapidTimer -= elapsedTimeSeconds;
				if (rapidTimer <= 0) {
                    ActivePowerUp = PowerUpTypes.None;
					rapidTimer = 5;
					fireRate = 200;
				}
			}
        }

        public void InflictDamage(int damage)
        {
            if (entityManager.ServerSide)
            {
                int shieldDamage = Math.Min(damage, Shield);
                Shield -= shieldDamage;
                Health -= damage - shieldDamage;
                entityManager.InvokePlayerHealthChanged(this, (byte)Health, (byte)Shield);
                if (Health <= 0)
                {
                    Respawn();
                }
            }
        }

        public void ChangeHealth(int health, int shield)
        {
            if (!entityManager.ServerSide)
            {
                Shield = shield;
                Health = health;

                if (Health <= 0)
                {
                    new PlayerExplosionEffect(Position, Color).Spawn(entityManager);
                }
            }
        }

        public override void CollidesWith(Entity other)
        {
            base.CollidesWith(other);
            if (!entityManager.ServerSide) return;
           
            if (other is Bullet b)
            {
                if (b.IsSniperBullet) InflictDamage(255);
                else InflictDamage(Bullet.Damage);
            }
            else if (other is Player || other is Asteroid)
            {
                InflictDamage(255);
            }
			else if (other is PowerUp powerUp) {
                if (powerUp.Type == PowerUpTypes.Shield)
                {
                    Shield = 100;
                    entityManager.InvokePlayerHealthChanged(this, (byte)Health, (byte)Shield);
                }
                else
                {
                    ActivePowerUp = powerUp.Type;
                }
                entityManager.Destroy(powerUp);

            }
		}

        public Vector2 FindSpawnPosition()
        {
            Vector2 position;
            bool SpawnPositionIsWrong()
            {
                foreach (Entity other in entityManager.Entities.Where(e => e is Player || e is Asteroid))
                {
                    if (other == this) continue;
                    if ((position - other.Position).Length() < 100)
                    {
                        return true;
                    }
                }

                return false;
            }

            do
            {
                position = new Vector2(GameMath.RandomFloat(-EntityManager.SpawnSize, EntityManager.SpawnSize),
                    GameMath.RandomFloat(-EntityManager.SpawnSize, EntityManager.SpawnSize));
            }
            while (SpawnPositionIsWrong());

            return position;
        }

		public void Respawn() {
            if (entityManager.ServerSide)
            {
                Position = FindSpawnPosition();
                Health = 100;
                Speed = MinSpeed;
                Shield = 0;
                ActivePowerUp = PowerUpTypes.None;
                entityManager.InvokePlayerRespawned(this);
            }
		}
	}
}