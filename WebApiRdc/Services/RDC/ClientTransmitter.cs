using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using WebApiRdc.Model.Data;

namespace WebApiRdc.Services.RDC
{
    public class ClientTransmitter
    {
        private Guid guid;
        private RegistryTransmitterInfo registryInfo;
        private TcpClient client;

        public Guid Guid => guid;


        private List<ClientReceiver> clientReceivers = new List<ClientReceiver>();

        public ClientTransmitter(Guid guid, RegistryTransmitterInfo registryInfo, TcpClient client)
        {
            this.guid = guid;
            this.registryInfo = registryInfo;
            this.client = client;
        }

        public void AddReceivers(ClientReceiver receiver)
        {
            clientReceivers.Add(receiver);
        }


        public void Listening(Action stopCallBack)
        {

            Task.Factory.StartNew(() =>
            {
                var stream = client.GetStream();
                var binary = new BinaryFormatter();

                while (client.Connected)
                {
                    try
                    {
                        //Reade
                        var data = (byte[])binary.Deserialize(stream);
                        foreach (var receiver in clientReceivers.Where(w => w.CanSend).ToList())
                        {
                            if (receiver.Connected)
                            {
                                receiver.SendData(data);
                            }
                            else
                            {
                                clientReceivers.Remove(receiver);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed reading/writing data by the <Transmitter>. Ex: {ex.Message}");
                    }

                }
                clientReceivers.Clear();
                stopCallBack?.Invoke();
            });
        }

        internal void SendData(byte[] data)
        {
            if (client.Connected)
            {
                var stream = client.GetStream();
                var binary = new BinaryFormatter();
                binary.Serialize(stream, data);
            }
        }

        internal void Clear()
        {
            if (client?.Connected == true)
            {
                client.Close();
            }
            else
            {
                clientReceivers.Clear();
            }
        }


    }
}
