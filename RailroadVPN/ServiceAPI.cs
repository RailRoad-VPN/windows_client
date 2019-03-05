using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using RestSharp;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Newtonsoft.Json.Linq;
using System.IO.Compression;

namespace RailRoadVPN
{

    public class ServiceAPI
    {
        private const string URL = "https://api.rroadvpn.net/api/v1";

        private const string GET_USER_BY_PINCODE_URL = "/users/pincode/{pincode}";
        private const string GET_USER_BY_UUID_URL = "/users/uuid/{user_uuid}";

        private const string CREATE_USER_DEVICE_URL = "/users/{user_uuid}/devices";
        private const string UPDATE_USER_DEVICE_URL = "/users/{user_uuid}/devices/{device_uuid}";
        private const string GET_USER_DEVICE_URL = "/users/{user_uuid}/devices/{device_uuid}";

        private const string GET_USER_RANDOM_SERVER_URL = "/users/{user_uuid}/servers?random";
        private const string GET_SERVER_URL = "/users/{user_uuid}/servers/{server_uuid}";

        private const string GET_USER_SERVER_CONFIGURATIONS_URL = "/users/{user_uuid}/servers/{server_uuid}/configurations?platform_id=3&vpn_type_id=1";
        private const string CREATE_USER_SERVER_CONNECTION_URL = "/users/{user_uuid}/servers/{server_uuid}/connections";
        private const string UPDATE_USER_SERVER_CONNECTION_URL = "/users/{user_uuid}/servers/{server_uuid}/connections/{connection_uuid}";

        private const string CREATE_USER_TICKET_URL = "/users/{user_uuid}/tickets";

        private const string GET_APP_VERSION_URL = "/vpns/apps/windows/version";

        private RestClient client;
        private Logger logger = Logger.GetInstance();

        public ServiceAPI()
        {
            this.client = new RestClient(URL);
        }

        private RestRequest prepareRequest(RestRequest request)
        {
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            string sec_token = this.generateSecurityToken();
            request.AddHeader("X-Auth-Token", sec_token);
            return request;
        }

        private string generateSecurityToken()
        {
            string token = "";

            string ruuid = Guid.NewGuid().ToString();
            ruuid = ruuid.Replace("-", "");
            int ruuid_len = ruuid.Length;
            int rnd_num = new Random().Next(1, ruuid_len);

            token += ruuid;

            TimeSpan epochTicks = new TimeSpan(new DateTime(1970, 1, 1).Ticks);
            TimeSpan unixTicks = new TimeSpan(DateTime.UtcNow.Ticks) - epochTicks;
            double unixtime = unixTicks.TotalSeconds;
            double unixtime_divided = unixtime / rnd_num;
            double unixtime_divided_round = Math.Round(unixtime_divided, 10);
            string unixtime_divided_round_str = unixtime_divided_round.ToString();
            int unixtime_divided_round_len = unixtime_divided_round_str.Length;
            string unixtime_divided_round_len_str = unixtime_divided_round_len.ToString();

            string left_token = token.Substring(0, rnd_num);
            string center_token = unixtime_divided_round.ToString();
            string right_token = token.Substring(rnd_num);

            token = String.Format("{0}{1}{2}", left_token, center_token, right_token);

            string rnd_num_str = rnd_num.ToString();
            if (rnd_num_str.Length == 1)
            {
                rnd_num_str = String.Format("{0}{1}", "0", rnd_num_str);
            }

            token = String.Format("{0}{1}{2}", rnd_num_str, unixtime_divided_round_len_str, token);

            return token;
        }

        public User getUserByPincode(string pincode)
        {
            this.logger.log(System.String.Format("getUserByPincode: pincode={0}", pincode));

            this.logger.log("create request");
            var request = new RestRequest(GET_USER_BY_PINCODE_URL, Method.GET);
            request.AddUrlSegment("pincode", pincode);

            request = this.prepareRequest(request);

            this.logger.log("execute request");
            var response = this.client.Execute<UserAPIModel>(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code IS NOT 200");
                this.logger.log("status code is: " + statusCode);
                this.logger.log("Response error code: " + response.Data.Errors[0].Code);
                this.logger.log("Response error developer message: " + response.Data.Errors[0].DeveloperMessage);
                throw new RailroadException("we cant get user by pincode");
            } else
            {
                this.logger.log("status code is 200");
                this.logger.log("user with email: " + response.Data.data.email);

                return response.Data.data;
            }
        }

