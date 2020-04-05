using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace JSONValidator
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var jsonValidator = new JsonValidationServer(args);
            jsonValidator.Start();
        }
    }
}