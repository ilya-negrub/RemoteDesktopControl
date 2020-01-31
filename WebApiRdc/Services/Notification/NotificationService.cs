using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedEntity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiRdc.Services.Notification
{
    public class NotificationService
    {
        private static ConcurrentBag<ClientNotification> subscribers = new ConcurrentBag<ClientNotification>();

        public IEnumerable<ClientNotification> Subscribers => subscribers;


        public IActionResult Subscribe(HttpContext context, Guid guid)
        {
            return new PushStreamResult<Guid>(OnStreamAvailable, "text/event-stream", context.RequestAborted, guid);
        }

        private async void OnStreamAvailable(Stream stream, CancellationToken requestAborted, Guid guid)
        {
            if (subscribers.Any(a => a.Guid == guid))
                return;


            var wait = requestAborted.WaitHandle;
            var writer = new StreamWriter(stream);
            var client = new ClientNotification(guid, writer);
            subscribers.Add(client);
            
            await writer.FlushAsync();

            wait.WaitOne();

            ClientNotification ignore;
            subscribers.TryTake(out ignore);
        }

        internal bool Send(Message message)
        {
            var client = subscribers.Where(w => w.Guid == message.To).FirstOrDefault();
            if (client != null)
            {
                client.Send(message);
                return true;
            }
            return false;
        }
    }
}
