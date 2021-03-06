using Newtonsoft.Json;
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

        private ServiceAPI serviceAPI;

        private PropertiesHelper propertiesHelper = PropertiesHelper.GetInstance();

        public NeedHelpForm()
        {
            InitializeComponent();

            this.serviceAPI = new ServiceAPI();

            this.emailTextBoxLabel.Text = Properties.strings.help_form_email_label;
            this.descriptionTextBoxLabel.Text = Properties.strings.help_form_description_label;
            this.sendBtn.Text = Properties.strings.help_form_send_btn;
            this.cancelBtn.Text = Properties.strings.help_form_cancel_btn;

            if (Properties.Settings.Default.user_email != "") {
                this.emailTextBox.Text = Properties.Settings.Default.user_email;
            }
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
            if (Properties.Settings.Default.vpn_status == "connected" || Utils.isTapDriverBusy())
            {
                MessageBox.Show("Disconnect VPN first", "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            this.logger.log("disable controls");
            this.sendBtn.Enabled = false;
            this.cancelBtn.Enabled = false;

            this.logger.log("help form send btn click");
            char[] charsToTrim = { '*', ' ', '\'' };

            this.logger.log("get email and trim it");
            string email = this.emailTextBox.Text.Trim(charsToTrim);
            this.logger.log("email: " + email);

            this.logger.log("get description and trim it");
            string description = this.problemDescriptionTextBox.Text.Trim(charsToTrim);
            this.logger.log("description: " + description);

            if (email == "")
            {
                this.logger.log("email is empty");
                MessageBox.Show(Properties.strings.help_form_empty_email_message, Properties.strings.help_form_bad_data_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.logger.log("enable controls");
                this.sendBtn.Enabled = true;
                this.cancelBtn.Enabled = true;
                return;
            }

            if (description == "")
            {
                this.logger.log("description is empty");
                MessageBox.Show(Properties.strings.help_form_empty_description_message, Properties.strings.help_form_bad_data_header, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.logger.log("enable controls");
                this.sendBtn.Enabled = true;
                this.cancelBtn.Enabled = true;
                return;
            }

            byte[] zipBytesArr = null;
            this.logger.log("get log files");
            try
            {
                List<FileInfo> logFiles = Utils.getLogFiles();

                this.logger.log("create zip with all log files");
                using (MemoryStream ms = new MemoryStream())
                {
                    using (ZipStorer zip = ZipStorer.Create(ms, "RailRoadVPN"))
                    {
                        zip.EncodeUTF8 = true;
                        foreach (FileInfo logFile in logFiles)
                        {
                            zip.AddFile(ZipStorer.Compression.Deflate, logFile.FullName, logFile.Name, "");
                        }
                    }
                    this.logger.log("memory stream to array");
                    zipBytesArr = ms.ToArray();
                }
            } catch (Exception ex)
            {
                this.logger.log("Exception when get logs to create ticket");
            }

            this.logger.log("get extra system information");
            ExtraSystemInformation esi = Utils.getSystemInformation();

            this.logger.log("convert extra system information to JSON");
            var esiJson = JsonConvert.SerializeObject(esi);

            int ticketNumber;

            this.logger.log("get user uuid");
            string userUuidStr = Properties.Settings.Default.user_uuid;

            try
            {
                ticketNumber = this.serviceAPI.createTicket(UserUuid: userUuidStr, ContactEmail: email, Description: description, ExtraInfo: esiJson, ZipFileBytesArr: zipBytesArr);
            }
            catch (Exception ex)
            {
                this.logger.log("Exception when create ticket: " + ex.Message);
                MessageBox.Show(Properties.strings.unknown_system_error_message, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.logger.log("enable controls");
                this.sendBtn.Enabled = true;
                this.cancelBtn.Enabled = true;
                return;
            }
            this.logger.log("got ticket number: " + ticketNumber);
            MessageBox.Show(Properties.strings.help_form_thank_message + " " + ticketNumber, "", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.sendBtn.Enabled = true;
            this.cancelBtn.Enabled = true;

            this.logger.log("close form");
            this.Close();
        }
    }
}
