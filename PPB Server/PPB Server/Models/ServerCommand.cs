using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PPB_Server.Models
{
    public class ServerCommand
    {
        public string Command { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = null;
    }
}
