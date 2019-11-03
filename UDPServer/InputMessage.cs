using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    class InputMessage : Message
    {
        public int SpeedState { get; set; }
        public bool Fire { get; set; }
        public int Direction { get; set; }
        public float TurretDirection { get; set; }

        public InputMessage(int speed, bool f, int dir, float tDir)
        {
            SpeedState = speed;
            Fire = f;
            Direction = dir;
            TurretDirection = tDir;
        }


    }
}
