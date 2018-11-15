using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Threading.Tasks;
using DevUtil.Aspects.Services.Remoting_MiddleWare.Util;
using DevUtil.Aspects.Settings;

namespace DevUtil.Aspects.Services.Remoting_MiddleWare
{
    public sealed partial class RO
    {
        public sealed class LocationStore : MarshalByRefObject
        {
            public sealed class LocationStoreSettings : SettingsManager
            {
                [ConfigurationProperty(nameof(Host), DefaultValue = "http://127.0.0.1", IsRequired = true)]
                [RegexStringValidator(
                    @"^(http:\/\/|https:\/\/)?[a-zA-Z0-9.]+$|^((http:\/\/|https:\/\/)?(?:\d{1,3}\.){3}\d{1,3})$|^(http:\/\/|https:\/\/)?[a-fA-F0-9:]+$")]
                public string Host
                {
                    get => (string) this[nameof(Host)];
                    set => this[nameof(Host)] = value;
                }

                [ConfigurationProperty(nameof(Port), DefaultValue = "49900", IsRequired = true)]
                [IntegerValidator(MinValue = 1, MaxValue = 65535, ExcludeRange = false)]
                public int Port
                {
                    get => (int) this[nameof(Port)];
                    set => this[nameof(Port)] = value;
                }
            }

            private static readonly Lazy<LocationStore> LazyInstance =
                new Lazy<LocationStore>(() => new LocationStore());

            public static LocationStore Instance => LazyInstance.Value;
            public readonly LocationStoreSettings _settings;

            private readonly ConcurrentDictionary<string, RandomNodeList> _serviceImplementors;

            private LocationStore()
            {
                _settings = SettingsManager.Load<LocationStoreSettings>();
                _serviceImplementors = new ConcurrentDictionary<string, RandomNodeList>();
            }

            public string GetUrl(string typeName)
            {
                if (!_serviceImplementors.ContainsKey(typeName))
                    return null;

                var getList = _serviceImplementors.TryGetValue(typeName, out var list);
                return !getList ? null : list.GetRandom();
            }

            public async Task<bool> RegisterService(string url, string typeName)
            {
                if (!_serviceImplementors.ContainsKey(typeName))
                    _serviceImplementors.TryAdd(typeName, new RandomNodeList());

                var gotImplementors = _serviceImplementors.TryGetValue(typeName, out var implementors);
                if (!gotImplementors)
                    return false;

                return await implementors.Add(url);
            }

            public async Task RemoveService(string url, string typeName)
            {
                if (!_serviceImplementors.ContainsKey(typeName))
                    return;

                var gotImplementors = _serviceImplementors.TryGetValue(typeName, out var implementors);
                if (!gotImplementors)
                    return;

                await implementors.Del(url);
            }
        }
    }
}