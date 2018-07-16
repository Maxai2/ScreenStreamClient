using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ScreenStreamServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7534);

            var answer = new byte[8192];

            while (true)
            {
                var msg = Console.ReadLine(); ;
                var data = Encoding.Default.GetBytes(msg);
                socket.SendTo(data, ep);
            }
        }
    }
}
