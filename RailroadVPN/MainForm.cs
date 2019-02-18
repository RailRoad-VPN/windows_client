using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailRoadVPN
{
    public partial class MainForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private Logger logger = Logger.GetInstance();

        private OpenVPNService openVPNService;
        private ServiceAPI serviceAPI;


        public MainForm()
        {
            InitializeComponent();

            string mgmtHost = Properties.Settings.Default.openvpn_management_host;
            int mgmtPort = Properties.Settings.Default.openvpn_management_port;
            this.openVPNService = new OpenVPNService(mgmtHost: mgmtHost, mgmtPort: mgmtPort);

            this.serviceAPI = new ServiceAPI();
        }

        private void menuLogoutBtn_Click(object sender, EventArgs e)
        {
            var user_uuid = Properties.Settings.Default["user_uuid"];
            Properties.Settings.Default.Reset();
            Properties.Settings.Default.Save();
            user_uuid = Properties.Settings.Default["user_uuid"];
            Properties.Settings.Default["user_uuid"] = null;
            Properties.Settings.Default.Save();
            user_uuid = Properties.Settings.Default["user_uuid"];
        }

        private void MainForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private int _menuStartPos = 0;      // start position of the panel
        private int _menuEndPos = 150;      // end position of the panel
        private int _stepSizeAnimation = 10;      // pixels to move

        private void menuBtn_Click(object sender, EventArgs e)
        {
            menuTimer.Enabled = true;
        }

        bool hidden = true;
        private void menuTimer_Tick(object sender, EventArgs e)
        {
            // if just starting, move to start location and make visible
            if (!menuNavPanel.Visible)
            {
                menuNavPanel.Width = _menuStartPos;
                menuNavPanel.Visible = true;
            }

            if (hidden)
            {
                // show menu

                // incrementally move
                menuNavPanel.Width += _stepSizeAnimation;
                this.menuBtn.Left += _stepSizeAnimation;
                // make sure we didn't over shoot
                if (menuNavPanel.Width > _menuEndPos) menuNavPanel.Width = _menuEndPos;

                // have we arrived?
                if (menuNavPanel.Width == _menuEndPos)
                {
                    hidden = false;
                    menuTimer.Enabled = false;
                }
            }
            else
            {
                // hide menu

                // incrementally move
                menuNavPanel.Width -= _stepSizeAnimation;
                this.menuBtn.Left -= _stepSizeAnimation;
                // make sure we didn't over shoot
                if (menuNavPanel.Width < _menuStartPos) menuNavPanel.Width = _menuStartPos;

                // have we arrived?
                if (menuNavPanel.Width == _menuStartPos)
                {
                    hidden = true;
                    menuTimer.Enabled = false;
                }
            }
        }

        private string VPN_CONNECT_STATUS = "NOT_CONNECTED";
        private Thread vpnConnectingThread;
        private Thread vpnDisconnectingThread;

        private void semaphorePic_Click(object sender, EventArgs e)
        {
            this.semaphoreTimer.Enabled = true;
            this.statusTextTimer.Enabled = true;

            if (VPN_CONNECT_STATUS == "NOT_CONNECTED")
            {
                VPN_CONNECT_STATUS = "CONNECTING";
                setVPNStatusText("Connecting...");
                this.semaphorePic.BackgroundImage = Properties.Resources.yellow;
                this.vpnConnectingThread = new Thread(() =>
                   {
                       Thread.CurrentThread.IsBackground = true;
                       /* run your code here */
                       setVPNStatusText("Get server...");
                       this.logger.log("get random server");
                       Guid userGuid = Guid.Parse(Properties.Settings.Default.user_uuid);
                       string randomServerUuid = this.serviceAPI.getUserRandomServer(userGuid);

                       setVPNStatusText("Get server configuration...");
                       this.logger.log("get server config");
                       string configStr = this.serviceAPI.getUserVPNServerConfiguration(userUuid: userGuid, serverUuid: Guid.Parse(randomServerUuid));
                       string configPath = Utils.getBinariesDirPath() + "\\openvpn_rroad_config.ovpn";
                       this.logger.log("server config path: " + configPath);

                       setVPNStatusText("Save server configuration...");
                       this.logger.log("persist config to file system");
                       System.IO.File.WriteAllText(configPath, configStr);

                       // TODO DEBUG!!! REMOVE THIS LINE!!!
                       File.AppendAllText(configPath, "management 127.0.0.1 7505" + Environment.NewLine);

                       setVPNStatusText("Check installing driver...");
                       this.logger.log("try to install driver");
                       this.openVPNService.installTapDriver();

                       setVPNStatusText("Start VPN...");
                       this.openVPNService.startOpenVPN();
                       setVPNStatusText("Start VPN Manager...");
                       this.openVPNService.connectManager();
                   });
                this.vpnConnectingThread.Start();
            }
            else if (VPN_CONNECT_STATUS == "CONNECTING")
            {
                VPN_CONNECT_STATUS = "DISCONNECTING";

                setVPNStatusText("Disconnecting...");
                if (this.vpnConnectingThread.IsAlive)
                {
                    this.vpnConnectingThread.Abort();
                }
                
                this.semaphorePic.BackgroundImage = Properties.Resources.yellow;
                this.vpnDisconnectingThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    this.logger.log("stop openvpn");
                    this.openVPNService.stopOpenVPN();
                    setVPNStatusText("Disconnected");
                    VPN_CONNECT_STATUS = "NOT_CONNECTED";
                    this.semaphorePic.BackgroundImage = Properties.Resources.red;
                });
                this.vpnDisconnectingThread.Start();
            }
            else if (VPN_CONNECT_STATUS == "CONNECTED")
            {
                VPN_CONNECT_STATUS = "DISCONNECTING";

                setVPNStatusText("Disconnecting...");
                this.semaphorePic.BackgroundImage = Properties.Resources.yellow;

                this.vpnDisconnectingThread = new Thread(() =>
                {
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    this.logger.log("stop openvpn");
                    this.openVPNService.stopOpenVPN();
                    setVPNStatusText("Disconnected");
                    VPN_CONNECT_STATUS = "NOT_CONNECTED";
                    this.semaphorePic.BackgroundImage = Properties.Resources.red;
                });
                this.vpnDisconnectingThread.Start();
            }
        }

        private bool isLeftYellow = true;
        private void semaphoreTimer_Tick(object sender, EventArgs e)
        {
            if (VPN_CONNECT_STATUS == "CONNECTING" || VPN_CONNECT_STATUS == "DISCONNECTING")
            {
                if (isLeftYellow)
                {
                    this.semaphorePic.BackgroundImage = Properties.Resources.yellow_right;
                    isLeftYellow = false;
                } else
                {
                    this.semaphorePic.BackgroundImage = Properties.Resources.yellow_left;
                    isLeftYellow = true;
                }
            }
        }

        private void statusTextTimer_Tick(object sender, EventArgs e)
        {
            string status = this.getOpenVPNStatus();
            if (status == "CONNECTED")
            {
                this.semaphoreTimer.Enabled = false;
                this.statusTextTimer.Enabled = false;
                isLeftYellow = true;
                this.semaphorePic.BackgroundImage = Properties.Resources.green;
                setVPNStatusText("Connected");
                VPN_CONNECT_STATUS = "CONNECTED";
            }
            else if (status == "EXITING")
            {
                // TODO think
                this.semaphoreTimer.Enabled = false;
                this.statusTextTimer.Enabled = false;
                isLeftYellow = true;
                this.semaphorePic.BackgroundImage = Properties.Resources.red;
                setVPNStatusText("Disconnected");
            }
        }

        /*
CONNECTING    -- OpenVPN's initial state.
WAIT          -- (Client only) Waiting for initial response
                 from server.
AUTH          -- (Client only) Authenticating with server.
GET_CONFIG    -- (Client only) Downloading configuration options
                 from server.
ASSIGN_IP     -- Assigning IP address to virtual network
                 interface.
ADD_ROUTES    -- Adding routes to system.
CONNECTED     -- Initialization Sequence Completed.
RECONNECTING  -- A restart has occurred.
EXITING       -- A graceful exit is in progress.
         */
        private string getOpenVPNStatus() {
            String stateRaw = this.openVPNService.GetState();
            if (stateRaw == null)
            {
                return "NOT_CONNECTED";
            }
            string[] stateArr = stateRaw.Split(new char[] { ',' });
            string statusText = stateArr[1]; // CONNECTED
            return statusText;
        }

        private string getOpenVPNVirtualIP()
        {
            String state = this.openVPNService.GetState();
            if (state == null)
            {
                // TODO
                return "";
            }
            string[] stateArr = state.Split(new char[] { ',' });
            string virtIp = stateArr[3]; // virtualIP (10.10.0.10)
            return virtIp;
        }

        // TODO
        private string getTrafficInfo()
        {
            string statusRaw = this.openVPNService.GetStatus();
            if (statusRaw == null)
            {
                // TODO
                return "";
            }

            string[] statusArr = statusRaw.Split(
                new[] { "\r\n", "\r", "\n" },
                StringSplitOptions.None
            );

            string read_bytes = "";
            string write_bytes = "";
            foreach (string statusTxt in statusArr)
            {
                if (statusTxt.StartsWith("TCP/UDP read"))
                {
                    read_bytes = statusTxt.Split(new char[] { ',' })[1];
                }
                else if (statusTxt.StartsWith("TCP/UDP write"))
                {
                    write_bytes = statusTxt.Split(new char[] { ',' })[1];
                }
            }

            return "";
        }

        delegate void SetTextCallback(string text);

        private void setVPNStatusText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.statusLabel.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(setVPNStatusText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.statusLabel.Text = text;
            }
        }
    }
}
