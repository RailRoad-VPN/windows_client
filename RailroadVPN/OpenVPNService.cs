using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Management;

namespace RailRoadVPN
{
    class OpenVPNService : IDisposable
    {
        public enum Signal
        {
            Hup,
            Term,
            Usr1,
            Usr2
        }

        private Logger logger = Logger.GetInstance();

        private Socket socket;
        private string mgmtHost;
        private int mgmtPort;
        private const int bufferSize = 1024;
        private string ovpnFileName;
        private const string eventName = "MyOpenVpnEvent";
        private readonly Process managerProcess = new Process();
        private readonly Process vpnProcess = new Process();

        private string MANAGER_STATUS = "NOT_CONNECTED";

        public OpenVPNService(string mgmtHost, int mgmtPort)
        {
            this.mgmtHost = mgmtHost;
            this.mgmtPort = mgmtPort;
        }

        public void installTapDriver()
        {
            this.logger.log("installTapDriver");

            var localAppDir = Utils.getLocalAppDirPath();
            string strCmdText = "/C " + localAppDir + "\\" + Properties.Settings.Default.local_app_openvpn_binaries_dir + "\\tap -windows.exe  /S /D=c:\\TapWindows";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = strCmdText
            };

            Process process = new Process
            {
                StartInfo = startInfo
            };
            process.Start();
        }

