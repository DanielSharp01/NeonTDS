using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2DEngine
{
   public class RapidFirePowerUp : IPowerUps

    {
        public void Use(Player player)
        {
            if (player.Damage == 50) player.FireRate = 60;
            else player.FireRate= 120;
        }
    }
}
