using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace Shop.Controller
{
    class ServerController
    {
        private static ShopController _shop;
        public ServerController(ShopController shop)
        {
            _shop = shop;
            var th = new Thread(StartServer);
            th.IsBackground = false;
            th.Start();
        }

        private static void StartServer()
        {
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add("Http://localhost:8080/");
            httpListener.Start();
            
            while (_shop.IsLoggedIn)
            {
                var requestContext = httpListener.GetContext();
                var request = requestContext.Request;
                var responseValue = "";
                
                if ( request.HttpMethod == "POST" )
                {
                    requestContext.Response.StatusCode = 200; //OK     
                    responseValue = "accepted";
                    
                    var postData = GetNameValues(request);
                    Console.WriteLine(postData["action"]);

                }
                else
                {
                    requestContext.Response.StatusCode = 500; //OK
                }
        
                

                var stream = requestContext.Response.OutputStream;
                var bytes = Encoding.UTF8.GetBytes(responseValue);
                stream.Write(bytes, 0, bytes.Length);
                requestContext.Response.Close();
            }
            
            httpListener.Stop();
            httpListener.Close();
        }
        
        static string DecodeUrl(string url)
        {
            string newUrl;
            while ((newUrl = Uri.UnescapeDataString(url)) != url)
                url = newUrl;
            return newUrl;
        }

        static Dictionary<string, string> GetNameValues(HttpListenerRequest request)
        {
            var result = new Dictionary<string, string>();
 
            using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                var requestBody = reader.ReadToEnd();
                string[] nameValues = requestBody.Split('&');
 
                foreach (var nameValue in nameValues.ToList())
                {
                    string[] splitted = nameValue.Split('=');
                    result.Add(DecodeUrl(splitted[0]), DecodeUrl(splitted[1]));
                }
            }
    
            return result;
        }        
    }
}