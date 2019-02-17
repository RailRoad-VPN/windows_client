using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
            loadingBar.Maximum = 10000;
            loadingBar.Step = 1;

            for (int j = 0; j < loadingBar.Maximum; j++)
            {
                double pow = Math.Pow(j, j); //Calculation
                loadingBar.PerformStep();
            }

            string user_uuid = null;
            logger.log("get user_uuid variable from properties");
            try
            {
                user_uuid = (string)Properties.Settings.Default.user_uuid;
            }
            catch (System.Configuration.ConfigurationException ex)
            {
                logger.log("Configuration Exception");
                logger.log(ex.BareMessage);
                return;
            }

            logger.log("user_uuid: " + user_uuid);

            Form form = null;
            if (user_uuid != null && user_uuid != "")
            {
                logger.log("Loading MainForm");
                form = new MainForm();
            }
            else
            {
                logger.log("Loading InpuntPinForm");
                form = new InputPinForm();
            }

            form.Show();
        }

        private void StartApp_Shown(Object sender, EventArgs e)
        {
            this.Hide();
            this.Visible = false;
        }
    }
}
