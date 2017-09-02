using Newtonsoft.Json;
using PPB_Server.Helpers;
using PPB_Server.Models;
using System;
using System.Net.Sockets;
using System.Text;

namespace PPB_Server
{
    public class MsgClient
    {
        public bool Send(ServerCommand command, TcpClient client, NetworkStream stream)
        {
            // Serializes the command object into a json object. 
            string json = JsonConvert.SerializeObject(command);

            // Encrypts the json string. 
            string encryptedJson = Encrypt.EncryptString(json, "ppb");

            // Encodes json string into a sequence of bytes.
            Byte[] data = Encoding.ASCII.GetBytes(encryptedJson);

            try
            {
                // Sends message to client.
                stream.Write(data, 0, data.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }   
}
