using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RailRoadVPN
{

    public class UserDeviceAPIModel : APIError
    {

        [JsonProperty("data")]
        public UserDevice data { get; set; }
    }

    public partial class UserDevice
    {
        [JsonProperty("uuid")]
        public Guid Uuid { get; set; }

        [JsonProperty("user_uuid")]
        public Guid UserUuid { get; set; }

        [JsonProperty("platform_id")]
        public int PlatformId { get; set; }

        [JsonProperty("vpn_type_id")]
        public int VpnTypeId { get; set; }

        [JsonProperty("device_id")]
        public string DeviceId { get; set; }

        [JsonProperty("virtual_ip")]
        public string VirtualIp { get; set; }

        [JsonProperty("device_ip")]
        public string DeviceIp { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("is_active")]
        public bool IsActive { get; set; }

        [JsonProperty("modify_reason")]
        public string ModifyReason { get; set; }
    }
}
