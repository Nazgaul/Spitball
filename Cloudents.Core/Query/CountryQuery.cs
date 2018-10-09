using System;
using System.Net;
using Cloudents.Core.Interfaces;

namespace Cloudents.Core.Query
{
    public class CountryQuery :IQuery<string>
    {
        public CountryQuery(IPAddress ipAddress)
        {
            IpAddress = ipAddress;
        }

        private IPAddress IpAddress { get;  }

        public uint IntAddress
        {
            get
            {
                byte[] bytes = IpAddress.GetAddressBytes();
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }
                return BitConverter.ToUInt32(bytes, 0);
            }
        }
    }
}