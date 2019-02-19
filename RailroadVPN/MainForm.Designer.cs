﻿namespace RailRoadVPN
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
            this.menuLogoutBtn = new System.Windows.Forms.Button();
            this.closeBtn = new System.Windows.Forms.PictureBox();
            this.minimizeBtn = new System.Windows.Forms.PictureBox();
            this.menuBtn = new System.Windows.Forms.PictureBox();
            this.menuNavPanel = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.menuProfileBtn = new System.Windows.Forms.Button();
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
            ((System.ComponentModel.ISupportInitialize)(this.semaphorePic)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpArrowImg)).BeginInit();
            this.SuspendLayout();
            // 
            // menuLogoutBtn
            // 
            this.menuLogoutBtn.Location = new System.Drawing.Point(12, 565);
            this.menuLogoutBtn.Name = "menuLogoutBtn";
            this.menuLogoutBtn.Size = new System.Drawing.Size(126, 23);
            this.menuLogoutBtn.TabIndex = 0;
            this.menuLogoutBtn.Text = "LogOut";
            this.menuLogoutBtn.UseVisualStyleBackColor = true;
            this.menuLogoutBtn.Click += new System.EventHandler(this.menuLogoutBtn_Click);
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
            this.menuNavPanel.BackColor = System.Drawing.Color.White;
            this.menuNavPanel.Controls.Add(this.button1);
            this.menuNavPanel.Controls.Add(this.menuProfileBtn);
            this.menuNavPanel.Controls.Add(this.menuLogoutBtn);
            this.menuNavPanel.Location = new System.Drawing.Point(0, 0);
            this.menuNavPanel.Name = "menuNavPanel";
            this.menuNavPanel.Size = new System.Drawing.Size(0, 600);
            this.menuNavPanel.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 536);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(126, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Need help?";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // menuProfileBtn
            // 
            this.menuProfileBtn.Location = new System.Drawing.Point(12, 19);
            this.menuProfileBtn.Name = "menuProfileBtn";
            this.menuProfileBtn.Size = new System.Drawing.Size(126, 23);
            this.menuProfileBtn.TabIndex = 1;
            this.menuProfileBtn.Text = "Profile";
            this.menuProfileBtn.UseVisualStyleBackColor = true;
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
            this.semaphorePic.Location = new System.Drawing.Point(109, 163);
            this.semaphorePic.Name = "semaphorePic";
            this.semaphorePic.Size = new System.Drawing.Size(179, 117);
            this.semaphorePic.TabIndex = 13;
            this.semaphorePic.TabStop = false;
            this.semaphorePic.Click += new System.EventHandler(this.semaphorePic_Click);
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.Location = new System.Drawing.Point(119, 130);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(158, 18);
            this.statusLabel.TabIndex = 14;
            this.statusLabel.Text = "VPN Status here";
            // 
            // semaphoreTimer
            // 
            this.semaphoreTimer.Interval = 800;
            this.semaphoreTimer.Tick += new System.EventHandler(this.semaphoreTimer_Tick);
            // 
            // statusTextTimer
            // 
            this.statusTextTimer.Interval = 500;
            this.statusTextTimer.Tick += new System.EventHandler(this.statusTextTimer_Tick);
            // 
            // helpArrowImg
            // 
            this.helpArrowImg.BackColor = System.Drawing.Color.Transparent;
            this.helpArrowImg.BackgroundImage = global::RailRoadVPN.Properties.Resources.arrow_ye;
            this.helpArrowImg.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.helpArrowImg.Location = new System.Drawing.Point(53, 271);
            this.helpArrowImg.Name = "helpArrowImg";
            this.helpArrowImg.Size = new System.Drawing.Size(108, 150);
            this.helpArrowImg.TabIndex = 15;
            this.helpArrowImg.TabStop = false;
            // 
            // helpText1Label
            // 
            this.helpText1Label.AutoSize = true;
            this.helpText1Label.Font = new System.Drawing.Font("Courier New", 9F);
            this.helpText1Label.Location = new System.Drawing.Point(78, 424);
            this.helpText1Label.Name = "helpText1Label";
            this.helpText1Label.Size = new System.Drawing.Size(42, 15);
            this.helpText1Label.TabIndex = 16;
            this.helpText1Label.Text = "Click";
            // 
            // helpTextRedLabel
            // 
            this.helpTextRedLabel.AutoSize = true;
            this.helpTextRedLabel.BackColor = System.Drawing.Color.Red;
            this.helpTextRedLabel.Font = new System.Drawing.Font("Courier New", 9F);
            this.helpTextRedLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.helpTextRedLabel.Location = new System.Drawing.Point(126, 424);
            this.helpTextRedLabel.Name = "helpTextRedLabel";
            this.helpTextRedLabel.Size = new System.Drawing.Size(28, 15);
            this.helpTextRedLabel.TabIndex = 17;
            this.helpTextRedLabel.Text = "red";
            // 
            // helpText2Label
            // 
            this.helpText2Label.AutoSize = true;
            this.helpText2Label.Font = new System.Drawing.Font("Courier New", 9F);
            this.helpText2Label.Location = new System.Drawing.Point(159, 424);
            this.helpText2Label.Name = "helpText2Label";
            this.helpText2Label.Size = new System.Drawing.Size(175, 15);
            this.helpText2Label.TabIndex = 18;
            this.helpText2Label.Text = "semaphore to connect VPN";
            // 
            // helpTextGreenLabel
            // 
            this.helpTextGreenLabel.AutoSize = true;
            this.helpTextGreenLabel.BackColor = System.Drawing.Color.DarkGreen;
            this.helpTextGreenLabel.Font = new System.Drawing.Font("Courier New", 9F);
            this.helpTextGreenLabel.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.helpTextGreenLabel.Location = new System.Drawing.Point(126, 424);
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
            this.Controls.Add(this.helpTextGreenLabel);
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
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RailroadVPN - Connect to VPN";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MainForm_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.closeBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizeBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.menuBtn)).EndInit();
            this.menuNavPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.semaphorePic)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.helpArrowImg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button menuLogoutBtn;
        private System.Windows.Forms.PictureBox closeBtn;
        private System.Windows.Forms.PictureBox minimizeBtn;
        private System.Windows.Forms.PictureBox menuBtn;
        private System.Windows.Forms.Panel menuNavPanel;
        private System.Windows.Forms.Timer menuTimer;
        private System.Windows.Forms.Button menuProfileBtn;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox semaphorePic;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Timer semaphoreTimer;
        private System.Windows.Forms.Timer statusTextTimer;
        private System.Windows.Forms.PictureBox helpArrowImg;
        private System.Windows.Forms.Label helpText1Label;
        private System.Windows.Forms.Label helpTextRedLabel;
        private System.Windows.Forms.Label helpText2Label;
        private System.Windows.Forms.Label helpTextGreenLabel;
    }
}