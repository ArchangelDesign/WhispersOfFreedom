using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace WofEngine.Entity
{
    public class EnterServerRequest : IRestRequest
    {
        const string Endpoint = "/user/enter";
        const string Method = "POST";
        [JsonProperty("username")]
        public string Username { get; set; }
        [JsonProperty("password")]
        public string Password { get; set; }

        public string GetEndpoint()
        {
            return Endpoint;
        }

        public string GetMethod()
        {
            return Method;
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
