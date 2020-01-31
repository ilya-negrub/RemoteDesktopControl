using Newtonsoft.Json;
using SharedEntity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiRdc.Services.Notification
{
    public class ClientNotification
    {
        private Guid guid;
        private StreamWriter stream;

        public Guid Guid => guid;

        public ClientNotification(Guid guid, StreamWriter stream)
        {
            this.guid = guid;
            this.stream = stream;            
        }

        public void Send(Message message)
        {
            string json = JsonConvert.SerializeObject(message);
            stream.WriteLineAsync(json);
            stream.FlushAsync();
        }
    }
}
