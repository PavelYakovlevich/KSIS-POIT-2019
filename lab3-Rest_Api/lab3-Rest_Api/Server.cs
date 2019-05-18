using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace lab3_Rest_Api
{
    public enum Method
    {
        POST, GET, PUT, DELETE, COPY, MOVE
    };

    public class Server
    {
        private TcpListener _listener;
        public bool Working { get; private set; }
        public readonly string SERVER_NAME = "Server: MyFirstServer";
        public readonly string MIME_HEADER = "text/xml";

        private Thread _handlingClientsThread;
        private Dictionary<string, TcpClient> _clients;
        private string _currentDirPath;
        private Dictionary<int, string> _requestPhrases;
        private readonly string HTTP_VERSION = "HTTP/1.1";
        public readonly int bufferSize = 5000;
        private Thread _handlingClietsThread;
        //private Dictionary<Method, Action<string, string>> _methodsHandlers;

        public Server(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _clients = new Dictionary<string, TcpClient>();
            _currentDirPath = Directory.GetCurrentDirectory() + @"\Files\";
            _requestPhrases = new Dictionary<int, string>();
            FeelPhrases();
            //_methodsHandlers = new Dictionary<Method, Action<string, string>>();
        }

        public void Stop()
        {
            _handlingClientsThread.Abort();
        }

        private void FeelPhrases()
        {
            _requestPhrases.Add(200, "OK");
            _requestPhrases.Add(201, "Created");
            _requestPhrases.Add(205, "Reset content");

            _requestPhrases.Add(404, "Not Found");
            _requestPhrases.Add(400, "Bad Request");
            _requestPhrases.Add(408, "You dont have a permission to acces this file");

            _requestPhrases.Add(500, "File delete error");
            _requestPhrases.Add(501, "File doesn`t exist");
            _requestPhrases.Add(502, "File writting error");
            _requestPhrases.Add(503, "File reading error");
            _requestPhrases.Add(504, "File copying error");
            _requestPhrases.Add(505, "File moving error");
        }

        private int Post(string fileName, string content) // done, anime level
        {
            if (File.Exists(fileName))
            {
                try
                {
                    using (FileStream stream = new FileStream(fileName, FileMode.Open))
                    {
                        if (stream.CanSeek)
                        {
                            stream.Seek(0, SeekOrigin.End);
                            byte[] buffer = Encoding.UTF8.GetBytes(content);
                            stream.Write(buffer, 0, buffer.Length);
                        }
                        else
                            return 408;
                    }
                }
                catch (Exception exception)
                {
                    return 500;
                }

                return 200;
            }

            return 404;
        }
        private int Get(string fileName, out string result) // get content of the file
        {
            result = "";
            if (File.Exists(fileName))
            {
                using (FileStream stream = new FileStream(fileName, FileMode.Open))
                {
                    FileInfo fileInfo = new FileInfo(fileName);
                    byte[] buffer = new byte[fileInfo.Length];
                    try
                    {
                        stream.Read(buffer, 0, buffer.Length);
                    }
                    catch (Exception exception)
                    {
                        return 503;
                    }

                    result = Encoding.UTF8.GetString(buffer);
                }

                return 200;
            }

            return 404;
        }
        private int Delete(string fileName) // удаление файла
        {
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception exception)
                {
                    return 500;
                }

                return 200;
            }

            return 404;
        }
        private int Put(string fileName, string newContent) // rewrite file
        {
            if (File.Exists(fileName))
            {
                try
                {
                    using (FileStream stream = new FileStream(fileName, FileMode.Create))
                    {
                        byte[] buffer = Encoding.UTF8.GetBytes(newContent);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
                catch (Exception exception)
                {
                    return 500;
                }

                return 200;
            }

            return 404;
        }
        private int Copy(string fileName, string copiedFileName) // copy the file
        {
            if (File.Exists(fileName))
            {
                try
                {
                    File.Copy(fileName, copiedFileName);
                }
                catch (Exception exception)
                {
                    return 500;
                }

                return 200;
            }

            return 404;
        }
        private int Move(string filePath, string dirPath) // move the file
        {
            if (File.Exists(filePath))
            { 
                try
                {
                    File.Move(filePath, dirPath + @"\" + Path.GetFileName(filePath));
                }
                catch (Exception exception)
                {
                    return 505;
                }

                return 200;

            }

            return 404;
        }


        public void Start()
        {
            try
            {
                _listener.Start();
                Working = true;

            }
            catch (Exception exception)
            {
                return;
            }

            _handlingClientsThread = new Thread(() => { HandlingClients(); });
            _handlingClientsThread.Start();
        }

        public static string GetMethodName(string requestString)
        {
            string methodName = "";

            for (int i = 0; i < requestString.Length && requestString[i] != ' '; i++)
                methodName += requestString[i];

            return methodName;
        }

        private void HandlingClients()
        {
            while (Working)
            {
                TcpClient newClient = _listener.AcceptTcpClient();
                Thread clientThread = new Thread(() => { HandlingClient(newClient); });
                clientThread.Start();
            }
            
        }

        private void HandlingClient(TcpClient client)
        {
            byte[] buffer = new byte[bufferSize];
            NetworkStream netStream;
            netStream = client.GetStream();
            int bytesRead = netStream.Read(buffer, 0, buffer.Length);
            string requestString = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            requestString = Uri.UnescapeDataString(requestString);
            string method = Server.GetMethodName(requestString);
            int methodResult = 0;
            string result = "";
            Dictionary<string, string> requestParams = Server.GetValue(requestString);
            try
            {
                switch (method)
                {
                    case "GET":
                        methodResult = Get(_currentDirPath + requestParams["filename"], out result); // checked // TODO: error handling
                        SendResponse(netStream, methodResult, result);
                        break;
                    case "POST":
                        methodResult = Post(_currentDirPath + requestParams["filename"], requestParams["content"]); // checked // TODO: error handling
                        SendResponse(netStream, methodResult);
                        break;
                    case "PUT":
                        methodResult = Put(_currentDirPath + requestParams["filename"], requestParams["content"]); // checked // TODO: error handling
                        SendResponse(netStream, methodResult);
                        break;
                    case "DELETE":
                        methodResult = Delete(_currentDirPath + requestParams["filename"]); // checked // TODO: error handling
                        SendResponse(netStream, methodResult);
                        break;
                    case "COPY":
                        methodResult = Copy(_currentDirPath + requestParams["filename"], _currentDirPath + requestParams["content"]); // checked // TODO: error handling
                        SendResponse(netStream, methodResult);
                        break;
                    case "MOVE":
                        methodResult = Move(_currentDirPath + requestParams["filename"], _currentDirPath + requestParams["content"]); // checked // TODO: error handling
                        SendResponse(netStream, methodResult);
                        break;
                    default:
                        methodResult = 400;
                        SendResponse(netStream, methodResult);
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            client.Close();
            netStream.Dispose();
            Thread.CurrentThread.Abort();
        }

        private void SendResponse(NetworkStream clientStream, int requestResult, string content = "")
        {
            byte[] buffer = Encoding.UTF8.GetBytes(content);
            if (SendHeader(clientStream, requestResult, buffer.Length))
            {
                try
                {
                    clientStream.Write(buffer, 0, buffer.Length);
                }
                catch (Exception exception)
                {

                }
            }
        }

        private bool SendHeader(NetworkStream requestStream, int requestResult, int contentLength)
        {
            string headerString = HTTP_VERSION +' ' + requestResult.ToString() + ' ' + GetResultPhrase(requestResult) + "\r\n";
            //headerString += " " + DateTime.Now.ToString("ddd, dd MMM yyy hh:mm:ss 'GMT'") + "\r\n";
            headerString += SERVER_NAME + "\r\n";
            headerString += "Content-Type: " + MIME_HEADER + "\r\n";
            headerString += "Accept-Ranges: bytes\r\n";
            headerString += "Content-Length: " + contentLength + "\r\n\r\n";

            byte[] buffer = Encoding.UTF8.GetBytes(headerString);
            try
            {
                requestStream.Write(buffer, 0, buffer.Length);
            }
            catch(Exception exception)
            {
                return false;
            }

            return true;
        }

        private string GetResultPhrase(int requestResult) => _requestPhrases[requestResult];

        private static Dictionary<string, string> GetValue(string requestString) // TODO: to fix whispace characters in values regex
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            Regex valueRegex = new Regex(@"(?<=\')([\d\w\.\s\\]+)(?=\')");
            MatchCollection values = valueRegex.Matches(requestString);

            Regex idRegex = new Regex(@"([A-Za-z\d]+)(?=\=)");
            MatchCollection ids = idRegex.Matches(requestString);

            for (int i = 0; i < values.Count; i++)
            {
                result.Add(ids[i].Value, values[i].Value);
            }

            return result;
        }
    }   
}
