using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chat
{
    public abstract class ChatObj
    {
        public string NickName { get; set; }
        public Socket ObjSocket { get; set; }
        protected Thread InnerThread { get; set; }
        public abstract void SendMessage(Socket socket, string sender, string message);
        //public abstract string ReceiveMessage(Socket socket);
        public abstract void StopWorking();
        public abstract void Connect(string ipString, int port);
    }
}
