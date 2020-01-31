using SharedEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using WebApiRdc.Model.Data;

namespace WebApiRdc.Services.RDC
{
    public class RdcService
    {
        private TcpListener tcpListener;

        private List<ClientTransmitter> transmitters = new List<ClientTransmitter>();
        private List<RegistryTransmitterInfo> registryTransmitterInfos = new List<RegistryTransmitterInfo>();

        public IEnumerable<Guid> RegistryTransmitters => registryTransmitterInfos.Select(s => s.Guid);

        public RdcService()
        {
            tcpListener = new TcpListener(System.Net.IPAddress.Parse("10.10.139.222"), 5002);
            Task.Factory.StartNew(StartTcpListener);
        }

        private async Task StartTcpListener()
        {
            tcpListener.Start();
            while (true)
            {
                var client = await tcpListener.AcceptTcpClientAsync();
                if (client != null)
                {
                    var stream = client.GetStream();
                    if (stream.CanRead && stream.CanWrite)
                    {
                        try
                        {
                            var binary = new BinaryFormatter();
                            var clientType = (ClientType)binary.Deserialize(stream);
                            var guid = new Guid((byte[])binary.Deserialize(stream));
                                                        
                            ClientTransmitter transmitter;

                            switch (clientType)
                            {
                                case ClientType.Transmitter:
                                    var regInfo = registryTransmitterInfos.FirstOrDefault();
                                    if (regInfo != null)
                                    {
                                        transmitter = new ClientTransmitter(guid, regInfo, client);
                                        transmitter.Listening(() => UnRegistryTransmitter(transmitter));
                                        transmitters.Add(transmitter);
                                    }
                                    break;
                                case ClientType.Receiver:
                                    transmitter = transmitters.Where(w => w.Guid == guid).FirstOrDefault();
                                    if (transmitter != null)
                                    {
                                        transmitter.AddReceivers(new ClientReceiver(client, transmitter));
                                    }
                                    break;
                                default:
                                    client.Close();
                                    break;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Read connection client failed. Ex: {ex.Message}");
                        }
                    }
                }
            }
        }

        public RegistryTransmitterInfo GetTransmitterInfo(Guid guid)
        {
            return registryTransmitterInfos.Where(w => w.Guid == guid).FirstOrDefault();
        }
        internal bool ChangeScreen(Guid guid, string screenName)
        {
            var tInfo = registryTransmitterInfos.Where(w => w.Guid == guid).FirstOrDefault();
            if (tInfo == null)
                return false;

            var sHide = tInfo.Screens.Where(w => w.Primary).FirstOrDefault();
            var sShow = tInfo.Screens.Where(w => w.DeviceName == screenName).FirstOrDefault();
            if (sShow != null)
            {
                if (sHide != null) sHide.Primary = false;
                sShow.Primary = true;
                return true;
            }
            return false;
        }

        public void RegistryTransmitter(RegistryTransmitterInfo info)
        {
            registryTransmitterInfos.Add(info);
        }

        public void UnRegistryTransmitter(ClientTransmitter transmitter)
        {
            transmitter.Clear();
            transmitters.Remove(transmitter);
            var tInfo = registryTransmitterInfos.Where(w => w.Guid == transmitter.Guid).FirstOrDefault();
            if (tInfo != null)
                registryTransmitterInfos.Remove(tInfo);
        }
    }
}