        public User getUserByUuid(Guid userUuid)
        {
            this.logger.log(System.String.Format("getUserByPincode: userUuid={0}", userUuid.ToString()));

            this.logger.log("create request");
            var request = new RestRequest(GET_USER_BY_UUID_URL, Method.GET);
            request.AddUrlSegment("user_uuid", userUuid.ToString());

            request = this.prepareRequest(request);

            this.logger.log("execute request");
            var response = this.client.Execute<UserAPIModel>(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code IS NOT 200");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant get user by uuid");
            }
            else
            {
                this.logger.log("status code is 200");
                this.logger.log("user email: " + response.Data.data.email);

                return response.Data.data;
            }
        }

        public void createUserDevice(Guid UserUuid, string DeviceId, string Location, bool IsActive)
        {
            this.logger.log(System.String.Format("createUserDevice: UserUuid={0}, DeviceId={1}, Location={2}, IsActive={3}", 
                UserUuid, DeviceId, Location, IsActive));

            int PlatformId = 3; // Windows hard-code
            int VpnTypeId = 1; // OpenVPN hard-code

            this.logger.log("create request");
            var request = new RestRequest(CREATE_USER_DEVICE_URL, Method.POST);
            request.AddUrlSegment("user_uuid", UserUuid.ToString());

            request = this.prepareRequest(request);

            this.logger.log("create UserDevice object");
            UserDevice userDevice = new UserDevice() {
                UserUuid = UserUuid,
                PlatformId = PlatformId,
                VpnTypeId = VpnTypeId,
                DeviceId = DeviceId,
                Location = Location,
                IsActive = IsActive,
                ModifyReason = "init"
            };

            this.logger.log("serialize UserDevice object to JSON");
            var json = JsonConvert.SerializeObject(userDevice);
            this.logger.log("JSON: " + json);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

            var response = this.client.Execute<UserDeviceAPIModel>(request);
            var statusCode = response.StatusCode;
            if (statusCode != System.Net.HttpStatusCode.Created)
            {
                this.logger.log("status code IS NOT 201");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant create user device");
            }
            else
            {
                this.logger.log("status code is 201");

                this.logger.log("get X-Device-Token header");
                string xDeviceToken = response.Headers.ToList().Find(x => x.Name == "X-Device-Token").Value.ToString();
                this.logger.log("userdevice X-Device-Token: " + xDeviceToken);

                this.logger.log("get Location header");
                string location = response.Headers.ToList().Find(x => x.Name == "Location").Value.ToString();
                this.logger.log("Location: " + location);

                this.logger.log("save user_uuid to preferences");
                Properties.Settings.Default.user_uuid = UserUuid.ToString();
                this.logger.log("save x-device-token to preferences");
                Properties.Settings.Default.x_device_token = xDeviceToken;
                this.logger.log("save device_id to preferences");
                Properties.Settings.Default.device_id = DeviceId;

                this.logger.log("location split and get devide_uuid");
                string device_uuid = location.Split(new char[] { '/' })[location.Split(new char[] { '/' }).Length - 1];
                this.logger.log("devide_uuid: " + device_uuid);

                this.logger.log("save device_uuid to preferences");
                Properties.Settings.Default.device_uuid = device_uuid;

                this.logger.log("save preferences to disk");
                Properties.Settings.Default.Save();
            }
        }

