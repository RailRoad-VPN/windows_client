using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
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

        private ServiceAPI serviceAPI = new ServiceAPI();

        public InputPinForm()
        {
            InitializeComponent();

            this.ActiveControl = pin_1;
        }

        private void GetVPNConfig_Cick(object sender, EventArgs e)
        {
            var user_uuid = Properties.Settings.Default["user_uuid"];
            Console.WriteLine(user_uuid);
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
                Console.WriteLine("entered pincode is: " + pincode);
                var success = this.registerNewUser(pincode);
                if (success)
                {
                    MainForm mf = new MainForm();
                    mf.Show();
                    this.Close();
                } else
                {

                }
            }
            
        }

        private bool registerNewUser(string pincode)
        {
            User user = null;
            try {
                user = this.serviceAPI.getUserByPincode(pincode);
            } catch (RailroadException e) {
                MessageBox.Show(e.Message, "Pincode problem", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Guid UserUuid = Guid.Parse(user.uuid);

            string DeviceId = Guid.NewGuid().ToString().Replace("-", "");
            string VirtualIp = null;
            
            // TODO user api to get geo location of device
            string DeviceIp = null;

            // TODO user api to get geo location of device
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string Location = ci.DisplayName; 
            bool IsActive = false;
            this.serviceAPI.createUserDevice(UserUuid, DeviceId, VirtualIp, DeviceIp, Location, IsActive);

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
