using System;
namespace VillaAPI.Logging
{
    public class Logging : ILogging
    {
        void ILogging.Log(string message, string type)
        {
            if(type == "error")
            {
                Console.WriteLine("ERORR - " + message);
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
}

