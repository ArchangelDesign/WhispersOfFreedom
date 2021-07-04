using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WofEngine.NetworkPacket
{
    public class GenericNetworkPacket
    {
        [JsonProperty("clientId")]
        public string ClientId { get; set; }

        [JsonProperty("command")]
        public string Command { get; set; }

        [JsonProperty("arg1")]
        public string Argument { get; set; }

        [JsonProperty("memo")]
        public string Memo { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, string> Parameters { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static GenericNetworkPacket FromJson(string json)
        {
            return JsonConvert.DeserializeObject<GenericNetworkPacket>(json);
        }

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(ToJson());
        }
    }
}
