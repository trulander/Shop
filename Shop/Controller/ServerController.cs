using System;
using System.Collections.Generic;
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
        private static ProgramController _programController;
        private  IOutput _output;
        public const string urlServerHost = "Http://localhost:8080/";
        
        public ServerController(IOutput output)
        {
            _output = output;
            _programController = new ProgramController(_output);
            _output.waitHandle = new[]
            {
                new ManualResetEvent(initialState: true),
                new ManualResetEvent(initialState: true)
            };
            
            Thread serverThread = new Thread(() => StartServer(_output));
            serverThread.IsBackground = false;
            serverThread.Start();
        }

        private static void StartServer(IOutput output)
        {
            Thread programLoopThread;
            programLoopThread = new Thread(()=>ProgramLoop(output));
            programLoopThread.IsBackground = true;
            
            var httpListener = new HttpListener();
            httpListener.Prefixes.Add(urlServerHost);
            httpListener.Start();
            
            while (true)
            {
                var requestContext = httpListener.GetContext();
                var request = requestContext.Request;
                
                if ( request.HttpMethod == "POST" )
                {
                    string consoleText;
                    int consoleKey;
                    
                    requestContext.Response.StatusCode = 200; //OK     
                    var postData = GetNameValues(request);
                    
                   
                    if (postData.TryGetValue("text", out consoleText))
                    {
                        output.ConsoleText = consoleText; 
                    }
                   
                    if (Int32.TryParse(postData?["key"], out consoleKey))
                    {
                        output.ConsoleKey = consoleKey;
                        
                        output.waitHandle[1].Reset();
                        output.waitHandle[0].Set();
                        
                        if (!programLoopThread.IsAlive)
                        {
                            programLoopThread = new Thread(()=>ProgramLoop(output));
                            programLoopThread.IsBackground = false;
                            programLoopThread.Start();
                        }
                        output.waitHandle[1].WaitOne();
                    }
                }
                else
                {
                    requestContext.Response.StatusCode = 500; //exception
                }
        
                Console.WriteLine(output.Buffer);

                requestContext.Response.Headers.Add("lastMethodRequired",output.lastMethodRequired);
                output.lastMethodRequired = "";
                
                var stream = requestContext.Response.OutputStream;
                var bytes = Encoding.UTF8.GetBytes(output.Buffer);
                try
                {
                    stream.Write(bytes, 0, bytes.Length);
                }
                catch
                {
                    Console.WriteLine("Client was disconnected");
                }
                
                requestContext.Response.Close();
            }
            /*
             * todo:   finish current thread, now the code will never use
             */
            httpListener.Stop();
            httpListener.Close();
        }

        private static void ProgramLoop(IOutput output)
        {
            _programController.MainLoop();
            output.waitHandle[1].Set();
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