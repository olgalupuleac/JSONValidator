using System;
using System.Net;
using Newtonsoft.Json.Linq;

namespace JSONValidator
{
    class MainClass
    {


        public static void Main(string[] args)
        {
            var s = @"{""menu"": {
  ""id"": ""file"",
  ""value"": ""File"",
  ""popup"": {
                ""menuitem"": [
                  {""value"": ""New"", ""onclick"": ""CreateNewDoc()""},
      {""value"": ""Open"", ""onclick"": ""OpenDoc()""},
      {""value"": ""Close"", ""onclick"": ""CloseDoc()""}
    ]
  }
}}";
           // var reader = new JsonReader()
// var document = JsonDocument.Parse(s);
            //document.RootElement.EnumerateObject;
            var simple = @"{""key"": ""value""}";
            var t =
                @"{""identity"": {""name"": ""Test"", ""translations"": [{""order"": 1, ""language"": ""ru"", ""value"": ""Тест""}]}}";
            //JObject obj = JObject.Parse(t);
            //Console.WriteLine(GetDepth(obj));
            //var reader = obj.CreateReader();
            //Console.WriteLine(reader.Depth);
            //reader.Read();
            //Console.WriteLine(reader.Depth);
           // reader.
           // obj.Children()
           /* foreach (var child in obj.Children())
            {
                Console.WriteLine(child);
                Console.WriteLine('\n');
                foreach (var c in child.Children())
                {
                    Console.WriteLine(c);
                }
                Console.WriteLine(child);
            }
            //Console.WriteLine(obj.);
            Console.WriteLine("j");
            /*foreach(var child in obj.Values())
            {
                Console.WriteLine($"=={child}");
            }*/
           var jsonValidator = new JsonValidationServer(args);
           jsonValidator.Start();

            //SimpleListenerExample(new string[] { "http://localhost:5431/" });
            //var server = new HttpServer();
            //var client = new HttpClient(server);

        }
    }
}
