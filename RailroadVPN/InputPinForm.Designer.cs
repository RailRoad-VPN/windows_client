using System;
using System.Windows.Forms;

namespace RailRoadVPN
{
    partial class InputPinForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputPinForm));
            this.pin_1 = new System.Windows.Forms.TextBox();
            this.pin_2 = new System.Windows.Forms.TextBox();
            this.pin_3 = new System.Windows.Forms.TextBox();
            this.pin_4 = new System.Windows.Forms.TextBox();
            this.enter_pin_label = new System.Windows.Forms.Label();
            this.closeBtn = new System.Windows.Forms.PictureBox();
            this.minimizeBtn = new System.Windows.Forms.PictureBox();
            this.menuBtn = new System.Windows.Forms.PictureBox();
            this.menuNavPanel = new System.Windows.Forms.Panel();
            this.ruLangBtn = new System.Windows.Forms.PictureBox();
            this.enLangBtn = new System.Windows.Forms.PictureBox();
            this.howGetPinTextLabel = new System.Windows.Forms.Label();
            this.getPinCodeLabelLink = new System.Windows.Forms.LinkLabel();
            this.menuTimer = new System.Windows.Forms.Timer(this.components);
            this.needHelpBtnLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.closeBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizeBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.menuBtn)).BeginInit();
            this.menuNavPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ruLangBtn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.enLangBtn)).BeginInit();
            this.SuspendLayout();
            // 
            // pin_1
            // 
            this.pin_1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pin_1.Font = new System.Drawing.Font("Courier New", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pin_1.Location = new System.Drawing.Point(36, 185);
            this.pin_1.MaximumSize = new System.Drawing.Size(50, 50);
            this.pin_1.MaxLength = 1;
            this.pin_1.MinimumSize = new System.Drawing.Size(50, 50);
            this.pin_1.Name = "pin_1";
            this.pin_1.Size = new System.Drawing.Size(50, 49);
            this.pin_1.TabIndex = 2;
            this.pin_1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pin_1.TextChanged += new System.EventHandler(this.pin_1_TextChanged);
            this.pin_1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pin_1_KeyDown);
            this.pin_1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pin_1_KeyPress);
            // 
            // pin_2
            // 
            this.pin_2.Font = new System.Drawing.Font("Courier New", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pin_2.Location = new System.Drawing.Point(124, 186);
            this.pin_2.MaximumSize = new System.Drawing.Size(50, 50);
            this.pin_2.MaxLength = 1;
            this.pin_2.MinimumSize = new System.Drawing.Size(50, 50);
            this.pin_2.Name = "pin_2";
            this.pin_2.Size = new System.Drawing.Size(50, 49);
            this.pin_2.TabIndex = 3;
            this.pin_2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pin_2.TextChanged += new System.EventHandler(this.pin_2_TextChanged);
            this.pin_2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pin_2_KeyDown);
            this.pin_2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pin_2_KeyPress);
            // 
            // pin_3
            // 
            this.pin_3.Font = new System.Drawing.Font("Courier New", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pin_3.Location = new System.Drawing.Point(209, 187);
            this.pin_3.MaximumSize = new System.Drawing.Size(50, 50);
            this.pin_3.MaxLength = 1;
            this.pin_3.MinimumSize = new System.Drawing.Size(50, 50);
            this.pin_3.Name = "pin_3";
            this.pin_3.Size = new System.Drawing.Size(50, 49);
            this.pin_3.TabIndex = 4;
            this.pin_3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pin_3.TextChanged += new System.EventHandler(this.pin_3_TextChanged);
            this.pin_3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pin_3_KeyDown);
            this.pin_3.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pin_3_KeyPress);
            // 
            // pin_4
            // 
            this.pin_4.Font = new System.Drawing.Font("Courier New", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pin_4.Location = new System.Drawing.Point(292, 188);
            this.pin_4.MaximumSize = new System.Drawing.Size(50, 50);
            this.pin_4.MaxLength = 1;
            this.pin_4.MinimumSize = new System.Drawing.Size(50, 50);
            this.pin_4.Name = "pin_4";
            this.pin_4.Size = new System.Drawing.Size(50, 49);
            this.pin_4.TabIndex = 5;
            this.pin_4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.pin_4.TextChanged += new System.EventHandler(this.pin_4_TextChanged);
            this.pin_4.KeyDown += new System.Windows.Forms.KeyEventHandler(this.pin_4_KeyDown);
            this.pin_4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pin_4_KeyPress);
            // 
            // enter_pin_label
            // 
            this.enter_pin_label.AutoSize = true;
            this.enter_pin_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(204)))), ((int)(((byte)(70)))));
            this.enter_pin_label.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enter_pin_label.Location = new System.Drawing.Point(104, 138);
            this.enter_pin_label.Name = "enter_pin_label";
            this.enter_pin_label.Size = new System.Drawing.Size(164, 21);
            this.enter_pin_label.TabIndex = 6;
            this.enter_pin_label.Text = "Enter PIN-CODE";
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
            this.menuNavPanel.Controls.Add(this.needHelpBtnLabel);
            this.menuNavPanel.Controls.Add(this.ruLangBtn);
            this.menuNavPanel.Controls.Add(this.enLangBtn);
            this.menuNavPanel.Location = new System.Drawing.Point(0, 0);
            this.menuNavPanel.Name = "menuNavPanel";
            this.menuNavPanel.Size = new System.Drawing.Size(150, 600);
            this.menuNavPanel.TabIndex = 12;
            // 
            // ruLangBtn
            // 
            this.ruLangBtn.BackgroundImage = global::RailRoadVPN.Properties.Resources.ru_lang;
            this.ruLangBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ruLangBtn.Location = new System.Drawing.Point(89, 562);
            this.ruLangBtn.Name = "ruLangBtn";
            this.ruLangBtn.Size = new System.Drawing.Size(25, 25);
            this.ruLangBtn.TabIndex = 13;
            this.ruLangBtn.TabStop = false;
            this.ruLangBtn.Click += new System.EventHandler(this.ruLangBtn_Click);
            // 
            // enLangBtn
            // 
            this.enLangBtn.BackgroundImage = global::RailRoadVPN.Properties.Resources.en_lang;
            this.enLangBtn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.enLangBtn.Location = new System.Drawing.Point(31, 562);
            this.enLangBtn.Name = "enLangBtn";
            this.enLangBtn.Size = new System.Drawing.Size(25, 25);
            this.enLangBtn.TabIndex = 13;
            this.enLangBtn.TabStop = false;
            this.enLangBtn.Click += new System.EventHandler(this.enLangBtn_Click);
            // 
            // howGetPinTextLabel
            // 
            this.howGetPinTextLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(204)))), ((int)(((byte)(70)))));
            this.howGetPinTextLabel.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.howGetPinTextLabel.Location = new System.Drawing.Point(24, 352);
            this.howGetPinTextLabel.Name = "howGetPinTextLabel";
            this.howGetPinTextLabel.Size = new System.Drawing.Size(341, 113);
            this.howGetPinTextLabel.TabIndex = 10;
            // 
            // getPinCodeLabelLink
            // 
            this.getPinCodeLabelLink.AutoSize = true;
            this.getPinCodeLabelLink.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.getPinCodeLabelLink.Location = new System.Drawing.Point(121, 266);
            this.getPinCodeLabelLink.Name = "getPinCodeLabelLink";
            this.getPinCodeLabelLink.Size = new System.Drawing.Size(128, 18);
            this.getPinCodeLabelLink.TabIndex = 11;
            this.getPinCodeLabelLink.TabStop = true;
            this.getPinCodeLabelLink.Text = "GET PIN-CODE";
            this.getPinCodeLabelLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.getPinCodeLabelLink_LinkClicked);
            // 
            // menuTimer
            // 
            this.menuTimer.Interval = 10;
            this.menuTimer.Tick += new System.EventHandler(this.menuTimer_Tick);
            // 
            // needHelpBtnLabel
            // 
            this.needHelpBtnLabel.AutoSize = true;
            this.needHelpBtnLabel.Font = new System.Drawing.Font("Courier New", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.needHelpBtnLabel.Location = new System.Drawing.Point(12, 524);
            this.needHelpBtnLabel.Name = "needHelpBtnLabel";
            this.needHelpBtnLabel.Size = new System.Drawing.Size(120, 22);
            this.needHelpBtnLabel.TabIndex = 13;
            this.needHelpBtnLabel.Text = "Need Help?";
            this.needHelpBtnLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // InputPinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(391, 600);
            this.ControlBox = false;
            this.Controls.Add(this.minimizeBtn);
            this.Controls.Add(this.closeBtn);
            this.Controls.Add(this.menuNavPanel);
            this.Controls.Add(this.enter_pin_label);
            this.Controls.Add(this.pin_4);
            this.Controls.Add(this.pin_3);
            this.Controls.Add(this.pin_2);
            this.Controls.Add(this.pin_1);
            this.Controls.Add(this.howGetPinTextLabel);
            this.Controls.Add(this.getPinCodeLabelLink);
            this.Controls.Add(this.menuBtn);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "InputPinForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "RRoadVPN - Enter pin";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.InputPinForm_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.closeBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimizeBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.menuBtn)).EndInit();
            this.menuNavPanel.ResumeLayout(false);
            this.menuNavPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ruLangBtn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.enLangBtn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox pin_1;
        private System.Windows.Forms.TextBox pin_2;
        private System.Windows.Forms.TextBox pin_3;
        private System.Windows.Forms.TextBox pin_4;
        private System.Windows.Forms.Label enter_pin_label;
        private System.Windows.Forms.PictureBox closeBtn;
        private System.Windows.Forms.PictureBox minimizeBtn;
        private System.Windows.Forms.PictureBox menuBtn;
        private System.Windows.Forms.Panel menuNavPanel;
        private System.Windows.Forms.Label howGetPinTextLabel;
        private System.Windows.Forms.LinkLabel getPinCodeLabelLink;
        private System.Windows.Forms.Timer menuTimer;
        private PictureBox enLangBtn;
        private PictureBox ruLangBtn;
        private Label needHelpBtnLabel;
    }
}

