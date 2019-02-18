﻿using System;
using System.Collections.Generic;
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
        public static string CreateMd5ForFolder(string path)
        {
            // assuming you want to include nested folders
            var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories)
                                 .OrderBy(p => p).ToList();

            MD5 md5 = MD5.Create();

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
                byte[] hash = md5.Hash;
                return BitConverter.ToString(hash).Replace("-", "").ToLower();
            } catch (System.NullReferenceException e)
            {
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
    }
}
