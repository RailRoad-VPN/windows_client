using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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

        private int _menuBtnStartPos;

        public MainForm()
        {
            InitializeComponent();

            string mgmtHost = Properties.Settings.Default.openvpn_management_host;
            int mgmtPort = Properties.Settings.Default.openvpn_management_port;
            this.openVPNService = new OpenVPNService(mgmtHost: mgmtHost, mgmtPort: mgmtPort);

            this.serviceAPI = new ServiceAPI();
            this._menuBtnStartPos = this.menuBtn.Left;
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
            if (!menuNavPanel.Visible && hidden)
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
                menuNavPanel.Visible = false;

                // incrementally move
                menuNavPanel.Width -= (_stepSizeAnimation);
                this.menuBtn.Left -= (_stepSizeAnimation);
                // make sure we didn't over shoot
                if (menuNavPanel.Width < _menuStartPos) menuNavPanel.Width = _menuStartPos;
                if (menuBtn.Left < _menuBtnStartPos) menuBtn.Left = _menuBtnStartPos;

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

        private int btnPressedWhileConnecting = 1;

        private void semaphorePic_Click(object sender, EventArgs e)
        {
            this.semaphoreTimer.Enabled = true;
            this.statusTextTimer.Enabled = true;

            if (this.VPN_CONNECT_STATUS == "NOT_CONNECTED")
            {
                this.logger.log("set vpn connect status to CONNECTING");
                this.VPN_CONNECT_STATUS = "CONNECTING";
                this.logger.log("set vpn connect status text to Connecting");
                this.setVPNStatusText("Connecting...");
                this.logger.log("set semaphore picture to yellow");
                this.semaphorePic.BackgroundImage = Properties.Resources.yellow;

                this.logger.log("create vpn connecting thread");
                this.vpnConnectingThread = new Thread(() =>
                   {
                       Thread.CurrentThread.IsBackground = true;
                       /* run your code here */
                       this.logger.log("set vpn status text to Get server...");
                       this.setVPNStatusText("Get server...");
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
                this.logger.log("VPN CONNECT STATUS is CONNECTING");

                this.logger.log("check count of button pressed");
                if (btnPressedWhileConnecting < 2)
                {
                    this.logger.log("btn pressed lt 2 and eq=" + btnPressedWhileConnecting);
                    
                    if (btnPressedWhileConnecting == 1)
                    {
                        this.logger.log("first press while connecting. show dialog");
                        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Properties.Settings.Default.locale);
                        MessageBox.Show(Properties.strings.disconnect_while_connect_message, Properties.strings.disconnect_while_connect_header, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    this.logger.log("+1 to btn pressed");
                    btnPressedWhileConnecting += 1;
                    return;
                }

                this.logger.log("reset btn pressed count to 1");
                btnPressedWhileConnecting = 1;

                this.logger.log("set vpn connect status to DISCONNECTING");
                this.VPN_CONNECT_STATUS = "DISCONNECTING";

                this.logger.log("set vpn connect status text to Disconnecting...");
                this.setVPNStatusText("Disconnecting...");

                this.logger.log("check vpn connecting thread is alive");
                if (this.vpnConnectingThread.IsAlive)
                {
                    this.logger.log("alive. try to abort");
                    this.vpnConnectingThread.Abort();
                }

                this.logger.log("set semaphore picture to yellow");
                this.semaphorePic.BackgroundImage = Properties.Resources.yellow;

                this.logger.log("create vpn disconnecting thread");
                this.vpnDisconnectingThread = new Thread(() =>
                {
                    this.logger.log("disconnecting vpn thread: start");
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    this.logger.log("disconnecting vpn thread: stop openvpn");
                    this.openVPNService.stopOpenVPN();
                    this.logger.log("disconnecting vpn thread: disable status text timer");
                    this.statusTextTimer.Enabled = false;
                    this.logger.log("disconnecting vpn thread: set help text green label visible false");
                    this.changeLabelVisible(this.helpTextGreenLabel, false);
                    this.logger.log("disconnecting vpn thread: set help text red label visible true");
                    this.changeLabelVisible(this.helpTextRedLabel, true);
                    this.logger.log("disconnecting vpn thread: change help text 2 label left attribute");
                    this.changeLabelLeftAttr(this.helpText2Label, this.helpText2LabelLeftRedPos);
                    this.logger.log("disconnecting vpn thread: change help text 2 label text");
                    this.changeLabelTextAttr(this.helpText2Label, this.helpText2LabelRedText);
                    this.logger.log("disconnecting vpn thread: set vpn status text to Disconnected");
                    this.setVPNStatusText("Disconnected");
                    this.logger.log("disconnecting vpn thread: set VPN CONNECT STATUS to NOT_CONNECTED");
                    this.VPN_CONNECT_STATUS = "NOT_CONNECTED";
                    this.logger.log("disconnecting vpn thread: set semaphore picture to red");
                    this.semaphorePic.BackgroundImage = Properties.Resources.red;
                });

                this.logger.log("start vpn disconnecting thread");
                this.vpnDisconnectingThread.Start();
            }
            else if (VPN_CONNECT_STATUS == "CONNECTED")
            {
                this.logger.log("vpn connected. start disconnecting process");
                VPN_CONNECT_STATUS = "DISCONNECTING";

                this.logger.log("set vpnstatustext to Disconnecting");
                this.setVPNStatusText("Disconnecting...");

                this.logger.log("set semaphore picture to yellow");
                this.semaphorePic.BackgroundImage = Properties.Resources.yellow;

                this.logger.log("create vpn disconnecting thread");
                this.vpnDisconnectingThread = new Thread(() =>
                {
                    this.logger.log("disconnecting vpn thread: start");
                    Thread.CurrentThread.IsBackground = true;
                    /* run your code here */
                    this.logger.log("disconnecting vpn thread: stop openvpn");
                    this.openVPNService.stopOpenVPN();
                    this.logger.log("disconnecting vpn thread: disable status text timer");
                    this.statusTextTimer.Enabled = false;
                    this.logger.log("disconnecting vpn thread: set help text green label visible false");
                    this.changeLabelVisible(this.helpTextGreenLabel, false);
                    this.logger.log("disconnecting vpn thread: set help text red label visible true");
                    this.changeLabelVisible(this.helpTextRedLabel, true);
                    this.logger.log("disconnecting vpn thread: change help text 2 label left attribute");
                    this.changeLabelLeftAttr(this.helpText2Label, this.helpText2LabelLeftRedPos);
                    this.logger.log("disconnecting vpn thread: change help text 2 label text");
                    this.changeLabelTextAttr(this.helpText2Label, this.helpText2LabelRedText);
                    this.logger.log("disconnecting vpn thread: set vpn status text to Disconnected");
                    this.setVPNStatusText("Disconnected");
                    this.logger.log("disconnecting vpn thread: set VPN CONNECT STATUS to NOT_CONNECTED");
                    this.VPN_CONNECT_STATUS = "NOT_CONNECTED";
                    this.logger.log("disconnecting vpn thread: set semaphore picture to red");
                    this.semaphorePic.BackgroundImage = Properties.Resources.red;
                });

                this.logger.log("start vpn disconnecting thread");
                this.vpnDisconnectingThread.Start();
            } else
            {
                this.logger.log("Semaphore button was pressed but nothing happened. VPN_CONNECT_STATUS: " + VPN_CONNECT_STATUS);
                return;
            }
        }

        private bool isLeftYellow = true;
        private void semaphoreTimer_Tick(object sender, EventArgs e)
        {
            if (this.VPN_CONNECT_STATUS == "CONNECTING" || this.VPN_CONNECT_STATUS == "DISCONNECTING")
            {
                if (isLeftYellow)
                {
                    this.semaphorePic.BackgroundImage = Properties.Resources.yellow_right;
                    this.isLeftYellow = false;
                } else
                {
                    this.semaphorePic.BackgroundImage = Properties.Resources.yellow_left;
                    this.isLeftYellow = true;
                }
            }
        }

        private int helpText2LabelLeftRedPos = 159;
        private int helpText2LabelLeftGreenPos = 174;
        private string helpText2LabelRedText = "semaphore to connect VPN";
        private string helpText2LabelGreenText = "semaphore to disconnect VPN";

        private void statusTextTimer_Tick(object sender, EventArgs e)
        {
            this.logger.log("statusTextTimer_Tick");

            this.logger.log("getOpenVPNStatus");
            string status = this.getOpenVPNStatus();
            this.logger.log("status:" + status);
            if (status == "CONNECTED")
            {
                this.logger.log("set vpn status text to Connected");
                this.setVPNStatusText("Connected");
                this.logger.log("set vpn connect status to CONNECTED");
                this.VPN_CONNECT_STATUS = "CONNECTED";
                this.logger.log("disable semaphore timer");
                this.semaphoreTimer.Enabled = false;
                this.logger.log("disable status text timer");
                this.statusTextTimer.Enabled = false;
                this.isLeftYellow = true;
                this.logger.log("set semaphore picture to green");
                this.semaphorePic.BackgroundImage = Properties.Resources.green;

                this.logger.log("set help text green label visible true");
                this.changeLabelVisible(this.helpTextGreenLabel, true);
                this.logger.log("set help text red label visible false");
                this.changeLabelVisible(this.helpTextRedLabel, false);
                this.logger.log("set help text 2 label Left attribute to green pos");
                this.changeLabelLeftAttr(this.helpText2Label, this.helpText2LabelLeftGreenPos);
                this.logger.log("set help text 2 label Text attribute to green text");
                this.changeLabelTextAttr(this.helpText2Label, this.helpText2LabelGreenText);
            }
            else if (status == "EXITING")
            {
                this.logger.log("set vpn connect status to NOT_CONNECTED");
                this.VPN_CONNECT_STATUS = "NOT_CONNECTED";
                this.logger.log("set vpn status text to Disconnected");
                this.setVPNStatusText("Disconnected");
                this.logger.log("disable semaphore timer");
                this.semaphoreTimer.Enabled = false;
                this.logger.log("disable status text timer");
                this.statusTextTimer.Enabled = false;
                this.isLeftYellow = true;
                this.logger.log("set semaphore picture to red");
                this.semaphorePic.BackgroundImage = Properties.Resources.red;
                this.logger.log("change help text green label visible false");
                this.changeLabelVisible(this.helpTextGreenLabel, false);
                this.logger.log("change help text red label visible true");
                this.changeLabelVisible(this.helpTextRedLabel, true);
                this.logger.log("change help text 2 label Left attribute to red pos");
                this.changeLabelLeftAttr(this.helpText2Label, this.helpText2LabelLeftRedPos);
                this.logger.log("change help text 2 label Text attibute to red text");
                this.changeLabelTextAttr(this.helpText2Label, this.helpText2LabelRedText);
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
            this.logger.log("getOpenVPNStatus");

            this.logger.log("get state");
            String stateRaw = this.openVPNService.GetState();
            this.logger.log("stateRaw: " + stateRaw);
            if (stateRaw == null)
            {
                this.logger.log("stateRaw is null. vpn not connected");
                return "NOT_CONNECTED";
            }
            this.logger.log("split state with comma");
            string[] stateArr = stateRaw.Split(new char[] { ',' });
            this.logger.log("get 1 element");
            string statusText = stateArr[1]; // CONNECTED
            this.logger.log("statusText is " + statusText);
            return statusText;
        }

        // TODO
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

        delegate void SetVPNStatusTextCallback(string text);

        private void setVPNStatusText(string text)
        {
            if (this.statusLabel.InvokeRequired)
            {
                SetVPNStatusTextCallback d = new SetVPNStatusTextCallback(setVPNStatusText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.statusLabel.Text = text;
            }
        }

        delegate void ChangeLabelVisibleCallback(Label label, bool isVisible);

        private void changeLabelVisible(Label label, bool isVisible)
        {
            if (label.InvokeRequired)
            {
                ChangeLabelVisibleCallback d = new ChangeLabelVisibleCallback(changeLabelVisible);
                this.Invoke(d, new object[] { label, isVisible });
            }
            else
            {
                label.Visible = isVisible;
            }
        }

        delegate void ChangeLabelLeftAttrCallback(Label label, int left);

        private void changeLabelLeftAttr(Label label, int left)
        {
            if (label.InvokeRequired)
            {
                ChangeLabelLeftAttrCallback d = new ChangeLabelLeftAttrCallback(changeLabelLeftAttr);
                this.Invoke(d, new object[] { label, left });
            }
            else
            {
                label.Left = left;
            }
        }

        delegate void ChangeLabelTextAttrCallback(Label label, string text);

        private void changeLabelTextAttr(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                ChangeLabelTextAttrCallback d = new ChangeLabelTextAttrCallback(changeLabelTextAttr);
                this.Invoke(d, new object[] { label, text });
            }
            else
            {
                label.Text = text;
            }
        }
    }
}
