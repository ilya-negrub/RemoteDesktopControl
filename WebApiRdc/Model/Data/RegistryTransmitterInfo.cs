using SharedEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiRdc.Model.Data
{
    public class RegistryTransmitterInfo : IRegistryTransmitterInfo
    {
        public Guid Guid { get; set; }
        public IEnumerable<ScreenInfo> Screens { get; set; }
        IEnumerable<IScreenInfo> IRegistryTransmitterInfo.Screens => Screens;
    }
}
