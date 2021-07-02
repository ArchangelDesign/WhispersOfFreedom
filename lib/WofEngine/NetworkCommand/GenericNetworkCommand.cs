using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WofEngine.NetworkCommand
{
    public class GenericNetworkCommand
    {
        [JsonProperty("command")]
        public string Command { get; set; }
        [JsonProperty("clientId")]
        public string ClientId { get; set; }
        [JsonProperty("arg1")]
        public string Argument { get; set; }
        [JsonProperty("memo")]
        public string Memo { get; set; }
        [JsonProperty("parameters")]
        public Dictionary<string, string> Parameters = new Dictionary<string, string>();
        [JsonProperty("timestamp")]
        public long Timestamp = DateTime.UtcNow.Ticks;

        public static GenericNetworkCommand FromJson(string jsonString)
        {
            return JsonConvert.DeserializeObject<GenericNetworkCommand>(jsonString);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
