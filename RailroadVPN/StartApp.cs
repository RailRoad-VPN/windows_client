using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailRoadVPN
{
    public partial class StartApp : Form
    {
        static Logger logger = Logger.GetInstance();

        public StartApp()
        {
            InitializeComponent();
        }

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void minimizeBtn_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void StartApp_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void StartApp_Load(object sender, EventArgs e)
        {
            loadingBar.Maximum = 10;
            loadingBar.Step = 1;

            string user_uuid = null;
            loadingBar.PerformStep();
            logger.log("get user_uuid variable from properties");
            loadingBar.PerformStep();
            try
            {
                user_uuid = (string)Properties.Settings.Default.user_uuid;
                loadingBar.PerformStep();
            }
            catch (System.Configuration.ConfigurationException ex)
            {
                logger.log("Configuration Exception");
                logger.log(ex.BareMessage);
                return;
            }

            logger.log("user_uuid: " + user_uuid);
            loadingBar.PerformStep();

            logger.log("check unzipped folder from resource");
            string checksum = Utils.CreateMd5ForFolder(Utils.getLocalAppDir() + "//" + Properties.Settings.Default.local_app_openvpn_binaries_dir);

            bool exist = false;
            loadingBar.PerformStep();

            if (exist == false)
            {
                loadingBar.PerformStep(); 
                logger.log("extract zip file from resource to Local AppData folder");

                logger.log("get zip file as bytes array");
                var zipBytes = Properties.Resources.rroad_openvpn;
                loadingBar.PerformStep();

                logger.log("create path where to extract zip");
                string zipOut = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\RailRoadVPN";
                logger.log("zipOut: " + zipOut);
                loadingBar.PerformStep();

                logger.log("create zip stream");
                Stream zipStream = new MemoryStream(zipBytes);
                loadingBar.PerformStep();

                logger.log("open zip");
                ZipStorer zip = ZipStorer.Open(zipStream, FileAccess.Read);
                loadingBar.PerformStep();

                logger.log("read the central directory collection");
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
                loadingBar.PerformStep();

                logger.log("iterate zip entries");
                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    loadingBar.PerformStep();
                    logger.log("forming path to extract entry from zip");
                    string entryOut = zipOut + "//" + entry.FilenameInZip;
                    logger.log("entryOut: " + entryOut);

                    logger.log("extract");
                    zip.ExtractFile(entry, entryOut);
                }
                logger.log("close zip");
                zip.Close();
                loadingBar.PerformStep();
            }

            Form form = null;
            loadingBar.PerformStep();
            if (user_uuid != null && user_uuid != "")
            {
                loadingBar.PerformStep();
                logger.log("Loading MainForm");
                loadingBar.PerformStep();
                form = new MainForm();
                loadingBar.PerformStep();
            }
            else
            {
                loadingBar.PerformStep();
                logger.log("Loading InpuntPinForm");
                loadingBar.PerformStep();
                form = new InputPinForm();
                loadingBar.PerformStep();
            }
            loadingBar.PerformStep();
            form.Show();
        }

        private void StartApp_Shown(Object sender, EventArgs e)
        {
        }
    }
}
