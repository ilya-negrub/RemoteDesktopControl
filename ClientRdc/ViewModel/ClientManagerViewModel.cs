using ClientRdc.Model;
using ClientRdc.Model.Data;
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
    public class ClientManagerViewModel : BaseViewModel
    {

        private BaseViewModel content;
        
        public BaseViewModel Content => content ?? (content = new TransmittersViewModel(ConnectCallBack));

        private void ConnectCallBack(Guid guid)
        {
            content = new RdcReceiverViewModel(guid, QuitRdcReceiver);            
            OnPropertyChanged(nameof(Content));
        }

        private void QuitRdcReceiver()
        {
            content = new TransmittersViewModel(ConnectCallBack);
            OnPropertyChanged(nameof(Content));
        }
    }    
}
