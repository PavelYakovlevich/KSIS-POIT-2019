using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Server
{
    public class User
    {
        private static bool _playing;
        public List<string> gameResultStrings;
        private Thread listeningThread;
        private int sideIndex;
        private readonly char horizontalLineChar = '|';
        public readonly int height = 25;
        public readonly int width = 80;
        public int linePosition = 40;

        public User()
        {
            gameResultStrings = new List<string>();
            gameResultStrings.Add("Win");
            gameResultStrings.Add("You loose");
        }

        public void Connect(Socket socket, EndPoint destIp)
        {
            byte[] buffer = new byte[2] { (byte)Command.cmdConnect, 0};

            socket.SendTo(buffer, destIp);
            
            socket.ReceiveFrom(buffer, ref destIp);

            Command gameCommand = (Command)buffer[0];
            switch (gameCommand)
            {
                //case Command.cmdGameLosse:
                //case Command.cmdGameWin:
                //    Console.Clear();
                //    StopGame(buffer[0]);
                //    break;
                case Command.cmdWaitOpponent: // Waiting for opponent
                    sideIndex = GetSideIndex(buffer[1]);
                    Wait(socket, destIp);
                    break;
                case Command.rightSide:
                case Command.leftSide:
                    sideIndex = GetSideIndex(buffer[0]);
                    Wait(socket, destIp);
                    break;
            }

        }

        public int GetSideIndex(byte value)
        {
            if (value == (byte)Command.leftSide)
                return 1;

            return -1;
        }

        public void Wait(Socket socket, EndPoint remoteIP)
        {
            Console.WriteLine("Wait opponent");
            byte[] buffer = new byte[2] { 4, 0 };
            while (buffer[0] == (byte)Command.cmdWaitOpponent)
            {
                socket.ReceiveFrom(buffer, ref remoteIP);
            }

            _playing = true;
            StartPlaying(socket, remoteIP);
        }

        public void StopGame(byte v)
        {
            PrintResult(v);
        }

        public void PrintResult(byte v) => Console.WriteLine(gameResultStrings[v]);

        public void StartPlaying(Socket socket, EndPoint serverEndPoint)
        {
            Console.SetWindowSize(80, 26);
            Console.Clear();
            RedrawVerticalLine((byte)linePosition);
            listeningThread = new Thread(delegate () { StartAcceptingMessages(socket, serverEndPoint); });
            listeningThread.Start();

            while (_playing)
            {
                Console.ReadKey();
                linePosition += sideIndex;
                MakeMove(socket, serverEndPoint, linePosition);
            }
        }

        public void MakeMove(Socket socket, EndPoint serverEndPoint, int linePosition)
        {
            byte[] buffer = new byte[2];
            buffer[0] = (byte)Command.cmdMove;
            buffer[1] = (byte)linePosition;
            socket.SendTo(buffer, serverEndPoint);
        }

        private void StartAcceptingMessages(Socket socket, EndPoint serverEndPoint)
        {
            byte[] buffer = new byte[2];
            while (Thread.CurrentThread.IsAlive)
            {
                socket.ReceiveFrom(buffer, ref serverEndPoint);
                Command gameCommand = (Command)buffer[0];
                switch (gameCommand)
                {
                    case Command.cmdGameWin:
                    case Command.cmdGameLosse:
                        StopGame(buffer[0]);
                        _playing = false;
                        listeningThread.Abort();
                        break;
                    case Command.cmdConnectionError:
                        Console.WriteLine("Connection Error");
                        break;
                    case Command.cmdMove:
                        RedrawVerticalLine(buffer[1]);
                        break;
                }
            }
        }

        private void RedrawVerticalLine(byte position)
        {
            string someString = MakeLine(position);

            for (int i = 0; i < height; i++)
            {
                Console.WriteLine(someString);
            }

            linePosition = position;
        }

        private string MakeLine(int position)
        {
            int i;
            string result = "";

            for (i = 0; i < position - 1; i++)
                result += ' ';

            result += horizontalLineChar;

            for (; i < width; i++)
                result += ' ';

            return result;
        }
    }


        

}
