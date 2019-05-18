using System.Collections.Generic;

namespace lab3_Rest_Api_client_
{
    partial class Program
    {
        public class Response
        {
            public Dictionary<int, string> requestPhrases;
            public Response()
            {
                FeelPhrases();
            }

            private void FeelPhrases()
            {
                requestPhrases.Add(200, "OK");
                requestPhrases.Add(201, "Created");
                requestPhrases.Add(205, "Reset content");

                requestPhrases.Add(404, "Not Found");
                requestPhrases.Add(400, "Bad Request");
                requestPhrases.Add(408, "You dont have a permission to acces this file");

                requestPhrases.Add(500, "File delete error");
                requestPhrases.Add(501, "File doesn`t exist");
                requestPhrases.Add(502, "File writting error");
                requestPhrases.Add(503, "File reading error");
                requestPhrases.Add(504, "File copying error");
                requestPhrases.Add(505, "File moving error");
            }


        }
    }
}
