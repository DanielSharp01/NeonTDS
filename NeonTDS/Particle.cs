using System.Numerics;

namespace NeonTDS
{
    public class Particle : Entity
    {
        public float Deceleration { get; set; }
        public Particle(EntityManager entityManager)
            : base(entityManager, Shape.Bullet)
        {
        }

        public override void Update(float elapsedTimeSeconds)
        {
            base.Update(elapsedTimeSeconds);
            Color = new Vector4(Color.X, Color.Y, Color.Z, Speed / 1500f);
            Speed -= Deceleration;
        }
    }
}
