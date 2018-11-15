using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace Util.IO_Services
{
    public sealed class Ports
    {
        /// <summary>
        /// Get an unused port (excluding 0) falling in a range.
        /// </summary>
        /// <param name="rangeStart">The start of the range (inclusive). At least 1.</param>
        /// <param name="rangeEnd">The end of the range (inclusive). At most <see cref="Int16.MaxValue"></param>
        /// <returns>An unused port or 0 otherwise.</returns>
        /// <remarks>RFC8126 legislates 0-1023, RFC6335 legislates 1024-49151, range 49152+ is unregulated.</remarks>
        public static int GetUnusedPort(int rangeStart = 49152, int rangeEnd = ushort.MaxValue)
        {
            if (rangeStart < 1 || rangeStart > ushort.MaxValue + 1)
                return 0;

            if (rangeEnd < 1 || rangeEnd > ushort.MaxValue + 1)
                rangeEnd = ushort.MaxValue;

            if (rangeStart > rangeEnd)
                return 0;

            var properties = IPGlobalProperties.GetIPGlobalProperties();

            return
                (from candidatePort in Enumerable.Range(rangeStart, rangeEnd)
                    where !(from activeConnection in properties.GetActiveTcpConnections()
                        where activeConnection.LocalEndPoint.Port >= rangeStart &&
                              activeConnection.LocalEndPoint.Port <= rangeEnd
                        select activeConnection.LocalEndPoint.Port).Contains(candidatePort)
                    where !(from tcpListener in properties.GetActiveTcpListeners()
                        where tcpListener.Port >= rangeStart && tcpListener.Port <= rangeStart
                        select tcpListener.Port).Contains(candidatePort)
                    where !(from udpListener in properties.GetActiveUdpListeners()
                        where udpListener.Port >= rangeStart && udpListener.Port <= rangeStart
                        select udpListener.Port).Contains(candidatePort)
                    select candidatePort)
                .FirstOrDefault();
        }
    }
}