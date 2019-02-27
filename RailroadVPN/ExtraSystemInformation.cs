using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadVPN
{
    public class ExtraSystemInformation
    {
        [JsonProperty("os")]
        public OSInfo OsInfo { get; set; }

        [JsonProperty("network_adapters")]
        public List<NetworkAdapterInfo> NetworkAdapterInfoList { get; set; }
    }

    public class OSInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("bit")]
        public string Bit { get; set; }
    }

    public class NetworkAdapterInfo
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("physical_address")]
        public string PhysicalAddress { get; set; }

        [JsonProperty("is_up")]
        public bool IsUp { get; set; }

        [JsonProperty("versions")]
        public List<string> Versions { get; set; }

        [JsonProperty("dns_suffix")]
        public string DnsSuffix { get; set; }

        [JsonProperty("mtu")]
        public string mtu { get; set; }

        [JsonProperty("is_dns_enabled")]
        public bool IsDNSEnabled { get; set; }

        [JsonProperty("is_dynamic_dns_enabled")]
        public bool IsDynamicDNSEnabled { get; set; }

        [JsonProperty("is_recieve_only")]
        public bool IsRecieveOnly { get; set; }

        [JsonProperty("is_supports_multicast")]
        public bool IsSupportsMulticast { get; set; }
    }
}
