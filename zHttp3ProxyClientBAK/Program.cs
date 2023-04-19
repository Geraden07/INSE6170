using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Http3ProxyClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using Socket sok = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            sok.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0));
            EndPoint remote = new IPEndPoint(IPAddress.Parse("10.0.1.171"), 4000);

            byte[] payload = Encoding.GetEncoding("UTF-8").GetBytes("Test Message LOOOOOOOOOONG");
            sok.SendTo(payload, remote);

            Console.WriteLine("Finished Sending");

            while (true)
            {
                byte[] buffer = new byte[4096];
                sok.ReceiveFrom(buffer, ref remote);
                
                if (remote.ToString() != "10.0.1.171:0")
                {
                    Console.WriteLine("\n###Continued");
                    continue;
                }

                byte firstByte = buffer[0];
                int helen = firstByte & 15;
                int messageStart = helen * 4;

                for (int i = messageStart; i < buffer.Length; ++i)
                    Console.Write((char)buffer[i]);
                Console.WriteLine("\n### End receive batch");
            }
            
        }
    }
}
