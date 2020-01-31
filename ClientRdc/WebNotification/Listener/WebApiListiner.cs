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
    public class WebApiListiner : BasaeApiListiner
    {   

        private static Dictionary<string, Action<Message>> dicAction = new Dictionary<string, Action<Message>>()
        {
            { typeof(MessageAccessToConnection).Name, (message) => OnMessageAccessToConnectionInvoke(message) },
            { typeof(MessageRunRdcReceiver).Name, (message) => OnMessageRunRdcReceiverInvoke(message) },
            { typeof(MessageChangeScreen).Name, (message) => OnMessageChangeScreenInvoke(message) },
        };

        public override void Listening(Stream stream)
        {
            var reader = new StreamReader(stream);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();

                try
                {
                    if (JsonConvert.DeserializeObject<Message>(line) is Message message)
                    {
                        if (dicAction.TryGetValue(message.Type, out Action<Message> action))
                        {
                            action?.Invoke(message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"-------. \nException: {ex.Message}");
                }
            }
        }
    }
}
