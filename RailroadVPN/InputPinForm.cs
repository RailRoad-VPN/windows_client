using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailRoadVPN
{
    public partial class InputPinForm : Form
    {

        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        private void InputPinForm_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private Logger logger = Logger.GetInstance();

        private ServiceAPI serviceAPI = new ServiceAPI();

        public InputPinForm()
        {
            InitializeComponent();

            this.ActiveControl = pin_1;
        }

        private void pin_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            } else
            {
                this.ActiveControl = pin_2;
            }
        }

        private void pin_2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                this.ActiveControl = pin_3;
            }
        }

        private void pin_3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                this.ActiveControl = pin_4;
            }
        }

        private void pin_4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            } else
            {
                var pincode = pin_1.Text + pin_2.Text + pin_3.Text + e.KeyChar;
                this.logger.log("entered pincode is: " + pincode);
                this.logger.log("try to register new user by entered pincode");
                var success = this.registerNewUser(pincode);
                if (success)
                {
                    this.logger.log("success. create Main Form");
                    MainForm mf = FormManager.Current.CreateForm<MainForm>();
                    this.logger.log("close InputPinForm and show Main Form");
                    this.Hide();
                    mf.Closed += (s, args) => this.Close();
                    mf.Show();
                } else
                {
                    this.logger.log("failed. something goes wrong");
                }
            }
            
        }

        private bool registerNewUser(string pincode)
        {
            this.logger.log("registerNewUser, pincode=" + pincode);

            User user = null;
            try {
                this.logger.log("call get user by pincode");
                user = this.serviceAPI.getUserByPincode(pincode);
                this.logger.log("got user with email: " + user);
            } catch (RailroadException e) {
                this.logger.log("RailroadException: " + e.Message);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(Properties.Settings.Default.locale);
                MessageBox.Show(Properties.strings.wrong_pin_error_message, Properties.strings.wrong_pin_error_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            this.logger.log("parse user uuid to Guid type");
            Guid UserUuid = Guid.Parse(user.uuid);

            this.logger.log("generate device id = guid without -");
            string DeviceId = Guid.NewGuid().ToString().Replace("-", "");
            string VirtualIp = null;
            
            // TODO user api to get geo location of device
            string DeviceIp = null;

            // TODO user api to get geo location of device
            this.logger.log("get culture info to set location field");
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string Location = ci.DisplayName; 
            bool IsActive = false;

            try
            {
                this.logger.log("call create user device");
                this.serviceAPI.createUserDevice(UserUuid, DeviceId, VirtualIp, DeviceIp, Location, IsActive);
            } catch (Exception e)
            {
                this.logger.log("Exception when create user device: " + e.Message);
            }

            return true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pin_1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                this.pin_1.ResetText();
                this.ActiveControl = pin_1;
            }
        }

        private void pin_2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                this.pin_2.ResetText();
                this.ActiveControl = pin_1;
            }
        }

        private void pin_3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                this.pin_3.ResetText();
                this.ActiveControl = pin_2;
            }
        }

        private void pin_4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Back)
            {
                this.pin_4.ResetText();
                this.ActiveControl = pin_3;
            }
        }

    }
}
