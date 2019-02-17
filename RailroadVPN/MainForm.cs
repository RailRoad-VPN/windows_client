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

        private void button2_Click(object sender, EventArgs e)
        {
            this.openVPNService.installTapDriver();
        }

        private void startOpenVPNBtn_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                /* run your code here */
                this.openVPNService.startOpenVPN();
                this.openVPNService.connectManager();
            }).Start();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                var a1 = this.openVPNService.GetStatus();
                Console.WriteLine(a1);

                var a2 = this.openVPNService.GetPid();
                Console.WriteLine(a2);
            } catch (OpenVPNNotConnectedException ex)
            {

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
            this.Close();
        }

        private void minimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.openVPNService.stopOpenVPN();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string checksum = Utils.CreateMd5ForFolder(Utils.getLocalAppDirPath() + "//" + Properties.Settings.Default.local_app_openvpn_binaries_dir);
            this.logger.log("checksum: " + checksum);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string randomServerUuid = this.serviceAPI.getUserRandomServer(Guid.Parse(Properties.Settings.Default.user_uuid));
            string userUuid = Properties.Settings.Default.user_uuid;

            string configStr = this.serviceAPI.getUserVPNServerConfiguration(userUuid: Guid.Parse(userUuid), serverUuid: Guid.Parse(randomServerUuid));
            string configPath = Utils.getLocalAppDirPath() + "\\" + Properties.Settings.Default.local_app_openvpn_binaries_dir + "\\openvpn_rroad_config.ovpn";
            System.IO.File.WriteAllText(configPath, configStr);
        }

        private int _menuStartPos = 0;      // start position of the panel
        private int _menuEndPos = 150;      // end position of the panel
        private int _stepSizeAnimation = 10;      // pixels to move

        private void menuBtn_Click(object sender, EventArgs e)
        {
            menuTimer.Enabled = true;
        }

        bool hidden = true;
        private void timer1_Tick(object sender, EventArgs e)
        {
            // if just starting, move to start location and make visible
            if (!menuNavPanel.Visible)
            {
                menuNavPanel.Width = _menuStartPos;
                menuNavPanel.Visible = true;
            }
            
            if (hidden) { 
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
            } else
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
    }
}
