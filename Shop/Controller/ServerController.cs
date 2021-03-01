using System;
using System.Diagnostics;
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
                requestContext.Response.StatusCode = 200; //OK

                var stream = requestContext.Response.OutputStream;

                var text = "test message";
                var bytes = Encoding.UTF8.GetBytes(text);
                stream.Write(bytes, 0, bytes.Length);
                requestContext.Response.Close();
            }
            
            httpListener.Stop();
            httpListener.Close();
        }
    }
}