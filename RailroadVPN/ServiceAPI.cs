﻿using System;
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

        private const string CREATE_USER_DEVICE_URL = "/users/{user_uuid}/devices";
        private const string UPDATE_USER_DEVICE_URL = "/users/{user_uuid}/devices/{device_uuid}";

        private const string GET_USER_RANDOM_SERVER_URL = "/users/{user_uuid}/servers?random";
        private const string GET_SERVER_URL = "/users/{user_uuid}/servers/{server_uuid}";

        private const string GET_USER_SERVER_CONFIGURATIONS_URL = "/users/{user_uuid}/servers/{server_uuid}/configurations?platform_id=3&vpn_type_id=1";
        private const string CREATE_USER_SERVER_CONNECTION_URL = "/users/{user_uuid}/servers/{server_uuid}/connections";
        private const string UPDATE_USER_SERVER_CONNECTION_URL = "/users/{user_uuid}/servers/{server_uuid}/connections/{connection_uuid}";

        private const string CREATE_USER_TICKET_URL = "/users/{user_uuid}/tickets";

        private RestClient client;
        private Logger logger = Logger.GetInstance();

        public ServiceAPI()
        {
            this.client = new RestClient(URL);
        }

        private RestRequest prepareRequest(RestRequest request)
        {
            //this.logger.log("prepare request");
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Auth-Token", "7d@qjf-hK:qwQuQqH]Pq+xJNseU<Gh]:A0A=AY\\PJKjNnQOP#YA'lXADW[k7FzGE");
            return request;
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
                this.logger.log("status code is NOT 200");
                this.logger.log("Response error code: " + response.Data.Errors[0].Code);
                this.logger.log("Response error developer message: " + response.Data.Errors[0].DeveloperMessage);
                throw new RailroadException("you have entered wrong pincode");
            } else
            {
                this.logger.log("status code is 200");
                this.logger.log("Got user with email: " + response.Data.data.email);

                return response.Data.data;
            }
        }

        public void createUserDevice(Guid UserUuid, string DeviceId, string VirtualIp, string DeviceIp, string Location, bool IsActive)
        {
            this.logger.log(System.String.Format("createUserDevice: UserUuid={0}, DeviceId={1}, VirtualIp={2}, DeviceIp={3}, " +
                "Location={4}, IsActive={5}", UserUuid, DeviceId, VirtualIp, DeviceIp, Location, IsActive));

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
                VirtualIp = VirtualIp,
                DeviceIp = DeviceIp,
                Location = Location,
                IsActive = IsActive,
                ModifyReason = "init"
            };

            this.logger.log("serialize UserDevice object to JSON");
            var json = JsonConvert.SerializeObject(userDevice);
            this.logger.log("JSON: " + json);

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

            var response = this.client.Execute<UserDeviceAPIModel>(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                this.logger.log(response.Data.Errors[0].Code);
                this.logger.log(response.Data.Errors[0].DeveloperMessage);
                throw new RailroadException("не получилось создать user device");
            }
            else
            {
                string xDeviceToken = response.Headers.ToList().Find(x => x.Name == "X-Device-Token").Value.ToString();
                string location = response.Headers.ToList().Find(x => x.Name == "Location").Value.ToString();
                this.logger.log("Got userdevice X-Device-Token: " + xDeviceToken);
                this.logger.log("Location: " + location);

                Properties.Settings.Default.user_uuid = UserUuid.ToString();
                Properties.Settings.Default.x_device_token = xDeviceToken;
                Properties.Settings.Default.device_id = DeviceId;
                Properties.Settings.Default.device_uuid = location.Split(new char[] { '/' })[location.Split(new char[] { '/' }).Length - 1];
                Properties.Settings.Default.Save();
            }
        }

        public string getUserRandomServer(Guid UserUuid)
        {
            this.logger.log(System.String.Format("getUserRandomServer: UserUuid={0}", UserUuid));

            this.logger.log("create request");
            var request = new RestRequest(GET_USER_RANDOM_SERVER_URL, Method.GET);
            request.AddUrlSegment("user_uuid", UserUuid.ToString());

            request = this.prepareRequest(request);

            this.logger.log("execute request");
            var response = this.client.Execute(request);
            var statusCode = response.StatusCode;
            this.logger.log(System.String.Format("response status code: {0}", statusCode));
            if (statusCode != System.Net.HttpStatusCode.OK)
            {
                this.logger.log("status code is NOT 200");
                throw new RailroadException("something wrong with API");
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
                throw new RailroadException("something wrong with API");
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
                throw new RailroadException("something wrong with API");
            }
            else
            {
                this.logger.log("status code is 200");

                this.logger.log("create JObject");
                JObject o = JObject.Parse(response.Content);
                this.logger.log("get data section and uuid field from JObject");
                string configurationBase64 = (string)o["data"]["configuration"];

                byte[] data = Convert.FromBase64String(configurationBase64);
                string configurationStr = Encoding.UTF8.GetString(data);

                return configurationStr;
            }
        }

        public void updateUserDevice(Guid DeviceUuid, Guid UserUuid, string DeviceId, string VirtualIp, string DeviceIp, string Location, bool IsActive, string ModifyReason)
        {
            this.logger.log(System.String.Format("updateUserDevice: DeviceUuid={0}, UserUuid={1}, DeviceId={2}, VirtualIp={3}, DeviceIp={4}, " +
                "Location={5}, IsActive={6}", DeviceUuid, UserUuid, DeviceId, VirtualIp, DeviceIp, Location, IsActive));

            this.logger.log("create request");
            var request = new RestRequest(UPDATE_USER_DEVICE_URL, Method.PUT);
            request.AddUrlSegment("user_uuid", UserUuid.ToString());
            request.AddUrlSegment("device_uuid", DeviceUuid.ToString());

            request = this.prepareRequest(request);

            this.logger.log("create UserDevice object");
            UserDevice userDevice = new UserDevice()
            {
                Uuid = DeviceUuid,
                UserUuid = UserUuid,
                DeviceId = DeviceId,
                VirtualIp = VirtualIp,
                DeviceIp = DeviceIp,
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
                throw new RailroadException("не получилось обновить user device");
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
                throw new RailroadException("something wrong with API");
            }
            else
            {
                this.logger.log("status code is 201");

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
                //this.logger.log("status code is NOT 200");
                throw new RailroadException("something wrong with API");
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
                throw new RailroadException("something wrong with API");
            }
            else
            {
                this.logger.log("status code is 201");

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
                throw new RailroadException("something wrong with API");
            }
            else
            {
                this.logger.log("status code is 201");

                string location = response.Headers.ToList().Find(x => x.Name == "Location").Value.ToString();
                string[] locationSplitted = location.Split(new char[] { '/' });
                int ticketNumber = Int32.Parse(locationSplitted[locationSplitted.Length - 1]);

                return ticketNumber;
            }
        }
    }
}
