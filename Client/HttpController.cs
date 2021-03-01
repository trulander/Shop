using System;
using System.Net.Http;

namespace Client
{
    public class HttpController
    {
        public HttpController()
        {
            HttpClient httpClient = new HttpClient();

            var response = httpClient.PostAsync("Http://localhost:8080/", null).Result;
            var content = response.Content.ReadAsStringAsync().Result;
            
            Console.WriteLine(content);
        }
    }
}