using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using Chat;

namespace client
{
    partial class Program
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
                    Console.Write("Enter nickname: ");
                    string nickName = Console.ReadLine();
                    var client = new Client(nickName);

                    client.Connect(ip, port);

                    while (true)
                    {
                        string message = Console.ReadLine();
                        client.SendMessage(message);
                        Client.DrawSeparator();
                    }
                }
            }
        }
    }
}
