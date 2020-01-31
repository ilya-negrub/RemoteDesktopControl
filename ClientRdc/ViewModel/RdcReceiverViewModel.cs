using ClientRdc.Model;
using ClientRdc.Model.Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientRdc.ViewModel
{
    public class RdcReceiverViewModel : BaseViewModel
    {
        private Action quitCallBack;
        private IEnumerable<ScreenViewModel> screens;
        public IEnumerable<ScreenViewModel> Screens => screens;


        private Guid guidTransmitters;


        public string HostName => Variables.TcpHostName;

        public int Port => Variables.TcpPort;

        public Guid GuidTransmitters => guidTransmitters;

        public RdcReceiverViewModel(Guid guid, Action quitCallBack)
        {
            guidTransmitters = guid;
            this.quitCallBack = quitCallBack;
            Task.Factory.StartNew(UpdateScreens);
        }

        private async Task UpdateScreens()
        {
            var scrs = await WebApiClient.GetTransmitterInfo(guidTransmitters);


            screens = scrs.Screens.Select(s =>
            {
                var svm = new ScreenViewModel(s);
                svm.OnSelectedChange += Svm_OnSelected;
                return svm;
            }
            );
            System.Windows.Application.Current?.Dispatcher?.Invoke(() =>
            {
                OnPropertyChanged(nameof(Screens));
            });
        }

        private async void Svm_OnSelected(object sender, bool val)
        {
            if (val)
            {
                string screenName = ((ScreenViewModel)sender).Name;
                await WebApiClient.RdcChangeScreen(guidTransmitters, screenName);
                await UpdateScreens();
            }
        }

        private ICommand quitCommand;
        public ICommand QuitCommand => quitCommand ?? (quitCommand = new Command(Quit_Click));

        private void Quit_Click()
        {
            quitCallBack?.Invoke();
        }
    }
}
