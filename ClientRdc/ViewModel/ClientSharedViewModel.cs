using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientRdc.Model;
using ClientRdc.Model.Data;
using ClientRdc.Model.Globals;
using SharedEntity;

namespace ClientRdc.ViewModel
{
    public class ClientSharedViewModel : BaseViewModel
    {
        private bool isRun;
        private RdcTransmitter transmitter = new RdcTransmitter();

        private Guid guid = Variables.ClentGuid;
        public Guid Guid => guid;

        public ClientSharedViewModel()
        {
            WebNotification.Listener.BasaeApiListiner.OnMessageAccessToConnection += WebApiListiner_OnMessageAccessToConnection;
        }

        

        private Command startCommand;
        public ICommand StartCommand => startCommand ?? (startCommand = new Command(Start_Click, () => !isRun));

        private async void Start_Click()
        {
            isRun = true;
            startCommand.CanExecuteChangedInvoke();

            var info = new RegistryTransmitterInfo()
            {
                Guid = guid,
                Screens = transmitter.Screens.Select(s => s.Convert()).ToList()
            };

            await WebApiClient.RegistryTransmitter(info);
            
            

            isRun = false;
            startCommand.CanExecuteChangedInvoke();
        }


        private async void WebApiListiner_OnMessageAccessToConnection(Message message, MessageAccessToConnection data)
        {
            var res = System.Windows.MessageBox.Show($"User {data.UserName} requests the right to join", "Connection", System.Windows.MessageBoxButton.YesNo, System.Windows.MessageBoxImage.Question);
            if (res == System.Windows.MessageBoxResult.Yes)
            {
                if (await WebApiClient.SendMessage<MessageRunRdcReceiver>(new Message
                {
                    From = guid,
                    To = message.From,
                },
                new MessageRunRdcReceiver()
                {
                    IsSuccess = true,
                    Message = "",
                }))
                {
                    transmitter.Start(guid, Variables.TcpHostName, Variables.TcpPort);
                }
            }
            else
            {
                await WebApiClient.SendMessage<MessageRunRdcReceiver>(new Message
                {
                    From = guid,
                    To = message.From,
                },
                new MessageRunRdcReceiver()
                {
                    IsSuccess = false,
                    Message = "",
                });
            }
        }

    }
}
