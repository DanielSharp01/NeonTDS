using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2DEngine
{
    public class Camera
    {
        public Vector2 Size { get; set; }
        public Vector2 Center { get; set; }
        public float Rotation { get; set; }

        public Matrix3x2 Transform => Matrix3x2.CreateTranslation(-Center) * Matrix3x2.CreateRotation(-Rotation) * Matrix3x2.CreateTranslation(Size / 2);

        public void Update(GameObject followObject)
        {
            Center = followObject.Position;
        }
    }
}
