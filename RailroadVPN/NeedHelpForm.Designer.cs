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
            this.label1 = new System.Windows.Forms.Label();
            this.emailTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.problemDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.sendBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.sendLogsCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(39, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "Email:";
            // 
            // emailTextBox
            // 
            this.emailTextBox.Location = new System.Drawing.Point(42, 41);
            this.emailTextBox.Name = "emailTextBox";
            this.emailTextBox.Size = new System.Drawing.Size(289, 20);
            this.emailTextBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(39, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(161, 14);
            this.label2.TabIndex = 2;
            this.label2.Text = "Describe your problem:";
            // 
            // problemDescriptionTextBox
            // 
            this.problemDescriptionTextBox.Location = new System.Drawing.Point(42, 103);
            this.problemDescriptionTextBox.Multiline = true;
            this.problemDescriptionTextBox.Name = "problemDescriptionTextBox";
            this.problemDescriptionTextBox.Size = new System.Drawing.Size(289, 83);
            this.problemDescriptionTextBox.TabIndex = 3;
            // 
            // sendBtn
            // 
            this.sendBtn.Location = new System.Drawing.Point(66, 242);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(75, 23);
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
            // sendLogsCheckBox
            // 
            this.sendLogsCheckBox.AutoSize = true;
            this.sendLogsCheckBox.Location = new System.Drawing.Point(84, 204);
            this.sendLogsCheckBox.Name = "sendLogsCheckBox";
            this.sendLogsCheckBox.Size = new System.Drawing.Size(208, 18);
            this.sendLogsCheckBox.TabIndex = 6;
            this.sendLogsCheckBox.Text = "Send log file with problem";
            this.sendLogsCheckBox.UseVisualStyleBackColor = true;
            // 
            // NeedHelpForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(372, 277);
            this.ControlBox = false;
            this.Controls.Add(this.sendLogsCheckBox);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.sendBtn);
            this.Controls.Add(this.problemDescriptionTextBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.emailTextBox);
            this.Controls.Add(this.label1);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NeedHelpForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.TopMost = true;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.NeedHelpForm_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox emailTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox problemDescriptionTextBox;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.CheckBox sendLogsCheckBox;
    }
}