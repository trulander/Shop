using System;

namespace Client
{
    public class ProgramController
    {
        private HttpController _httpController;
        private Result _response;
        public ProgramController()
        {
            Console.WriteLine("Http client for ShowCase developed by code1code.");
            _httpController = new HttpController();
            _response = _httpController.Request(37);
            MainLoop();
        }

        private void MainLoop()
        {
            do
            {
                View.Clear();
                View.WriteLine(_response.text);
                switch (_response.action)
                {
                    case "ReadLine":
                        _response = _httpController.Request(0,Console.ReadLine());
                        break;
                    case "ReadKey":
                        _response = _httpController.Request((int)Console.ReadKey(true).Key);
                        break;
                    default:
                        _response = _httpController.Request(37);
                        break;
                }
            }
            while (true);
        }
    }
}