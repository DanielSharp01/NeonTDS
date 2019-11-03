using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UDPServer
{
    class ConnectResponseMessage : Message
    {
        public bool Approved { get; set; }
        public string PlayerEntityID { get; set; }

        public ConnectResponseMessage(bool app, string pEID)
        {
            Approved = app;
            PlayerEntityID = pEID;
        }

    }
}
