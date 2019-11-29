using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class EntityManager
    {
        public uint Clock { get; private set; } = 0;

        public void Tick()
        {
            Clock++;
        }

        private readonly HashSet<Entity> creatableEntities = new HashSet<Entity>();
        private readonly Dictionary<uint, Entity> entities = new Dictionary<uint, Entity>();
        private readonly HashSet<uint> destroyableEntities = new HashSet<uint>();
        private readonly HashSet<uint> temporaryEntities = new HashSet<uint>();

        public event Action<Entity> EntityCreated;
        public event Action<Entity> EntityDestroyed;
        public event Action<Player> PlayerRespawned;
        public event Action<Player, PowerUpTypes> PlayerActivePowerUpChanged;
        public event Action<Player, byte, byte> PlayerHealthChanged;

        private float PowerUpSpawnTimer = 5;
        public const int SpawnSize = 1000;
        public const int GameSize = 2000;

        public IEnumerable<Entity> Entities => entities.Values;

        public bool ServerSide { get; }

        public EntityManager(bool serverSide)
        {
            ServerSide = serverSide;
        }

        public bool HasEntityWithId(uint id)
        {
            return entities.ContainsKey(id);
        }

        public Entity GetEntityById(uint id)
        {
            return entities[id];
        }

        public IEnumerable<Entity> GetCollidableEntities(Entity entity)
        {
            return GetCollidableEntities(entity.GetType());
        }

        public IEnumerable<Entity> GetCollidableEntities<T>() where T : Entity
        {
            return GetCollidableEntities(typeof(T));
        }

        public IEnumerable<Entity> GetCollidableEntities(Type type)
        {
            if (type == typeof(Bullet))
            {
                return Entities.Where(e => e is Player);
            }
            else if (type == typeof(Player))
            {
                return Entities.Where(e => e is Player || e is PowerUp);
            }
			

			return Enumerable.Empty<Entity>();
        }

        public Entity Create(Entity entity, bool bypassQueue = false)
        {
            if (!ServerSide || !entity.IsRenderOnly)
            {
                if (bypassQueue)
                {
                    entities.Add(entity.ID, entity);
                    entity.OnCreate();
                    EntityCreated?.Invoke(entity);
                }
                else
                {
                    creatableEntities.Add(entity);
                }
            }
            if (!ServerSide && entity.ID >= uint.MaxValue / 2)
            {
                lock (temporaryEntities)
                {
                    temporaryEntities.Add(entity.ID);
                }
            }
            return entity;
        }

        public void Destroy(Entity entity, bool bypassQueue = false)
        {
            DestroyById(entity.ID, bypassQueue);
        }

        public void DestroyById(uint id, bool bypassQueue = false)
        {
            if (entities.ContainsKey(id))
            {
                if (bypassQueue)
                {
                    entities[id].ClearEvents();
                    entities[id].OnDestroy();
                    EntityDestroyed?.Invoke(entities[id]);
                    entities.Remove(id);
                }
                else
                {
                    destroyableEntities.Add(id);
                }
            }
        }

        private void UpdatePowerUps(float elapsedTimeSeconds)
		{
            if (!ServerSide) return;

            PowerUpSpawnTimer -= elapsedTimeSeconds;
            if (PowerUpSpawnTimer <= 0)
            {
                int valaszto = new Random().Next(0, 16);
                if (valaszto <= 8) Create(new ShieldPU(this));
                if (valaszto > 8 && valaszto <= 14) Create(new RapidPU(this));
                if (valaszto > 14) Create(new SniperPU(this));
                PowerUpSpawnTimer = 30;
            }
		}

        public void Update(float elapsedTimeSeconds)
        {
            UpdatePowerUps(elapsedTimeSeconds);

            foreach (Entity entity in creatableEntities)
            {
                entities.Add(entity.ID, entity);
                entity.OnCreate();
                EntityCreated?.Invoke(entity);
            }
            creatableEntities.Clear();
            foreach (Entity entity in entities.Values)
            {
                entity.Update(elapsedTimeSeconds);
                if (CollisionAlgorithms.TestLineIntersect(entity, new Vector2(-GameSize, -GameSize), new Vector2(-GameSize, GameSize))
                    || CollisionAlgorithms.TestLineIntersect(entity, new Vector2(GameSize, -GameSize), new Vector2(GameSize, GameSize))
                    || CollisionAlgorithms.TestLineIntersect(entity, new Vector2(-GameSize, -GameSize), new Vector2(GameSize, -GameSize))
                    || CollisionAlgorithms.TestLineIntersect(entity, new Vector2(-GameSize, GameSize), new Vector2(GameSize, GameSize))
                    || (
                        entity.Position.X < -GameSize ||entity.Position.X > GameSize || entity.Position.Y < -GameSize || entity.Position.Y > GameSize
                    ))
                {
                    if (entity is Player player) {
                        player.InflictDamage(255);  
                    }
                    else
                    {
                        Destroy(entity);
                    }
                }
            }
            foreach (uint id in destroyableEntities)
            {
                entities[id].ClearEvents();
                entities[id].OnDestroy();
                EntityDestroyed?.Invoke(entities[id]);
                entities.Remove(id);
            }
            destroyableEntities.Clear();
        }

        public void InvokePlayerRespawned(Player player)
        {
            PlayerRespawned?.Invoke(player);
        }

        public void InvokePlayerActivePowerUpChanged(Player player, PowerUpTypes powerUpType)
        {
            PlayerActivePowerUpChanged?.Invoke(player, powerUpType);
        }

        public void InvokePlayerHealthChanged(Player player, byte reaminingHealth, byte remainingShield)
        {
            PlayerHealthChanged?.Invoke(player, reaminingHealth, remainingShield);
        }
    }
}
