using ClientRdc.Helpers;
using ClientRdc.Helpers.SerializableEntity;
using SharedEntity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;
using PointWin = System.Windows.Point;

namespace ClientRdc.Model
{
    public class RdcTransmitter 
    {
        [Flags]
        public enum MouseEventFlags : uint
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }

        [System.Runtime.InteropServices.DllImport("User32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);


        int fps = 20;
        int bps = 5000;         // target bitrate. 5Mbps. 5000 * 1000
        float keyFrameInterval = 5.0f; // insert key frame interval. unit is second.


        public IEnumerable<Screen> Screens = Screen.AllScreens.OrderBy(o => o.Bounds.X);
        private TcpClient tcpClient;
        //https://github.com/secile/OpenH264Lib.NET
        OpenH264Lib.Encoder encoder;
        Screen screen;
        Bitmap bmp;
        Graphics graphics;


        private object lockEncode = new object();

        public RdcTransmitter()
        {
            encoder = new OpenH264Lib.Encoder("openh264-2.0.0-win32.dll");
            WebNotification.Listener.BasaeApiListiner.OnMessageChangeScreen += WebApiListiner_OnMessageChangeScreen;
        }

        private void WebApiListiner_OnMessageChangeScreen(SharedEntity.Message message, MessageChangeScreen data)
        {
            screen = Screen.AllScreens.Where(w => w.DeviceName == data.ScreenName).FirstOrDefault();
            if (screen != null)
            {
                lock (lockEncode)
                {
                    SetScreen(screen);
                }
            }
        }

        internal void Start(Guid guid, string hostname, int port)
        {   

            tcpClient = new TcpClient();
            tcpClient.Connect(hostname, port);
            var stream = tcpClient.GetStream();
            var binary = new BinaryFormatter();
            binary.Serialize(stream, (uint)ClientType.Transmitter);
            binary.Serialize(stream, guid.ToByteArray());

            Task.Factory.StartNew(() =>
            {
                SetScreen(Screen.PrimaryScreen);

                Screencast();
            });
        }

        private void SetScreen(Screen screen)
        {
            this.screen = screen;
            bmp = new Bitmap(screen.Bounds.Width, screen.Bounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            graphics = Graphics.FromImage(bmp);
            encoder.Setup(screen.Bounds.Width, screen.Bounds.Height, bps * 1000, fps, keyFrameInterval, OnEncode);
        }

        public void Stop()
        {
            tcpClient?.Close();
            tcpClient = null;
        }

        private bool IsRun()
        {
            return tcpClient != null && tcpClient.Connected && bmp != null;
        }

        private void Screencast()
        {  
            //Write data
            Task.Factory.StartNew(() => SharedData(datas));

            //Read screen
            Task.Factory.StartNew(() =>
            {
                while (IsRun())
                {
                    graphics.CopyFromScreen(screen.Bounds.X, screen.Bounds.Y, 0, 0, screen.Bounds.Size);
                }
                graphics?.Dispose();
            });

            //Encode img
            Task.Factory.StartNew(() =>
            {
                while (IsRun())
                {
                    lock (lockEncode)
                    {
                        encoder.Encode(bmp);
                    }
                }
            });

            //ReadData
            Task.Factory.StartNew(ReadData);
        }

        QueueWaitOfDequeue<byte[]> datas = new QueueWaitOfDequeue<byte[]>();

        private void OnEncode(byte[] data, int length, OpenH264Lib.Encoder.FrameType keyFrame)
        {
            datas.Clear(q => q.Count > 10);
            datas.Enqueue(data);
        }

        private void SharedData(QueueWaitOfDequeue<byte[]> datas)
        {
            Stream stream = tcpClient.GetStream();
            Task.Factory.StartNew(() => 
            {
                while (IsRun())
                {
                    var data = datas.Dequeue();
                    var binary = new BinaryFormatter();
                    binary.Serialize(stream, data);
                }
            });
        }

        private void ReadData()
        {
            InputSimulator inputSimulator = new InputSimulator();

            //read
            Task.Factory.StartNew(() =>
            {
                Stream stream = tcpClient.GetStream();
                while (IsRun())
                {                    
                    var binary = new BinaryFormatter();
                    var bytes = (byte[])binary.Deserialize(stream);
                    var command = bytes.DeserializeData<RemoteCommand>();
                    //points.Enqueue(data);

                    switch (command.Type)
                    {
                        case TypeCommand.MouseMove:
                            PointWin point = (PointWin)command.ObjData;
                            PointWin cursorPos = new PointWin(point.X * screen.Bounds.Width + screen.Bounds.X, point.Y * screen.Bounds.Height + screen.Bounds.Y);
                            SetCursorPos((int)cursorPos.X, (int)cursorPos.Y);
                            break;
                        case TypeCommand.MouseButtons:
                            var eMouse = (MouseButtonsEvent)command.ObjData;                            
                            PointWin pos = new PointWin(eMouse.Position.X * screen.Bounds.Width + screen.Bounds.X, eMouse.Position.Y * screen.Bounds.Height + screen.Bounds.Y);

                            uint cmd = (uint)MouseEventFlags.MOVE;
                            switch (eMouse.ChangedButton)
                            {
                                case System.Windows.Input.MouseButton.Left:
                                    cmd = (uint)(eMouse.ButtonState == System.Windows.Input.MouseButtonState.Pressed ?
                                    MouseEventFlags.LEFTDOWN : MouseEventFlags.LEFTUP);
                                    break;
                                case System.Windows.Input.MouseButton.Middle:
                                    cmd = (uint)(eMouse.ButtonState == System.Windows.Input.MouseButtonState.Pressed ?
                                   MouseEventFlags.MIDDLEDOWN : MouseEventFlags.MIDDLEUP);
                                    break;
                                case System.Windows.Input.MouseButton.Right:
                                    cmd = (uint)(eMouse.ButtonState == System.Windows.Input.MouseButtonState.Pressed ?
                                   MouseEventFlags.RIGHTDOWN : MouseEventFlags.RIGHTUP);
                                    break;
                                case System.Windows.Input.MouseButton.XButton1:
                                    break;
                                case System.Windows.Input.MouseButton.XButton2:
                                    break;
                            }

                            SetCursorPos((int)pos.X, (int)pos.Y);
                            mouse_event(cmd, (int)pos.X, (int)pos.Y, 0, 0);
                            break;
                        case TypeCommand.KeyboardEvent:
                            var eKey = (KeyboardEvent)command.ObjData;
                            var vkKey = (VirtualKeyCode)eKey.VkKey;

                            if (eKey.IsDown)
                                inputSimulator.Keyboard.KeyDown(vkKey);
                            else if (eKey.IsUp)
                                inputSimulator.Keyboard.KeyUp(vkKey);

                            break;
                    }
                    
                }
            });
        }
    }
}
