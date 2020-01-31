using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientRdc.Model.Globals
{
    internal static class Variables
    {
        private static readonly Guid clentGuid = Guid.NewGuid();
        private static Uri webApiAddress = new Uri("http://10.10.139.222:5000");
        private static string tcpHostName = "10.10.139.222";
        private static int tcpPort = 5002;

        public static Guid ClentGuid => clentGuid;

        public static Uri WebApiAddress => webApiAddress;
        public static string TcpHostName => tcpHostName;
        public static int TcpPort => tcpPort;

    }
}
