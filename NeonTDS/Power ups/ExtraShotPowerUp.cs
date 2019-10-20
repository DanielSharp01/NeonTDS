using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Win2DEngine
{
   public class ExtraShotPowerUp : IPowerUps
    {
        public void Use(Player player)
        {
            player.Damage = 50;
            player.FireRate = 30;
        }
    }
}
