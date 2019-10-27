using System;
using System.Collections.Generic;

namespace NeonTDS
{
    public class EntityManager
    {
        private HashSet<Entity> creatableEntities = new HashSet<Entity>();
        private Dictionary<Guid, Entity> entities = new Dictionary<Guid, Entity>();
        private HashSet<Guid> destroyableEntities = new HashSet<Guid>();

        private Dictionary<Entity, Guid> entityIds = new Dictionary<Entity, Guid>();

		public List<Player> Players { get; set; }  // a setet kiszervezhetjük egy addpalyer metódusba ha kell/úgy jobb

        public event Action<Entity> EntityCreated;
        public event Action<Entity> EntityDestroyed;

        public IEnumerable<Entity> Entities => entities.Values;

        public Entity Create(Entity entity)
        {
            creatableEntities.Add(entity);
			if (entity is Player player) Players.Add(player);  
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
                EntityCreated?.Invoke(entity);
            }
            creatableEntities.Clear();
            foreach (Entity entity in entities.Values)
            {
                entity.Update(elapsedTimeSeconds);
            }
            foreach (Guid id in destroyableEntities)
            {
                EntityDestroyed?.Invoke(entities[id]);
                entities.Remove(id);
            }
            destroyableEntities.Clear();
        }
    }
}
