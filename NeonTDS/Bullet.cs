using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
		public override void Update(float elapsedTimeSeconds) {
			
			foreach(Player player in entityManager.Players) {
				
				if (collidingWithPlayer(player)) {
					player.hitByBullet(this);
					return;
				}
			}
			base.Update(elapsedTimeSeconds);
		}
		public bool collidingWithPlayer(Player player) {

			Vector2 A = player.Shape.Points.ElementAt(0) + player.Position;
			Vector2 B = player.Shape.Points.ElementAt(1) + player.Position;
			Vector2 C = player.Shape.Points.ElementAt(2) + player.Position;

			float playerArea = AreaOfTriangle(A, B, C);

			foreach(Vector2 point in Shape.Points) {
				Vector2 P = point + Position;
				if (playerArea >= AreaOfTriangle(A, B, P) + AreaOfTriangle(P, B, C) + AreaOfTriangle(A, P, C)) return true;
			}
			return false;

			
		}

		public float AreaOfTriangle(Vector2 A, Vector2 B, Vector2 C) {
			float a = (A - B).Length();
			float b = (A - C).Length();
			float c = (C - B).Length();

			float s = (a + b + c) / 2;
			return (float)System.Math.Sqrt(s * (s - a) * (s - b) * (s - c)); //Heron képlet
		}
	}
}