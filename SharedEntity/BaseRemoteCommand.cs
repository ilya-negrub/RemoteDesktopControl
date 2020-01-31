using System;
using System.Collections.Generic;
using System.Text;

namespace SharedEntity
{
    public enum TypeCommand
    {
        None = 0,
        MouseMove = 1,
        MouseButtons = 2,
        KeyboardEvent = 3,
    }


    [Serializable]
    public class RemoteCommand
    {
        public TypeCommand Type { get; set; }
        public object ObjData { get; set; }


        public RemoteCommand()
        {

        }


        public RemoteCommand(TypeCommand type, object obj)
        {
            Type = type;
            ObjData = obj;
        }
    }
}
