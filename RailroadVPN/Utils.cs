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
                logger.log("work with file:" + file);

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
            return Path.Combine(getLocalAppDirPath(), Properties.Settings.Default.local_app_openvpn_servers_config_dir);
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
