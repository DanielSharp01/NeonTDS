﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class InputMessage : Message
    {
        public SpeedState SpeedState { get; set; }
        public TurnState TurnState { get; set; }
        public bool Firing { get; set; }
        public float TurretDirection { get; set; }
    }
}