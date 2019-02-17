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

        private OpenVPNService openVPNService;
        private OpenVPNManager openVPNManager;
        private ServiceAPI serviceAPI;


        public MainForm()
        {
            InitializeComponent();

            string mgmtHost = Properties.Settings.Default.openvpn_management_host;
            int mgmtPort = Properties.Settings.Default.openvpn_management_port;
            this.openVPNService = new OpenVPNService(mgmtHost: mgmtHost, mgmtPort: mgmtPort);

            this.serviceAPI = new ServiceAPI();
        }

        private void button1_Click(object sender, EventArgs e)
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

        private void button3_Click(object sender, EventArgs e)
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
            var zipBytes = Properties.Resources.rroad_openvpn;

            string zipOut = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RailRoadVPN";

            Stream zipStream = new MemoryStream(zipBytes);
            ZipStorer zip = ZipStorer.Open(zipStream, FileAccess.Read);

            // Read the central directory collection
            List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

            // Look for the desired file
            foreach (ZipStorer.ZipFileEntry entry in dir)
            {
                string entryOut = zipOut + "//" + entry.FilenameInZip;
                zip.ExtractFile(entry, entryOut);
            }
            zip.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string randomServerUuid = this.serviceAPI.getUserRandomServer(Guid.Parse(Properties.Settings.Default.user_uuid));
            string userUuid = Properties.Settings.Default.user_uuid;

            string configStr = this.serviceAPI.getUserVPNServerConfiguration(userUuid: Guid.Parse(userUuid), serverUuid: Guid.Parse(randomServerUuid));
            //string[] configArr = configStr.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            //Console.WriteLine(configStr);
            string configPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RailRoadVPN\\rroad_openvpn\\openvpn_rroad_config.ovpn";
            System.IO.File.WriteAllText(configPath, configStr);
        }
    }
}
