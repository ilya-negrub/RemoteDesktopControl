using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiRdc.Services.Notification
{
    public class PushStreamResult<TPar> : IActionResult
    {
        private readonly Action<Stream, CancellationToken, TPar> onStreamAvailable;
        private readonly string contentType;
        private readonly CancellationToken requestAborted;
        private readonly TPar par;

        public PushStreamResult(Action<Stream, CancellationToken, TPar> onStreamAvailable, string contentType, CancellationToken requestAborted, TPar par)
        {
            this.onStreamAvailable = onStreamAvailable;
            this.contentType = contentType;
            this.requestAborted = requestAborted;
            this.par = par;
        }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var stream = context.HttpContext.Response.Body;
            context.HttpContext.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue(contentType);
            onStreamAvailable(stream, requestAborted, par);
            return Task.CompletedTask;
        }
    }
}
