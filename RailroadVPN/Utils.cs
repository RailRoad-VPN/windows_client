using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailRoadVPN
{
    public sealed class PropertiesHelper
    {
        public Dictionary<string, VPNServer> vpnServersDict;

        private static readonly Lazy<PropertiesHelper> lazy = new Lazy<PropertiesHelper>(() => new PropertiesHelper());

        public static PropertiesHelper GetInstance()
        { return lazy.Value; }

        private PropertiesHelper()
        {
            this.vpnServersDict = new Dictionary<string, VPNServer>();

            var serversJson = Properties.Settings.Default.servers_json;
            List<VPNServer> vpnServersList = JsonConvert.DeserializeObject<List<VPNServer>>(serversJson);
            if (vpnServersList == null)
            {
                return;
            }

            foreach (VPNServer vpnServer in vpnServersList)
            {
                if (!this.vpnServersDict.ContainsKey(vpnServer.Uuid.ToString()))
                {
                    this.vpnServersDict.Add(vpnServer.Uuid.ToString(), vpnServer);
                }
            }
        }

        public VPNServer getVPNServer(string uuid)
        {
            return this.vpnServersDict[uuid];
        }

        public List<VPNServer> getVPNServersList()
        {
            return this.vpnServersDict.Values.ToList();
        }

        public void addVPNServer(VPNServer vpnServer)
        {
            this.vpnServersDict.Add(vpnServer.Uuid.ToString(), vpnServer);
            this.persistVPNServers();
        }

        public void removeVPNServer(VPNServer vpnServer)
        {
            this.vpnServersDict.Remove(vpnServer.Uuid.ToString());
            this.persistVPNServers();
        }

        private void persistVPNServers()
        {
            List<VPNServer> vpnServersList = this.vpnServersDict.Values.ToList();
            string json = JsonConvert.SerializeObject(vpnServersList);
            Properties.Settings.Default.servers_json = json;
            Properties.Settings.Default.Save();
        }

        public bool hasVPNServer(string uuid)
        {
            return this.vpnServersDict.ContainsKey(uuid);
        }
    }

    class Utils
    {
        public static Logger logger = Logger.GetInstance();

        public static string CreateMd5ForFolder(string path)
        {
            logger.log("CreateMd5ForFolder for " + path);

            logger.log("get files");
            // assuming you want to include nested folders
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();

            logger.log("create md5 object");
            MD5 md5 = MD5.Create();

            logger.log("iterate files and do some md5 staff");
            for (int i = 0; i < files.Count; i++)
            {
                string file = files[i];

                // hash path
                string relativePath = file.Substring(path.Length + 1);
                byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);

                // hash contents
                byte[] contentBytes = File.ReadAllBytes(file);
                if (i == files.Count - 1)
                    md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                else
                    md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
            }
            try {
                logger.log("generate MD5 hash");
                byte[] hash = md5.Hash;
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            } catch (System.NullReferenceException e)
            {
                logger.log("exception while gen md5 hash, error:" + e.Message);
                return "";
            } 
        }

        public static string getBinariesDirPath()
        {
            return Path.Combine(getLocalAppDirPath(), Properties.Settings.Default.local_app_openvpn_binaries_dir);
        }

        public static string getOpenVPNExePath()
        {
            return Path.Combine(getBinariesDirPath(), Properties.Settings.Default.local_app_openvpn_exe_path);
        }

        public static string getTapWindowsExePath()
        {
            return Path.Combine(getBinariesDirPath(), Properties.Settings.Default.local_app_openvpn_tap_exe_path);
        }

        public static string getServersConfigDirPath()
        {
            string serversPath = Path.Combine(getLocalAppDirPath(), Properties.Settings.Default.local_app_openvpn_servers_config_dir);

            if (!Directory.Exists(serversPath))
            {
                Directory.CreateDirectory(serversPath);
            }

            return serversPath;
        }

        public static string localDirAppName = "\\RailRoadVPN";

        public static string getLocalAppDirPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + localDirAppName;
        }

        public static void killAllMyProcesses()
        {
            killAllOpenVPNProcesses();
            killAllApplicationProcessess();
        }

        public static void killAllOpenVPNProcesses()
        {
            string name = "rroad_openvpn.exe";
            killAllProcessessByName(name_exe: name);
        }

        public static void killAllApplicationProcessess()
        {
            string name = "RailRoadVPN.exe";
            killAllProcessessByName(name_exe: name);

        }

        private static void killAllProcessessByName(string name_exe)
        {
            logger.log("kill all processes " + name_exe);
            string strCmdText = "/K taskkill /F /IM " + name_exe;
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = strCmdText,
                WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden
            };
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
