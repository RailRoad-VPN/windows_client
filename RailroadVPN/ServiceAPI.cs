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

namespace RailRoadVPN
{

    public class ServiceAPI
    {
        private const string URL = "https://api.rroadvpn.net/api/v1";
        private const string GET_USER_BY_PINCODE_URL = "/users/pincode/{pincode}";
        private const string CREATE_USER_DEVICE_URL = "/users/{user_uuid}/devices";

        RestClient client;

        public ServiceAPI()
        {
            this.client = new RestClient(URL);
        }

        private RestRequest prepareRequest(RestRequest request)
        {
            request.RequestFormat = DataFormat.Json;
            request.AddHeader("Content-Type", "application/json");
            request.AddHeader("X-Auth-Token", "7d@qjf-hK:qwQuQqH]Pq+xJNseU<Gh]:A0A=AY\\PJKjNnQOP#YA'lXADW[k7FzGE");
            return request;
        }

        public User getUserByPincode(string pincode)
        {
            var request = new RestRequest(GET_USER_BY_PINCODE_URL, Method.GET);
            request.AddUrlSegment("pincode", pincode);

            request = this.prepareRequest(request);

            var response = this.client.Execute<UserAPIModel>(request);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine(response.Data.Errors[0].Code);
                Console.WriteLine(response.Data.Errors[0].DeveloperMessage);
                throw new RailroadException("you have entered wrong pincode");
            } else
            {
                Console.WriteLine("Got user with email: " + response.Data.data.email);

                return response.Data.data;
            }
        }

        public void createUserDevice(Guid UserUuid, string DeviceId, string VirtualIp, string DeviceIp, string Location, bool IsActive)
        {
            int PlatformId = 3; // Windows hard-code
            int VpnTypeId = 1; // OpenVPN hard-code

            var request = new RestRequest(CREATE_USER_DEVICE_URL, Method.POST);
            request.AddUrlSegment("user_uuid", UserUuid.ToString());

            request = this.prepareRequest(request);

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

            var json = JsonConvert.SerializeObject(userDevice);

            //request.AddParameter("user_uuid", UserUuid.ToString());
            //request.AddParameter("platform_id", PlatformId);
            //request.AddParameter("vpn_type_id", VpnTypeId);
            //request.AddParameter("device_id", DeviceId);
            //request.AddParameter("virtual_ip", VirtualIp);
            //request.AddParameter("device_ip", DeviceIp);
            //request.AddParameter("location", Location);
            //request.AddParameter("is_active", IsActive);
            //request.AddParameter("modify_reason", "init");

            request.AddParameter("application/json; charset=utf-8", json, ParameterType.RequestBody);

            var response = this.client.Execute<UserDeviceAPIModel>(request);
            if (response.StatusCode != System.Net.HttpStatusCode.Created)
            {
                Console.WriteLine(response.Data.Errors[0].Code);
                Console.WriteLine(response.Data.Errors[0].DeveloperMessage);
                throw new RailroadException("не получилось создать user device");
            }
            else
            {
                string xDeviceToken = response.Headers.ToList().Find(x => x.Name == "X-Device-Token").Value.ToString();
                string location = response.Headers.ToList().Find(x => x.Name == "Location").Value.ToString();
                Console.WriteLine("Got userdevice X-Device-Token: " + xDeviceToken);
                Console.WriteLine("Location: " + location);

                Properties.Settings.Default["user_uuid"] = UserUuid.ToString();
                Properties.Settings.Default["x_device_token"] = xDeviceToken;
                Properties.Settings.Default.Save();
            }
        }
    }
}
