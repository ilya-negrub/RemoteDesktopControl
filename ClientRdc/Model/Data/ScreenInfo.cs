using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedEntity;

namespace ClientRdc.Model.Data
{
    public class ScreenInfo : IScreenInfo
    {
        public bool Primary { get; set; }

        public string DeviceName { get; set; }

        public Rectangle Bounds { get; set; }

        public int BitsPerPixel { get; set; }
    }

    public static class ScreenInfoExtension
    {
        public static Rectangle Convert(this System.Drawing.Rectangle rect)
        {
            return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static System.Drawing.Rectangle Convert(this Rectangle rect)
        {
            return new System.Drawing.Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static ScreenInfo Convert(this System.Windows.Forms.Screen screen)
        {
            return new ScreenInfo
            {
                Primary = screen.Primary,
                DeviceName = screen.DeviceName,
                Bounds = screen.Bounds.Convert(),
                BitsPerPixel = screen.BitsPerPixel,
            };
        }
    }


}
