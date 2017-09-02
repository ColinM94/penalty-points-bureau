using System.Collections.Generic;

namespace PPB_Client.Models
{
    /// <summary>
    /// Contains a command so the server knows what to do and a Dictionary to store any data. 
    /// </summary>
    public class ServerCommand
    {
        public string Command { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = null;
    }
}
