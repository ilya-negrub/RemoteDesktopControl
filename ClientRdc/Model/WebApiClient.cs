using ClientRdc.Model.Data;
using Newtonsoft.Json;
using SharedEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ClientRdc.Model
{
    public static class WebApiClient
    {
        private static HttpClient httpClient;
        static WebApiClient()
        {
            httpClient = new HttpClient();            
            httpClient.BaseAddress = new Uri("http://10.10.139.222:5000");
        }

        public static async Task<T> SubscribeNotifications<T>(Guid guid) where T : WebNotification.Listener.WebApiListiner, new()
        {
            var stream = await httpClient.GetStreamAsync($"api/rdc/Subscribe/{guid}");
            if (stream != null)
            {
                T listiner = new T();
                _ = Task.Factory.StartNew(() => listiner.Listening(stream));
                return listiner;
            }
            return null;
        }

        public static async Task<bool> RegistryTransmitter(IRegistryTransmitterInfo info)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync<IRegistryTransmitterInfo>("api/rdc/RegistryTransmitter", info);

            return response.IsSuccessStatusCode;
        }

        public static async Task<IEnumerable<Guid>> GetSubscribers()
        {
            HttpResponseMessage response = await httpClient.GetAsync("api/rdc/GetSubscribers");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<Guid>>();
            }

            return null;

        }

        public static async Task<IEnumerable<Guid>> GetRdcClients()
        {
            HttpResponseMessage response = await httpClient.GetAsync("api/rdc/GetRdcClients");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsAsync<IEnumerable<Guid>>();
            }

            return null;

        }
            
        public static async Task<bool> SendMessage<TData>(Message message, TData data)
        {
            message.Type = typeof(TData).Name;
            message.Data = JsonConvert.SerializeObject(data);
            HttpResponseMessage response = await httpClient.PostAsJsonAsync<Message>("api/rdc/Send", message);
            return response.IsSuccessStatusCode;
        }

        public static async Task<IRegistryTransmitterInfo> GetTransmitterInfo(Guid guid)
        {
            HttpResponseMessage response = await httpClient.PostAsJsonAsync<Guid>("api/rdc/GetTransmitterInfo", guid);
            if (!response.IsSuccessStatusCode)
                return null;

            var res = await response.Content.ReadAsAsync<RegistryTransmitterInfo>();
            return res;
        }

        internal static async Task<bool> RdcChangeScreen(Guid guid, string screenName)
        {
            HttpResponseMessage response = await httpClient.GetAsync($"api/rdc/ChangeScreen/{guid}?screenName={screenName}");
            return response.IsSuccessStatusCode;
        }
    }
}
