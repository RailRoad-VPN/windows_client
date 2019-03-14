using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

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

    public class RailroadException : Exception, ISerializable
    {
        public RailroadException(string message) : base(message)
        {

        }
    }

    public class UserDeviceNotFound : RailroadException
    {
        public UserDeviceNotFound(string message) : base(message)
        {

        }
    }

    public class OpenVPNNotConnectedException : RailroadException
    {
        public OpenVPNNotConnectedException(string message) : base(message)
        {

        }
    }
}
