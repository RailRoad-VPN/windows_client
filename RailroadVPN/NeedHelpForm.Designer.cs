namespace RailRoadVPN
{
    partial class NeedHelpForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NeedHelpForm));
            this.emailTextBoxLabel = new System.Windows.Forms.Label();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.descriptionTextBoxLabel = new System.Windows.Forms.Label();
            this.problemDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // emailTextBoxLabel
            // 
            this.emailTextBoxLabel.AutoSize = true;
            this.emailTextBoxLabel.Location = new System.Drawing.Point(39, 24);
            this.emailTextBoxLabel.Name = "emailTextBoxLabel";
            this.emailTextBoxLabel.Size = new System.Drawing.Size(49, 14);
            this.emailTextBoxLabel.TabIndex = 0;
            this.emailTextBoxLabel.Text = "Email:";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(42, 41);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(289, 20);
            this.emailTextBox.TabIndex = 1;
            // 
            // descriptionTextBoxLabel
            // 
            this.descriptionTextBoxLabel.AutoSize = true;
            this.descriptionTextBoxLabel.Location = new System.Drawing.Point(39, 86);
            this.descriptionTextBoxLabel.Name = "descriptionTextBoxLabel";
            this.descriptionTextBoxLabel.Size = new System.Drawing.Size(161, 14);
            this.descriptionTextBoxLabel.TabIndex = 2;
            this.descriptionTextBoxLabel.Text = "Describe your problem:";
            // 
            // problemDescriptionTextBox
            // 
            this.problemDescriptionTextBox.Location = new System.Drawing.Point(42, 103);
            this.problemDescriptionTextBox.Multiline = true;
            this.problemDescriptionTextBox.Name = "problemDescriptionTextBox";
            this.problemDescriptionTextBox.Size = new System.Drawing.Size(289, 114);
            this.problemDescriptionTextBox.TabIndex = 3;
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(66, 242);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(81, 23);
            this.sendBtn.TabIndex = 4;
            this.sendBtn.Text = "Send";
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.sendBtn_Click);
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(217, 242);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelBtn.TabIndex = 5;
            this.cancelBtn.Text = "Cancel";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // NeedHelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 277);
            this.ControlBox = false;
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.problemDescriptionTextBox);
            this.Controls.Add(this.descriptionTextBoxLabel);
            this.Controls.Add(this.emailTextBox);
            this.Controls.Add(this.emailTextBoxLabel);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NeedHelpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "RRoadVPN - Help";
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NeedHelpForm_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label emailTextBoxLabel;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.Label descriptionTextBoxLabel;
        private System.Windows.Forms.TextBox problemDescriptionTextBox;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.Button cancelBtn;
    }
}