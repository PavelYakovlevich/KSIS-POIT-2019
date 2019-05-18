using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Text;
using System.Net;

namespace Chat
{
    public class Server : ChatObj
    {
        private Dictionary<Socket, string> ClientList { get; set; }
        private Dictionary<Socket, Thread> ThreadList { get; set; }
        public int ClientCount { get; set; }
        public byte[] buffer { get; set; }

        public Server(string ip, int port, int clientCount)
        {
            ClientList = new Dictionary<Socket, string>();
            ThreadList = new Dictionary<Socket, Thread>();
            ClientCount = clientCount;
            buffer = new byte[255];
            ObjSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ObjSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
        }

        public override void Connect(string ipString, int port)
        {

        }

        public void StartWorking()
        {
            try
            {
                ObjSocket.Listen(ClientCount);
                Console.WriteLine("Listening...");
                StartAccepting();
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        public override void StopWorking()
        {
            ObjSocket.Shutdown(SocketShutdown.Both);
            ObjSocket.Close();
        }

        public static void MessageHandler(Server serv, Socket socket)
        {
            string message = "";
            while (true)
            {
                message = Server.ReceiveMessage(serv, socket);
                foreach (var client in serv.ClientList)
                {
                    if (client.Key == socket)
                        continue;
                    serv.SendMessage(client.Key, serv.ClientList[socket], message);
                }
            }

            serv.ThreadList[socket].Abort();
            serv.ThreadList.Remove(socket);
            serv.StopWorking();
        }

        public static string ReceiveMessage(Server serv, Socket socket)
        {
            var lengthBuffer = new byte[1];
            var bytesReceived = socket.Receive(lengthBuffer);

            var buffer = new byte[lengthBuffer[0]];

            bytesReceived = socket.Receive(buffer);
            var builder = new StringBuilder();
            builder.Append(Encoding.ASCII.GetString(buffer), 0, bytesReceived);
            Console.WriteLine("User[{0}] send message: {1}", serv.ClientList[socket], builder);
            return builder.ToString();
        }

        public override void SendMessage(Socket socket, string sender, string message)
        {
            try
            { 
                socket.Send(new byte[1] { (byte) sender.Length});
                socket.Send(Encoding.ASCII.GetBytes(sender));

                socket.Send(new byte[1] { (byte) message.Length});
                socket.Send(Encoding.ASCII.GetBytes(message));
            }
            catch (Exception exception)
            {
                Console.WriteLine("SendMessage error occured with message: {0}", exception.Message);
            }
        }

        public void StartAccepting()
        {
            Socket listenSocket = null;
            var builder = new StringBuilder();
            var bytesReceived = 0;
            var lengthBuffer = new byte[1];

            while (true)
            {
                listenSocket = ObjSocket.Accept();
                Console.Write("New Connection: ");

                bytesReceived = listenSocket.Receive(lengthBuffer);

                var buffer = new byte[lengthBuffer[0]];

                bytesReceived = listenSocket.Receive(buffer);

                builder.Clear();
                builder.Append(Encoding.ASCII.GetString(buffer) , 0, bytesReceived);
                
                Console.WriteLine(builder);

                ClientList.Add(listenSocket, Encoding.ASCII.GetString(buffer));
                ThreadList.Add(listenSocket, new Thread(delegate () { Server.MessageHandler(this, listenSocket); }));
                 
                ThreadList[listenSocket].Start();
            }
        }
    }
}