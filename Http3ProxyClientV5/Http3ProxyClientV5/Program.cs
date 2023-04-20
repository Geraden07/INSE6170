using QuickNet.Utilities;
using QuicNet;
using QuicNet.Connections;
using QuicNet.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Http3ProxyClientV5
{
    public class Program
    {
        public static void Main(string[] args)
        {
            QuicClient quicclient = new QuicClient();
            QuicConnection quicconnection = quicclient.Connect("127.0.0.1", 4000);
            QuicStream stream = quicconnection.CreateStream(StreamType.ClientBidirectional);

            string httpVerb = "GET";
            string httpUri = "https://arstechnica.com/";
            string httpVersion = "HTTP/3";
            string httpBody = "";
            string[] headers = new string[] {
                "User-Agent: INSE6170-Project-Client",
                "Accept-Language: en-US",
                "Accept-Charset: utf-8",
                "Accept: text/html",
                "Cache-Control: no-cache"
            };

            string httpRequestFrame = $"{httpVerb} {httpUri} {httpVersion}\n";
            foreach (string header in headers)
                httpRequestFrame += $"{header}\n";
            httpRequestFrame += $"{httpBody}";

            stream.Send(Encoding.UTF8.GetBytes(httpRequestFrame));
            byte[] data = stream.Receive(); // blocks until data received

            Console.WriteLine(Encoding.UTF8.GetString(data));

        }
    }
}
