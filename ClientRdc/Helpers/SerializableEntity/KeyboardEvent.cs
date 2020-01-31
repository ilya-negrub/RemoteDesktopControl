using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientRdc.Helpers.SerializableEntity
{
    [Serializable]
    public class KeyboardEvent
    {
        public KeyStates KeyStates { get; set; }
        public int VkKey { get; set; }
        public int VkSystemKey { get; set; }
        public bool IsSystemKey { get; set; }
        public bool IsRepeat { get; set; }
        public bool IsDown { get; set; }
        public bool IsUp { get; set; }
        public bool IsToggled { get; set; }
    }

    public static class KeyEventExtension
    {
        public static KeyboardEvent Convert(this KeyEventArgs e)
        {
            return new KeyboardEvent
            {
                KeyStates = e.KeyStates,
                VkKey = 
                    e.Key == Key.System ? KeyInterop.VirtualKeyFromKey(e.SystemKey) :
                    e.Key == Key.ImeProcessed ? KeyInterop.VirtualKeyFromKey(e.ImeProcessedKey) :
                    e.Key == Key.DeadCharProcessed ? KeyInterop.VirtualKeyFromKey(e.DeadCharProcessedKey) :
                KeyInterop.VirtualKeyFromKey(e.Key),

                IsSystemKey = e.Key == Key.System,
                VkSystemKey = KeyInterop.VirtualKeyFromKey(e.SystemKey),
                IsRepeat = e.IsRepeat,
                IsDown = e.IsDown,
                IsUp = e.IsUp,
                IsToggled = e.IsToggled,
            };
        }
    }
}
