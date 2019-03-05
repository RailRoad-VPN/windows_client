using Newtonsoft.Json;
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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailRoadVPN
{
    public partial class StartAppForm : Form
    {
        static Logger logger = Logger.GetInstance();
        private OpenVPNService openVPNService;

        private System.ComponentModel.BackgroundWorker initAppWorker;

        public StartAppForm()
        {
            InitializeComponent();
            initializeWorkers();
            initAppWorker.RunWorkerAsync();

            this.openVPNService = new OpenVPNService();
        }

        private void initializeWorkers()
        {
            initAppWorker.DoWork += new DoWorkEventHandler(initAppWorkder_DoWork);
            initAppWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(initAppWorkder_RunWorkerCompleted);
            initAppWorker.ProgressChanged += new ProgressChangedEventHandler(initAppWorkder_ProgressChanged);
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
            loadingBar.Maximum = 100;
            loadingBar.Step = 1;
        }

        private void initAppWorkder_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            e.Result = prepareApplicationToStart(worker, e);
        }

        private void initAppWorkder_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.Message);
                return;
            }

            if (e.Result == null)
            {
                this.Close();
                return;
            }

            Form resFo = (Form) e.Result;
            resFo.Location = this.Location;
            this.Hide();
            resFo.Closed += (s, args) => this.Close();
            resFo.Show();
        }

        private void initAppWorkder_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.loadingBar.Value = e.ProgressPercentage;
        }

        delegate void setProgressLabelTextCallback(string text);

        private void setProgressLabelText(string text)
        {
            if (this.startProgressLabel.InvokeRequired)
            {
                setProgressLabelTextCallback d = new setProgressLabelTextCallback(setProgressLabelText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                this.startProgressLabel.Text = text;
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

        private Form prepareApplicationToStart(BackgroundWorker worker, DoWorkEventArgs e)
        {
            setProgressLabelText("Cleanup application processes..");
            Utils.killAllOpenVPNProcesses();

            ServiceAPI serviceAPI = new ServiceAPI();

            setProgressLabelText("Check updates...");
            string currentAppVersion = Properties.Settings.Default.app_version;
            logger.log("Current application version: " + currentAppVersion);
            try
            {
                string version = serviceAPI.getAppVersion();
                logger.log("New application version: " + version);
                if (currentAppVersion != version)
                {
                    logger.log("Versions are no equal. Ask to update");
                    DialogResult dialogResult = MessageBox.Show(Properties.strings.new_version_message, Properties.strings.new_version_header, MessageBoxButtons.YesNo);
                    if (dialogResult == DialogResult.Yes)
                    {
                        logger.log("Dialog Yes");
                        System.Diagnostics.Process.Start("https://rroadvpn.net/download");
                        return null;
                    }
                    else if (dialogResult == DialogResult.No)
                    {
                        logger.log("Dialog No");
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.log("Exception when check application version: " + ex.Message);
            }

            setProgressLabelText("Read properties..");
            string user_uuid = null;
            string device_token = null;
            string device_uuid = null;
            string locale = null;
            string local_app_openvpn_binaries_dir_checksum = null;
            int reportProgress = 3;
            worker.ReportProgress(reportProgress);
            try
            {
                logger.log("get local_app_openvpn_binaries_dir_checksum variable from properties");
                local_app_openvpn_binaries_dir_checksum = Properties.Settings.Default.local_app_openvpn_binaries_dir_checksum;
                logger.log("local_app_openvpn_binaries_dir_checksum: " + local_app_openvpn_binaries_dir_checksum);

                logger.log("get locale variable from properties");
                locale = Properties.Settings.Default.locale;
                logger.log("locale: " + locale);

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
                setProgressLabelText("Error code: 01. Write to support..");

                openHelpForm();

                return null;
            }
            reportProgress = 9;
            worker.ReportProgress(reportProgress);
            // check user device
            try
            {
                if (user_uuid != null && device_uuid != null && device_token != null && user_uuid != "" && device_uuid != "" && device_token != "")
                {
                    logger.log("check user device. all properties are not null and not empty");

                    logger.log("get user device from server");
                    UserDevice userDevice = serviceAPI.getUserDevice(userUuid: Guid.Parse(user_uuid), deviceUuid: Guid.Parse(device_uuid));
                    if (userDevice.Uuid.ToString() != device_uuid)
                    {
                        logger.log("device uuid from properties NOT EQUAL to device uuid from server");
                        setProgressLabelText("Error code: 04. Write to support..");

                        openHelpForm();

                        return null;
                    }
                    else if (userDevice.UserUuid.ToString() != user_uuid)
                    {
                        logger.log("user uuid from properties NOT EQUAL to user uuid from server");
                        setProgressLabelText("Error code: 05. Write to support..");

                        openHelpForm();

                        return null;
                    }
                    else if (userDevice.IsActive == false)
                    {
                        logger.log("user device IS NOT active. DO SOMETHING!!!");
                        //setProgressLabelText("Error code: 03. You device is not active");

                        //DialogResult dialogResult = MessageBox.Show(Properties.strings.device_not_active_message, Properties.strings.device_not_active_header, MessageBoxButtons.YesNo);
                        //if (dialogResult == DialogResult.Yes)
                        //{
                        //    logger.log("Dialog Yes");
                        //    openHelpForm();
                        //    return null;
                        //}
                        //else if (dialogResult == DialogResult.No)
                        //{
                        //    logger.log("Dialog No");
                        //    return null;
                        //}
                    }
                }
            }
            catch (Exception ex)
            {
                logger.log("Exception when check user device: " + ex.Message);
                setProgressLabelText("Error code: 02. Write to support..");

                openHelpForm();

                return null;
            }

            // check user
            if (user_uuid != null && user_uuid != "")
            {
                logger.log("check user. user uuid is NOT empty");
                try
                {
                    User user = serviceAPI.getUserByUuid(userUuid: Guid.Parse(user_uuid));
                    if (!user.enabled)
                    {
                        logger.log("user enabled is FALSE");
                        MessageBox.Show(Properties.strings.user_disabled_message, Properties.strings.user_bad_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        openHelpForm();
                        return null;
                    }
                    else if (user.is_locked)
                    {
                        logger.log("user is_locked is TRUE");
                        MessageBox.Show(Properties.strings.user_locked_message, Properties.strings.user_bad_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        openHelpForm();
                        return null;
                    }
                    else if (user.is_expired)
                    {
                        logger.log("user is_expired is TRUE");
                        MessageBox.Show(Properties.strings.user_expired_message, Properties.strings.user_bad_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        openHelpForm();
                        return null;
                    }
                    else if (user.is_password_expired)
                    {
                        logger.log("user is_password_expired is TRUE");
                        MessageBox.Show(Properties.strings.user_password_expired_message, "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    logger.log("Exception when get user by uuid: " + ex.Message);
                }
            } else
            {
                logger.log("user_uuid is empty. no check needed");
            }

            reportProgress = 15;
            worker.ReportProgress(reportProgress);

            
            logger.log("User locale: " + locale);
            logger.log("create culture info");
            var culture = new CultureInfo(locale);
            logger.log("setup culture to any UI thread");
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            setProgressLabelText("Check binaries..");
            logger.log("get binaries path");
            string binaries_path = Utils.getBinariesDirPath();
            logger.log("binaries path: " + binaries_path);
            bool need_extract = false;
            reportProgress = 20;
            worker.ReportProgress(reportProgress);

            Thread.Sleep(3000);

            logger.log("check binaries dir exist");
            if (Directory.Exists(binaries_path))
            {
                setProgressLabelText("Calculating checksum of binaries..");
                logger.log("binaries dir exist. get checksum of it");
                string checksum = Utils.CreateMd5ForFolder(binaries_path);
                logger.log("checksum: " + checksum);

                reportProgress = 25;
                worker.ReportProgress(reportProgress);
                if (local_app_openvpn_binaries_dir_checksum == null || local_app_openvpn_binaries_dir_checksum == "" || checksum != local_app_openvpn_binaries_dir_checksum)
                {
                    reportProgress = 26;
                    worker.ReportProgress(reportProgress);
                    logger.log("checksum does not equal, need to re-extract");
                    need_extract = true;
                    setProgressLabelText("Delete binaries..");
                    logger.log("delete exist dir");
                    Directory.Delete(binaries_path, true);
                    reportProgress = 30;
                    worker.ReportProgress(reportProgress);
                }
            }
            else
            {
                need_extract = true;
            }

            if (need_extract)
            {
                setProgressLabelText("Extracting binaries..");
                reportProgress = 31;
                worker.ReportProgress(reportProgress);
                logger.log("extract zip file from resource to Local AppData folder");

                logger.log("get zip file as bytes array");
                var zipBytes = Properties.Resources.rroad_openvpn;
                reportProgress = 35;
                worker.ReportProgress(reportProgress);

                logger.log("get path where to extract zip");
                string zipOut = Utils.getLocalAppDirPath();
                logger.log("zipOut: " + zipOut);
                reportProgress = 36;
                worker.ReportProgress(reportProgress);

                logger.log("create zip stream");
                Stream zipStream = new MemoryStream(zipBytes);
                reportProgress = 40;
                worker.ReportProgress(reportProgress);

                logger.log("open zip");
                ZipStorer zip = ZipStorer.Open(zipStream, FileAccess.Read);
                reportProgress = 45;
                worker.ReportProgress(reportProgress);

                logger.log("read the central directory collection");
                List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();
                reportProgress = 47;
                worker.ReportProgress(reportProgress);

                logger.log("iterate zip entries");
                foreach (ZipStorer.ZipFileEntry entry in dir)
                {
                    logger.log("forming path to extract entry from zip");
                    string entryOut = zipOut + "\\" + entry.FilenameInZip;
                    logger.log("entryOut: " + entryOut);

                    reportProgress += 1;
                    worker.ReportProgress(reportProgress);

                    logger.log("extract");
                    zip.ExtractFile(entry, entryOut);
                }
                logger.log("close zip");
                zip.Close();

                reportProgress = 68;
                worker.ReportProgress(reportProgress);
            }

            Thread.Sleep(500);

            setProgressLabelText("Checking installed driver..");

            ExtraSystemInformation esi = Utils.getSystemInformation();
            List<NetworkAdapterInfo> nicList = esi.NetworkAdapterInfoList;
            bool isTapDriverInstalled = Utils.isTapDriverInstalled(nicList);
            if (!isTapDriverInstalled)
            {
                setProgressLabelText("Driver is not installed. Installing...");
                Thread.Sleep(2000);
                this.openVPNService.installTapDriver();
            }

            string todayLog = DateTime.UtcNow.Date.ToString("yyyyMMdd") + "_" + Properties.Settings.Default.app_logfile_name;
            string todayVPNLog = DateTime.UtcNow.Date.ToString("yyyyMMdd") + "_" + Properties.Settings.Default.openvpn_logfile_name;
            setProgressLabelText("Cleaning old log files..");
            List<FileInfo> logFiles = Utils.getLogFiles();
            foreach (FileInfo logFile in logFiles)
            {
                string fileName = logFile.Name;
                if (fileName == todayLog || fileName == todayVPNLog)
                {
                    logger.log("today log or today openvpn log file. log file name: " + fileName);
                    continue;
                } else
                {
                    logger.log("delete log file with name: " + fileName);
                    logFile.Delete();
                }
            }

            setProgressLabelText("Loading..");

            Form form = null;
            reportProgress = 80;
            worker.ReportProgress(reportProgress);
            if (user_uuid != null && user_uuid != "")
            {
                reportProgress = 85;
                worker.ReportProgress(reportProgress);
                logger.log("Loading MainForm");
                //form = new MainForm();
                form = FormManager.Current.CreateForm<MainForm>();
                Thread.Sleep(500);
                reportProgress = 90;
                worker.ReportProgress(reportProgress);
            }
            else
            {
                reportProgress = 85;
                worker.ReportProgress(reportProgress);
                logger.log("Loading InpuntPinForm");
                //form = new InputPinForm();
                form = FormManager.Current.CreateForm<InputPinForm>();
                Thread.Sleep(500);
                reportProgress = 90;
                worker.ReportProgress(reportProgress);
            }
            Thread.Sleep(500);
            reportProgress = 99;
            worker.ReportProgress(reportProgress);

            return form;
        }
    }
}