using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace WebApiRdc.Services.RDC
{
    public class ClientReceiver
    {
        //private Guid guid = Guid.NewGuid();
        private TcpClient client;
        private NetworkStream stream;
        private ClientTransmitter clientTransmitter;
        private object writeLocker = new object();

        public bool Connected => client?.Connected == true;
        public bool CanSend => stream != null;
        //public Guid Guid => guid;


        public ClientReceiver(TcpClient client, ClientTransmitter clientTransmitter)
        {
            SetTcpClient(client, clientTransmitter);
        }


        private void SetTcpClient(TcpClient client, ClientTransmitter clientTransmitter)
        {
            this.client = client;
            this.stream = client.GetStream();
            this.clientTransmitter = clientTransmitter;
            Task.Factory.StartNew(ReadData);
        }

        private void ReadData()
        {
            while (Connected)
            {
                try
                {
                    var bin = new BinaryFormatter();
                    var data = (byte[])bin.Deserialize(stream);
                    clientTransmitter.SendData(data);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed reading/writing data by the <Receiver>. Ex: {ex.Message}");
                }
            }
        }

        public void SendData(byte[] data)
        {
            lock (writeLocker)
            {
                var binary = new BinaryFormatter();
                binary.Serialize(stream, data);
            }
        }

    }
}
