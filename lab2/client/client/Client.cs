using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Chat
{
    public class Client : ChatObject
    {
        public Client(string nickName)
        {
            NickName = nickName;
            ObjSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public static void DrawSeparator() => Console.WriteLine("__________________");

        public override void Connect(string ipString, int port)
        {
            IPEndPoint destPoint = new IPEndPoint(IPAddress.Parse(ipString), port);

            try
            {
                ObjSocket.Connect(destPoint);
                ObjSocket.Send(new byte[1] { (byte)NickName.Length });
                ObjSocket.Send(Encoding.ASCII.GetBytes(NickName));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
            finally
            {
                Console.WriteLine("Connection established");
                InnerThread = new Thread(new ThreadStart(MessageHandler));
                InnerThread.Start();
            }

        }

        public override void Disconnect()
        {
            Console.WriteLine("Disconnecting...");
            InnerThread.Abort();
            ObjSocket.Shutdown(SocketShutdown.Both);
            ObjSocket.Close();
        }

        public override void MessageHandler()
        {
            while (true)
            {
                ReceiveMessage();
            }
        }

        public override void ReceiveMessage()
        {
            var str = new StringBuilder();
            var length = new byte[1];

            var bytesReceived = ObjSocket.Receive(length);
            var buffer = new byte[length[0]];

            bytesReceived = ObjSocket.Receive(buffer);
            str.Clear();
            str.Append(Encoding.ASCII.GetString(buffer), 0, bytesReceived);
            Console.Write("{0}: ", str);

            bytesReceived = ObjSocket.Receive(length);
            buffer = new byte[length[0]];

            bytesReceived = ObjSocket.Receive(buffer);
            str.Clear();
            str.Append(Encoding.ASCII.GetString(buffer), 0, bytesReceived);
            Console.WriteLine(str);
            Client.DrawSeparator();
        }

        public override void SendMessage(string message)
        {
            try
            {
                ObjSocket.Send(new byte[1] { (byte)message.Length });
                ObjSocket.Send(Encoding.ASCII.GetBytes(message));
            }
            catch(Exception exception)
            {
                Console.WriteLine("Message send failed with message: {0}", exception.Message);
            }
        }
    }
}
