using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Threading.Tasks;
using Util.IO_Services;

namespace DevUtil.Aspects.Services.Remoting_MiddleWare
{
    public sealed partial class RO
    {
        public async Task<string> RegisterService<T>(T service) where T : MarshalByRefObject
        {
            if (service == null)
                return string.Empty;

            // Special exception for the LocationStore.
            if (typeof(T).Name == typeof(LocationStore).Name)
            {
                await StartLocationStore();
                return string.Empty;
            }

            if (_locationStore == null)
                return string.Empty;

            var port = Ports.GetUnusedPort();
            if (port == 0)
                return string.Empty;

            var channel = new HttpServerChannel($"{typeof(T).Name}:{port}", port);
            var channelRegistered = false;
            try
            {
                ChannelServices.RegisterChannel(channel, false);
                channelRegistered = true;
                RemotingServices.Marshal(service, typeof(T).Name);

                List<(MarshalByRefObject, HttpServerChannel, string, int)> infoList = null;
                if (!_serviceRegistrationInfo.ContainsKey(typeof(T).Name))
                {
                    var tryAdd = _serviceRegistrationInfo.TryAdd(typeof(T).Name,
                        new List<(MarshalByRefObject, HttpServerChannel, string, int)>());
                    if (!tryAdd || !_serviceRegistrationInfo.TryGetValue(typeof(T).Name, out var list))
                    {
                        RemotingServices.Disconnect(service);
                        ChannelServices.UnregisterChannel(channel);
                        return string.Empty;
                    }

                    infoList = list;
                }

                var url = CreateUrl(port, typeof(T).Name);
                await _locationStore.RegisterService(url, typeof(T).Name);
                infoList?.Add((service, channel, url, port));
                return string.Empty;
            }
            catch
            {
                if (channelRegistered)
                    ChannelServices.UnregisterChannel(channel);
                return string.Empty;
            }
        }

        private string CreateUrl(int port, string type)
            => $"{_settings.Host}:{port}/{type}";

        public async Task<bool> UnregisterService<T>() where T : MarshalByRefObject
        {
            if (_locationStore == null)
                return false;

            if (!_serviceRegistrationInfo.ContainsKey(typeof(T).Name))
                return false;

            var gotList = _serviceRegistrationInfo.TryGetValue(typeof(T).Name, out var list);

            if (list != null)
                foreach (var infoEntry in list)
                {
                    var url = CreateUrl(infoEntry.Item4, typeof(T).Name);
                    if (url.Equals(infoEntry.Item3))
                    {
                        RemotingServices.Disconnect(infoEntry.Item1);
                        ChannelServices.UnregisterChannel(infoEntry.Item2);
                        await _locationStore.RemoveService(infoEntry.Item3, typeof(T).Name);
                    }
                }
            else
                return false;

            return true;
        }
    }
}