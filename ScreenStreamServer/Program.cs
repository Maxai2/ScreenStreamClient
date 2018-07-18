using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing.Imaging;
using System.IO.Compression;
using System.Collections.Generic;
using System;

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

            var SocketLength = socket.ReceiveBufferSize / 2;

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

                        var picLength = sendBytes.Length;
                        var tempLength = sendBytes.Length;
                        //var parts = picLength % SocketLength + 1;
                        int minChunkLength = SocketLength;

                        for (int i = 0; i < picLength;)
                        {
                            var chunk = new byte[minChunkLength];

                            for (int j = 0; j < chunk.Length; j++, i++)
                            {
                                chunk[j] = sendBytes[i];
                            }

                            socket.SendTo(chunk, client);
                            //picChunkL.Add(chunk);

                            if (tempLength - SocketLength > 0)
                            {
                                tempLength -= SocketLength;

                                if (tempLength < SocketLength)
                                {
                                    minChunkLength = tempLength;
                                }
                            }
                        }

                        //var binFormatter = new BinaryFormatter();
                        //var mStream = new MemoryStream();

                        //binFormatter.Serialize(mStream, picChunkL);
                        //socket.SendTo(mStream.ToArray(), client);

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

            byte[] picB = null;

            using (var mStream = new MemoryStream())
            {
                bmp.Save(mStream, ImageFormat.Jpeg);

                picB = mStream.ToArray();
            }

            return picB;
        }
    }
}

//byte[] picB = null;
//  picB = mStream.ToArray();

//using (var wms = new MemoryStream())
//{
//    using (var ds = new GZipStream(wms, CompressionMode.Compress))
//    {
//        mStream.CopyTo(ds);
//    }
//}
