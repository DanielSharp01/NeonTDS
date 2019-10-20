using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2DEngine
{
   public class RapidFirePowerUp : PowerUpsInt

    {
        public void Use(Player player)
        {
            player.Turret.Color = new Vector4(1, 1, 1, 1);
            if (player.Bullet.Damage == 50) player.FireRate = 60;
            else player.FireRate= 120;
        }
    }
}
