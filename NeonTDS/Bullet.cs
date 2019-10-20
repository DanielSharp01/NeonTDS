using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Win2DEngine
{
   public class Bullet : GameObject
    {
        public Bullet()
        {
            Damage = 20;
            Speed = 0;
            Sprite = Game.Instance.BulletSprite;
        }
        public int Damage { get; set; }


    }
}