        public void startOpenVPN()
        {
            this.logger.log("startOpenVPN");

            var localAppDir = Utils.getLocalAppDirPath();
            string strCmdText = "/C " + localAppDir + "\\" + Properties.Settings.Default.local_app_openvpn_binaries_dir + "\\openvpn\\rroad_openvpn.exe --config " + localAppDir + "\\" + Properties.Settings.Default.local_app_openvpn_binaries_dir + "\\openvpn_rroad_config.ovpn >> " + Utils.getLocalAppDirPath() + "\\" + Properties.Settings.Default.openvpn_logfile_name;
            //string strCmdText = "/K " + localAppDir + "\\rroad_openvpn\\openvpn\\rroad_openvpn.exe --config " + localAppDir + "\\rroad_openvpn\\openvpn_rroad_config.ovpn";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = strCmdText
            };
            this.vpnProcess.StartInfo = startInfo;
            this.vpnProcess.Start();
        }

        public void stopOpenVPN()
        {
            this.logger.log("stopOpenVPN");

            this.logger.log(string.Format("MANAGER_STATUS={0}", MANAGER_STATUS));

            if (MANAGER_STATUS == "CONNECTED")
            {
                this.logger.log("try to send Term signal");
                this.logger.log("send sign Term to manager");
                string result = this.SendSignal(Signal.Term);
                this.logger.log("send signal result: " + result);
                if (result is null)
                {
                    this.logger.log("send sign does not help");
                    KillProcessAndChildren(vpnProcess.Id);
                }
            } else {
                KillProcessAndChildren(vpnProcess.Id);
            }

            if (MANAGER_STATUS == "CONNECTING")
            {
                this.logger.log("cancel manager connecting");
                this.cancelManagerConnect();
            }

            MANAGER_STATUS = "NOT_CONNECTED";
        }

        public void KillProcessAndChildren(int pid)
        {
            this.logger.log(string.Format("KillProcessAndChildren, pid={0}", pid));
            using (var searcher = new ManagementObjectSearcher
                ("Select * From Win32_Process Where ParentProcessID=" + pid))
            {
                var moc = searcher.Get();
                foreach (ManagementObject mo in moc)
                {
                    KillProcessAndChildren(Convert.ToInt32(mo["ProcessID"]));
                }
                try
                {
                    var proc = Process.GetProcessById(pid);
                    proc.Kill();
                }
                catch (Exception e)
                {
                    // Process already exited.
                }
            }
        }

        public void connectManager()
        {
            this.logger.log("connectManager");

            logger.log("Change MANAGER_STATUS to CONNECTING");
            MANAGER_STATUS = "CONNECTING";

            this.logger.log("create socket");
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            bool connected = false;
            while (!connected || MANAGER_STATUS == "CANCELED")
            {
                try
                {
                    logger.log("try to connect to openvpn management service");
                    socket.Connect(this.mgmtHost, this.mgmtPort);
                    logger.log("Connected!");
                    connected = true;

                    logger.log("Change MANAGER_STATUS to CONNECTED");
                    MANAGER_STATUS = "CONNECTED";
                }
                catch (System.Net.Sockets.SocketException e)
                {
                    logger.log("SocketException, still not ready. wait 1s");
                    Thread.Sleep(1000);
                }
            }

            if (MANAGER_STATUS == "CANCELED")
            {
                MANAGER_STATUS = "NOT_CONNECTED";
            } else
            {
                SendGreeting();
            }
        }

        public void cancelManagerConnect()
        {
            MANAGER_STATUS = "CANCELED";
        }

        #region Commands
        public string GetStatus()
        {
            return this.managerSendCommand("status");
        }

        public string GetState()
        {
            return this.managerSendCommand("state");
        }

        public string GetState(int n = 1)
        {
            return this.managerSendCommand(string.Format("state {0}", n));
        }

        public string GetStateAll()
        {
            return this.managerSendCommand("state all");
        }

        public string SetStateOn()
        {
            return this.managerSendCommand("state on");
        }

        public string SetStateOnAll()
        {
            return this.managerSendCommand("state on all");
        }

        public string GetStateOff()
        {
            return this.managerSendCommand("state off");
        }

        public string GetVersion()
        {
            return this.managerSendCommand("version");
        }

        public string GetPid()
        {
            return this.managerSendCommand("pid");
        }

        public string SendSignal(Signal sgn)
        {
            return this.managerSendCommand(string.Format("signal SIG{0}", sgn.ToString().ToUpper()));
        }

        public string Mute()
        {
            return this.managerSendCommand("pid");
        }

        public string GetEcho()
        {
            return this.managerSendCommand("echo");
        }

        public string GetHelp()
        {
            return this.managerSendCommand("help");
        }

        public string Kill(string name)
        {
            return this.managerSendCommand(string.Format("kill {0}", name));
        }

        public string GetNet()
        {
            return this.managerSendCommand("net");
        }

        public string GetLogAll()
        {
            return this.managerSendCommand("state off");
        }

        public string SetLogOn()
        {
            return this.managerSendCommand("log on");
        }

        public string SetLogOnAll()
        {
            return this.managerSendCommand("log on all");
        }

        public string SetLogOff()
        {
            return this.managerSendCommand("log off");
        }

        public string GetLog(int n = 1)
        {
            return this.managerSendCommand(string.Format("log {0}", n));
        }

        public string SendMalCommand()
        {
            return this.managerSendCommand("fdsfds");
        }

        private static string TreamRetrievedString(string s)
        {
            return s.Replace("\0", "");
        }

        private void SendGreeting()
        {
            var bf = new byte[bufferSize];
            int rb = socket.Receive(bf, 0, bf.Length, SocketFlags.None);
            if (rb < 1)
            {
                throw new SocketException();
            }
        }
        #endregion

        private string managerSendCommand(String cmd)
        {
            this.logger.log(string.Format("managerSendCommand: cmd={0}", cmd));

            this.logger.log("Check manager status, socket and socket status");
            if (this.MANAGER_STATUS != "CONNECTED" || socket == null || socket.Connected == false)
            {
                this.logger.log("something bad with manager connect");
                return null;
            }
            try
            {
                socket.Send(Encoding.Default.GetBytes(cmd + "\r\n"));
                var bf = new byte[bufferSize];
                var sb = new System.Text.StringBuilder();
                int rb;
                string str = "";
                while (true)
                {
                    Thread.Sleep(100);
                    rb = socket.Receive(bf, 0, bf.Length, 0);
                    str = Encoding.UTF8.GetString(bf).Replace("\0", "");
                    if (rb < bf.Length)
                    {
                        if (str.Contains("\r\nEND"))
                        {
                            var a = str.Substring(0, str.IndexOf("\r\nEND"));
                            sb.Append(a);
                        }
                        else if (str.Contains("SUCCESS: "))
                        {
                            var a = str.Replace("SUCCESS: ", "").Replace("\r\n", "");
                            sb.Append(a);
                        }
                        else if (str.Contains("ERROR: "))
                        {
                            var msg = str.Replace("ERROR: ", "").Replace("\r\n", "");
                            throw new ArgumentException(msg);
                        }
                        else
                        {
                            //todo
                            continue;
                        }

                        break;
                    }
                    else
                    {
                        sb.Append(str);
                    }
                }

                return sb.ToString();
            } catch (System.Net.Sockets.SocketException ex)
            {
                return null;
            }
        }

        public void Dispose()
        {
            if (socket != null)
            {
                if (ovpnFileName != null)
                {
                    SendSignal(Signal.Term);
                }
            }

            socket.Dispose();
            managerProcess.Close();
        }
    }
}
