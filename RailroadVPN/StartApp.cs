﻿using System;
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

        private System.ComponentModel.BackgroundWorker initAppWorker;

        public StartAppForm()
        {
            InitializeComponent();
            initializeWorkers();
            initAppWorker.RunWorkerAsync();
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
            }

            Form resFo = (Form) e.Result;
            this.Hide();
            resFo.Closed += (s, args) => this.Close();
            resFo.Show();
        }

        private void initAppWorkder_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.loadingBar.Value = e.ProgressPercentage;
        }

        private Form prepareApplicationToStart(BackgroundWorker worker, DoWorkEventArgs e)
        {
            Utils.killAllOpenVPNProcesses();

            string user_uuid = null;
            int reportProgress = 3;
            worker.ReportProgress(reportProgress);
            logger.log("get user_uuid variable from properties");
            reportProgress = 6;
            worker.ReportProgress(reportProgress);
            try
            {
                user_uuid = (string)Properties.Settings.Default.user_uuid;
                reportProgress = 9;
                worker.ReportProgress(reportProgress);
            }
            catch (System.Configuration.ConfigurationException ex)
            {
                logger.log("Configuration Exception");
                logger.log(ex.BareMessage);
                throw new SystemException("bad news with configuration");
            }

            logger.log("user_uuid: " + user_uuid);
            reportProgress = 15;
            worker.ReportProgress(reportProgress);

            logger.log("get user language");
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string locale = ci.ToString();
            logger.log("user locale: " + locale);

            logger.log("persist to settings");
            Properties.Settings.Default.locale = locale;

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
                logger.log("binaries dir exist. get checksum of it");
                string checksum = Utils.CreateMd5ForFolder(binaries_path);
                logger.log("checksum: " + checksum);

                reportProgress = 25;
                worker.ReportProgress(reportProgress);
                if (checksum != Properties.Settings.Default.local_app_openvpn_binaries_dir_checksum)
                {
                    reportProgress = 26;
                    worker.ReportProgress(reportProgress);
                    logger.log("checksum does not equal, need to re-extract");
                    need_extract = true;
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