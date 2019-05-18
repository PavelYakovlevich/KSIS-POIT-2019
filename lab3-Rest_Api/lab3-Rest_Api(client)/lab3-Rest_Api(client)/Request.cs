namespace lab3_Rest_Api_client_
{
    partial class Program
    {
        public class Request
        {
            public static string GetMethod(string requestString)
            {
                string methodString = "";
                for (int i = 0; i < requestString.Length && requestString[i] != '?'; i++)
                    methodString += requestString[i];

                return methodString;
            }
        }
    }
}
