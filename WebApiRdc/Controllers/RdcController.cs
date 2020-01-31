using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedEntity;
using WebApiRdc.Model.Data;
using WebApiRdc.Services.Notification;
using WebApiRdc.Services.RDC;

namespace WebApiRdc.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RdcController : ControllerBase
    {
        private NotificationService notificationService;
        private RdcService rdcService;

        public RdcController(NotificationService notificationService, RdcService rdcService)
        {
            this.notificationService = notificationService;
            this.rdcService = rdcService;
        }


        // GET: api/Rdc
        [HttpGet]
        public IEnumerable<Guid> Get()
        {
            return GetSubscribers();
        }

        // GET: api/Rdc/GetSubscribers
        [HttpGet()]
        [Route("GetSubscribers")]
        public IEnumerable<Guid> GetSubscribers()
        {
            return notificationService.Subscribers.Select(s => s.Guid);
        }

        [HttpGet()]
        [Route("GetRdcClients")]
        public IEnumerable<Guid> GetRdcClients()
        {
            return rdcService.RegistryTransmitters;
        }

        // POST: api/Rdc/RegistryTransmitter
        [HttpPost]
        [Route("RegistryTransmitter")]
        public void RegistryTransmitter([FromBody] RegistryTransmitterInfo info)
        {
            rdcService.RegistryTransmitter(info);
        }

        [HttpGet]
        [Route("Subscribe/{guid}")]
        public IActionResult Subscribe(Guid guid)
        {
            return notificationService.Subscribe(HttpContext, guid);
        }

        [HttpPost]
        [Route("Send")]
        public IActionResult Send([FromBody]Message message)
        {
            if (notificationService.Send(message))
                return Ok();
            else
                return NotFound();
        }

        [HttpPost]
        [Route("GetTransmitterInfo")]
        public RegistryTransmitterInfo GetTransmitterInfo([FromBody]Guid guid)
        {
            return rdcService.GetTransmitterInfo(guid);
        }

        [HttpGet]
        [Route("ChangeScreen/{guid}")]
        public void ChangeScreen(Guid guid, string screenName)
        {
            if (rdcService.ChangeScreen(guid, screenName))
            {
                var message = new Message
                {
                    From = Guid.Empty,
                    To = guid,
                    Type = typeof(MessageChangeScreen).Name,
                    Data = JsonConvert.SerializeObject(new MessageChangeScreen { ScreenName = screenName }),
                };

                notificationService.Send(message);
            }
        }
    }
}
