using Server;
using System;
using System.Net;
using System.Net.Sockets;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = int.Parse(Console.ReadLine());
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            IPEndPoint destIp = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            User user = new User();
            user.Connect(socket, destIp);

        }
    }


     
}
