using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace  Win2DEngine
{
   public class ShieldPowerUp : IPowerUps
    {
        public void Use(Player player)
        {
            player.Color = new Vector4(1, 1, 1, 1);
            player.Shield = 100;
        }
    }
}
