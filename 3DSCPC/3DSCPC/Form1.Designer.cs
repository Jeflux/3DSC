namespace _3DSCPC
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.btnExit = new System.Windows.Forms.Button();
            this.broadcastTimer = new System.Windows.Forms.Timer(this.components);
            this.ckMinimizeToTray = new System.Windows.Forms.CheckBox();
            this.cbboxIP = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBroadcast = new System.Windows.Forms.Button();
            this.btnStopBroadcast = new System.Windows.Forms.Button();
            this.notifMinimize = new System.Windows.Forms.NotifyIcon(this.components);
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Location = new System.Drawing.Point(270, 94);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 0;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // broadcastTimer
            // 
            this.broadcastTimer.Tick += new System.EventHandler(this.broadcastTimer_Tick);
            // 
            // ckMinimizeToTray
            // 
            this.ckMinimizeToTray.AutoSize = true;
            this.ckMinimizeToTray.Location = new System.Drawing.Point(12, 100);
            this.ckMinimizeToTray.Name = "ckMinimizeToTray";
            this.ckMinimizeToTray.Size = new System.Drawing.Size(98, 17);
            this.ckMinimizeToTray.TabIndex = 1;
            this.ckMinimizeToTray.Text = "Minimize to tray";
            this.ckMinimizeToTray.UseVisualStyleBackColor = true;
            // 
            // cbboxIP
            // 
            this.cbboxIP.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbboxIP.FormattingEnabled = true;
            this.cbboxIP.Location = new System.Drawing.Point(75, 13);
            this.cbboxIP.Name = "cbboxIP";
            this.cbboxIP.Size = new System.Drawing.Size(189, 21);
            this.cbboxIP.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "IP address:";
            // 
            // btnBroadcast
            // 
            this.btnBroadcast.Location = new System.Drawing.Point(270, 11);
            this.btnBroadcast.Name = "btnBroadcast";
            this.btnBroadcast.Size = new System.Drawing.Size(75, 23);
            this.btnBroadcast.TabIndex = 4;
            this.btnBroadcast.Text = "Broadcast";
            this.btnBroadcast.UseVisualStyleBackColor = true;
            this.btnBroadcast.Click += new System.EventHandler(this.btnBroadcast_Click);
            // 
            // btnStopBroadcast
            // 
            this.btnStopBroadcast.Location = new System.Drawing.Point(270, 41);
            this.btnStopBroadcast.Name = "btnStopBroadcast";
            this.btnStopBroadcast.Size = new System.Drawing.Size(75, 23);
            this.btnStopBroadcast.TabIndex = 5;
            this.btnStopBroadcast.Text = "Stop";
            this.btnStopBroadcast.UseVisualStyleBackColor = true;
            this.btnStopBroadcast.Click += new System.EventHandler(this.btnStopBroadcast_Click);
            // 
            // notifMinimize
            // 
            this.notifMinimize.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.notifMinimize.Icon = ((System.Drawing.Icon)(resources.GetObject("notifMinimize.Icon")));
            this.notifMinimize.Text = "notifyIcon1";
            this.notifMinimize.DoubleClick += new System.EventHandler(this.notifMinimize_DoubleClick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 129);
            this.Controls.Add(this.btnStopBroadcast);
            this.Controls.Add(this.btnBroadcast);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cbboxIP);
            this.Controls.Add(this.ckMinimizeToTray);
            this.Controls.Add(this.btnExit);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "3DSC";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Timer broadcastTimer;
        private System.Windows.Forms.CheckBox ckMinimizeToTray;
        private System.Windows.Forms.ComboBox cbboxIP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBroadcast;
        private System.Windows.Forms.Button btnStopBroadcast;
        private System.Windows.Forms.NotifyIcon notifMinimize;
    }
}

