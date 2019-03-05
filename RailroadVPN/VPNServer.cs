using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadVPN
{
    public class VPNUserServerConnectionAPIModel : APIError
    {
        [JsonProperty("data")]
        public VPNServer data { get; set; }
    }

    public partial class VPNUserServerConnection
    {
        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("server_uuid")]
        public Guid ServerUuid { get; set; }

        [JsonProperty("user_uuid")]
        public Guid UserUuid { get; set; }

        [JsonProperty("user_device_uuid")]
        public Guid UserDeviceUuid { get; set; }

        [JsonProperty("device_ip")]
        public string DeviceIp { get; set; }

        [JsonProperty("virtual_ip")]
        public string VirtualIp { get; set; }

        [JsonProperty("bytes_i")]
        public long BytesI { get; set; }

        [JsonProperty("bytes_o")]
        public long BytesO { get; set; }

        [JsonProperty("is_connected")]
        public bool IsConnected { get; set; }

        [JsonProperty("connected_since")]
        public string ConnectedSince { get; set; }

        [JsonProperty("modify_reason")]
        public string ModifyReason { get; set; }
    }

    public class VPNServerAPIModel : APIError
    {

        [JsonProperty("data")]
        public VPNServer data { get; set; }
    }

    public partial class VPNServer
    {
        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("port")]
        public long Port { get; set; }

        [JsonProperty("version")]
        public long Version { get; set; }

        [JsonProperty("condition_version")]
        public long ConditionVersion { get; set; }

        [JsonProperty("type_id")]
        public long TypeId { get; set; }

        [JsonProperty("status_id")]
        public long StatusId { get; set; }

        [JsonProperty("bandwidth")]
        public long Bandwidth { get; set; }

        [JsonProperty("load")]
        public long Load { get; set; }

        [JsonProperty("num")]
        public long Num { get; set; }

        [JsonProperty("geo_position_id")]
        public long GeoPositionId { get; set; }

        [JsonProperty("geo")]
        public Geo Geo { get; set; }
    }

    public partial class Geo
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("latitude")]
        public string Latitude { get; set; }

        [JsonProperty("longitude")]
        public string Longitude { get; set; }

        [JsonProperty("country")]
        public Country Country { get; set; }

        [JsonProperty("city")]
        public City City { get; set; }

        [JsonProperty("state")]
        public State State { get; set; }

        [JsonProperty("region_common")]
        public long RegionCommon { get; set; }

        [JsonProperty("region_dvd")]
        public long RegionDvd { get; set; }

        [JsonProperty("region_xbox360")]
        public long RegionXbox360 { get; set; }

        [JsonProperty("region_xboxone")]
        public long RegionXboxone { get; set; }

        [JsonProperty("region_playstation3")]
        public long RegionPlaystation3 { get; set; }

        [JsonProperty("region_playstation4")]
        public long RegionPlaystation4 { get; set; }

        [JsonProperty("region_nintendo")]
        public long RegionNintendo { get; set; }
    }

    public partial class City
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public partial class Country
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("str_code")]
        public string StrCode { get; set; }
    }

    public partial class State
    {
        [JsonProperty("code")]
        public long Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
