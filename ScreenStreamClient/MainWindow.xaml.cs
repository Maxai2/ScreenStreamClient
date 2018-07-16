using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScreenStreamClient
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp); // 65Kb
            var ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7534);

            socket.Bind(ep);

            var bytes = new byte[socket.ReceiveBufferSize];
            EndPoint client = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var length = socket.ReceiveFrom(bytes, ref client);
                var msg = Encoding.Default.GetString(bytes, 0, length);
            }
        }
    }
}
