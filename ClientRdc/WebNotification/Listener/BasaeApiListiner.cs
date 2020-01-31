using Newtonsoft.Json;
using SharedEntity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientRdc.WebNotification.Listener
{
    public delegate void EventHandlerMessageAccessToConnection(Message message, MessageAccessToConnection data);
    public delegate void EventHandlerMessageRunRdcReceiver(Message message, MessageRunRdcReceiver data);
    public delegate void EventHandlerMessageChangeScreen(Message message, MessageChangeScreen data);

    public abstract class BasaeApiListiner
    {
        public static event EventHandlerMessageAccessToConnection OnMessageAccessToConnection;
        public static event EventHandlerMessageRunRdcReceiver OnMessageRunRdcReceiver;
        public static event EventHandlerMessageChangeScreen OnMessageChangeScreen;

        public abstract void Listening(Stream stream);

        protected static void OnMessageAccessToConnectionInvoke(Message message)
            => OnMessageAccessToConnection?.Invoke(message, JsonConvert.DeserializeObject<MessageAccessToConnection>(message.Data));

        protected static void OnMessageRunRdcReceiverInvoke(Message message)
            => OnMessageRunRdcReceiver?.Invoke(message, JsonConvert.DeserializeObject<MessageRunRdcReceiver>(message.Data));

        protected static void OnMessageChangeScreenInvoke(Message message)
            => OnMessageChangeScreen?.Invoke(message, JsonConvert.DeserializeObject<MessageChangeScreen>(message.Data));
    }
}
