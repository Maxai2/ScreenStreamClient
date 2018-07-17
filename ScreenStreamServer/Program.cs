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

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // 65Kb
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7534);

            socket.Bind(ep);

            var bytes = new byte[socket.ReceiveBufferSize];

            while (true)
            {
                EndPoint client = new IPEndPoint(IPAddress.Any, 0);

                while (true)
                {
                    var length = socket.ReceiveFrom(bytes, ref client);
                    var msg = Encoding.Default.GetString(bytes, 0, length);

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

            using (var mStream = new MemoryStream())
            {
                bmp.Save(mStream, ImageFormat.Jpeg);

            }
            return mStream.ToArray();

            using (var wms = new MemoryStream())
            {
                using (var ds = new DeflateStream(wms, CompressionMode.Compress))
                {
                    ds.Write();
                } 
            }

        }
    }
}
