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

        private readonly Dictionary<Entity, string> entityIds = new Dictionary<Entity, string>();

        public event Action<Entity> EntityCreated;
        public event Action<Entity> EntityDestroyed;

        public IEnumerable<Entity> Entities => entities.Values;

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
            diffEntities.Select(e => e.ID).ForEach(id => destroyableEntities.Add(id));
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
