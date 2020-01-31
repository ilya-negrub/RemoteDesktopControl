using ClientRdc.Model;
using ClientRdc.Model.Globals;
using SharedEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientRdc.ViewModel
{
    public class TransmittersViewModel : BaseViewModel
    {
        public string UserName { get; set; } = $"User ID: [{Variables.ClentGuid}]";

        private bool isConnecting = false;
        private Action<Guid> connectCallBack;
        private IEnumerable<Guid> registryTransmitters;
        public IEnumerable<Guid> RegistryTransmitters => registryTransmitters;

        public TransmittersViewModel(Action<Guid> connectCallBack)
        {
            this.connectCallBack = connectCallBack;
            WebNotification.Listener.BasaeApiListiner.OnMessageRunRdcReceiver += WebApiListiner_OnMessageRunRdcReceiver;
        }



        private Command updateCommand;
        private Command<Guid> connectCommand;

        public ICommand UpdateCommand => updateCommand ?? (updateCommand = new Command(Update_Ckick));

        private async void Update_Ckick()
        {
            registryTransmitters = await WebApiClient.GetRdcClients();
            OnPropertyChanged(nameof(RegistryTransmitters));
        }


        public ICommand ConnectCommand => connectCommand ?? (connectCommand = new Command<Guid>(Connect_Click, () => !isConnecting));

        private async void Connect_Click(Guid guid)
        {
            isConnecting = true;
            connectCommand?.CanExecuteChangedInvoke();

            await WebApiClient.SendMessage<MessageAccessToConnection>(new Message
            {
                From = Variables.ClentGuid,
                To = guid,
            },
            new MessageAccessToConnection { UserName = this.UserName });
        }


        private void WebApiListiner_OnMessageRunRdcReceiver(Message message, MessageRunRdcReceiver data)
        {

            if (data.IsSuccess)
            {
                connectCallBack?.Invoke(message.From);
            }
            else
            {
                System.Windows.MessageBox.Show("The remote user rejected the connection request.", $"Connetion to [{message.From}]",
                    System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Exclamation);
            }


            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                isConnecting = false;
                connectCommand?.CanExecuteChangedInvoke();
            });
        }

    }
}
