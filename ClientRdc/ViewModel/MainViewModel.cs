using ClientRdc.Model;
using ClientRdc.Model.Globals;
using ClientRdc.WebNotification.Listener;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientRdc.ViewModel
{
    public class MainViewModel : BaseViewModel
    {
        private ClientManagerViewModel clientManager = new ClientManagerViewModel();
        private ClientSharedViewModel clientShared = new ClientSharedViewModel();
        private BasaeApiListiner apiListiner = new WebApiListiner();

        public ClientManagerViewModel ClientManager => clientManager;
        public ClientSharedViewModel ClientShared => clientShared;

        public MainViewModel()
        {
            Task.Factory.StartNew(async () =>
            {
                apiListiner = await WebApiClient.SubscribeNotifications<WebApiListiner>(Variables.ClentGuid);                
            });
        }
    }
}
