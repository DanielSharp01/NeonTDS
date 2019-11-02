using System;
using System.Collections.Generic;
using System.Linq;

namespace NeonTDS
{
    public class EntityManager
    {
        private HashSet<Entity> creatableEntities = new HashSet<Entity>();
        private Dictionary<Guid, Entity> entities = new Dictionary<Guid, Entity>();
        private HashSet<Guid> destroyableEntities = new HashSet<Guid>();

        private Dictionary<Entity, Guid> entityIds = new Dictionary<Entity, Guid>();

        public event Action<Entity> EntityCreated;
        public event Action<Entity> EntityDestroyed;

        public IEnumerable<Entity> Entities => entities.Values;

        public IEnumerable<Entity> GetCollidableEntities<T>(T entity) where T: Entity
        {
            return GetCollidableEntities<T>();
        }

        public IEnumerable<Entity> GetCollidableEntities<T>() where T : Entity
        {
            if (typeof(T) == typeof(Bullet))
            {
                return Entities.Where(e => e is Player);
            }
            else if (typeof(T) == typeof(Player))
            {
                return Entities.Where(e => e is Player);
            }

            return Enumerable.Empty<Entity>();
        }

        public Entity Create(Entity entity)
        {
            creatableEntities.Add(entity);
            return entity;
        }

        public void Destroy(Entity entity)
        {
            destroyableEntities.Add(entityIds[entity]);
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
            foreach (Guid id in destroyableEntities)
            {
                EntityDestroyed?.Invoke(entities[id]);
                entities[id].CleaEvents();
                entities[id].OnDestroy();
                entityIds.Remove(entities[id]);
                entities.Remove(id);
            }
            destroyableEntities.Clear();
        }
    }
}
