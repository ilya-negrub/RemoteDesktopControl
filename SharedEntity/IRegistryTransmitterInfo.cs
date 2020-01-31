using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEntity
{
    public interface IRegistryTransmitterInfo
    {
        Guid Guid { get; }
        IEnumerable<IScreenInfo> Screens { get; }
    }

    public interface IScreenInfo
    {
        bool Primary { get; }
        string DeviceName { get; }
        Rectangle Bounds { get; }
        int BitsPerPixel { get; }
    }

    public struct Rectangle
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Y { get; set; }
        public int X { get; set; }

        public Rectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}
