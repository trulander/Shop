using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;

namespace Client
{
    public class HttpController
    {
        public HttpController(int action)
        {
            HttpClient httpClient = new HttpClient();

            var postData = new StringContent("action=" + action, Encoding.UTF8);
            
            var response = httpClient.PostAsync("Http://localhost:8080/", postData).Result;
            
            var content = response.Content.ReadAsStringAsync().Result;
            
            Console.WriteLine(content);
        }
    }
}