using System.Collections.Generic;

namespace PPB_Client.Models
{
    public class ServerCommand
    {
        public string Command { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = null;
    }
}
