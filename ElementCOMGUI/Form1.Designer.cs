namespace ElementCOMGUI
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
            this.MainCOMPortSelector = new System.Windows.Forms.ListBox();
            this.COMOut = new System.Windows.Forms.TextBox();
            this.MainCOMPort = new System.IO.Ports.SerialPort(this.components);
            this.MainConnectButton = new System.Windows.Forms.Button();
            this.CommandButton = new System.Windows.Forms.Button();
            this.LogFileCOMPort = new System.IO.Ports.SerialPort(this.components);
            this.MainRefreshButton = new System.Windows.Forms.Button();
            this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
            this.CommandComboBox = new System.Windows.Forms.ComboBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.LogFileCOMGroupBox = new System.Windows.Forms.GroupBox();
            this.AutoConnectProgressBar = new System.Windows.Forms.ProgressBar();
            this.LogDisconnectButton = new System.Windows.Forms.Button();
            this.LogFileCOMNameLabel = new System.Windows.Forms.Label();
            this.LogFileCOMPortConnectionStatusLabel = new System.Windows.Forms.Label();
            this.LogAutoConnectButton = new System.Windows.Forms.Button();
            this.MainCOMGroupBox = new System.Windows.Forms.GroupBox();
            this.MainDisconnectButton = new System.Windows.Forms.Button();
            this.MainCOMNameLabel = new System.Windows.Forms.Label();
            this.MainCOMConnectionStatusLabel = new System.Windows.Forms.Label();
            this.COMOutLabel = new System.Windows.Forms.Label();
            this.TempCOMPort = new System.IO.Ports.SerialPort(this.components);
            this.LogFileSaveDialog = new System.Windows.Forms.SaveFileDialog();
            this.LogFileSavePathLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.LogFileCOMGroupBox.SuspendLayout();
            this.MainCOMGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // MainCOMPortSelector
            // 
            this.MainCOMPortSelector.FormattingEnabled = true;
            this.MainCOMPortSelector.Location = new System.Drawing.Point(10, 18);
            this.MainCOMPortSelector.Name = "MainCOMPortSelector";
            this.MainCOMPortSelector.Size = new System.Drawing.Size(132, 69);
            this.MainCOMPortSelector.TabIndex = 0;
            this.MainCOMPortSelector.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // COMOut
            // 
            this.COMOut.Location = new System.Drawing.Point(25, 371);
            this.COMOut.Multiline = true;
            this.COMOut.Name = "COMOut";
            this.COMOut.ReadOnly = true;
            this.COMOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.COMOut.Size = new System.Drawing.Size(414, 125);
            this.COMOut.TabIndex = 1;
            // 
            // MainCOMPort
            // 
            this.MainCOMPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.MainCOMDataRecievedHandler);
            // 
            // MainConnectButton
            // 
            this.MainConnectButton.Location = new System.Drawing.Point(148, 18);
            this.MainConnectButton.Name = "MainConnectButton";
            this.MainConnectButton.Size = new System.Drawing.Size(75, 20);
            this.MainConnectButton.TabIndex = 2;
            this.MainConnectButton.Text = "Connect";
            this.MainConnectButton.UseVisualStyleBackColor = true;
            this.MainConnectButton.Click += new System.EventHandler(this.MainCOMConnectButton_Click);
            // 
            // CommandButton
            // 
            this.CommandButton.Location = new System.Drawing.Point(137, 128);
            this.CommandButton.Name = "CommandButton";
            this.CommandButton.Size = new System.Drawing.Size(97, 20);
            this.CommandButton.TabIndex = 4;
            this.CommandButton.Text = "Send Command";
            this.CommandButton.UseVisualStyleBackColor = true;
            this.CommandButton.Click += new System.EventHandler(this.CommandButton_Click);
            // 
            // LogFileCOMPort
            // this.LogFileCOMPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.LogFileCOMDataRecievedHandler);
            // 
            // MainRefreshButton
            // 
            this.MainRefreshButton.Location = new System.Drawing.Point(148, 67);
            this.MainRefreshButton.Name = "MainRefreshButton";
            this.MainRefreshButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.MainRefreshButton.Size = new System.Drawing.Size(75, 20);
            this.MainRefreshButton.TabIndex = 6;
            this.MainRefreshButton.Text = "Refresh";
            this.MainRefreshButton.UseVisualStyleBackColor = true;
            this.MainRefreshButton.Click += new System.EventHandler(this.COMRefreshButton_Click);
            // 
            // UpdateTimer
            // 
            this.UpdateTimer.Enabled = true;
            this.UpdateTimer.Tick += new System.EventHandler(this.Update_Tick);
            // 
            // CommandComboBox
            // 
            this.CommandComboBox.FormattingEnabled = true;
            this.CommandComboBox.Items.AddRange(new object[] {
            "STATUS=?",
            "STARTLSR=1",
            "SHUTTER=1",
            "SHUTTER=0",
            "STOPLSR=1",
            "GETLOG=?",
            "RESETLOG=1"});
            this.CommandComboBox.Location = new System.Drawing.Point(10, 127);
            this.CommandComboBox.Name = "CommandComboBox";
            this.CommandComboBox.Size = new System.Drawing.Size(121, 21);
            this.CommandComboBox.TabIndex = 7;
            // 
            // ClearButton
            // 
            this.ClearButton.Location = new System.Drawing.Point(25, 502);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(75, 20);
            this.ClearButton.TabIndex = 8;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // LogFileCOMGroupBox
            // 
            this.LogFileCOMGroupBox.Controls.Add(this.label1);
            this.LogFileCOMGroupBox.Controls.Add(this.button1);
            this.LogFileCOMGroupBox.Controls.Add(this.AutoConnectProgressBar);
            this.LogFileCOMGroupBox.Controls.Add(this.LogDisconnectButton);
            this.LogFileCOMGroupBox.Controls.Add(this.LogFileCOMNameLabel);
            this.LogFileCOMGroupBox.Controls.Add(this.LogFileCOMPortConnectionStatusLabel);
            this.LogFileCOMGroupBox.Controls.Add(this.LogAutoConnectButton);
            this.LogFileCOMGroupBox.Location = new System.Drawing.Point(323, 12);
            this.LogFileCOMGroupBox.Name = "LogFileCOMGroupBox";
            this.LogFileCOMGroupBox.Size = new System.Drawing.Size(153, 281);
            this.LogFileCOMGroupBox.TabIndex = 9;
            this.LogFileCOMGroupBox.TabStop = false;
            this.LogFileCOMGroupBox.Text = "Log File COM Port Panel";
            // 
            // AutoConnectProgressBar
            // 
            this.AutoConnectProgressBar.Location = new System.Drawing.Point(97, 37);
            this.AutoConnectProgressBar.Name = "AutoConnectProgressBar";
            this.AutoConnectProgressBar.Size = new System.Drawing.Size(50, 15);
            this.AutoConnectProgressBar.TabIndex = 12;
            this.AutoConnectProgressBar.Visible = false;
            // 
            // LogDisconnectButton
            // 
            this.LogDisconnectButton.Location = new System.Drawing.Point(6, 61);
            this.LogDisconnectButton.Name = "LogDisconnectButton";
            this.LogDisconnectButton.Size = new System.Drawing.Size(88, 20);
            this.LogDisconnectButton.TabIndex = 11;
            this.LogDisconnectButton.Text = "Disconnect";
            this.LogDisconnectButton.UseVisualStyleBackColor = true;
            this.LogDisconnectButton.Click += new System.EventHandler(this.LogDisconnectButton_Click);
            // 
            // LogFileCOMNameLabel
            // 
            this.LogFileCOMNameLabel.AutoSize = true;
            this.LogFileCOMNameLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.LogFileCOMNameLabel.Location = new System.Drawing.Point(91, 18);
            this.LogFileCOMNameLabel.Name = "LogFileCOMNameLabel";
            this.LogFileCOMNameLabel.Size = new System.Drawing.Size(10, 13);
            this.LogFileCOMNameLabel.TabIndex = 10;
            this.LogFileCOMNameLabel.Text = " ";
            // 
            // LogFileCOMPortConnectionStatusLabel
            // 
            this.LogFileCOMPortConnectionStatusLabel.AutoSize = true;
            this.LogFileCOMPortConnectionStatusLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.LogFileCOMPortConnectionStatusLabel.Location = new System.Drawing.Point(6, 18);
            this.LogFileCOMPortConnectionStatusLabel.Name = "LogFileCOMPortConnectionStatusLabel";
            this.LogFileCOMPortConnectionStatusLabel.Size = new System.Drawing.Size(79, 13);
            this.LogFileCOMPortConnectionStatusLabel.TabIndex = 9;
            this.LogFileCOMPortConnectionStatusLabel.Text = "Not Connected";
            // 
            // LogAutoConnectButton
            // 
            this.LogAutoConnectButton.Location = new System.Drawing.Point(6, 35);
            this.LogAutoConnectButton.Name = "LogAutoConnectButton";
            this.LogAutoConnectButton.Size = new System.Drawing.Size(88, 20);
            this.LogAutoConnectButton.TabIndex = 0;
            this.LogAutoConnectButton.Text = "Auto-Connect";
            this.LogAutoConnectButton.UseVisualStyleBackColor = true;
            this.LogAutoConnectButton.Click += new System.EventHandler(this.LogFileCOMPortAutoConnect_Click);
            // 
            // MainCOMGroupBox
            // 
            this.MainCOMGroupBox.Controls.Add(this.MainDisconnectButton);
            this.MainCOMGroupBox.Controls.Add(this.MainCOMNameLabel);
            this.MainCOMGroupBox.Controls.Add(this.MainCOMConnectionStatusLabel);
            this.MainCOMGroupBox.Controls.Add(this.CommandComboBox);
            this.MainCOMGroupBox.Controls.Add(this.MainRefreshButton);
            this.MainCOMGroupBox.Controls.Add(this.CommandButton);
            this.MainCOMGroupBox.Controls.Add(this.MainConnectButton);
            this.MainCOMGroupBox.Controls.Add(this.MainCOMPortSelector);
            this.MainCOMGroupBox.Location = new System.Drawing.Point(12, 12);
            this.MainCOMGroupBox.Name = "MainCOMGroupBox";
            this.MainCOMGroupBox.Size = new System.Drawing.Size(311, 281);
            this.MainCOMGroupBox.TabIndex = 10;
            this.MainCOMGroupBox.TabStop = false;
            this.MainCOMGroupBox.Text = "Main COM Port Panel";
            // 
            // MainDisconnectButton
            // 
            this.MainDisconnectButton.Location = new System.Drawing.Point(148, 41);
            this.MainDisconnectButton.Name = "MainDisconnectButton";
            this.MainDisconnectButton.Size = new System.Drawing.Size(75, 20);
            this.MainDisconnectButton.TabIndex = 10;
            this.MainDisconnectButton.Text = "Disconnect";
            this.MainDisconnectButton.UseVisualStyleBackColor = true;
            this.MainDisconnectButton.Click += new System.EventHandler(this.MainDisconnectButton_Click);
            // 
            // MainCOMNameLabel
            // 
            this.MainCOMNameLabel.AutoSize = true;
            this.MainCOMNameLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.MainCOMNameLabel.Location = new System.Drawing.Point(226, 35);
            this.MainCOMNameLabel.Name = "MainCOMNameLabel";
            this.MainCOMNameLabel.Size = new System.Drawing.Size(10, 13);
            this.MainCOMNameLabel.TabIndex = 9;
            this.MainCOMNameLabel.Text = " ";
            // 
            // MainCOMConnectionStatusLabel
            // 
            this.MainCOMConnectionStatusLabel.AutoSize = true;
            this.MainCOMConnectionStatusLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.MainCOMConnectionStatusLabel.Location = new System.Drawing.Point(226, 22);
            this.MainCOMConnectionStatusLabel.Name = "MainCOMConnectionStatusLabel";
            this.MainCOMConnectionStatusLabel.Size = new System.Drawing.Size(79, 13);
            this.MainCOMConnectionStatusLabel.TabIndex = 8;
            this.MainCOMConnectionStatusLabel.Text = "Not Connected";
            // 
            // COMOutLabel
            // 
            this.COMOutLabel.AutoSize = true;
            this.COMOutLabel.Location = new System.Drawing.Point(22, 355);
            this.COMOutLabel.Name = "COMOutLabel";
            this.COMOutLabel.Size = new System.Drawing.Size(92, 13);
            this.COMOutLabel.TabIndex = 11;
            this.COMOutLabel.Text = "Main COM Output";
            // 
            // TempCOMPort
            // 
            this.TempCOMPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.TempCOMDataRecievedHandler);
            // 
            // LogFileSavePathLabel
            // 
            this.LogFileSavePathLabel.AutoSize = true;
            this.LogFileSavePathLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
            this.LogFileSavePathLabel.Location = new System.Drawing.Point(12, 296);
            this.LogFileSavePathLabel.Name = "LogFileSavePathLabel";
            this.LogFileSavePathLabel.Size = new System.Drawing.Size(0, 13);
            this.LogFileSavePathLabel.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(31, 214);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 255);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 14;
            this.label1.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(488, 527);
            this.Controls.Add(this.LogFileSavePathLabel);
            this.Controls.Add(this.COMOutLabel);
            this.Controls.Add(this.MainCOMGroupBox);
            this.Controls.Add(this.LogFileCOMGroupBox);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.COMOut);
            this.Name = "Form1";
            this.Text = "Element COM GUI";
            this.LogFileCOMGroupBox.ResumeLayout(false);
            this.LogFileCOMGroupBox.PerformLayout();
            this.MainCOMGroupBox.ResumeLayout(false);
            this.MainCOMGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox MainCOMPortSelector;
        private System.Windows.Forms.TextBox COMOut;
        private System.IO.Ports.SerialPort MainCOMPort;
        private System.Windows.Forms.Button MainConnectButton;
        private System.Windows.Forms.Button CommandButton;
        private System.IO.Ports.SerialPort LogFileCOMPort;
        private System.Windows.Forms.Button MainRefreshButton;
        private System.Windows.Forms.Timer UpdateTimer;
        private System.Windows.Forms.ComboBox CommandComboBox;
        private System.Windows.Forms.Button ClearButton;
        private System.Windows.Forms.GroupBox LogFileCOMGroupBox;
        private System.Windows.Forms.GroupBox MainCOMGroupBox;
        private System.Windows.Forms.Label COMOutLabel;
        private System.Windows.Forms.Label MainCOMConnectionStatusLabel;
        private System.Windows.Forms.Label LogFileCOMPortConnectionStatusLabel;
        private System.Windows.Forms.Button LogAutoConnectButton;
        private System.IO.Ports.SerialPort TempCOMPort;
        private System.Windows.Forms.Label LogFileCOMNameLabel;
        private System.Windows.Forms.Label MainCOMNameLabel;
        private System.Windows.Forms.Button LogDisconnectButton;
        private System.Windows.Forms.Button MainDisconnectButton;
        private System.Windows.Forms.ProgressBar AutoConnectProgressBar;
        private System.Windows.Forms.SaveFileDialog LogFileSaveDialog;
        private System.Windows.Forms.Label LogFileSavePathLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
    }
}

