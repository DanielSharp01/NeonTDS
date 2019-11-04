using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class ConnectResponseMessage : Message
    {
        public bool Approved { get; set; }
        public string PlayerEntityID { get; set; }
    }
}
