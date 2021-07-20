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

        public long GetTime()
        {
            return long.Parse(Parameters["currentTime"]);
        }

        public long GetMilliseconds()
        {
            return long.Parse(Parameters["currentTimeFraction"]) / 1000000;
        }

        public long GetLatencyMilliseconds()
        {
            double t = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            long currentSeconds = long.Parse(Math.Floor(t).ToString());
            long currentMilliseconds = DateTime.UtcNow.Millisecond;
            long packetSeconds = GetTime();
            long packetMilliseconds = GetMilliseconds();

            long seconds = (currentSeconds - packetSeconds) * 1000;
            long milis = currentMilliseconds - packetMilliseconds;

            return seconds + milis;
        }
    }
}
