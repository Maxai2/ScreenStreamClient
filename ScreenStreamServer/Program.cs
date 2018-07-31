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

            var SocketLength = socket.ReceiveBufferSize - 100;

            var bytes = new byte[SocketLength];

            while (true)
            {
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    var length = socket.ReceiveFrom(bytes, ref client);
                    var msg = Encoding.Default.GetString(bytes, 0, length);

                    Console.WriteLine(msg);

                    switch (msg)
                    {
                        case "Play":

                            break;
                        case "Connect":
                        case "Pause":
                            break;
                    }

                    //picChunkL.Clear();

                    if (msg != "")
                    {
                        var sendBytes = SendScreen();
                        var tempSendBytes = sendBytes.Length;
                        var sendBytesCounter = 0;
                        bool exit = true;

                        while (exit)
                        {
                            var sendArr = new byte[SocketLength];

                            Array.Copy(sendBytes, sendBytesCounter, sendArr, 0, sendArr.Length);

                            //for (int j = 0; j < sendArr.Length; ++j, ++i)
                            //{
                            //    sendArr[j] = sendBytes[i];
                            //}

                            socket.SendTo(sendArr, client);

                            tempSendBytes -= SocketLength;
                            sendBytesCounter += SocketLength;

                            if (tempSendBytes - SocketLength < 0)
                            {
                                SocketLength = tempSendBytes;
                                exit = false;
                                socket.SendTo(new byte[0], client);
                            }
                        }

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

            //byte[] picB = Encoding.Default.GetBytes(JsonConvert.SerializeObject(bmp));

            //var picB = new byte[1000000];

            using (var mStream = new MemoryStream())
            {
                bmp.Save(mStream, ImageFormat.Jpeg);

                return mStream.ToArray();

                //using (var ds = new DeflateStream(mStream, CompressionLevel.Optimal))
                //{
                //    mStream.Write(picB, 0, picB.Length);
                //}
            }

            //return picB;
        }
    }
}

