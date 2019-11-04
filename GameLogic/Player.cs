using Newtonsoft.Json;
using System;
using System.Numerics;

namespace NeonTDS
{
    public class Player : Entity
    {
        public string Name { get; set; }
        public int Health { get; set; }
        [JsonIgnore]
        public float MinSpeed { get; } = 100;
        [JsonIgnore]
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

        // TODO: Add back
		/*private bool hasSniper = false;
		private bool hasRapid = false;
		private bool hasShield = false;*/

		// private float rapidTimer = 5;
		// private float DamageModifier = 1;

        public WeaponType WeaponType { get; set; }
        public TurnState TurnState { get; set; }
        public SpeedState SpeedState { get; set; }

        public float TurretDirection { get; set; }

        [JsonIgnore]
        public Matrix3x2 TurretTransformation => Matrix3x2.CreateTranslation(-Shape.Origin) *
            Matrix3x2.CreateRotation(TurretDirection) *
            Matrix3x2.CreateTranslation(Position);

        public Player(EntityManager entityManager, string name)
            : base(entityManager, Shape.Player)
        {
            FireRate = 150;
            Name = name;
        }

        public override void PostSerialize(EntityManager entityManager)
        {
            base.PostSerialize(entityManager);
            Shape = Shape.Player;
            CalculateBoundingRadius();
        }

        public override void UpdateEntity(Entity data)
        {
            base.UpdateEntity(data);
            Player playerData = (Player)data;
            Health = playerData.Health;
            Shield = playerData.Shield;
            FireRate = playerData.FireRate;
            Firing = playerData.Firing;
            FireTimer = playerData.FireTimer;
            WeaponType = playerData.WeaponType;
            TurnState = playerData.TurnState;
            SpeedState = playerData.SpeedState;
            TurretDirection = playerData.TurretDirection;
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
                entityManager.Create(new Bullet(entityManager, this) { Position = Position, Speed = 2000, Direction = TurretDirection, Color = Color });

                // TODO: Add back
				/*if (hasSniper) {
					entityManager.Create(new Bullet(entityManager, this) { Position = Position, Speed = 4000, Direction = TurretDirection, Damage = 200, IsSniperBullet = true, Color = new Vector4(0, 1, 1, 1) });
					hasSniper = false;
					Color = new Vector4(0, 1, 0, 1);
				}
				else if (hasRapid) entityManager.Create(new Bullet(entityManager, this) { Position = Position, Speed = 2000, Direction = TurretDirection, Damage = 5, Color = new Vector4(1, 1, 0, 1) });
				else entityManager.Create(new Bullet(entityManager, this) { Position = Position, Speed = 2000, Direction = TurretDirection, Damage = 5 });*/
            }

            foreach (Entity entity in entityManager.GetCollidableEntities(this))
            {
                if (entity != this && CollisionAlgorithms.TestClosedShapes(this, entity))
                {
                    entity.CollidesWith(this);
                    CollidesWith(entity);
                }
            }
			
            // TODO: Add back
			/*if (hasRapid) {
				rapidTimer -= elapsedTimeSeconds;
				if (rapidTimer <= 0) {
					hasRapid = false;
					rapidTimer = 5;
					Color = new Vector4(0, 1, 0, 1);
					fireRate = 150;
				}
			}*/
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
            if (other is Bullet b)
            {
                DamagePlayer(b.Damage);
            }
            else if (other is Player)
            {
                Health = 0;
                Shield = 0;
                entityManager.Destroy(this);
            }
			else if (other is SniperPU) {
				hasRapid = false;
				fireRate = 150;
				rapidTimer = 5;
				Color = new Vector4(0, 1, 0.5f, 1);
				hasSniper = true;
				
			}

			else if (other is RapidPU) {
				hasSniper = false;
				Color = new Vector4(1,0.5f,0,1);
				hasRapid = true;
				fireRate = 300;

			}
			else if (other is ShieldPU) {
				Shield = 100;
				Color = new Vector4(0, 0, 1, 1);
			}
		}

		public override void OnDestroy() {
			//entityManager.Create(new Player(entityManager) { Color = new Vector4(1, 0, 0, 1), MinSpeed = 0, Direction = 0, MaxSpeed = 700, Speed = 50, Position = new Vector2(100, 0), Health = 100 });

		}
	}
}