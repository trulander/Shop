using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace Client
{
    public class HttpController
    {
        public const string urlServerHost = "Http://localhost:8080/";

        public Result Request(int key, string text = "")
        {
            HttpClient httpClient = new HttpClient();
            Result result;
            var postData = new StringContent("key=" + key + "&text=" + text, Encoding.UTF8);
            
            try
            {
                var response = httpClient.PostAsync(urlServerHost, postData).Result;
                result = new Result
                (
                    response.Headers.Contains("lastMethodRequired") ? response.Headers.GetValues("lastMethodRequired").Single() : "empty",
                    response.Content.ReadAsStringAsync().Result
                );
            }
            catch
            {
                View.WriteLine("Server not found");
                result = new Result(null, null);
            }
            return result;
        } 
    }
}