using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Shop.Model;

namespace Shop.Controller
{
    class ServerController
    {
        private static ShopController _shop;
        private static MenuController _menu;
        public ServerController(ShopController shop, MenuController menu)
        {
            _shop = shop;
            _menu = menu;
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

                    int actionPost;

                    if (Int32.TryParse(postData["action"], out actionPost))
                    {
                        switch(actionPost)
                        {
                            case (int)ConsoleKey.Enter:
                                switch (_menu.Current.Children[_menu.SelectedIndex])
                                {
                                    case ActionMenuItem action:
                                        _shop.RouteTo(action.Command);
                                        break;
                                    case IContainerMenuItem container:
                                        _menu.Expand(container);
                                        break;
                                }
                                break;
                            case (int)ConsoleKey.UpArrow:
                                _menu.Prev();
                                break;
                            case (int)ConsoleKey.DownArrow:
                                _menu.Next();
                                break;

                            case (int)ConsoleKey.Backspace:
                            case (int)ConsoleKey.LeftArrow:
                            case (int)ConsoleKey.Escape:
                                _menu.Return();
                                break;
                            case (int)ConsoleKey.RightArrow:
                                if (_menu.Current.Children[_menu.SelectedIndex] is IContainerMenuItem containerItem)
                                    _menu.Expand(containerItem);
                                break;
                            default:
                                Output.WriteLine("Server: unsupported method");
                                responseValue = "unsupported method";
                                break;
                        }
                        Console.Clear();
                        
                        responseValue += _menu.Current.GetFullPathText() + "\r\n";
                        
                        Output.WriteLine(_menu.Current.GetFullPathText(), ConsoleColor.Yellow);
                        Console.WriteLine();

                        for (int i = 0; i < _menu.Current.Children.Count; i++)
                        {
                            IMenuItem item = _menu.Current.Children[i];

                            if (i != _menu.SelectedIndex)
                            {
                                responseValue += "  " + item.Text + "\r\n";
                                Console.WriteLine("  " + item.Text);
                            }
                            else
                            {
                                responseValue += "\u00A7 " + item.Text + "\r\n";
                                Output.WriteLine("\u00A7 " + item.Text, ConsoleColor.Cyan);
                            }
                        }                        
                    }
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