        public string getUserRandomServer(Guid UserUuid)
        {
            this.logger.log(System.String.Format("getUserRandomServer: UserUuid={0}", UserUuid));

            this.logger.log("create request");
            var request = new RestRequest(GET_USER_RANDOM_SERVER_URL, Method.GET);
            request.AddUrlSegment("user_uuid", UserUuid.ToString());

            this.logger.log("add request header X-Device-Token");
            request.AddHeader("X-Device-Token", Properties.Settings.Default.x_device_token);

            request = this.prepareRequest(request);

            this.logger.log("execute request");
            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code is NOT 200");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant get user random server");
            }
            else
            {
                this.logger.log("create JObject");
                JObject o = JObject.Parse(response.Content);
                this.logger.log("get data section and uuid field from JObject");
                string server_uuid = (string)o["data"]["uuid"];
                this.logger.log("server uuid: " + server_uuid);

                this.logger.log("status code is 200");

                return server_uuid;
            }
        }

        public VPNServer getVPNServer(Guid userUuid, Guid serverUuid)
        {
            this.logger.log(System.String.Format("getVPNServer: userUuid={0}, serverUuid={1}", userUuid, serverUuid));

            this.logger.log("create request");
            var request = new RestRequest(GET_SERVER_URL, Method.GET);
            this.logger.log("add url segment user_uuid");
            request.AddUrlSegment("user_uuid", userUuid.ToString());
            this.logger.log("add url segment server_uuid");
            request.AddUrlSegment("server_uuid", serverUuid.ToString());
            this.logger.log("add request header X-Device-Token");
            request.AddHeader("X-Device-Token", Properties.Settings.Default.x_device_token);

            request = this.prepareRequest(request);

            this.logger.log("execute request");
            var response = this.client.Execute<VPNServerAPIModel>(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code is NOT 200");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant get vpn server");
            }
            else
            {
                this.logger.log("status code is 200");
                return response.Data.data;
            }
        }

        public string getUserVPNServerConfiguration(Guid userUuid, Guid serverUuid)
        {
            this.logger.log(System.String.Format("getUserVPNServerConfiguration: userUuid={0}, serverUuid={1}", userUuid, serverUuid));

            this.logger.log("create request");
            var request = new RestRequest(GET_USER_SERVER_CONFIGURATIONS_URL, Method.GET);
            this.logger.log("add url segment user_uuid");
            request.AddUrlSegment("user_uuid", userUuid.ToString());
            this.logger.log("add url segment server_uuid");
            request.AddUrlSegment("server_uuid", serverUuid.ToString());
            this.logger.log("add request header X-Device-Token");
            request.AddHeader("X-Device-Token", Properties.Settings.Default.x_device_token);

            request = this.prepareRequest(request);

            this.logger.log("execute request");
            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code is NOT 200");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant get user vpn server configuration");
            }
            else
            {
                this.logger.log("status code is 200");

                this.logger.log("create JObject");
                JObject o = JObject.Parse(response.Content);
                this.logger.log("get data section and uuid field from JObject");
                string configurationBase64 = (string)o["data"]["configuration"];

                this.logger.log("convert base64 to byte array");
                byte[] data = Convert.FromBase64String(configurationBase64);
                this.logger.log("convert byte array to string");
                string configurationStr = Encoding.UTF8.GetString(data);

                return configurationStr;
            }
        }

        public UserDevice getUserDevice(Guid userUuid, Guid deviceUuid)
        {
            this.logger.log(System.String.Format("getUserDevice: userUuid={0}, userDeviceUuid={1}", userUuid, deviceUuid));

            this.logger.log("create request");
            var request = new RestRequest(GET_USER_DEVICE_URL, Method.GET);
            this.logger.log("add url segment user_uuid");
            request.AddUrlSegment("user_uuid", userUuid.ToString());
            this.logger.log("add url segment device_uuid");
            request.AddUrlSegment("device_uuid", deviceUuid.ToString());
            this.logger.log("add request header X-Device-Token");
            request.AddHeader("X-Device-Token", Properties.Settings.Default.x_device_token);

            request = this.prepareRequest(request);

            this.logger.log("execute request");
            var response = this.client.Execute<UserDeviceAPIModel>(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code is NOT 200");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant get user device");
            }
            else
            {
                this.logger.log("status code is 200");
                return response.Data.data;
            }
        }

        public void updateUserDevice(Guid DeviceUuid, Guid UserUuid, string DeviceId, string Location, bool IsActive, string ModifyReason)
        {
            this.logger.log(System.String.Format("updateUserDevice: DeviceUuid={0}, UserUuid={1}, DeviceId={2}, Location={3}, IsActive={4}, ModifyReason={5}", 
                DeviceUuid, UserUuid, DeviceId, Location, IsActive, ModifyReason));

            this.logger.log("create request");
            var request = new RestRequest(UPDATE_USER_DEVICE_URL, Method.PUT);
            request.AddUrlSegment("user_uuid", UserUuid.ToString());
            request.AddUrlSegment("device_uuid", DeviceUuid.ToString());

            this.logger.log("add request header X-Device-Token");
            request.AddHeader("X-Device-Token", Properties.Settings.Default.x_device_token);

            request = this.prepareRequest(request);

            this.logger.log("UserDevice object");
            UserDevice userDevice = new UserDevice()
            {
                Uuid = DeviceUuid,
                UserUuid = UserUuid,
                DeviceId = DeviceId,
                Location = Location,
                IsActive = IsActive,
                ModifyReason = ModifyReason
            };

            this.logger.log("serialize UserDevice object to JSON");
            var json = JsonConvert.SerializeObject(userDevice);
            this.logger.log("JSON: " + json);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code IS NOT 200");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant update user device");
            }
        }

        public void deleteUserDevice(Guid DeviceUuid, Guid UserUuid)
        {
            this.logger.log(System.String.Format("deleteUserDevice: deviceUuid={0}, userUuid={1}", DeviceUuid.ToString(), UserUuid.ToString()));

            this.logger.log("create request");
            var request = new RestRequest(UPDATE_USER_DEVICE_URL, Method.DELETE);
            request.AddUrlSegment("user_uuid", UserUuid.ToString());
            request.AddUrlSegment("device_uuid", DeviceUuid.ToString());

            this.logger.log("add request header X-Device-Token");
            request.AddHeader("X-Device-Token", Properties.Settings.Default.x_device_token);

            request = this.prepareRequest(request);
            
            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code is NOT 200");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant delete user device");
            }
        }

        public string createConnection(Guid UserUuid, Guid ServerUuid, Guid UserDeviceUuid, string DeviceIp, string VirtualIp, long BytesI, long BytesO, bool IsConnected, string ConnectedSince)
        {
            this.logger.log(System.String.Format("createConnection: userUuid={0}, serverUuid={1}, deviceIp={2}, virtualIp={3}, bytesI={4}, bytesO={5}, isConnected={6}, connectedSince={7}", UserUuid, ServerUuid, DeviceIp, VirtualIp, BytesI, BytesO, IsConnected, ConnectedSince));

            this.logger.log("create request");
            var request = new RestRequest(CREATE_USER_SERVER_CONNECTION_URL, Method.POST);
            this.logger.log("add url segment user_uuid");
            request.AddUrlSegment("user_uuid", UserUuid.ToString());
            this.logger.log("add url segment server_uuid");
            request.AddUrlSegment("server_uuid", ServerUuid.ToString());
            this.logger.log("add request header X-Device-Token");
            request.AddHeader("X-Device-Token", Properties.Settings.Default.x_device_token);

            request = this.prepareRequest(request);

            this.logger.log("create VPNUserServerConnection object");
            VPNUserServerConnection userDevice = new VPNUserServerConnection()
            {
                UserUuid = UserUuid,
                ServerUuid = ServerUuid,
                UserDeviceUuid = UserDeviceUuid,
                DeviceIp = DeviceIp,
                VirtualIp = VirtualIp,
                BytesI = BytesI,
                BytesO = BytesO,
                IsConnected = IsConnected
            };

            this.logger.log("serialize VPNUserServerConnection object to JSON");
            var json = JsonConvert.SerializeObject(userDevice);
            this.logger.log("JSON: " + json);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

            this.logger.log("execute request");
            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.Created)
            {
                this.logger.log("status code is NOT 201");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant update connection");
            }
            else
            {
                this.logger.log("status code is 201");

                this.logger.log("split location to get connection uuid");
                string location = response.Headers.ToList().Find(x => x.Name == "Location").Value.ToString();
                string[] locationSplitted = location.Split(new char[] { '/' });
                string connectionUuid = locationSplitted[locationSplitted.Length - 1];

                return connectionUuid;
            }
        }

        public void updateConnection(Guid ConnectionUuid, Guid UserUuid, Guid ServerUuid, long BytesI, long BytesO, bool IsConnected, string ModifyReason)
        {
            //this.logger.log(System.String.Format("updateConnection: userUuid={0}, serverUuid={1}, deviceIp={2}, virtualIp={3}, bytesI={4}, bytesO={5}, isConnected={6}, connectedSince={7}, connectionUuid={8}, ModifyReason={9}", ));
            //this.logger.log(System.String.Format("updateConnection:"));

            //this.logger.log("create request");
            var request = new RestRequest(UPDATE_USER_SERVER_CONNECTION_URL, Method.PUT);
            //this.logger.log("add url segment user_uuid");
            request.AddUrlSegment("user_uuid", UserUuid.ToString());
            //this.logger.log("add url segment server_uuid");
            request.AddUrlSegment("server_uuid", ServerUuid.ToString());
            //this.logger.log("add url segment connection_uuid");
            request.AddUrlSegment("connection_uuid", ConnectionUuid.ToString());
            //this.logger.log("add request header X-Device-Token");
            request.AddHeader("X-Device-Token", Properties.Settings.Default.x_device_token);

            request = this.prepareRequest(request);

            //this.logger.log("create VPNUserServerConnection object");
            VPNUserServerConnection userDevice = new VPNUserServerConnection()
            {
                Uuid = ConnectionUuid,
                UserUuid = UserUuid,
                ServerUuid = ServerUuid,
                BytesI = BytesI,
                BytesO = BytesO,
                IsConnected = IsConnected,
                ModifyReason = ModifyReason
            };

            //this.logger.log("serialize VPNUserServerConnection object to JSON");
            var json = JsonConvert.SerializeObject(userDevice);
            //this.logger.log("JSON: " + json);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

            //this.logger.log("execute request");
            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            //this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code is NOT 200");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant update user connection");
            }
        }

        public int createTicket(Guid UserUuid, string ContactEmail, string Description, string ExtraInfo, byte[] ZipFileBytesArr)
        {
            this.logger.log(System.String.Format("createTicket: userUuid={0}, ContactEmail={1}, Description={2}, ExtraInfo={3}", UserUuid, ContactEmail, Description, ExtraInfo));

            this.logger.log("create request");
            var request = new RestRequest(CREATE_USER_TICKET_URL, Method.POST);
            this.logger.log("add url segment user_uuid");
            request.AddUrlSegment("user_uuid", UserUuid.ToString());
            this.logger.log("add request header X-Device-Token");
            request.AddHeader("X-Device-Token", Properties.Settings.Default.x_device_token);

            request = this.prepareRequest(request);

            this.logger.log("create UserTicket object");
            UserTicket userDevice = new UserTicket()
            {
                UserUuid = UserUuid,
                ContactEmail = ContactEmail,
                Description = Description,
                ExtraInfo = ExtraInfo,
                ZipFileBytesArr = ZipFileBytesArr
            };
            // when i serialize it to JSON, byte array become as base64 string, so in other side you have to decode base64 to get bytes
            this.logger.log("serialize UserTicket object to JSON");
            var json = JsonConvert.SerializeObject(userDevice);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

            this.logger.log("execute request");
            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.Created)
            {
                this.logger.log("status code is NOT 201");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant create user ticket");
            }
            else
            {
                this.logger.log("status code is 201");

                this.logger.log("split location to get ticket number");
                string location = response.Headers.ToList().Find(x => x.Name == "Location").Value.ToString();
                string[] locationSplitted = location.Split(new char[] { '/' });
                int ticketNumber = Int32.Parse(locationSplitted[locationSplitted.Length - 1]);

                return ticketNumber;
            }
        }

        public int createAnonymousTicket(string ContactEmail, string Description, string ExtraInfo, byte[] ZipFileBytesArr)
        {
            this.logger.log(System.String.Format("createAnonymousTicket: ContactEmail={0}, Description={1}, ExtraInfo={2}", ContactEmail, Description, ExtraInfo));

            this.logger.log("create request");
            var request = new RestRequest(CREATE_USER_TICKET_URL, Method.POST);
            this.logger.log("add url segment user_uuid");
            request.AddUrlSegment("user_uuid", "anonymous");

            request = this.prepareRequest(request);

            this.logger.log("create UserTicket object");
            UserTicket userDevice = new UserTicket()
            {
                ContactEmail = ContactEmail,
                Description = Description,
                ExtraInfo = ExtraInfo,
                ZipFileBytesArr = ZipFileBytesArr
            };

            this.logger.log("serialize UserTicket object to JSON");
            var json = JsonConvert.SerializeObject(userDevice);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

            this.logger.log("execute request");
            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.Created)
            {
                this.logger.log("status code is NOT 201");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant create anonymous ticket");
            }
            else
            {
                this.logger.log("status code is 201");

                this.logger.log("split location to get ticket number");
                string location = response.Headers.ToList().Find(x => x.Name == "Location").Value.ToString();
                string[] locationSplitted = location.Split(new char[] { '/' });
                int ticketNumber = Int32.Parse(locationSplitted[locationSplitted.Length - 1]);

                return ticketNumber;
            }
        }

        public string getAppVersion()
        {
            this.logger.log(System.String.Format("getAppVersion"));

            this.logger.log("create request");
            var request = new RestRequest(GET_APP_VERSION_URL, Method.GET);

            request = this.prepareRequest(request);

            this.logger.log("execute request");
            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code is NOT 200");
                this.logger.log("status code is: " + statusCode);
                throw new RailroadException("we cant get app version");
            }
            else
            {
                this.logger.log("status code is 200");

                this.logger.log("create JObject");
                JObject o = JObject.Parse(response.Content);
                this.logger.log("get data section and uuid field from JObject");
                string server_uuid = (string)o["data"]["version"];
                this.logger.log("server uuid: " + server_uuid);

                return server_uuid;
            }
        }
    }
}
