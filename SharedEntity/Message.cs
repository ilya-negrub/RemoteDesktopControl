using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEntity
{
    public class Message
    {
        public Guid From { get; set; }
        public Guid To { get; set; }
        public string Type { get; set; }
        public string Data { get; set; }
    }

    public class MessageBox
    {
        public int Num { get; set; }
        public string Name { get; set; }
        public DateTime DateTime { get; set; }
    }

    public class MessageAccessToConnection
    {
        public string UserName { get; set; }
    }

    public class MessageRunRdcReceiver
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } 
    }

    public class MessageChangeScreen
    {
        public string ScreenName { get; set; }
    }
}
