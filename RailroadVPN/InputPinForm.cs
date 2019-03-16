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

        private int _menuBtnStartPos;

        public InputPinForm()
        {
            InitializeComponent();

            this.ActiveControl = pin_1;

            this._menuBtnStartPos = this.menuBtn.Left;

            string labelText = "";
            string[] textArr = Properties.strings.how_get_pin_text_label.Split(new[] { "\\r\\n", "\\r", "'\n" }, StringSplitOptions.None);
            foreach (string txt in textArr)
            {
                labelText += txt + Environment.NewLine;
            }
            this.howGetPinTextLabel.Text = labelText;
            this.menuNeedHelpLabel.Text = Properties.strings.menu_needhelp_text_label;
            this.enter_pin_label.Text = Properties.strings.enter_pin_label;
            this.getPinCodeLabelLink.Text = Properties.strings.get_pincode_text_link;
        }

        private void pin_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.pin_1.ResetText();
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            } else
            {
                this.ActiveControl = pin_2;
                this.checkPinEntered();
            }
        }

        private void pin_2_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.pin_2.ResetText();
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                this.ActiveControl = pin_3;
                this.checkPinEntered();
            }
        }

        private void pin_3_KeyPress(object sender, KeyPressEventArgs e)
        {
            this.pin_3.ResetText();
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            else
            {
                this.ActiveControl = pin_4;
                this.checkPinEntered();
            }
        }

        private void pin_4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            } else
            {
                this.checkPinEntered();
            }
            
        }

        private void checkPinEntered()
        {
            var pincode = pin_1.Text + pin_2.Text + pin_3.Text + pin_4.Text;
            if (pincode.Length < 4)
            {
                return;
            }
            this.logger.log("entered pincode is: " + pincode);
            this.logger.log("try to register new user by entered pincode");
            var success = this.registerNewUser(pincode);
            if (success)
            {
                this.logger.log("success. create Main Form");
                MainForm mf = FormManager.Current.CreateForm<MainForm>();
                mf.Location = this.Location;
                this.logger.log("close InputPinForm and show Main Form");
                this.Hide();
                mf.Closed += (s, args) => this.Close();
                mf.Show();
            }
            else
            {
                this.logger.log("failed. something goes wrong");
            }
        }

        private bool registerNewUser(string pincode)
        {
            this.logger.log("registerNewUser, pincode=" + pincode);

            User user = null;
            try {
                this.logger.log("call get user by pincode");
                user = this.serviceAPI.getUserByPincode(pincode);
                this.logger.log("got user with email: " + user.email);
            } catch (RailroadException e) {
                this.logger.log("RailroadException when register new user: " + e.Message);
                MessageBox.Show(Properties.strings.wrong_pin_error_message, Properties.strings.wrong_pin_error_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            } catch (Exception e)
            {
                this.logger.log("Exception when register new user: " + e.Message);
                MessageBox.Show(Properties.strings.unknown_system_error_message, Properties.strings.unknown_system_error_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            this.logger.log("parse user uuid to Guid type");
            Guid UserUuid = Guid.Parse(user.uuid);

            this.logger.log("generate device id = guid without -");
            string DeviceId = Guid.NewGuid().ToString().Replace("-", "");

            // TODO user api to get geo location of device
            this.logger.log("get culture info to set location field");
            CultureInfo ci = CultureInfo.InstalledUICulture;
            string Location = ci.DisplayName; 
            bool IsActive = true;

            try
            {
                this.logger.log("call create user device");
                this.serviceAPI.createUserDevice(UserUuid: UserUuid, DeviceId: DeviceId, Location: Location, IsActive: IsActive);
            } catch (Exception e)
            {
                this.logger.log("Exception when create user device: " + e.Message);
                MessageBox.Show(Properties.strings.unknown_system_error_message, Properties.strings.unknown_system_error_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            Properties.Settings.Default.user_uuid = user.uuid;
            Properties.Settings.Default.user_email = user.email;

            return true;
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
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
                // hide menu
                menuNavPanel.Visible = false;

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

        private void getPinCodeLabelLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://rroadvpn.net/order");
        }

        private void pin_1_TextChanged(object sender, EventArgs e)
        {
            this.checkPinEntered();
        }

        private void pin_2_TextChanged(object sender, EventArgs e)
        {
            this.checkPinEntered();
        }

        private void pin_3_TextChanged(object sender, EventArgs e)
        {
            this.checkPinEntered();
        }

        private void pin_4_TextChanged(object sender, EventArgs e)
        {
            this.checkPinEntered();
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

        private void menuNeedHelpLabel_Click(object sender, EventArgs e)
        {
            NeedHelpForm nhf = new NeedHelpForm();

            int parent_left = this.Left;
            int parent_top = this.Top;
            parent_left = parent_left + ((this.Width - nhf.Width) / 2);
            parent_top = parent_top + ((this.Height - nhf.Height) / 2);

            nhf.Location = new Point(parent_left, parent_top);
            nhf.ShowDialog();
        }
    }
}
