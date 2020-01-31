using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace SharedEntity
{

    [Serializable]
    public enum ClientType : uint
    {
        Transmitter = 0,
        Receiver = 1,
    }

}
