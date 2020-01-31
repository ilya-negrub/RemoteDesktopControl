using SharedEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiRdc.Model.Data
{
    public class ScreenInfo : IScreenInfo
    {
        public bool Primary { get; set; }

        public string DeviceName { get; set; }

        public Rectangle Bounds { get; set; }

        public int BitsPerPixel { get; set; }
    }
}
