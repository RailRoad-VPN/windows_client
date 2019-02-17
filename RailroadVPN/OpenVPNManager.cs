﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.IO;

namespace RailRoadVPN
{
    class OpenVPNManager : IDisposable
    {

        private Logger logger = Logger.GetInstance();

        public enum Signal
        {
            Hup,
            Term,
            Usr1,
            Usr2
        }

        private Socket socket;
        private string host;
        private int port;
        private const int bufferSize = 1024;
        private string ovpnFileName;
        private const string eventName = "MyOpenVpnEvent";
        private readonly Process prc = new Process();
        private readonly string openVpnExePath;

        private void RunOpenVpnProcess()
        {
            //managerProcess.StartInfo.CreateNoWindow = false;
            //managerProcess.EnableRaisingEvents = true;
            //managerProcess.StartInfo.Arguments = string.Format("--config {0} --service {1} 0", ovpnFileName, eventName);
            //managerProcess.StartInfo.FileName = openVpnExePath;
            //managerProcess.Start();
        }

        public OpenVPNManager(string host, int port)
        {
            this.host = host;
            this.port = port;
        }

        public void start()
        {
            RunOpenVpnProcess();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            bool connected = false;
            while (!connected) {
                try {
                    logger.log("try to connect to openvpn management service");
                    socket.Connect(this.host, this.port);
                    logger.log("Connected!");
                    connected = true;
                } catch (System.Net.Sockets.SocketException e)
                {
                    logger.log("SocketException, still not ready. wait 1s");
                    Thread.Sleep(1000);
                }
            }
           
            SendGreeting();
        }

        #region Commands
        public string GetStatus()
        {
            return this.SendCommand("status");
        }

        public string GetState()
        {
            return this.SendCommand("state");
        }

        public string GetState(int n = 1)
        {
            return this.SendCommand(string.Format("state {0}", n));
        }

        public string GetStateAll()
        {
            return this.SendCommand("state all");
        }

        public string SetStateOn()
        {
            return this.SendCommand("state on");
        }

        public string SetStateOnAll()
        {
            return this.SendCommand("state on all");
        }

        public string GetStateOff()
        {
            return this.SendCommand("state off");
        }

        public string GetVersion()
        {
            return this.SendCommand("version");
        }

        public string GetPid()
        {
            return this.SendCommand("pid");
        }

        public string SendSignal(Signal sgn)
        {
            return this.SendCommand(string.Format("signal SIG{0}", sgn.ToString().ToUpper()));
        }

        public string Mute()
        {
            return this.SendCommand("pid");
        }

        public string GetEcho()
        {
            return this.SendCommand("echo");
        }

        public string GetHelp()
        {
            return this.SendCommand("help");
        }

        public string Kill(string name)
        {
            return this.SendCommand(string.Format("kill {0}", name));
        }

        public string GetNet()
        {
            return this.SendCommand("net");
        }

        public string GetLogAll()
        {
            return this.SendCommand("state off");
        }

        public string SetLogOn()
        {
            return this.SendCommand("log on");
        }

        public string SetLogOnAll()
        {
            return this.SendCommand("log on all");
        }

        public string SetLogOff()
        {
            return this.SendCommand("log off");
        }

        public string GetLog(int n = 1)
        {
            return this.SendCommand(string.Format("log {0}", n));
        }

        public string SendMalCommand()
        {
            return this.SendCommand("fdsfds");
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

        private string SendCommand(String cmd)
        {
            if (socket == null || socket.Connected == false)
            {
                throw new OpenVPNNotConnectedException("need to connect to OpenVPN first");
            }
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
            prc.Close();
        }
    }
}
