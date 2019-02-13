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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputPinForm));
            this.pin_1 = new System.Windows.Forms.TextBox();
            this.pin_2 = new System.Windows.Forms.TextBox();
            this.pin_3 = new System.Windows.Forms.TextBox();
            this.pin_4 = new System.Windows.Forms.TextBox();
            this.enter_pin_label = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.menu_btn = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.menu_btn)).BeginInit();
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
            this.pin_4.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.pin_4_KeyPress);
            // 
            // enter_pin_label
            // 
            this.enter_pin_label.AutoSize = true;
            this.enter_pin_label.BackColor = System.Drawing.Color.WhiteSmoke;
            this.enter_pin_label.Location = new System.Drawing.Point(147, 139);
            this.enter_pin_label.Name = "enter_pin_label";
            this.enter_pin_label.Size = new System.Drawing.Size(98, 14);
            this.enter_pin_label.TabIndex = 6;
            this.enter_pin_label.Text = "Enter pincode";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImage = global::RailRoadVPN.Properties.Resources.close;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(367, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(20, 20);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImage = global::RailRoadVPN.Properties.Resources.minimize;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(325, 2);
            this.pictureBox2.Margin = new System.Windows.Forms.Padding(0, 100, 0, 20);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(40, 40);
            this.pictureBox2.TabIndex = 8;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Click += new System.EventHandler(this.pictureBox2_Click);
            // 
            // menu_btn
            // 
            this.menu_btn.BackColor = System.Drawing.Color.Transparent;
            this.menu_btn.BackgroundImage = global::RailRoadVPN.Properties.Resources.menu;
            this.menu_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.menu_btn.Location = new System.Drawing.Point(0, 1);
            this.menu_btn.Margin = new System.Windows.Forms.Padding(0, 100, 0, 20);
            this.menu_btn.Name = "menu_btn";
            this.menu_btn.Size = new System.Drawing.Size(40, 40);
            this.menu_btn.TabIndex = 9;
            this.menu_btn.TabStop = false;
            // 
            // InputPinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(391, 600);
            this.ControlBox = false;
            this.Controls.Add(this.menu_btn);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.enter_pin_label);
            this.Controls.Add(this.pin_4);
            this.Controls.Add(this.pin_3);
            this.Controls.Add(this.pin_2);
            this.Controls.Add(this.pin_1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "InputPinForm";
            this.Text = "RRoadVPN - Enter pin";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.menu_btn)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox pin_1;
        private System.Windows.Forms.TextBox pin_2;
        private System.Windows.Forms.TextBox pin_3;
        private System.Windows.Forms.TextBox pin_4;
        private System.Windows.Forms.Label enter_pin_label;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox menu_btn;
    }
}

