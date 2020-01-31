using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ClientRdc.Controls.Helpers;
using ClientRdc.Helpers;
using ClientRdc.Helpers.SerializableEntity;
using SharedEntity;
using Drawing = System.Drawing;

namespace ClientRdc.Controls
{
    public class RdcReceiverContent : FrameworkElement
    {
        #region Fileds        
        private Point cursorPossition;
        private BitmapImage imageSource;
        private TcpClient tcpClient;

        QueueWaitOfDequeue<byte[]> sendData = new QueueWaitOfDequeue<byte[]>();
        #endregion

        #region DependencyProperty

        private static void InvalidateVisualPropChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is RdcReceiverContent rdcControl)
                rdcControl.InvalidateVisual();
        }




        public string HostName
        {
            get { return (string)GetValue(HostNameroperty); }
            set { SetValue(HostNameroperty, value); }
        }

        
        public static readonly DependencyProperty HostNameroperty =
            DependencyProperty.Register("HostName", typeof(string), typeof(RdcReceiverContent), new PropertyMetadata(null));



        public int Port
        {
            get { return (int)GetValue(PortProperty); }
            set { SetValue(PortProperty, value); }
        }

        public static readonly DependencyProperty PortProperty =
            DependencyProperty.Register("Port", typeof(int), typeof(RdcReceiverContent), new PropertyMetadata(0));




        public Guid GuidTransmitters
        {
            get { return (Guid)GetValue(GuidTransmittersProperty); }
            set { SetValue(GuidTransmittersProperty, value); }
        }

        
        public static readonly DependencyProperty GuidTransmittersProperty =
            DependencyProperty.Register("GuidTransmitters", typeof(Guid), typeof(RdcReceiverContent), new PropertyMetadata(Guid.Empty));




        public bool IsShowFps
        {
            get { return (bool)GetValue(IsShowFpsProperty); }
            set { SetValue(IsShowFpsProperty, value); }
        }
        
        public static readonly DependencyProperty IsShowFpsProperty =
            DependencyProperty.Register("IsShowFps", typeof(bool), typeof(RdcReceiverContent), new PropertyMetadata(false, InvalidateVisualPropChange));



        #endregion



        public RdcReceiverContent()
        {
            this.Loaded += RdcReceiverContent_Loaded;
            this.Unloaded += RdcReceiverContent_Unloaded;

            if (System.Windows.Application.Current?.MainWindow is Window window)
            {
                window.PreviewKeyDown += Window_KeyDown;
                window.PreviewKeyUp += Window_KeyUp;
            }
        }

        

        private void RdcReceiverContent_Loaded(object sender, RoutedEventArgs e)
        {
            StartTcpClient();            
        }

        private void RdcReceiverContent_Unloaded(object sender, RoutedEventArgs e)
        {
            StopTcpClient();
        }


        private void StartTcpClient()
        {
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(this.HostName, this.Port);

                NetworkStream stream = tcpClient.GetStream();               


                var binary = new BinaryFormatter();
                binary.Serialize(stream, (uint)ClientType.Receiver);
                binary.Serialize(stream, GuidTransmitters.ToByteArray());

                //Reader
                Task.Factory.StartNew(() => 
                {
                    var decoder = new OpenH264Lib.Decoder("openh264-2.0.0-win32.dll");


                    while (tcpClient?.Connected == true)
                    {
                        try
                        {
                            var bin = new BinaryFormatter();
                            var data = (byte[])bin.Deserialize(stream);

                            var bmp = decoder.Decode(data, data.Length);
                            if (bmp != null)
                            {
                                System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
                                {
                                    SetBmp(bmp);
                                    this.InvalidateVisual();
                                });
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed read data. Ex: {ex.Message}");
                        }
                    }
                });

                //Writer
                Task.Factory.StartNew(() => 
                {
                    while (tcpClient?.Connected == true)
                    {
                        var data = sendData.Dequeue();
                        if (stream.CanWrite)
                        {
                            var bin = new BinaryFormatter();
                            bin.Serialize(stream, data);
                        }
                        else
                            sendData.Enqueue(data);

                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed connection. Ex: {ex.Message}");
            }
        }

        private void StopTcpClient()
        {            
            tcpClient?.Close();
            tcpClient = null;
        }


        private void SetBmp(Drawing.Bitmap bmp)
        {
            imageSource = bmp.ToBitmapImage(Drawing.Imaging.ImageFormat.Bmp);
            Width = bmp.Width;
            Height = bmp.Height;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var p = e.GetPosition(this);

            if (imageSource != null)
            {
                cursorPossition = new Point(p.X / imageSource.Width, p.Y / imageSource.Height);


                var command = new RemoteCommand(TypeCommand.MouseMove, cursorPossition);
                sendData.Enqueue(command.SerializeData());
            }
        }


        private void Window_KeyDown(object sender, KeyEventArgs e)
        {   
            var command = new RemoteCommand(TypeCommand.KeyboardEvent, e.Convert());
            sendData.Enqueue(command.SerializeData());
            e.Handled = true;

            System.Diagnostics.Debug.WriteLine($"Key:{e.Key}");
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            var command = new RemoteCommand(TypeCommand.KeyboardEvent, e.Convert());
            sendData.Enqueue(command.SerializeData());
            e.Handled = true;
        }


        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            Point p = e.GetPosition(this);
            Point pos = new Point(p.X / imageSource.Width, p.Y / imageSource.Height);

            var command = new RemoteCommand(TypeCommand.MouseButtons, e.Convert(pos));
            sendData.Enqueue(command.SerializeData());
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            Point p = e.GetPosition(this);
            Point pos = new Point(p.X / imageSource.Width, p.Y / imageSource.Height);
            
            var command = new RemoteCommand(TypeCommand.MouseButtons, e.Convert(pos));
            sendData.Enqueue(command.SerializeData());
        }

        DateTime dtFps = DateTime.Now;
        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);


            if (imageSource == null)
            {
                dc.DrawRectangle(Brushes.Black, new Pen(), rect);
                return;
            }

            dc.DrawImage(imageSource, rect);


            if (IsShowFps)
            {
                var fpsView = 1000 / (DateTime.Now - dtFps).TotalMilliseconds;
                dtFps = DateTime.Now;

                dc.DrawText(
                new FormattedText(fpsView.ToString("0.00"),
                    System.Globalization.CultureInfo.GetCultureInfo("ru-ru"),
                    FlowDirection.LeftToRight,
                    new Typeface("Verdana"),
                    36, System.Windows.Media.Brushes.Red),
                    new System.Windows.Point(0, 0));
            }
        }

    }
}
