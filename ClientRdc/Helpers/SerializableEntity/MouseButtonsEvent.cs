using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ClientRdc.Helpers.SerializableEntity
{
    [Serializable]
    public class MouseButtonsEvent
    {
        public MouseButton ChangedButton { get; set; }
        public MouseButtonState ButtonState { get; set; }
        public Point Position { get; set; }
    }

    public static class MouseButtonsEventExtension
    {
        public static MouseButtonsEvent Convert(this MouseButtonEventArgs e, Point pos)
        {
            return new MouseButtonsEvent()
            {
                ButtonState = e.ButtonState,
                ChangedButton = e.ChangedButton,
                Position = pos,
            };
        }
    }
}
