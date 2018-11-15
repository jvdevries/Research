using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Threading;
using System.Threading.Tasks;
using Util.Network_Services;
using DevUtil.Aspects.Settings;

namespace DevUtil.Aspects.Services.Remoting_MiddleWare
{
    public sealed partial class RO : IDisposable
    {
        public sealed class RemotingMiddlewareSettings : SettingsManager
        {
            [ConfigurationProperty(nameof(Host), DefaultValue = "http://127.0.0.1", IsRequired = true)]
            [RegexStringValidator(
                @"^(http:\/\/|https:\/\/)?[a-zA-Z0-9.]+$|^((http:\/\/|https:\/\/)?(?:\d{1,3}\.){3}\d{1,3})$|^(http:\/\/|https:\/\/)?[a-fA-F0-9:]+$")]
            public string Host
            {
                get => (string) this[nameof(Host)];
                set => this[nameof(Host)] = value;
            }

            [ConfigurationProperty(nameof(Port), DefaultValue = "49950", IsRequired = true)]
            [IntegerValidator(MinValue = 1, MaxValue = 65535, ExcludeRange = false)]
            public int Port
            {
                get => (int) this[nameof(Port)];
                set => this[nameof(Port)] = value;
            }
        }

        private readonly RemotingMiddlewareSettings _settings;
        private static readonly Lazy<RO> LazyInstance = new Lazy<RO>(() => new RO());
        public static RO Instance => LazyInstance.Value;

        private readonly ConcurrentDictionary<string, List<(MarshalByRefObject obj, HttpServerChannel chan, string url, int port)>>
            _serviceRegistrationInfo;

        private LocationStore _locationStore;
        private HttpServerChannel _locationStoreChannel;
        private readonly SemaphoreSlim _locationStoreLock;

        public RO()
        {
            _settings = SettingsManager.Load<RemotingMiddlewareSettings>();
            _locationStore = null;
            _locationStoreChannel = null;
            _locationStoreLock = new SemaphoreSlim(1, 1);
            _serviceRegistrationInfo = new ConcurrentDictionary<string, List<(MarshalByRefObject, HttpServerChannel, string, int)>>();
        }

        private async Task<bool> IsLocationStoreHost()
            => await Network.HostAddressCompare(_settings.Host);

        /// <summary>
        /// Starts the <see cref="RO.LocationStore"/> if possible.
        /// </summary>
        /// <returns>true/false.</returns>
        public async Task<bool> StartLocationStore()
        {
            var locationStore = LocationStore.Instance;
            if (!await IsLocationStoreHost()) 
                // Do not register the RPC manager on the wrong address: otherwise nobody can find it.
                return false;

            var locationStoreChannel = new HttpServerChannel(locationStore._settings.Port);
            var channelRegistered = false;
            try
            {
                await _locationStoreLock.WaitAsync();
                if (locationStore != null)
                {
                    ChannelServices.RegisterChannel(locationStoreChannel, false);
                    channelRegistered = true;
                    _locationStoreChannel = locationStoreChannel;
                    RemotingServices.Marshal(locationStore, typeof(LocationStore).Name);
                    _locationStore = locationStore;
                }

                return true;
            }
            catch
            {
                if (channelRegistered)
                    ChannelServices.UnregisterChannel(locationStoreChannel);
                return false;
            }
            finally
            {
                _locationStoreLock.Release();
            }
        }

        public void Dispose()
        {
            try
            {
                if (_locationStore != null)
                {
                    RemotingServices.Disconnect(_locationStore);
                    ChannelServices.UnregisterChannel(_locationStoreChannel);
                }

                foreach (var serviceRegistrationInfoEntry in _serviceRegistrationInfo)
                {
                    foreach (var infoEntry in serviceRegistrationInfoEntry.Value)
                    {
                        var url = CreateUrl(infoEntry.Item4, serviceRegistrationInfoEntry.Key);
                        if (url.Equals(infoEntry.Item3))
                        {
                            RemotingServices.Disconnect(infoEntry.Item1);
                            ChannelServices.UnregisterChannel(infoEntry.Item2);
                            _locationStore.RemoveService(infoEntry.Item3, serviceRegistrationInfoEntry.Key).Wait();
                        }
                    }
                }
            }
            catch
            {
                // Can't do anything.
            }
        }
    }
}