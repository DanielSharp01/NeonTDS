using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace NeonTDS
{
    public class CollisionAlgorithms
    {
        public static float? LineSegmentsIntersect(Vector2 s1, Vector2 e1, Vector2 s2, Vector2 e2)
        {
            var v1 = (e1 - s1);
            var v2 = (e2 - s2);

            var t1 = GameMath.PerpDot(s2 - s1, v2) / GameMath.PerpDot(v1, v2);
            var t2 = GameMath.PerpDot(s2 - s1, v1) / GameMath.PerpDot(v1, v2);

            if (t1 < 0 || t1 > 1 || t2 < 0 || t2 > 1) return null;
            return t1;
        }

        public static float? RayLineSegmentIntersects(Vector2 rayPos, Vector2 dir, Vector2 s2, Vector2 e2)
        {
            var v2 = (e2 - s2);

            var t1 = GameMath.PerpDot(s2 - rayPos, v2) / GameMath.PerpDot(dir, v2);
            var t2 = GameMath.PerpDot(s2 - rayPos, dir) / GameMath.PerpDot(dir, v2);

            if (t1 < 0 || t2 < 0 || t2 > 1) return null;
            return t1;
        }

        public static Vector2? TestBulletHit(Bullet bullet, Entity entity, float elapsedTimeSeconds)
        {
            float? minIntersect = null;
            List<Vector2> entityPoints = entity.Shape.Points.Select(p => Vector2.Transform(p, entity.Transformation)).ToList();
            Vector2 bulletStart = bullet.Position - bullet.Speed * elapsedTimeSeconds * GameMath.Vector2FromAngle(bullet.Direction);
            Vector2 bulletEnd = Vector2.Transform(bullet.Shape.Points.ElementAt(1), bullet.Transformation);
            for (int i = 0; i < entityPoints.Count - 1; i++)
            {
                float? intersect = LineSegmentsIntersect(bulletStart, bulletEnd, entityPoints[i], entityPoints[i + 1]);
                if (intersect != null && intersect >= 0 && (minIntersect == null || minIntersect > intersect))
                {
                    minIntersect = intersect;
                }
            }

            if (minIntersect == null) return null;
            return bulletStart + (bulletEnd - bulletStart) * minIntersect;
        }

        public static bool TestClosedShapes(Entity entityA, Entity entityB)
        {
            if ((entityA.Position - entityB.Position).Length() > entityA.Shape.Radius + entityB.Shape.Radius) return false;

            List<Vector2> entityAPoints = entityA.Shape.Points.Select(p => Vector2.Transform(p, entityA.Transformation)).ToList();
            List<Vector2> entityBPoints = entityB.Shape.Points.Select(p => Vector2.Transform(p, entityB.Transformation)).ToList();

            for (int i = 0; i < entityAPoints.Count - 1; i++)
            {
                for (int j = 0; j < entityBPoints.Count - 1; j++)
                {
                    if (LineSegmentsIntersect(entityAPoints[i], entityAPoints[i + 1], entityBPoints[j], entityBPoints[j + 1]) != null) return true;
                }
            }

            return false;
        }
    }
}
