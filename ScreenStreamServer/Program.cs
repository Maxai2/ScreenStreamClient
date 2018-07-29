using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System;
using System.IO.Compression;
using Newtonsoft.Json;

namespace ScreenStreamServer
{
    class Program
    {
        //static bool PlayStop = true;
        //static System.Timers.Timer timer;

        static void Main(string[] args)
        {
            //timer = new System.Timers.Timer();
            //timer.Interval = 10;

            //timer.Elapsed += SendScreen;
            //timer.AutoReset = true;
            //timer.Enabled = true;

            //List<byte[]> picChunkL = new List<byte[]>();

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // 65Kb
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7534);

            socket.Bind(ep);

            var SocketLength = socket.ReceiveBufferSize;

            var bytes = new byte[SocketLength];

            while (true)
            {
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    var length = socket.ReceiveFrom(bytes, ref client);
                    var msg = Encoding.Default.GetString(bytes, 0, length);

                    //picChunkL.Clear();

                    if (msg != "")
                    {
                        var sendBytes = SendScreen();

                        socket.SendTo(sendBytes, client);

                        msg = string.Empty;
                    }
                    else
                        break;
                }
            }
        }

        //------------------------------------------------------------------------

        static byte[] SendScreen()
        {
            Graphics graph = null;
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            graph = Graphics.FromImage(bmp);
            graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);

            byte[] picB = Encoding.Default.GetBytes(JsonConvert.SerializeObject(bmp));

            return picB;
        }
    }
}

            //using (var mStream = new MemoryStream())
            //{
            //    bmp.Save(mStream, ImageFormat.Jpeg);

            //    picB = mStream.ToArray();

            //    //using (var ds = new DeflateStream(mStream, CompressionLevel.Optimal))
            //    //{
            //    //    mStream.Write(picB, 0, picB.Length);
            //    //}
            //}
