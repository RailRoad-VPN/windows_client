using System.Drawing;
using System.Windows.Forms;

namespace RailRoadVPN
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.closeBtn = new System.Windows.Forms.PictureBox();
            this.minimizeBtn = new System.Windows.Forms.PictureBox();
            this.menuBtn = new System.Windows.Forms.PictureBox();
            this.menuNavPanel = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuLogoImg = new System.Windows.Forms.PictureBox();
            this.menuLogoutLabel = new System.Windows.Forms.Label();
            this.menuNeedHelpLabel = new System.Windows.Forms.Label();
            this.menuMyProfileLabel = new System.Windows.Forms.Label();
            this.ruLangBtn = new System.Windows.Forms.PictureBox();
            this.enLangBtn = new System.Windows.Forms.PictureBox();
            this.menuTimer = new System.Windows.Forms.Timer(this.components);
            this.semaphorePic = new System.Windows.Forms.PictureBox();
            this.statusLabel = new System.Windows.Forms.Label();
            this.semaphoreTimer = new System.Windows.Forms.Timer(this.components);
            this.statusTextTimer = new System.Windows.Forms.Timer(this.components);
            this.helpArrowImg = new System.Windows.Forms.PictureBox();
            this.helpText1Label = new System.Windows.Forms.Label();
            this.helpTextRedLabel = new System.Windows.Forms.Label();
            this.helpText2Label = new System.Windows.Forms.Label();
            this.helpTextGreenLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.closeBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizeBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.menuBtn)).BeginInit();
            this.menuNavPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.menuLogoImg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ruLangBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.enLangBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.semaphorePic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpArrowImg)).BeginInit();
            this.SuspendLayout();
            // 
            // closeBtn
            // 
            this.closeBtn.BackColor = System.Drawing.Color.Transparent;
            this.closeBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("closeBtn.BackgroundImage")));
            this.closeBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.closeBtn.Location = new System.Drawing.Point(367, 4);
            this.closeBtn.Name = "closeBtn";
            this.closeBtn.Size = new System.Drawing.Size(20, 20);
            this.closeBtn.TabIndex = 7;
            this.closeBtn.TabStop = false;
            this.closeBtn.Click += new System.EventHandler(this.closeBtn_Click);
            // 
            // minimizeBtn
            // 
            this.minimizeBtn.BackColor = System.Drawing.Color.Transparent;
            this.minimizeBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("minimizeBtn.BackgroundImage")));
            this.minimizeBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.minimizeBtn.Location = new System.Drawing.Point(325, 2);
            this.minimizeBtn.Margin = new System.Windows.Forms.Padding(0, 100, 0, 20);
            this.minimizeBtn.Name = "minimizeBtn";
            this.minimizeBtn.Size = new System.Drawing.Size(40, 40);
            this.minimizeBtn.TabIndex = 8;
            this.minimizeBtn.TabStop = false;
            this.minimizeBtn.Click += new System.EventHandler(this.minimizeBtn_Click);
            // 
            // menuBtn
            // 
            this.menuBtn.BackColor = System.Drawing.Color.Transparent;
            this.menuBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("menuBtn.BackgroundImage")));
            this.menuBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.menuBtn.Location = new System.Drawing.Point(0, 1);
            this.menuBtn.Margin = new System.Windows.Forms.Padding(0, 100, 0, 20);
            this.menuBtn.Name = "menuBtn";
            this.menuBtn.Size = new System.Drawing.Size(40, 40);
            this.menuBtn.TabIndex = 9;
            this.menuBtn.TabStop = false;
            this.menuBtn.Click += new System.EventHandler(this.menuBtn_Click);
            // 
            // menuNavPanel
            // 
            this.menuNavPanel.BackColor = System.Drawing.Color.Black;
            this.menuNavPanel.Controls.Add(this.panel4);
            this.menuNavPanel.Controls.Add(this.panel3);
            this.menuNavPanel.Controls.Add(this.panel2);
            this.menuNavPanel.Controls.Add(this.panel1);
            this.menuNavPanel.Controls.Add(this.menuLogoImg);
            this.menuNavPanel.Controls.Add(this.menuLogoutLabel);
            this.menuNavPanel.Controls.Add(this.menuNeedHelpLabel);
            this.menuNavPanel.Controls.Add(this.menuMyProfileLabel);
            this.menuNavPanel.Controls.Add(this.ruLangBtn);
            this.menuNavPanel.Controls.Add(this.enLangBtn);
            this.menuNavPanel.Location = new System.Drawing.Point(0, 0);
            this.menuNavPanel.Name = "menuNavPanel";
            this.menuNavPanel.Size = new System.Drawing.Size(0, 600);
            this.menuNavPanel.TabIndex = 12;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.White;
            this.panel4.Location = new System.Drawing.Point(8, 59);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(133, 1);
            this.panel4.TabIndex = 21;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Location = new System.Drawing.Point(0, 521);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(149, 1);
            this.panel3.TabIndex = 21;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Location = new System.Drawing.Point(8, 129);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(133, 1);
            this.panel2.TabIndex = 21;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Location = new System.Drawing.Point(8, 94);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(133, 1);
            this.panel1.TabIndex = 20;
            // 
            // menuLogoImg
            // 
            this.menuLogoImg.BackColor = System.Drawing.Color.Black;
            this.menuLogoImg.BackgroundImage = global::RailRoadVPN.Properties.Resources.logo;
            this.menuLogoImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.menuLogoImg.Location = new System.Drawing.Point(28, 12);
            this.menuLogoImg.Name = "menuLogoImg";
            this.menuLogoImg.Size = new System.Drawing.Size(101, 24);
            this.menuLogoImg.TabIndex = 20;
            this.menuLogoImg.TabStop = false;
            // 
            // menuLogoutLabel
            // 
            this.menuLogoutLabel.AutoSize = true;
            this.menuLogoutLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.menuLogoutLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.menuLogoutLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(204)))), ((int)(((byte)(70)))));
            this.menuLogoutLabel.Location = new System.Drawing.Point(25, 531);
            this.menuLogoutLabel.Name = "menuLogoutLabel";
            this.menuLogoutLabel.Size = new System.Drawing.Size(51, 13);
            this.menuLogoutLabel.TabIndex = 20;
            this.menuLogoutLabel.Text = "LOGOUT";
            this.menuLogoutLabel.Click += new System.EventHandler(this.menuLogoutLabel_Click);
            this.menuLogoutLabel.MouseLeave += new System.EventHandler(this.menuLogoutLabel_MouseLeave);
            this.menuLogoutLabel.MouseHover += new System.EventHandler(this.menuLogoutLabel_MouseHover);
            // 
            // menuNeedHelpLabel
            // 
            this.menuNeedHelpLabel.AutoSize = true;
            this.menuNeedHelpLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.menuNeedHelpLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.menuNeedHelpLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(204)))), ((int)(((byte)(70)))));
            this.menuNeedHelpLabel.Location = new System.Drawing.Point(12, 106);
            this.menuNeedHelpLabel.Name = "menuNeedHelpLabel";
            this.menuNeedHelpLabel.Size = new System.Drawing.Size(71, 13);
            this.menuNeedHelpLabel.TabIndex = 20;
            this.menuNeedHelpLabel.Text = "NEED HELP?";
            this.menuNeedHelpLabel.Click += new System.EventHandler(this.menuNeedHelpLabel_Click);
            this.menuNeedHelpLabel.MouseLeave += new System.EventHandler(this.menuNeedHelpLabel_MouseLeave);
            this.menuNeedHelpLabel.MouseHover += new System.EventHandler(this.menuNeedHelpLabel_MouseHover);
            // 
            // menuMyProfileLabel
            // 
            this.menuMyProfileLabel.AutoSize = true;
            this.menuMyProfileLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.menuMyProfileLabel.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuMyProfileLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(204)))), ((int)(((byte)(70)))));
            this.menuMyProfileLabel.Location = new System.Drawing.Point(12, 70);
            this.menuMyProfileLabel.Name = "menuMyProfileLabel";
            this.menuMyProfileLabel.Size = new System.Drawing.Size(71, 13);
            this.menuMyProfileLabel.TabIndex = 16;
            this.menuMyProfileLabel.Text = "MY PROFILE";
            this.menuMyProfileLabel.Click += new System.EventHandler(this.menuMyProfileLabel_Click);
            this.menuMyProfileLabel.MouseLeave += new System.EventHandler(this.menuMyProfileLabel_MouseLeave);
            this.menuMyProfileLabel.MouseHover += new System.EventHandler(this.menuMyProfileLabel_MouseHover);
            // 
            // ruLangBtn
            // 
            this.ruLangBtn.BackgroundImage = global::RailRoadVPN.Properties.Resources.ru_lang;
            this.ruLangBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ruLangBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ruLangBtn.Location = new System.Drawing.Point(87, 563);
            this.ruLangBtn.Name = "ruLangBtn";
            this.ruLangBtn.Size = new System.Drawing.Size(25, 25);
            this.ruLangBtn.TabIndex = 14;
            this.ruLangBtn.TabStop = false;
            this.ruLangBtn.Click += new System.EventHandler(this.ruLangBtn_Click);
            // 
            // enLangBtn
            // 
            this.enLangBtn.BackgroundImage = global::RailRoadVPN.Properties.Resources.en_lang;
            this.enLangBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.enLangBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.enLangBtn.Location = new System.Drawing.Point(28, 563);
            this.enLangBtn.Name = "enLangBtn";
            this.enLangBtn.Size = new System.Drawing.Size(25, 25);
            this.enLangBtn.TabIndex = 15;
            this.enLangBtn.TabStop = false;
            this.enLangBtn.Click += new System.EventHandler(this.enLangBtn_Click);
            // 
            // menuTimer
            // 
            this.menuTimer.Interval = 10;
            this.menuTimer.Tick += new System.EventHandler(this.menuTimer_Tick);
            // 
            // semaphorePic
            // 
            this.semaphorePic.BackColor = System.Drawing.Color.Transparent;
            this.semaphorePic.BackgroundImage = global::RailRoadVPN.Properties.Resources.red;
            this.semaphorePic.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.semaphorePic.Location = new System.Drawing.Point(110, 163);
            this.semaphorePic.Name = "semaphorePic";
            this.semaphorePic.Size = new System.Drawing.Size(179, 117);
            this.semaphorePic.TabIndex = 13;
            this.semaphorePic.TabStop = false;
            this.semaphorePic.Click += new System.EventHandler(this.semaphorePic_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoEllipsis = true;
            this.statusLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(204)))), ((int)(((byte)(70)))));
            this.statusLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(69, 129);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(258, 18);
            this.statusLabel.TabIndex = 14;
            this.statusLabel.Text = "VPN STATUS";
            this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // semaphoreTimer
            // 
            this.semaphoreTimer.Interval = 800;
            this.semaphoreTimer.Tick += new System.EventHandler(this.semaphoreTimer_Tick);
            // 
            // statusTextTimer
            // 
            this.statusTextTimer.Interval = 1000;
            this.statusTextTimer.Tick += new System.EventHandler(this.statusTextTimer_Tick);
            // 
            // helpArrowImg
            // 
            this.helpArrowImg.BackColor = System.Drawing.Color.Transparent;
            this.helpArrowImg.BackgroundImage = global::RailRoadVPN.Properties.Resources.arrow_ye;
            this.helpArrowImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.helpArrowImg.Location = new System.Drawing.Point(45, 271);
            this.helpArrowImg.Name = "helpArrowImg";
            this.helpArrowImg.Size = new System.Drawing.Size(108, 150);
            this.helpArrowImg.TabIndex = 15;
            this.helpArrowImg.TabStop = false;
            // 
            // helpText1Label
            // 
            this.helpText1Label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpText1Label.AutoSize = true;
            this.helpText1Label.Font = new System.Drawing.Font("Courier New", 9F);
            this.helpText1Label.Location = new System.Drawing.Point(25, 424);
            this.helpText1Label.Name = "helpText1Label";
            this.helpText1Label.Size = new System.Drawing.Size(42, 15);
            this.helpText1Label.TabIndex = 16;
            this.helpText1Label.Text = "Click";
            // 
            // helpTextRedLabel
            // 
            this.helpTextRedLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpTextRedLabel.AutoSize = true;
            this.helpTextRedLabel.BackColor = System.Drawing.Color.Red;
            this.helpTextRedLabel.Font = new System.Drawing.Font("Courier New", 9F);
            this.helpTextRedLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.helpTextRedLabel.Location = new System.Drawing.Point(73, 424);
            this.helpTextRedLabel.Name = "helpTextRedLabel";
            this.helpTextRedLabel.Size = new System.Drawing.Size(28, 15);
            this.helpTextRedLabel.TabIndex = 17;
            this.helpTextRedLabel.Text = "red";
            // 
            // helpText2Label
            // 
            this.helpText2Label.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpText2Label.AutoSize = true;
            this.helpText2Label.Font = new System.Drawing.Font("Courier New", 9F);
            this.helpText2Label.Location = new System.Drawing.Point(106, 424);
            this.helpText2Label.Name = "helpText2Label";
            this.helpText2Label.Size = new System.Drawing.Size(175, 15);
            this.helpText2Label.TabIndex = 18;
            this.helpText2Label.Text = "semaphore to connect VPN";
            // 
            // helpTextGreenLabel
            // 
            this.helpTextGreenLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.helpTextGreenLabel.AutoSize = true;
            this.helpTextGreenLabel.BackColor = System.Drawing.Color.DarkGreen;
            this.helpTextGreenLabel.Font = new System.Drawing.Font("Courier New", 9F);
            this.helpTextGreenLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.helpTextGreenLabel.Location = new System.Drawing.Point(73, 424);
            this.helpTextGreenLabel.Name = "helpTextGreenLabel";
            this.helpTextGreenLabel.Size = new System.Drawing.Size(42, 15);
            this.helpTextGreenLabel.TabIndex = 19;
            this.helpTextGreenLabel.Text = "green";
            this.helpTextGreenLabel.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::RailRoadVPN.Properties.Resources.input_pin_bg_cartoon;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(391, 600);
            this.ControlBox = false;
            this.Controls.Add(this.menuNavPanel);
            this.Controls.Add(this.menuBtn);
            this.Controls.Add(this.minimizeBtn);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.semaphorePic);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.helpArrowImg);
            this.Controls.Add(this.helpText2Label);
            this.Controls.Add(this.helpTextRedLabel);
            this.Controls.Add(this.helpText1Label);
            this.Controls.Add(this.helpTextGreenLabel);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "RRoadVPN";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.closeBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizeBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.menuBtn)).EndInit();
            this.menuNavPanel.ResumeLayout(false);
            this.menuNavPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.menuLogoImg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ruLangBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.enLangBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.semaphorePic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpArrowImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.PictureBox closeBtn;
        private System.Windows.Forms.PictureBox minimizeBtn;
        private System.Windows.Forms.PictureBox menuBtn;
        private System.Windows.Forms.Panel menuNavPanel;
        private System.Windows.Forms.Timer menuTimer;
        private System.Windows.Forms.PictureBox semaphorePic;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Timer semaphoreTimer;
        private System.Windows.Forms.Timer statusTextTimer;
        private System.Windows.Forms.PictureBox helpArrowImg;
        private System.Windows.Forms.Label helpText1Label;
        private System.Windows.Forms.Label helpTextRedLabel;
        private System.Windows.Forms.Label helpText2Label;
        private System.Windows.Forms.Label helpTextGreenLabel;
        private PictureBox ruLangBtn;
        private PictureBox enLangBtn;
        private Label menuMyProfileLabel;
        private Label menuNeedHelpLabel;
        private Label menuLogoutLabel;
        private PictureBox menuLogoImg;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
    }
}