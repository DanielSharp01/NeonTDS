using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NeonTDS
{
    public class Message
    {
        public string MessageType { get; set; }
        public long Timestamp { get; set; }

        [JsonIgnore]
        public IPEndPoint RemoteEndPoint { get; set; }

        public static Message FromJson(JObject json)
        {
            Type type = Type.GetType(json["MessageType"].ToString());
            return (Message)json.ToObject(type);
        }

        public static JObject ToJson(Message message)
        {
            message.Timestamp = DateTime.Now.Ticks;
            message.MessageType = message.GetType().FullName;
            var json = JObject.FromObject(message);
            return json;
        }
    }
}
