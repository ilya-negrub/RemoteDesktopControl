using SharedEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientRdc.ViewModel
{
    public class ScreenViewModel
    {
        public event EventHandler<bool> OnSelectedChange;

        private bool isSelected = false;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                isSelected = value;
                OnSelectedChange?.Invoke(this, value);
            }
        }

        public string Name => screenInfo.DeviceName;

        private IScreenInfo screenInfo;

        public ScreenViewModel(IScreenInfo screenInfo)
        {
            this.screenInfo = screenInfo;
            isSelected = screenInfo.Primary;
        }
    }
}
