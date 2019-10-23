namespace NeonTDS
{
    public class Bullet : Entity
    {
        public Player Owner { get; }
        public Bullet(EntityManager entityManager, Player owner):
            base(entityManager, Shape.Bullet)
        {
            Owner = owner;
        }
    }
}