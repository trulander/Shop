using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;

namespace Client
{
    public class HttpController
    {
        public HttpController()
        {
            HttpClient httpClient = new HttpClient();

            var postData = new StringContent("action=actionnumbertest", Encoding.UTF8);
            postData.Headers.Add("test","tsssst");
            
            
            var response = httpClient.PostAsync("Http://localhost:8080/?date=today", postData).Result;
            

            
            var content = response.Content.ReadAsStringAsync().Result;
            
            Console.WriteLine(content);
        }
    }
}