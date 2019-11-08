using System;
using System.Collections.Generic;
using System.Linq;

namespace NeonTDS
{
    public class EntityManager
    {
        private readonly HashSet<Entity> creatableEntities = new HashSet<Entity>();
        private readonly Dictionary<string, Entity> entities = new Dictionary<string, Entity>();
        private readonly HashSet<string> destroyableEntities = new HashSet<string>();
        private readonly HashSet<string> temporaryEntities = new HashSet<string>();

        private readonly Dictionary<Entity, string> entityIds = new Dictionary<Entity, string>();

        public event Action<Entity> EntityCreated;
        public event Action<Entity> EntityDestroyed;

        public IEnumerable<Entity> Entities => entities.Values;

        private readonly bool serverSide;

        public EntityManager(bool serverSide)
        {
            this.serverSide = serverSide;
        }

        public Entity GetEntityById(string id)
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

        public Entity Create(Entity entity)
        {
            if (!serverSide || !entity.IsRenderOnly) creatableEntities.Add(entity);
            if (!serverSide && !entity.IsRenderOnly)
            {
                lock (temporaryEntities)
                {
                    temporaryEntities.Add(entity.ID);
                }
            }
            return entity;
        }

        public void Destroy(Entity entity)
        {
            if (entityIds.ContainsKey(entity)) destroyableEntities.Add(entityIds[entity]);
        }

        public void Update(float elapsedTimeSeconds)
        {
            foreach (Entity entity in creatableEntities)
            {
                entities.Add(entity.ID, entity);
                entityIds.Add(entity, entity.ID);
                EntityCreated?.Invoke(entity);
                entity.OnCreate();
            }
            creatableEntities.Clear();
            foreach (Entity entity in entities.Values)
            {
                entity.Update(elapsedTimeSeconds);
            }
            foreach (string id in destroyableEntities)
            {
                EntityDestroyed?.Invoke(entities[id]);
                entities[id].ClearEvents();
                entities[id].OnDestroy();
                entityIds.Remove(entities[id]);
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
                    entity.PostSerialize(this);
                    creatableEntities.Add(entity);
                }
                else
                {
                    entities[entity.ID].UpdateEntity(entity);
                }
            }
        }
    }
}
