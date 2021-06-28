using System;
using System.Collections.Generic;
using System.Text;

namespace WofEngine.Entity
{
    public class AbstractRestResponse : IRestResponse
    {
        public string RawJson { get; }
        public AbstractRestResponse(string jsonString)
        {
            RawJson = jsonString;
        }
    }
}
