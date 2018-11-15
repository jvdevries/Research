using System;

namespace DevUtil.Aspects.Services.Remoting_MiddleWare
{
    public sealed partial class RO
    {
        /// <summary>
        /// Get a Remote Object.
        /// </summary>
        /// <typeparam name="T">The type of the remote object to get.</typeparam>
        /// <returns>The remote object or null.</returns>
        public T GetRemoteObject<T>() where T : MarshalByRefObject
        {
            var url = _locationStore?.GetUrl(typeof(T).Name);
            if (url == null)
                return null;

            try
            {
                return Activator.GetObject(typeof(T), url) as T;
            }
            catch
            {
                return null;
            }
        }
    }
}