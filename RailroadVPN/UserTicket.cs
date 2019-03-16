using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadVPN
{
    public class UserTicketAPIModel : APIError
    {

        [JsonProperty("data")]
        public UserTicket data { get; set; }
    }

    public class UserTicket
    {
        [JsonProperty("uuid")]
        public String Uuid { get; set; }

        [JsonProperty("user_uuid")]
        public String UserUuid { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("contact_email")]
        public string ContactEmail { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("extra_info")]
        public string ExtraInfo { get; set; }

        [JsonProperty("zipfile")]
        public byte[] ZipFileBytesArr { get; set; }
    }
}
