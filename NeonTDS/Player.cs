﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI;
using Windows.System;

namespace Win2DEngine
{
    public class Player : GameObject
    {
        private int fireRate;
        public Bullet Bullet { get; set; }
        public Turret Turret { get; set; }

        public int FireRate
        {
            get { return fireRate; }
            set
            {
                if (value >= 120) ;
                else fireRate = value;
            }
        }

        private int shield;

        public int Shield
        {
            get { return shield; }
            private set { }
        }

        public void SetShield()
        {
            shield = 100;
        }



        public Player()
        {
            Speed = 400;
            Health = 100;
            FireRate = 60;
            Shield = 0;
            Bullet = new Bullet();
            Bullet.Speed = this.Speed;
        }

        public override void Update(CanvasTimingInformation timing)
        {
            if (Game.Instance.InputManager.IsKey(VirtualKey.A, PressState.Down))
            {
                Direction -= (float)(Math.PI * timing.ElapsedTime.TotalSeconds);
            }
            if (Game.Instance.InputManager.IsKey(VirtualKey.D, PressState.Down))
            {
                Direction += (float)(Math.PI * timing.ElapsedTime.TotalSeconds);
            }
            

            if (Game.Instance.InputManager.IsKey(VirtualKey.W, PressState.Down))
            {
                Speed += 300 * (float)timing.ElapsedTime.TotalSeconds;
            }
            if (Game.Instance.InputManager.IsKey(VirtualKey.S, PressState.Down))
            {
                Speed -= 300 * (float)timing.ElapsedTime.TotalSeconds;
            }




            // Clamp values
            if (Speed < 100) Speed = 100;
            if (Speed > 700) Speed = 700;
            Bullet.Speed = this.Speed;
            base.Update(timing);
        }
    }
}
