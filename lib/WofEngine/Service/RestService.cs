using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using WofEngine.Entity;

namespace WofEngine.Service
{
    class RestService
    {
        const string BaseUrl = "http://127.0.0.1:8080";
        private string SessionToken;
        public string LastError { get; private set; }
        public AbstractRestResponse Send(IRestRequest request)
        {
            string rawRequest = request.ToJsonString();
            string response = request.GetMethod().ToLower().Equals("post") ?
                Post(request.GetEndpoint(), rawRequest) :
                Get(request.GetEndpoint());
            return new AbstractRestResponse(response);
        }

        public void SetSessionToken(string sessionToken)
        {
            SessionToken = sessionToken;
        }

        private string GetFullUrl(string endpoint)
        {
            return BaseUrl + endpoint;
        }

        private string Post(string endpoint, string body)
        {
            return SendRequest(GetFullUrl(endpoint), body);
        }

        private string Get(string endpoint)
        {
            return SendRequest(GetFullUrl(endpoint), "", "GET");
        }

        private string SendRequest(string url, string body, string method = "POST")
        {
            byte[] toSent = Encoding.UTF8.GetBytes(body);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            request.ContentLength = toSent.Length;
            request.ContentType = "application/json";
            if (SessionToken != null)
                request.Headers.Add("token", SessionToken);
            request.Method = method;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(toSent, 0, toSent.Length);
            }

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                return sr.ReadToEnd();
            }
        }
    }
}
