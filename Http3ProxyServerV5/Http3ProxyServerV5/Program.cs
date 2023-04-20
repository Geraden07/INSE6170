using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuicNet;
using QuicNet.Streams;
using QuicNet.Connections;
using System.Net.Http;
using System.Net;

namespace Http3ProxyServerV5
{
    public class Program
    {
        private static readonly HttpClient client = new HttpClient();

        public static void ClientConnected(QuicConnection connection)
        {
            connection.OnStreamOpened += StreamOpened;
            Console.WriteLine($"Client Connected");
        }

        public static void StreamOpened(QuicStream stream)
        {
            stream.OnStreamDataReceived += StreamDataReceived;
        }

        public static void StreamDataReceived(QuicStream stream, byte[] data)
        {
            string decoded = Encoding.UTF8.GetString(data);
            Console.WriteLine("--------------------------------------");
            Console.WriteLine($"Received:\n{decoded}");
            Console.WriteLine("--------------------------------------");
            Task<string> responseTask = HandleHttpRequest(decoded);
            responseTask.Wait();
            string response = responseTask.Result;
            Console.WriteLine($"Response:\n{response}");
            stream.Send(Encoding.UTF8.GetBytes(response));
        }

        public static void Main(string[] args)
        {
            QuicListener listener = new QuicListener(4000);
            listener.OnClientConnected += ClientConnected;

            listener.Start();
        }

        private async static Task<string> HandleHttpRequest(string request)
        {
            string[] requestLines = request.Split('\n');
            string[] requestLine = requestLines[0].Split(' ');
            HttpMethod method = new HttpMethod(requestLine[0]);
            string requestUri = requestLine[1];
            HttpRequestMessage outgoingRequest = new HttpRequestMessage(method, requestUri);
            outgoingRequest.Version = HttpVersion.Version11;
            
            // Loop through lines
            foreach(string headerLine in requestLines.Skip(1).ToArray())
            {
                if (headerLine.Contains(":"))
                {
                    string[] currentHeader = headerLine.Split(':');
                    outgoingRequest.Headers.Add(currentHeader[0].Trim(), currentHeader[1].Trim());
                }
                else
                {
                    break; // Once we reach a line that does not have a header
                }
            }

            HttpResponseMessage response = await client.SendAsync(outgoingRequest);
            
            string returnString = $"HTTP/{response.Version} {(int)response.StatusCode} {response.ReasonPhrase}\n";
            foreach(KeyValuePair<string, IEnumerable<string>> header in response.Headers)
            {
                foreach(string headerValue in header.Value)
                {
                    returnString += $"{header.Key}: {headerValue}\n";
                }                
            }
            foreach (KeyValuePair<string, IEnumerable<string>> header in response.Content.Headers)
            {
                foreach (string headerValue in header.Value)
                {
                    returnString += $"{header.Key}: {headerValue}\n";
                }
            }
            returnString += "\n";
            returnString += await response.Content.ReadAsStringAsync();

            return returnString;
        }
    }
}
