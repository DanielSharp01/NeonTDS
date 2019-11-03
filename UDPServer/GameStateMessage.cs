using NeonTDS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    class GameStateMessage : Message
    {
        public string PlayerEntityID { get; set; }
        Entity[] entities;
        string[] createdentityIDs;
        string[] destroyedentityIDs;
    }
}
