using System;

namespace Util.Network_Services
{
    public partial class Network
    {
        public static string StripProtocol(string address)
        {
            return !address.Contains("://")
                ? address
                : address.Substring(address.IndexOf("://", StringComparison.Ordinal) + 3);
        }
    }
}