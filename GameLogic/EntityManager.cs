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
            if (!ServerSide && !entity.IsRenderOnly)
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
            if (entities.ContainsKey(entity.ID))
            {
                if (bypassQueue)
                {
                    entities[entity.ID].ClearEvents();
                    entities[entity.ID].OnDestroy();
                    EntityDestroyed?.Invoke(entities[entity.ID]);
                    entities.Remove(entity.ID);
                }
                else
                {
                    destroyableEntities.Add(entity.ID);
                }
                
            }
        }
		private void UpdatePowerUps(float elapsedTimeSeconds)
		{
            if (!ServerSide) return;

            PowerUpSpawnTimer -= elapsedTimeSeconds;
            if (PowerUpSpawnTimer <= 0)
            {
                int valaszto = new Random().Next(1, 4);
                if (valaszto == 1) Create(new ShieldPU(this, Shape.PowerUp));
                if (valaszto == 2) Create(new RapidPU(this, Shape.PowerUp));
                if (valaszto == 3) Create(new SniperPU(this, Shape.PowerUp));
                PowerUpSpawnTimer = 5;
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
                    || CollisionAlgorithms.TestLineIntersect(entity, new Vector2(-GameSize, GameSize), new Vector2(GameSize, GameSize)))
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

        public void DiffEntities(Entity[] diffEntities)
        {
            lock (temporaryEntities)
            {
                creatableEntities.RemoveWhere(e => temporaryEntities.Contains(e.ID));
                temporaryEntities.Clear();
            }
            entities.Values.ForEach(e =>
            {
                if (!e.IsRenderOnly) destroyableEntities.Add(e.ID);
            });
            foreach (Entity entity in diffEntities)
            {
                destroyableEntities.Remove(entity.ID);
                if (!entities.ContainsKey(entity.ID))
                {
                    creatableEntities.Add(entity);
                }
                else
                {
                    entities[entity.ID].UpdateEntity(entity);
                }
            }
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
            PlayerHealthChanged?.Invoke(player, remainingShield, remainingShield);
        }
    }
}
