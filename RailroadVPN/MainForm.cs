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
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
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
    }
}
