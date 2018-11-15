using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Util.Network_Services
{
    public partial class Network
    {
        public static async Task<bool> HostAddressCompare(string address)
        {
            IPHostEntry thisIpAddresses;
            IPHostEntry targetedIpAddresses;
            try
            {
                var thisHostName = Dns.GetHostName();
                thisIpAddresses = await Dns.GetHostEntryAsync(thisHostName);
                targetedIpAddresses = await Dns.GetHostEntryAsync(StripProtocol(address));
            }
            catch
            {
                return false;
            }


            return thisIpAddresses.AddressList.Intersect(targetedIpAddresses.AddressList).Any();
        }
    }
}