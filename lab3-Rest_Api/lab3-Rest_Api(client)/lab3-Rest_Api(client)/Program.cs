using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;

namespace lab3_Rest_Api_client_
{
    partial class Program
    {
        public static string localHost = @"http://localhost:80/";

        static void Main(string[] args)
        {

            while (true)
            {
                Console.Write("Введите строку запроса: " + localHost);
                string requestString = Console.ReadLine();

                if (requestString != @"\exit")
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(localHost + requestString);
                    request.Method = Request.GetMethod(requestString);
                    try
                    {
                        var response = request.GetResponse();
                        using (var dataStream = response.GetResponseStream())
                        {
                            StreamReader reader = new StreamReader(dataStream);
                            string responseFromServer = reader.ReadToEnd();
                            Console.WriteLine("Server returned: " + responseFromServer);
                        }

                        response.Close();
                    }
                    catch (WebException exception)
                    {
                        Console.WriteLine(exception.Message);
                    }

                }
                else
                    break;
            }
        }
    }
}
