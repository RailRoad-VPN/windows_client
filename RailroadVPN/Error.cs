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
    public class APIError
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("status")]
        public List<APIErrorObj> Errors { get; set; }
    }

    public class APIErrorObj
    {
        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("developer_message")]
        public string DeveloperMessage { get; set; }
    }

    public class RailroadException : Exception
    {
        public RailroadException(string message) : base(message)
        {

        }
    }
}
