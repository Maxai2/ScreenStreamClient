﻿using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ScreenStreamClient
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        Socket socket;
        EndPoint ep;

        private string conDisConIp = "127.0.0.1";
        public string ConDisConIp
        {
            get { return conDisConIp; }
            set { conDisConIp = value; OnChanged(); }
        }

        private ImageSource screenPic;
        public ImageSource ScreenPic
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

        private Visibility streamingPanelVis;
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

        byte[] picB = new byte[1000000];

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

                            Streaming();
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

        void Streaming()
        {
            var answer = new byte[socket.ReceiveBufferSize - 100];

            while (true)
            {
                var msg = "Connect";
                var data = Encoding.Default.GetBytes(msg);
                socket.SendTo(data, ep);

                var length = socket.Receive(answer);

                if (length != 0)
                {
                    for (int i = 0, j = 0; i < answer.Length; i++, j++)
                    {
                        picB[j] = answer[i];
                    }

                }
                else
                    break;
            }

            ShowPic(picB);
        }

        //----------------------------------------------------------------------

        void ShowPic(byte[] arr)
        {
            ImageSourceConverter awd = new ImageSourceConverter();
            ImageSource source = (ImageSource)awd.ConvertFrom(arr);
            ScreenPic = source;
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
