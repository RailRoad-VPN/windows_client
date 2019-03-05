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

        private PropertiesHelper propertiesHelper = PropertiesHelper.GetInstance();

        private OpenVPNService openVPNService;
        private ServiceAPI serviceAPI;

        private string connectionUuid;
        private string ConnectedSince;
        private string VirtualIp;
        private long BytesI;
        private long BytesO;

        private int _menuBtnStartPos;

        private Thread handleConnectionThread;
        private Thread checkUserDeviceThread;
        private Thread vpnConnectingThread;
        private Thread vpnDisconnectingThread;

        private volatile bool handeConnectionThreadCanceled = false;
        private volatile bool checkUserDeviceThreadCanceled = false;
        private volatile bool updateVPNConnectionThreadCanceled = false;

        private bool inactiveDeviceMessageShown = false;

        public MainForm()
        {
            InitializeComponent();

            string mgmtHost = Properties.Settings.Default.openvpn_management_host;
            int mgmtPort = Properties.Settings.Default.openvpn_management_port;
            this.openVPNService = new OpenVPNService(mgmtHost: mgmtHost, mgmtPort: mgmtPort);

            this.serviceAPI = new ServiceAPI();
            this._menuBtnStartPos = this.menuBtn.Left;

            this.statusLabel.Text = Properties.strings.vpn_connect_status;
            this.helpText1Label.Text = Properties.strings.help_1_text_label;
            this.helpTextRedLabel.Text = Properties.strings.help_red_text_label;
            this.helpText2Label.Text = Properties.strings.help_2_text_label;
            this.helpTextGreenLabel.Text = Properties.strings.help_green_text_label;

            this.menuLogoutLabel.Text = Properties.strings.menu_logout_text_label;
            this.menuNeedHelpLabel.Text = Properties.strings.menu_needhelp_text_label;
            this.menuMyProfileLabel.Text = Properties.strings.menu_myprofile_text_label;

            this.updateHelpTextUI();

            string userUuidStr = Properties.Settings.Default.user_uuid;
            if (userUuidStr == "")
            {
                propertiesHelper.clearProperties();
                InputPinForm ipf = FormManager.Current.CreateForm<InputPinForm>();
                ipf.Location = this.Location;
                this.Hide();
                ipf.Closed += (s, args) => this.Close();
                ipf.Show();
            }

            this.handleConnectionThread = new Thread(() => {
                Thread.CurrentThread.IsBackground = true;

                bool connectionCreated = false;
                while(!handeConnectionThreadCanceled)
                {
                    string status = this.getOpenVPNStatus();

                    if (status == "CONNECTED" && connectionCreated == false)
                    {
                        this.logger.log("create connection");
                        // CREATE CONNECTION
                        try
                        {
                            Guid UserUuid = Guid.Parse(Properties.Settings.Default.user_uuid);
                            Guid ServerUuid = Guid.Parse(Properties.Settings.Default.server_uuid);
                            Guid UserDeviceUuid = Guid.Parse(Properties.Settings.Default.device_uuid);

                            OpenVPNTrafficInfo tafficInfo = this.getOpenVPNTrafficInfo();
                            string virtualIp = this.getOpenVPNVirtualIP();

                            int cnt = 0;
                            while (virtualIp == "" || cnt != 5)
                            {
                                virtualIp = this.getOpenVPNVirtualIP();
                                cnt += 1;
                            }

                            if (virtualIp == "")
                            {
                                this.logger.log("we can't get virtual IP after connect. skip create connection");
                                return;
                            }

                            long bytesI = 0;
                            long bytesO = 0;
                            if (tafficInfo != null)
                            {
                                bytesI = tafficInfo.BytesI;
                                bytesO = tafficInfo.BytesO;
                            }
                            else
                            {
                                this.logger.log("we can't get information about traffic after connect. skip create connection");
                                return;
                            }
                            bool IsConnected = true;
                            this.ConnectedSince = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK");
                            this.connectionUuid = this.serviceAPI.createConnection(UserUuid: UserUuid, ServerUuid: ServerUuid, UserDeviceUuid: UserDeviceUuid, DeviceIp: null,
                                VirtualIp: virtualIp, BytesI: bytesI, BytesO: bytesO, IsConnected: IsConnected, ConnectedSince: ConnectedSince);
                            this.logger.log("Connection created with uuid: " + this.connectionUuid);
                            connectionCreated = true;
                            this.BytesI = bytesI;
                            this.BytesO = bytesO;
                            this.VirtualIp = virtualIp;
                        }
                        catch (Exception ex)
                        {
                            this.logger.log("Exception when create connection: " + ex.Message);
                        }
                    }
                    else if (status == "CONNECTED" && connectionCreated == true)
                    {
                        // UPDATE CONNECTION
                        try
                        {
                            this.updateConnection(IsConnected: true, ConnectedSince: this.ConnectedSince);
                            Thread.Sleep(10000);
                        }
                        catch (Exception ex)
                        {
                            this.logger.log("Exception when update connection while connected to VPN: " + ex.Message);
                        }
                    } else if (status == "NOT_CONNECTED" && connectionCreated == true)
                    {
                        this.logger.log("update connection after disconnect");
                        connectionCreated = false;
                        // UPDATE CONNECTION when disconnect from VPN
                        try
                        {
                            this.updateConnection(IsConnected: false, ConnectedSince: this.ConnectedSince);
                            Thread.Sleep(10000);
                        }
                        catch (Exception ex)
                        {
                            this.logger.log("Exception when update connection while disconnect to VPN: " + ex.Message);
                        }
                    } else
                    {
                        Thread.Sleep(10000);
                    }
                }
                this.handeConnectionThreadCanceled = false;
            });
            handleConnectionThread.Start();

            this.checkUserDeviceThread = new Thread(() => {
                while (!checkUserDeviceThreadCanceled)
                {
                    bool isOk = checkUserDevice();
                    Properties.Settings.Default.is_user_device_active = isOk;

                    if (!isOk)
                    {
                        if (this.VPN_CONNECT_STATUS == "CONNECTED")
                        {
                            this.logger.log("update connection because checkuserdevice");
                            try { this.updateConnection(IsConnected: false, ConnectedSince: this.ConnectedSince); } catch (Exception ex) { this.logger.log("Exception when update connection while checkuserdevice: " + ex.Message); }
                            if (this.vpnConnectingThread.IsAlive)
                            {
                                this.vpnConnectingThread.Abort();
                            }
                            this.createVPNDisconnectingThread();
                            this.vpnDisconnectingThread.Start();
                        }

                        if (!this.inactiveDeviceMessageShown)
                        {
                            this.inactiveDeviceMessageShown = true;
                            this.setVPNStatusText("Code: 03. You device is not active");

                            DialogResult dialogResult = MessageBox.Show(Properties.strings.device_not_active_message, Properties.strings.device_not_active_header, MessageBoxButtons.YesNo);
                            if (dialogResult == DialogResult.Yes)
                            {
                                logger.log("Dialog Yes");
                                openHelpForm();
                            }
                            else if (dialogResult == DialogResult.No)
                            {
                                logger.log("Dialog No");
                            }
                        }
                    } else
                    {
                        if (this.inactiveDeviceMessageShown)
                        {
                            this.inactiveDeviceMessageShown = false;
                            this.setVPNStatusText("Your device is active!");
                            MessageBox.Show(Properties.strings.device_active_message, Properties.strings.device_active_message, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                        }
                    }

                    if (this.inactiveDeviceMessageShown)
                    {
                        Thread.Sleep(10000);
                    } else
                    {
                        Thread.Sleep(600000);
                    }
                }
            });
            this.checkUserDeviceThread.Start();
        }

        private void createVPNConnectingThread()
        {
            this.logger.log("create vpn connecting thread");
            this.vpnConnectingThread = new Thread(() =>
            {
                this.logger.log("connecting vpn thread: start");

                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                this.logger.log("connecting vpn thread: set vpn status text to Get server...");
                this.setVPNStatusText("Detect server...");
                this.logger.log("connecting vpn thread: get random server");
                Guid userGuid = Guid.Parse(Properties.Settings.Default.user_uuid);

                string randomServerUuid;
                try
                {
                    randomServerUuid = this.serviceAPI.getUserRandomServer(userGuid);
                }
                catch (Exception ex)
                {
                    this.logger.log("connecting vpn thread: Exception when get random server uuid: " + ex.Message);
                    this.VPN_CONNECT_STATUS = "INTERUPTED";
                    this.setVPNStatusText(Properties.strings.check_internet_connect_header);
                    this.semaphorePic.BackgroundImage = Properties.Resources.red;
                    MessageBox.Show(Properties.strings.unknown_system_error_message, Properties.strings.unknown_system_error_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;

                }

                Properties.Settings.Default.server_uuid = randomServerUuid;

                string configPath = Utils.getServersConfigDirPath() + "\\" + randomServerUuid + ".ovpn";

                this.logger.log("connecting vpn thread: check existing configuration of this server");
                if (!propertiesHelper.hasVPNServer(uuid: randomServerUuid) || !File.Exists(configPath))
                {
                    this.logger.log("connecting vpn thread: we have not this server. get it");
                    setVPNStatusText("Get server...");
                    Guid userUuid = Guid.Parse(Properties.Settings.Default.user_uuid);
                    Guid serverUuid = Guid.Parse(randomServerUuid);

                    VPNServer vpnServer = null;
                    try
                    {
                        vpnServer = this.serviceAPI.getVPNServer(userUuid: userUuid, serverUuid: serverUuid);
                    }
                    catch (Exception ex)
                    {
                        this.VPN_CONNECT_STATUS = "INTERUPTED";
                        this.setVPNStatusText(Properties.strings.check_internet_connect_header);
                        this.semaphorePic.BackgroundImage = Properties.Resources.red;

                        this.logger.log("connecting vpn thread: Exception: " + ex.Message);
                        MessageBox.Show(Properties.strings.unknown_system_error_message, Properties.strings.unknown_system_error_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                    propertiesHelper.addVPNServer(vpnServer: vpnServer);

                    setVPNStatusText("connecting vpn thread: Get server configuration...");
                    this.logger.log("connecting vpn thread: get server configuration file");

                    string configStr;
                    try
                    {
                        configStr = this.serviceAPI.getUserVPNServerConfiguration(userUuid: userGuid, serverUuid: Guid.Parse(randomServerUuid));
                    }
                    catch (Exception ex)
                    {
                        this.VPN_CONNECT_STATUS = "INTERUPTED";
                        this.setVPNStatusText(Properties.strings.check_internet_connect_header);
                        this.semaphorePic.BackgroundImage = Properties.Resources.red;

                        this.logger.log("connecting vpn thread: Exception: " + ex.Message);
                        MessageBox.Show(Properties.strings.unknown_system_error_message, Properties.strings.unknown_system_error_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    try
                    {
                        this.logger.log("connecting vpn thread: save server configuration to file: " + configPath);
                        System.IO.File.WriteAllText(configPath, configStr);
                    }
                    catch (Exception ex)
                    {
                        this.logger.log("connecting vpn thread: Exception when save server configuration to file: " + ex.Message);
                        this.VPN_CONNECT_STATUS = "INTERUPTED";
                        this.setVPNStatusText(Properties.strings.check_internet_connect_header);
                        this.semaphorePic.BackgroundImage = Properties.Resources.red;
                        MessageBox.Show(Properties.strings.unknown_system_error_message, Properties.strings.unknown_system_error_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                setVPNStatusText("Starting VPN...");
                this.openVPNService.startOpenVPN(serverUuid: randomServerUuid);
                Thread.Sleep(2000);
                setVPNStatusText("Starting Manager...");
                try
                {
                    this.openVPNService.connectManager();
                }
                catch (Exception ex)
                {
                    logger.log("connecting vpn thread: Exception when connec to manager: " + ex.Message);
                }
                // TODO
                Thread.Sleep(1000);

                Properties.Settings.Default.vpn_status = "connected";
            });

        }

        private void createVPNDisconnectingThread()
        {
            this.logger.log("create vpn disconnecting thread");
            this.vpnDisconnectingThread = new Thread(() =>
            {
                this.logger.log("disconnecting vpn thread: start");
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                this.logger.log("disconnecting vpn thread: update connection");
                try
                {
                    this.updateConnection(IsConnected: false, ConnectedSince: this.ConnectedSince);
                    Thread.Sleep(2000);
                }
                catch (Exception ex)
                {
                    this.logger.log("Exception when update connection while disconnect to VPN: " + ex.Message);
                }
                this.logger.log("disconnecting vpn thread: stop openvpn");
                try { this.openVPNService.stopOpenVPN(); } catch (Exception) { }

                try
                {
                    this.logger.log("disconnecting vpn thread: disable status text timer");
                    this.semaphoreTimer.Enabled = false;
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
                    this.updateHelpTextUI();
                } catch (Exception ex)
                {
                    this.logger.log("disconnecting vpn thread: exception with resources");
                }
                Properties.Settings.Default.vpn_status = "disconnected";
            });
        }

        delegate void UpdateHelpTextUICallback();

        private void updateHelpTextUI()
        {
            if (this.statusLabel.InvokeRequired)
            {
                UpdateHelpTextUICallback d = new UpdateHelpTextUICallback(updateHelpTextUI);
                this.Invoke(d);
            }
            else
            {
                this.statusLabel.Refresh();
                this.helpText1Label.Refresh();
                this.helpTextRedLabel.Refresh();
                this.helpText2Label.Refresh();
                this.helpTextGreenLabel.Refresh();

                if (VPN_CONNECT_STATUS == "CONNECTED")
                {
                    this.helpTextGreenLabel.Location = new Point(this.helpText1Label.Location.X + this.helpText1Label.Size.Width + 5, this.helpText1Label.Location.Y);
                    this.helpText2Label.Location = new Point(this.helpTextGreenLabel.Location.X + this.helpTextGreenLabel.Size.Width + 5, this.helpTextGreenLabel.Location.Y);
                }
                else
                {
                    this.helpTextRedLabel.Location = new Point(this.helpText1Label.Location.X + this.helpText1Label.Size.Width + 5, this.helpText1Label.Location.Y);
                    this.helpText2Label.Location = new Point(this.helpTextRedLabel.Location.X + this.helpTextRedLabel.Size.Width + 5, this.helpTextRedLabel.Location.Y);
                }
            }
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
            this.logger.log("update connection (disconnect)");
            try { this.updateConnection(IsConnected: false, ConnectedSince: this.ConnectedSince); } catch (Exception) { }

            this.logger.log("close btn click");
            try {
                if (this.vpnConnectingThread.IsAlive)
                {
                    this.logger.log("connecting alive. try to abort");
                    this.vpnConnectingThread.Abort();
                }
                if (this.vpnDisconnectingThread.IsAlive)
                {
                    this.logger.log("disconnecting alive. try to abort");
                    this.vpnDisconnectingThread.Abort();
                }
            }
            catch (Exception) { }
            this.logger.log("try to stop vpn");
            if (this.VPN_CONNECT_STATUS == "CONNECTED")
            {
                try { this.openVPNService.stopOpenVPN(); } catch (Exception) { }
            }
            this.logger.log("abort handleconnection thread");
            try { this.handleConnectionThread.Abort(); } catch (Exception) { }
            this.logger.log("abort checkuserdevice thread");
            try { this.checkUserDeviceThread.Abort();} catch (Exception) { }

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

        private int btnPressedWhileConnecting = 1;

        private void updateUserDevice(bool IsActive, string VirtualIp, string ModifyReason)
        {
            this.logger.log("updateUserDevice method in Form");

            this.logger.log("get device uuid");
            Guid DeviceUuid = Guid.Parse(Properties.Settings.Default.device_uuid);
            this.logger.log("get user uuid");
            Guid UserUuid = Guid.Parse(Properties.Settings.Default.user_uuid);
            this.logger.log("get device id");
            string DeviceId = Properties.Settings.Default.device_id;

            // TODO user api to get geo location of device
            this.logger.log("get culture info to set location field");
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string Location = ci.DisplayName;

            try
            {
                this.logger.log("call update user device");
                this.serviceAPI.updateUserDevice(DeviceUuid: DeviceUuid, UserUuid: UserUuid, DeviceId: DeviceId, Location: Location, IsActive: IsActive, ModifyReason: ModifyReason);
            }
            catch (Exception e)
            {
                this.logger.log("RailroadException when update device: " + e.Message);
            }
        }

        private void openHelpForm()
        {
            logger.log("open help form");

            NeedHelpForm nhf = new NeedHelpForm();

            int parent_left = this.Left;
            int parent_top = this.Top;
            parent_left = parent_left + ((this.Width - nhf.Width) / 2);
            parent_top = parent_top + ((this.Height - nhf.Height) / 2);

            nhf.Location = new Point(parent_left, parent_top);
            nhf.ShowDialog();
        }

        private bool checkUserDevice(bool showForm = false)
        {
            string user_uuid = null;
            string device_token = null;
            string device_uuid = null;
            try
            {
                logger.log("get user_uuid variable from properties");
                user_uuid = Properties.Settings.Default.user_uuid;
                logger.log("user_uuid: " + user_uuid);

                logger.log("get device_uuid variable from properties");
                device_uuid = Properties.Settings.Default.device_uuid;
                logger.log("device_uuid: " + device_uuid);

                logger.log("get device_token variable from properties");
                device_token = Properties.Settings.Default.x_device_token;
                logger.log("device_token: " + device_token);
            }
            catch (System.Configuration.ConfigurationException ex)
            {
                logger.log("Configuration Exception");
                logger.log(ex.BareMessage);
                this.setVPNStatusText("Error code: 01");

                openHelpForm();

                return false;
            }

            this.setVPNStatusText("Device checklist...");
            // check user device
            try
            {
                logger.log("check user device. all properties are not null and not empty");

                logger.log("get user device from server");
                UserDevice userDevice = serviceAPI.getUserDevice(userUuid: Guid.Parse(user_uuid), deviceUuid: Guid.Parse(device_uuid));
                if (userDevice.Uuid.ToString() != device_uuid)
                {
                    logger.log("device uuid from properties NOT EQUAL to device uuid from server");
                    this.setVPNStatusText("Error code: 04");

                    openHelpForm();

                    return false;
                }
                else if (userDevice.UserUuid.ToString() != user_uuid)
                {
                    logger.log("user uuid from properties NOT EQUAL to user uuid from server");
                    this.setVPNStatusText("Error code: 05");

                    openHelpForm();

                    return false;
                }
                else if (userDevice.IsActive == false)
                {
                    logger.log("user device IS NOT active. DO SOMETHING!!!");

                    this.setVPNStatusText("You device is not active");

                    if (showForm)
                    {
                        DialogResult dialogResult = MessageBox.Show(Properties.strings.device_not_active_message, Properties.strings.device_not_active_header, MessageBoxButtons.YesNo);
                        if (dialogResult == DialogResult.Yes)
                        {
                            logger.log("Dialog Yes");
                            openHelpForm();
                            return false;
                        }
                        else if (dialogResult == DialogResult.No)
                        {
                            logger.log("Dialog No");
                            return false;
                        }
                    } else
                    {
                        return false;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                logger.log("Exception when check user device: " + ex.Message);
            }

            // check user
            this.setVPNStatusText("Account checklist...");
            logger.log("check user");
            try
            {
                User user = serviceAPI.getUserByUuid(userUuid: Guid.Parse(user_uuid));
                if (!user.enabled)
                {
                    logger.log("user enabled is FALSE");
                    this.setVPNStatusText("Account problem");
                    MessageBox.Show(Properties.strings.user_disabled_message, Properties.strings.user_bad_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    openHelpForm();
                    return false;
                }
                else if (user.is_locked)
                {
                    logger.log("user is_locked is TRUE");
                    this.setVPNStatusText("Account problem");
                    MessageBox.Show(Properties.strings.user_locked_message, Properties.strings.user_bad_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    openHelpForm();
                    return false;
                }
                else if (user.is_expired)
                {
                    logger.log("user is_expired is TRUE");
                    this.setVPNStatusText("Account problem");
                    MessageBox.Show(Properties.strings.user_expired_message, Properties.strings.user_bad_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    openHelpForm();
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.log("Exception when get user by uuid: " + ex.Message);
            }

            if (this.VPN_CONNECT_STATUS == "CONNECTED")
            {
                this.setVPNStatusText("Connected");
            } else if (this.VPN_CONNECT_STATUS == "NOT_CONNECTED")
            {
                this.setVPNStatusText(Properties.strings.vpn_connect_status);
            }

            return true;
        }

        private void semaphorePic_Click(object sender, EventArgs e)
        {

            if (this.VPN_CONNECT_STATUS == "NOT_CONNECTED")
            {
                bool isUserDeviceActive = Properties.Settings.Default.is_user_device_active;
                if (!isUserDeviceActive)
                {
                    if (!this.checkUserDevice(showForm: true)) {
                        return;
                    }
                }

                this.setVPNStatusText("Checking driver...");
                List<NetworkAdapterInfo> nicList = Utils.getNetworkInformation();
                this.logger.log("check is tap driver exist");
                bool isTapDriverInstalled = Utils.isTapDriverInstalled(nicList);
                if (!isTapDriverInstalled)
                {
                    this.setVPNStatusText("Installing driver..");
                    this.logger.log("tap driver is not exist. start installing");
                    this.openVPNService.installTapDriver();

                    nicList = Utils.getNetworkInformation();
                    isTapDriverInstalled = Utils.isTapDriverInstalled(nicList);

                    if (!isTapDriverInstalled)
                    {
                        MessageBox.Show(Properties.strings.main_form_tap_adapter_not_installed_message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                } else
                {
                    this.logger.log("check is tap driver busy");
                    bool isTapDriverBusy = Utils.isTapDriverBusy(nicList);
                    if (isTapDriverBusy)
                    {
                        this.setVPNStatusText("Disconnect other VPN first");
                        this.logger.log("tap driver is busy. cancel connecting");
                        MessageBox.Show(Properties.strings.main_form_tap_adapter_busy_message, Properties.strings.main_form_tap_adapter_busy_header, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                this.logger.log("set vpn connect status to CONNECTING");
                this.VPN_CONNECT_STATUS = "CONNECTING";
                this.logger.log("set vpn connect status text to Connecting");
                this.setVPNStatusText("Connecting...");
                this.logger.log("set semaphore picture to yellow");
                this.semaphorePic.BackgroundImage = Properties.Resources.yellow;

                this.semaphoreTimer.Enabled = true;
                this.statusTextTimer.Enabled = true;

                this.createVPNConnectingThread();
                this.vpnConnectingThread.Start();
            }
            else if (VPN_CONNECT_STATUS == "CONNECTING")
            {
                this.semaphoreTimer.Enabled = true;
                this.statusTextTimer.Enabled = true;
                this.logger.log("VPN CONNECT STATUS is CONNECTING");

                this.logger.log("check count of button pressed");
                if (btnPressedWhileConnecting < 2)
                {
                    this.logger.log("btn pressed lt 2 and eq=" + btnPressedWhileConnecting);
                    
                    if (btnPressedWhileConnecting == 1)
                    {
                        this.logger.log("first press while connecting. show dialog");
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
                try
                {
                    if (this.vpnConnectingThread.IsAlive)
                    {
                        this.logger.log("alive. try to abort");
                        this.vpnConnectingThread.Abort();
                    }
                } catch (Exception ex)
                {
                    this.logger.log("Exception when check vpnConnectingThread: " + ex.Message);
                }

                this.logger.log("set semaphore picture to yellow");
                this.semaphorePic.BackgroundImage = Properties.Resources.yellow;

                this.createVPNDisconnectingThread();
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

                this.createVPNDisconnectingThread();
                this.logger.log("start vpn disconnecting thread");
                this.vpnDisconnectingThread.Start();
            } else
            {
                this.logger.log("Semaphore button was pressed but nothing happened. VPN_CONNECT_STATUS: " + VPN_CONNECT_STATUS);
                Properties.Settings.Default.vpn_status = "disconnected";
                return;
            }
        }

        private bool isLeftYellow = true;
        private void semaphoreTimer_Tick(object sender, EventArgs e)
        {
            if (this.VPN_CONNECT_STATUS != "CONNECTED" && this.VPN_CONNECT_STATUS != "NOT_CONNECTED")
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
        private string helpText2LabelRedText = Properties.strings.help_2_text_label;
        private string helpText2LabelGreenText = Properties.strings.help_2_green_text_label;

        private void statusTextTimer_Tick(object sender, EventArgs e)
        {
            string status = this.getOpenVPNStatus();
            //this.logger.log("OpenVPN status: " + status);

            if ((this.VPN_CONNECT_STATUS == "NOT_CONNECTED" && status == "NOT_CONNECTED") || (this.VPN_CONNECT_STATUS == "CONNECTED" && status == "CONNECTED"))
            {
                return;
            }

            this.VPN_CONNECT_STATUS = status;

            if (this.VPN_CONNECT_STATUS == "NOT_CONNECTED" || this.VPN_CONNECT_STATUS == "INTERUPTED")
            {
                this.logger.log("VPN_CONNECT_STATUS: " + this.VPN_CONNECT_STATUS);
                this.logger.log("set VPN_CONNECT_STATUS to NOT_CONNECTED");
                this.VPN_CONNECT_STATUS = "NOT_CONNECTED";
                this.logger.log("disable semaphore timer");
                this.semaphoreTimer.Enabled = false;
                this.isLeftYellow = true;
                this.logger.log("disable status text timer");
                this.statusTextTimer.Enabled = false;
                this.logger.log("change help text green label visible false");
                this.changeLabelVisible(this.helpTextGreenLabel, false);
                this.logger.log("change help text red label visible true");
                this.changeLabelVisible(this.helpTextRedLabel, true);
                this.logger.log("change help text 2 label Left attribute to red pos");
                this.changeLabelLeftAttr(this.helpText2Label, this.helpText2LabelLeftRedPos);
                this.logger.log("change help text 2 label Text attibute to red text");
                this.changeLabelTextAttr(this.helpText2Label, this.helpText2LabelRedText);
            } else if (this.VPN_CONNECT_STATUS == "CONNECTED")
            {
                this.logger.log("set vpn status text to Connected");
                this.setVPNStatusText("Connected");
                this.logger.log("disable semaphore timer");
                this.semaphoreTimer.Enabled = false;
                this.isLeftYellow = true;
                this.logger.log("disable status text timer");
                this.statusTextTimer.Enabled = false;
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
            } else if (this.VPN_CONNECT_STATUS == "EXITING")
            {
                this.VPN_CONNECT_STATUS = "NOT_CONNECTED";
                this.logger.log("set vpn status text to Disconnected");
                this.setVPNStatusText("Disconnected");
                this.logger.log("disable semaphore timer");
                this.semaphoreTimer.Enabled = false;
                this.isLeftYellow = true;
                this.logger.log("disable status text timer");
                this.statusTextTimer.Enabled = false;
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

            this.updateHelpTextUI();
        }

        //private void createConnection()
        //{
        //    this.logger.log("create connection from Form");
        //    Guid UserUuid = Guid.Parse(Properties.Settings.Default.user_uuid);
        //    Guid ServerUuid = Guid.Parse(Properties.Settings.Default.server_uuid);
        //    Guid UserDeviceUuid = Guid.Parse(Properties.Settings.Default.device_uuid);
        //    string DeviceIp = null;

        //    try
        //    {
        //        this.logger.log("try to get VPNTrafficInfo");
        //        OpenVPNTrafficInfo tafficInfo = this.getOpenVPNTrafficInfo();
        //        this.logger.log("try to get openvpnvirutal ip");
        //        this.VirtualIp = this.getOpenVPNVirtualIP();

        //        long BytesI = 0;
        //        long BytesO = 0;
        //        if (tafficInfo != null)
        //        {
        //            BytesI = tafficInfo.BytesI;
        //            BytesO = tafficInfo.BytesO;
        //        }
        //        bool IsConnected = true;
        //        this.ConnectedSince = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssK");
        //        this.connectionUuid = this.serviceAPI.createConnection(UserUuid: UserUuid, ServerUuid: ServerUuid, UserDeviceUuid: UserDeviceUuid, DeviceIp: DeviceIp,
        //            VirtualIp: VirtualIp, BytesI: BytesI, BytesO: BytesO, IsConnected: IsConnected, ConnectedSince: ConnectedSince);
        //        this.logger.log("Connection created with uuid: " + this.connectionUuid);
        //    }
        //    catch (Exception ex)
        //    {
        //        this.logger.log("Exception when create connection: " + ex.Message);
        //    }
        //}

        private void updateConnection(bool IsConnected, string ConnectedSince)
        {
            OpenVPNTrafficInfo tafficInfo = this.getOpenVPNTrafficInfo();
            Guid UserUuid = Guid.Parse(Properties.Settings.Default.user_uuid);
            Guid ServerUuid = Guid.Parse(Properties.Settings.Default.server_uuid);
            Guid UserDeviceUuid = Guid.Parse(Properties.Settings.Default.device_uuid);

            try
            {
                long bytesI = this.BytesI;
                long bytesO = this.BytesO;
                if (tafficInfo != null)
                {
                    bytesI = tafficInfo.BytesI;
                    bytesO = tafficInfo.BytesO;
                }
                this.serviceAPI.updateConnection(ConnectionUuid: Guid.Parse(this.connectionUuid), UserUuid: UserUuid, 
                    ServerUuid: ServerUuid, BytesI: bytesI, BytesO: bytesO, IsConnected: IsConnected, 
                    ModifyReason: "update connection");
                this.BytesI = bytesI;
                this.BytesO = bytesO;
            } catch (Exception ex)
            {
                this.logger.log("Exception when update connection: " + ex.Message);
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
            try
            {
                String stateRaw = this.openVPNService.GetState();
                if (stateRaw == null || this.VPN_CONNECT_STATUS == "NOT_CONNECTED")
                {
                    return this.VPN_CONNECT_STATUS;
                }
                string[] stateArr = stateRaw.Split(new char[] { ',' });
                string statusText = stateArr[1]; // CONNECTED
                return statusText;
            }
            catch (Exception ex)
            {
                this.logger.log("Exception when getOpenVPNStatus: " + ex.Message);
                return this.VPN_CONNECT_STATUS;
            }
        }

        private string getOpenVPNVirtualIP()
        {
            try
            {
                String state = this.openVPNService.GetState();
                if (state == null)
                {
                    this.logger.log("getOpenVPNVirtualIP state is null");
                    return null;
                }
                string[] stateArr = state.Split(new char[] { ',' });
                string virtIp = stateArr[3]; // virtualIP (10.10.0.10)
                return virtIp;
            }
            catch (Exception ex)
            {
                this.logger.log("Exception when getOpenVPNVirtualIP: " + ex.Message);
                return null;
            }
        }

        private OpenVPNTrafficInfo getOpenVPNTrafficInfo()
        {
            try
            {
                string statusRaw = this.openVPNService.GetStatus();
                if (statusRaw == null)
                {
                    // TODO
                    return null;
                }

                string[] statusArr = statusRaw.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None
                );

                long read_bytes = 0;
                long write_bytes = 0;
                foreach (string statusTxt in statusArr)
                {
                    if (statusTxt.StartsWith("TCP/UDP read"))
                    {
                        read_bytes = Convert.ToInt64(statusTxt.Split(new char[] { ',' })[1]);
                    }
                    else if (statusTxt.StartsWith("TCP/UDP write"))
                    {
                        write_bytes = Convert.ToInt64(statusTxt.Split(new char[] { ',' })[1]);
                    }
                }

                OpenVPNTrafficInfo trafficInfo = new OpenVPNTrafficInfo(BytesI: read_bytes, BytesO: write_bytes);

                return trafficInfo;
            } catch (Exception ex)
            {
                this.logger.log("Exception when getOpenVPNTrafficInfo: " + ex.Message);
                return null;
            }
        }

        private void enLangBtn_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.locale = "en-US";
            Properties.Settings.Default.Save();
            MessageBox.Show("Restart application", "Change language", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ruLangBtn_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.locale = "ru-RU";
            Properties.Settings.Default.Save();
            MessageBox.Show("Перезапустите приложение", "Смена языка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void menuLogoutLabel_Click(object sender, EventArgs e)
        {
            logger.log("logout menu click");

            bool is_vpn_connected = this.VPN_CONNECT_STATUS == "CONNECTED" || Properties.Settings.Default.vpn_status == "connected" || Utils.isTapDriverBusy();

            if (is_vpn_connected)
            {
                MessageBox.Show(Properties.strings.disconnect_vpn_first, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                // in case something goes so so so so wrong
                if (this.VPN_CONNECT_STATUS == "CONNECTED")
                {
                    this.createVPNDisconnectingThread();
                    this.vpnDisconnectingThread.Start();
                    this.vpnDisconnectingThread.Join();

                    this.handeConnectionThreadCanceled = true;
                    this.checkUserDeviceThreadCanceled = true;
                    if (this.vpnConnectingThread.IsAlive)
                    {
                        this.logger.log("alive. try to abort");
                        this.vpnConnectingThread.Abort();
                    }
                }
            }
            catch (Exception ex) { this.logger.log("Exception stop vpn and cancel threads when logout"); }

            this.logger.log("update user device (logout)");
            try { this.updateUserDevice(IsActive: false, VirtualIp: this.VirtualIp, ModifyReason: "logout action"); } catch (Exception ex) { this.logger.log("Exception when update user device while logout: " + ex.Message); }

            this.logger.log("clear properties");
            propertiesHelper.clearProperties();

            this.logger.log("open input pin form");
            InputPinForm ipf = FormManager.Current.CreateForm<InputPinForm>();
            ipf.Location = this.Location;
            this.Hide();
            ipf.Closed += (s, args) => this.Close();
            ipf.Show();
        }

        private void menuNeedHelpLabel_Click(object sender, EventArgs e)
        {
            if (this.VPN_CONNECT_STATUS == "CONNECTED" || Properties.Settings.Default.vpn_status == "connected" || Utils.isTapDriverBusy())
            {
                MessageBox.Show(Properties.strings.disconnect_vpn_first, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            NeedHelpForm nhf = new NeedHelpForm();

            int parent_left = this.Left;
            int parent_top = this.Top;
            parent_left = parent_left + ((this.Width - nhf.Width) / 2);
            parent_top = parent_top + ((this.Height - nhf.Height) / 2);

            nhf.Location = new Point(parent_left, parent_top);
            nhf.ShowDialog();
        }

        private void menuMyProfileLabel_MouseHover(object sender, EventArgs e)
        {
            this.menuMyProfileLabel.ForeColor = System.Drawing.Color.White;
            this.menuMyProfileLabel.Refresh();
        }

        private void menuMyProfileLabel_MouseLeave(object sender, EventArgs e)
        {
            this.menuMyProfileLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(204)))), ((int)(((byte)(70)))));
            this.menuMyProfileLabel.Refresh();
        }

        private void menuLogoutLabel_MouseHover(object sender, EventArgs e)
        {
            this.menuLogoutLabel.ForeColor = System.Drawing.Color.White;
            this.menuLogoutLabel.Refresh();
        }

        private void menuLogoutLabel_MouseLeave(object sender, EventArgs e)
        {
            this.menuLogoutLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(204)))), ((int)(((byte)(70)))));
            this.menuLogoutLabel.Refresh();
        }

        private void menuNeedHelpLabel_MouseHover(object sender, EventArgs e)
        {
            this.menuNeedHelpLabel.ForeColor = System.Drawing.Color.White;
            this.menuNeedHelpLabel.Refresh();
        }

        private void menuNeedHelpLabel_MouseLeave(object sender, EventArgs e)
        {
            this.menuNeedHelpLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(204)))), ((int)(((byte)(70)))));
            this.menuNeedHelpLabel.Refresh();
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
