using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ScreenStreamClient
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        Socket socket;
        EndPoint ep;

        private string conDisConIp;
        public string ConDisConIp
        {
            get { return conDisConIp; }
            set { conDisConIp = value; OnChanged(); }
        }

        private string screenPic;
        public string ScreenPic
        {
            get { return screenPic; }
            set { screenPic = value; OnChanged(); }
        }

        private Visibility conButVis;
        public Visibility ConButVis
        {
            get { return conButVis; }
            set { conButVis = value; OnChanged(); }
        }

        private Visibility disconButVis = Visibility.Collapsed;
        public Visibility DisconButVis
        {
            get { return disconButVis; }
            set { disconButVis = value; OnChanged(); }
        }

        private Visibility streamingPanelVis = Visibility.Collapsed;
        public Visibility StreamingPanelVis
        {
            get { return streamingPanelVis; }
            set { streamingPanelVis = value; OnChanged(); }
        }

        private Visibility pauseButVis;
        public Visibility PauseButVis
        {
            get { return pauseButVis; }
            set { pauseButVis = value; OnChanged(); }
        }

        private Visibility playButVis;
        public Visibility PlayButVis
        {
            get { return playButVis; }
            set { playButVis = value; OnChanged(); }
        }

        //----------------------------------------------------------------------

        private ICommand connectCom;
        public ICommand ConnectCom
        {
            get
            {
                if (connectCom is null)
                {
                    connectCom = new RelayCommand(
                        (param) =>
                        {
                            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                            ep = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 7534);

                            StreamingPanelVis = Visibility.Visible;

                            ConButVis = Visibility.Collapsed;
                            DisconButVis = Visibility.Visible;
                        },
                        (param) =>
                        {
                            if (ConDisConIp != "")
                                return true;
                            else
                                return false;
                        });
                }

                return connectCom;
            }
        }

        private ICommand disconnectCom;
        public ICommand DisconnectCom
        {
            get
            {
                if (disconnectCom is null)
                {
                    disconnectCom = new RelayCommand(
                        (param) =>
                        {
                            socket.Shutdown(SocketShutdown.Both);
                            socket.Close();

                            ConButVis = Visibility.Visible;
                            DisconButVis = Visibility.Collapsed;
                        });
                }

                return disconnectCom;
            }
        }

        private ICommand pauseCom;
        public ICommand PauseCom
        {
            get
            {
                if (pauseCom is null)
                {
                    pauseCom = new RelayCommand(
                        (param) =>
                        {

                        });
                }

                return pauseCom;
            }
        }
        public DependencyObjectType Sourcecc { get; set; }

        private ICommand playCom;
        public ICommand PlayCom
        {
            get
            {
                if (playCom is null)
                {
                    playCom = new RelayCommand(
                        (param) =>
                        {
                            while (true)
                            {
                                Streaming();

                                using (var ms = new MemoryStream(picB))
                                {
                                    var image = Bitmap.FromStream(ms);
                                    ImageSourceConverter awd = new ImageSourceConverter();
                                    var source = (ImageSource)awd.ConvertFrom(image);
                                    Im.Source = source;
                                }
                            }
                        });
                }

                return playCom;
            }
        }

        //----------------------------------------------------------------------

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;
        }

        //----------------------------------------------------------------------

        byte[] picB = new byte[1000000000];

        void Streaming()
        {
            var msg = "Connect";
            var data = Encoding.Default.GetBytes(msg);
            socket.SendTo(data, ep);

            int i = 0;

            while (true)
            {
                var answer = new byte[socket.ReceiveBufferSize / 2];

                var length = socket.Receive(answer);

                if (length != 0)
                {
                    for (int j = 0; j < length; j++, i++)
                    {
                        picB[i] = answer[j];
                    }
                }
                else
                    break;
            }
        }

        //----------------------------------------------------------------------

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnChanged([CallerMemberName]string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //----------------------------------------------------------------------
    }
}
