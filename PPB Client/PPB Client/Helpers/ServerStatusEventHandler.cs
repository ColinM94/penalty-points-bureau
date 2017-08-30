using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPB_Client.Helpers
{

    public delegate void ConnectionDelegate(string str);

    

    class ServerStatusEventHandler
    {

        public event ConnectionDelegate ConnectEvent;

    }
}
