using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class GameStateMessage : Message
    {
        public string PlayerEntityID { get; set; }
        public Entity[] Entities { get; set; }
        public string[] CreatedentityIDs { get; set; }
        public string[] DestroyedentityIDs { get; set; }
    }
}
