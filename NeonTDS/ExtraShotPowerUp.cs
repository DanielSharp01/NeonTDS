using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2DEngine
{
   public class ExtraShotPowerUp : PowerUpsInt
    {
        public void Use(Player player)
        {
            player.Bullet.Damage = 50;
            player.Bullet.Color = new Vector4(1, 1, 1, 1);
            player.FireRate = 30;
        }
    }
}
