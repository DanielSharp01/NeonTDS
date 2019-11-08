using System.Numerics;

namespace NeonTDS
{
    public class BulletImpactEffect : IParticleEffect
    {
        private readonly Vector2 position;
        private readonly float direction;
        private readonly Vector4 color;

        public BulletImpactEffect(Vector2 position, float direction, Vector4 color)
        {
            this.position = position;
            this.direction = direction;
            this.color = color;
        }

        public void Spawn(EntityManager entityManager)
        {
            for (int i = 0; i < GameMath.RandomInt(8, 16); i++)
            {
                entityManager.Create(new Particle(entityManager) {
                    Color = color,
                    Position = position,
                    Speed = GameMath.RandomFloat(1000, 1500),
                    Deceleration = GameMath.RandomFloat(20, 100),
                    Direction = (GameMath.RandomFloat() > 0.25 ? 0 : 1) * GameMath.PI + direction - GameMath.PI / 6 * GameMath.RandomFloat(-1, 1) });
            }
        }
    }
}
