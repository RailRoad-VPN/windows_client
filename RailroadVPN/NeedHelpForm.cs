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
    public partial class NeedHelpForm : Form
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private Logger logger = Logger.GetInstance();

        private PropertiesHelper propertiesHelper = PropertiesHelper.GetInstance();

        public NeedHelpForm()
        {
            InitializeComponent();
        }

        private void NeedHelpForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void sendBtn_Click(object sender, EventArgs e)
        {
            if (this.sendLogsCheckBox.Checked)
            {
                // TODO archive logs and send
                List<FileInfo> logFiles = Utils.getLogFiles();
                using (ZipStorer zip = ZipStorer.Create(Utils.getLocalAppDirPath() + "\\out.zip", "RailRoadVPN"))
                {
                    foreach (FileInfo logFile in logFiles)
                    {
                        zip.AddFile(ZipStorer.Compression.Deflate, logFile.FullName, logFile.Name, "");
                    }
                }   // automatic close operation here
            }

            // TODO create help ticket
        }
    }
}
