using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WofEngine.Entity
{
    public class EnterServerResponse : IRestResponse
    {
        [JsonProperty("sessionToken")]
        public string SessionToken;
        public static EnterServerResponse FromAbstractResponse(AbstractRestResponse response)
        {
            return JsonConvert.DeserializeObject<EnterServerResponse>(response.RawJson);
        }
    }
}
