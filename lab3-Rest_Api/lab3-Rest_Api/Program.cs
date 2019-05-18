using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace lab3_Rest_Api
{
    class Program
    {
        static void Main(string[] args)
        {
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            Server server = new Server(80);

            server.Start();
            Console.WriteLine("Press any button to close server...");
            Console.ReadKey();
            Console.WriteLine("Closing...");
            server.Stop();
        }
    }
}
