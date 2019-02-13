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
    public class UserAPIModel : APIError
    {

        [JsonProperty("data")]
        public User data { get; set; }
    }

    public partial class User
    {
        [JsonProperty("uuid")]
        public string uuid { get; set; }

        [JsonProperty("email")]
        public string email { get; set; }

        [JsonProperty("created_date")]
        public DateTime created_date { get; set; }

        [JsonProperty("is_expired")]
        public bool is_expired { get; set; }

        [JsonProperty("is_locked")]
        public bool is_locked { get; set; }

        [JsonProperty("is_password_expired")]
        public bool is_password_expired { get; set; }

        [JsonProperty("enabled")]
        public bool enabled { get; set; }

        [JsonProperty("pin_code")]
        public int pin_code { get; set; }

        [JsonProperty("pin_code_expire_date")]
        public DateTime pin_code_expire_date { get; set; }
    }
}
