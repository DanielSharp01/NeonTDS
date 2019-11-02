using System.Numerics;

namespace NeonTDS
{
    public class PlayerExplosionEffect : IParticleEffect
    {
        private readonly Vector2 position;
        private readonly Vector4 color;

        public PlayerExplosionEffect(Vector2 position, Vector4 color)
        {
            this.position = position;
            this.color = color;
        }

        public void Spawn(EntityManager entityManager)
        {
            for (int i = 0; i < GameMath.RandomInt(16, 32); i++)
            {
                entityManager.Create(new Particle(entityManager)
                {
                    Color = color,
                    Position = position,
                    Speed = GameMath.RandomFloat(1000, 1500),
                    Deceleration = GameMath.RandomFloat(20, 100),
                    Direction = GameMath.RandomFloat(0, GameMath.PI * 2)
                });
            }
        }
    }
}
