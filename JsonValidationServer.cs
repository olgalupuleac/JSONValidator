using System;
using System.Linq;
using System.Net;
using Newtonsoft.Json.Linq;
using System.IO;
using Newtonsoft.Json;
using NLog;
using NLog.Conditions;
using NLog.Targets;

namespace JSONValidator
{
    public class JsonValidationServer
    {
        private static readonly string ErrorMessage = new JProperty("error", "Invalid JSON").ToString();

        static JsonValidationServer()
        {
            var consoleTarget = new ColoredConsoleTarget();
            var highlightInfoRule = new ConsoleRowHighlightingRule
            {
                Condition = ConditionParser.ParseExpression("level == LogLevel.Info"),
                ForegroundColor = ConsoleOutputColor.Green
            };
            var highlightErrorRule = new ConsoleRowHighlightingRule
            {
                Condition = ConditionParser.ParseExpression("level == LogLevel.Error"),
                ForegroundColor = ConsoleOutputColor.Red
            };
            consoleTarget.RowHighlightingRules.Add(highlightInfoRule);
            consoleTarget.RowHighlightingRules.Add(highlightErrorRule);
            var config = new NLog.Config.LoggingConfiguration();
            config.AddRule(LogLevel.Info, LogLevel.Fatal, consoleTarget);
            LogManager.Configuration = config;
            Logger = LogManager.GetCurrentClassLogger();
        }

        public JsonValidationServer(string[] uris)
        {
            UriStrings = uris;
        }

        private static readonly Logger Logger;

        public string[] UriStrings { get; }

        public void Start()
        {
            try
            {
                if (!HttpListener.IsSupported)
                {
                    Logger.Error("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                    return;
                }

                var listener = new HttpListener();
                foreach (var uriString in UriStrings)
                {
                    listener.Prefixes.Add(uriString);
                }

                listener.Start();
                Logger.Info("Listening...");
                while (true)
                {
                    var context = listener.GetContext();
                    var request = context.Request;
                    Logger.Info("Got request");
                    var responseString = GetResponse(request);
                    Logger.Info($"\nJson response\n=========\n{responseString}\n=========\n");
                    var response = context.Response;
                    response.ContentType = "application/json";
                    using (var output = response.OutputStream)
                    {
                        var buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                        response.ContentLength64 = buffer.Length;
                        output.Write(buffer, 0, buffer.Length);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
            }
        }

        private static string GetResponse(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Logger.Warn("No client data was sent with the request.");
                return ErrorMessage;
            }

            using (var body = request.InputStream)
            {
                var encoding = request.ContentEncoding;
                using (var reader = new StreamReader(body, encoding))
                {
                    return HandleJson(reader.ReadToEnd());
                }
            }
        }

        private static string HandleJson(string jsonString)
        {
            Logger.Info($"\nJson request\n=========\n{jsonString}\n=========\n");
            try
            {
                var o = JObject.Parse(jsonString);
                return new JProperty("levels", GetDepth(o)).ToString();
            }
            catch (JsonReaderException e)
            {
                Logger.Warn(e);
                return ErrorMessage;
            }
        }

        private static int GetDepth(JToken jToken)
        {
            var depth = 0;
            switch (jToken.Type)
            {
                case JTokenType.Object:
                    depth = jToken.Children<JProperty>().Select(child => GetDepth(child) + 1).Concat(new[] {depth})
                        .Max();
                    break;
                case JTokenType.Array:
                    depth = jToken.Children().Select(child => GetDepth(child) + 1).Concat(new[] {depth}).Max();
                    break;
                case JTokenType.Property:
                    depth = GetDepth(((JProperty) jToken).Value);
                    break;
            }

            return depth;
        }
    }
}