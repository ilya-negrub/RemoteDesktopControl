using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ClientRdc.Controls.Helpers
{
    public static class ImageExtensions
    {
        public static BitmapImage ToBitmapImage(this Bitmap bmp, System.Drawing.Imaging.ImageFormat imageFormat)
        {
            MemoryStream ms = new MemoryStream();
            bmp.Save(ms, imageFormat);
            BitmapImage img = new BitmapImage();
            img.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            img.StreamSource = ms;
            img.EndInit();
            return img;
        }

        public static BitmapImage ToBitmapImage(this byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var image = new BitmapImage();
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad; // here
                image.StreamSource = ms;
                image.EndInit();
                return image;
            }
        }
    }
}
