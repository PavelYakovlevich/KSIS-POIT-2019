using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            const byte clientCount = 0;
            Console.Write("Enter IP: ");
            string ip = Console.ReadLine();
            IPAddress temp = null;

            if (!IPAddress.TryParse(ip, out temp))
            {
                Console.WriteLine("Unvalid ip address");
            }
            else
            {
                Console.Write("Enter port: ");
                string portString = Console.ReadLine();
                int port = 0;

                if (!int.TryParse(portString, out port))
                {
                    Console.WriteLine("Unvalid port");
                }
                else
                {
                    var server = new Chat.Server(ip, port, clientCount);
                    server.StartWorking();
                }
            }

        }
    }
}
