using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Chat
{
    public abstract class ChatObject
    {
        public string NickName { get; set; }
        public Socket ObjSocket { get; set; }
        protected Thread InnerThread { get; set; }
        public abstract void SendMessage(string message);
        public abstract void ReceiveMessage();
        public abstract void Disconnect();
        public abstract void Connect(string ipString, int port);
        public abstract void MessageHandler();
    }
}
