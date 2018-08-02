using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System;

namespace ScreenStreamServer
{
    class Program
    {
     //   static bool PlayStop = true;
        static System.Timers.Timer timer;

        static Socket socket;
        static EndPoint ep;
        static EndPoint client;
        static int SocketLength;

        static void Main(string[] args)
        {
            timer = new System.Timers.Timer();
            timer.Interval = 1000;

            timer.Elapsed += ((s, e) => StreamingScreenToClient() );
            timer.AutoReset = true;
            //timer.Enabled = true;

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // 65Kb
            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7534);

            socket.Bind(ep);

            SocketLength = socket.ReceiveBufferSize - 100;
            var bytes = new byte[SocketLength];

            while (true)
            {
                NewClient:
                client = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    var length = socket.ReceiveFrom(bytes, ref client);
                    var msg = Encoding.Default.GetString(bytes, 0, length);

                    Console.WriteLine(msg);

                    switch (msg)
                    {
                        case "Pause":
                            timer.Stop();
                            break;
                        case "Play":
                            timer.Start();
                            break;
                        case "":
                            goto NewClient;
                    }
                }
            }
        }

        //------------------------------------------------------------------------

        static void StreamingScreenToClient()
        {
            Graphics graph = null;
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            graph = Graphics.FromImage(bmp);
            graph.CopyFromScreen(0, 0, 0, 0, bmp.Size);

            byte[] sendBytes = null;

            using (var mStream = new MemoryStream())
            {
                bmp.Save(mStream, ImageFormat.Jpeg);

                sendBytes = mStream.ToArray();
            }

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
            }

                //using (var ds = new DeflateStream(mStream, CompressionLevel.Optimal))
                //{
                //    mStream.Write(picB, 0, picB.Length);
                //}

            //return picB;
        }
    }
}

