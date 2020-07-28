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
			this.MainCOMPort = new System.IO.Ports.SerialPort(this.components);
			this.MainConnectButton = new System.Windows.Forms.Button();
			this.CommandButton = new System.Windows.Forms.Button();
			this.LogFileCOMPort = new System.IO.Ports.SerialPort(this.components);
			this.MainRefreshButton = new System.Windows.Forms.Button();
			this.UpdateTimer = new System.Windows.Forms.Timer(this.components);
			this.CommandComboBox = new System.Windows.Forms.ComboBox();
			this.ClearButton = new System.Windows.Forms.Button();
			this.LogFileCOMGroupBox = new System.Windows.Forms.GroupBox();
			this.LogfileTransferTimeLabel = new System.Windows.Forms.Label();
			this.AutoConnectProgressBar = new System.Windows.Forms.ProgressBar();
			this.LogDisconnectButton = new System.Windows.Forms.Button();
			this.LogFileCOMNameLabel = new System.Windows.Forms.Label();
			this.LogFileCOMPortConnectionStatusLabel = new System.Windows.Forms.Label();
			this.LogAutoConnectButton = new System.Windows.Forms.Button();
			this.MainCOMGroupBox = new System.Windows.Forms.GroupBox();
			this.AutoTurnOnCheckBox = new System.Windows.Forms.CheckBox();
			this.AutoTurnOnTimePicker = new System.Windows.Forms.DateTimePicker();
			this.MainDisconnectButton = new System.Windows.Forms.Button();
			this.MainCOMNameLabel = new System.Windows.Forms.Label();
			this.MainCOMConnectionStatusLabel = new System.Windows.Forms.Label();
			this.TempCOMPort = new System.IO.Ports.SerialPort(this.components);
			this.LogFileSaveDialog = new System.Windows.Forms.SaveFileDialog();
			this.LogFileSavePathLabel = new System.Windows.Forms.Label();
			this.ErrorLabel = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.StatusTab = new System.Windows.Forms.TabPage();
			this.LaserReadyLabel = new System.Windows.Forms.Label();
			this.StatusDisplay = new System.Windows.Forms.DataGridView();
			this.Property = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Current = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Reference = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Difference = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MainCOMOutputTab = new System.Windows.Forms.TabPage();
			this.COMOut = new System.Windows.Forms.TextBox();
			this.EventLogTab = new System.Windows.Forms.TabPage();
			this.EventLogClearButton = new System.Windows.Forms.Button();
			this.OpenEventLogFolderButton = new System.Windows.Forms.Button();
			this.EventLog = new System.Windows.Forms.DataGridView();
			this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Event = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AutoTurnOnDatePicker = new System.Windows.Forms.DateTimePicker();
			this.LogFileCOMGroupBox.SuspendLayout();
			this.MainCOMGroupBox.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.StatusTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.StatusDisplay)).BeginInit();
			this.MainCOMOutputTab.SuspendLayout();
			this.EventLogTab.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.EventLog)).BeginInit();
			this.SuspendLayout();
			// 
			// MainCOMPortSelector
			// 
			this.MainCOMPortSelector.FormattingEnabled = true;
			this.MainCOMPortSelector.Location = new System.Drawing.Point(10, 18);
			this.MainCOMPortSelector.Name = "MainCOMPortSelector";
			this.MainCOMPortSelector.Size = new System.Drawing.Size(132, 69);
			this.MainCOMPortSelector.TabIndex = 0;
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
			// 
			this.LogFileCOMPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.LogFileCOMDataRecievedHandler);
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
			this.ClearButton.Location = new System.Drawing.Point(6, 222);
			this.ClearButton.Name = "ClearButton";
			this.ClearButton.Size = new System.Drawing.Size(75, 20);
			this.ClearButton.TabIndex = 8;
			this.ClearButton.Text = "Clear";
			this.ClearButton.UseVisualStyleBackColor = true;
			this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
			// 
			// LogFileCOMGroupBox
			// 
			this.LogFileCOMGroupBox.Controls.Add(this.LogfileTransferTimeLabel);
			this.LogFileCOMGroupBox.Controls.Add(this.AutoConnectProgressBar);
			this.LogFileCOMGroupBox.Controls.Add(this.LogDisconnectButton);
			this.LogFileCOMGroupBox.Controls.Add(this.LogFileCOMNameLabel);
			this.LogFileCOMGroupBox.Controls.Add(this.LogFileCOMPortConnectionStatusLabel);
			this.LogFileCOMGroupBox.Controls.Add(this.LogAutoConnectButton);
			this.LogFileCOMGroupBox.Location = new System.Drawing.Point(323, 12);
			this.LogFileCOMGroupBox.Name = "LogFileCOMGroupBox";
			this.LogFileCOMGroupBox.Size = new System.Drawing.Size(153, 204);
			this.LogFileCOMGroupBox.TabIndex = 9;
			this.LogFileCOMGroupBox.TabStop = false;
			this.LogFileCOMGroupBox.Text = "Log File COM Port Panel";
			// 
			// LogfileTransferTimeLabel
			// 
			this.LogfileTransferTimeLabel.AutoSize = true;
			this.LogfileTransferTimeLabel.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.LogfileTransferTimeLabel.Location = new System.Drawing.Point(3, 89);
			this.LogfileTransferTimeLabel.Name = "LogfileTransferTimeLabel";
			this.LogfileTransferTimeLabel.Size = new System.Drawing.Size(147, 26);
			this.LogfileTransferTimeLabel.TabIndex = 13;
			this.LogfileTransferTimeLabel.Text = "File Transfer Time Remaining:\r\n0 Second(s)";
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
			this.MainCOMGroupBox.Controls.Add(this.AutoTurnOnDatePicker);
			this.MainCOMGroupBox.Controls.Add(this.AutoTurnOnCheckBox);
			this.MainCOMGroupBox.Controls.Add(this.AutoTurnOnTimePicker);
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
			this.MainCOMGroupBox.Size = new System.Drawing.Size(311, 204);
			this.MainCOMGroupBox.TabIndex = 10;
			this.MainCOMGroupBox.TabStop = false;
			this.MainCOMGroupBox.Text = "Main COM Port Panel";
			// 
			// AutoTurnOnCheckBox
			// 
			this.AutoTurnOnCheckBox.AutoSize = true;
			this.AutoTurnOnCheckBox.Location = new System.Drawing.Point(8, 168);
			this.AutoTurnOnCheckBox.Name = "AutoTurnOnCheckBox";
			this.AutoTurnOnCheckBox.Size = new System.Drawing.Size(106, 17);
			this.AutoTurnOnCheckBox.TabIndex = 14;
			this.AutoTurnOnCheckBox.Text = "Auto Turn On At:";
			this.AutoTurnOnCheckBox.UseVisualStyleBackColor = true;
			this.AutoTurnOnCheckBox.CheckedChanged += new System.EventHandler(this.AutoTurnOnCheckBox_CheckedChanged);
			// 
			// AutoTurnOnTimePicker
			// 
			this.AutoTurnOnTimePicker.CustomFormat = "hh:mm tt";
			this.AutoTurnOnTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.AutoTurnOnTimePicker.Location = new System.Drawing.Point(120, 165);
			this.AutoTurnOnTimePicker.Name = "AutoTurnOnTimePicker";
			this.AutoTurnOnTimePicker.ShowUpDown = true;
			this.AutoTurnOnTimePicker.Size = new System.Drawing.Size(80, 20);
			this.AutoTurnOnTimePicker.TabIndex = 13;
			this.AutoTurnOnTimePicker.ValueChanged += new System.EventHandler(this.AutoTurnOnTimePicker_ValueChanged);
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
			// ErrorLabel
			// 
			this.ErrorLabel.AutoSize = true;
			this.ErrorLabel.ForeColor = System.Drawing.Color.Red;
			this.ErrorLabel.Location = new System.Drawing.Point(12, 505);
			this.ErrorLabel.Name = "ErrorLabel";
			this.ErrorLabel.Size = new System.Drawing.Size(0, 13);
			this.ErrorLabel.TabIndex = 13;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.StatusTab);
			this.tabControl1.Controls.Add(this.MainCOMOutputTab);
			this.tabControl1.Controls.Add(this.EventLogTab);
			this.tabControl1.Location = new System.Drawing.Point(12, 222);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(464, 274);
			this.tabControl1.TabIndex = 14;
			// 
			// StatusTab
			// 
			this.StatusTab.Controls.Add(this.LaserReadyLabel);
			this.StatusTab.Controls.Add(this.StatusDisplay);
			this.StatusTab.Location = new System.Drawing.Point(4, 22);
			this.StatusTab.Name = "StatusTab";
			this.StatusTab.Padding = new System.Windows.Forms.Padding(3);
			this.StatusTab.Size = new System.Drawing.Size(456, 248);
			this.StatusTab.TabIndex = 2;
			this.StatusTab.Text = "Status";
			this.StatusTab.UseVisualStyleBackColor = true;
			// 
			// LaserReadyLabel
			// 
			this.LaserReadyLabel.AutoSize = true;
			this.LaserReadyLabel.Location = new System.Drawing.Point(6, 3);
			this.LaserReadyLabel.Name = "LaserReadyLabel";
			this.LaserReadyLabel.Size = new System.Drawing.Size(67, 13);
			this.LaserReadyLabel.TabIndex = 1;
			this.LaserReadyLabel.Text = "Laser Ready";
			// 
			// StatusDisplay
			// 
			this.StatusDisplay.AllowUserToAddRows = false;
			this.StatusDisplay.AllowUserToDeleteRows = false;
			this.StatusDisplay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.StatusDisplay.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Property,
            this.Current,
            this.Reference,
            this.Difference});
			this.StatusDisplay.Location = new System.Drawing.Point(9, 19);
			this.StatusDisplay.Name = "StatusDisplay";
			this.StatusDisplay.ReadOnly = true;
			this.StatusDisplay.Size = new System.Drawing.Size(441, 214);
			this.StatusDisplay.TabIndex = 0;
			// 
			// Property
			// 
			this.Property.HeaderText = "";
			this.Property.Name = "Property";
			this.Property.ReadOnly = true;
			// 
			// Current
			// 
			this.Current.HeaderText = "Current";
			this.Current.Name = "Current";
			this.Current.ReadOnly = true;
			// 
			// Reference
			// 
			this.Reference.HeaderText = "Reference";
			this.Reference.Name = "Reference";
			this.Reference.ReadOnly = true;
			// 
			// Difference
			// 
			this.Difference.HeaderText = "Difference";
			this.Difference.Name = "Difference";
			this.Difference.ReadOnly = true;
			// 
			// MainCOMOutputTab
			// 
			this.MainCOMOutputTab.Controls.Add(this.COMOut);
			this.MainCOMOutputTab.Controls.Add(this.ClearButton);
			this.MainCOMOutputTab.Location = new System.Drawing.Point(4, 22);
			this.MainCOMOutputTab.Name = "MainCOMOutputTab";
			this.MainCOMOutputTab.Padding = new System.Windows.Forms.Padding(3);
			this.MainCOMOutputTab.Size = new System.Drawing.Size(456, 248);
			this.MainCOMOutputTab.TabIndex = 0;
			this.MainCOMOutputTab.Text = "Main COM Output";
			this.MainCOMOutputTab.UseVisualStyleBackColor = true;
			// 
			// COMOut
			// 
			this.COMOut.Location = new System.Drawing.Point(6, 6);
			this.COMOut.Multiline = true;
			this.COMOut.Name = "COMOut";
			this.COMOut.ReadOnly = true;
			this.COMOut.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.COMOut.Size = new System.Drawing.Size(444, 210);
			this.COMOut.TabIndex = 12;
			// 
			// EventLogTab
			// 
			this.EventLogTab.Controls.Add(this.EventLogClearButton);
			this.EventLogTab.Controls.Add(this.OpenEventLogFolderButton);
			this.EventLogTab.Controls.Add(this.EventLog);
			this.EventLogTab.Location = new System.Drawing.Point(4, 22);
			this.EventLogTab.Name = "EventLogTab";
			this.EventLogTab.Padding = new System.Windows.Forms.Padding(3);
			this.EventLogTab.Size = new System.Drawing.Size(456, 248);
			this.EventLogTab.TabIndex = 1;
			this.EventLogTab.Text = "Event Log";
			this.EventLogTab.UseVisualStyleBackColor = true;
			// 
			// EventLogClearButton
			// 
			this.EventLogClearButton.Location = new System.Drawing.Point(9, 222);
			this.EventLogClearButton.Name = "EventLogClearButton";
			this.EventLogClearButton.Size = new System.Drawing.Size(65, 23);
			this.EventLogClearButton.TabIndex = 16;
			this.EventLogClearButton.Text = "Clear";
			this.EventLogClearButton.UseVisualStyleBackColor = true;
			this.EventLogClearButton.Click += new System.EventHandler(this.EventLogClearButton_Click);
			// 
			// OpenEventLogFolderButton
			// 
			this.OpenEventLogFolderButton.Location = new System.Drawing.Point(311, 222);
			this.OpenEventLogFolderButton.Name = "OpenEventLogFolderButton";
			this.OpenEventLogFolderButton.Size = new System.Drawing.Size(139, 23);
			this.OpenEventLogFolderButton.TabIndex = 15;
			this.OpenEventLogFolderButton.Text = "Open Event Log Folder";
			this.OpenEventLogFolderButton.UseVisualStyleBackColor = true;
			this.OpenEventLogFolderButton.Click += new System.EventHandler(this.OpenEventLogFolderButton_Click);
			// 
			// EventLog
			// 
			this.EventLog.AllowUserToAddRows = false;
			this.EventLog.AllowUserToDeleteRows = false;
			this.EventLog.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.EventLog.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Time,
            this.Event});
			this.EventLog.Location = new System.Drawing.Point(9, 6);
			this.EventLog.Name = "EventLog";
			this.EventLog.ReadOnly = true;
			this.EventLog.Size = new System.Drawing.Size(441, 214);
			this.EventLog.TabIndex = 0;
			// 
			// Time
			// 
			this.Time.HeaderText = "Time";
			this.Time.Name = "Time";
			this.Time.ReadOnly = true;
			// 
			// Event
			// 
			this.Event.HeaderText = "Event";
			this.Event.Name = "Event";
			this.Event.ReadOnly = true;
			// 
			// AutoTurnOnDatePicker
			// 
			this.AutoTurnOnDatePicker.CustomFormat = "ddd d/MM";
			this.AutoTurnOnDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.AutoTurnOnDatePicker.Location = new System.Drawing.Point(206, 165);
			this.AutoTurnOnDatePicker.Name = "AutoTurnOnDatePicker";
			this.AutoTurnOnDatePicker.Size = new System.Drawing.Size(99, 20);
			this.AutoTurnOnDatePicker.TabIndex = 15;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(488, 527);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.ErrorLabel);
			this.Controls.Add(this.LogFileSavePathLabel);
			this.Controls.Add(this.MainCOMGroupBox);
			this.Controls.Add(this.LogFileCOMGroupBox);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "Form1";
			this.Text = "Element COM GUI Ver 4.4.5";
			this.LogFileCOMGroupBox.ResumeLayout(false);
			this.LogFileCOMGroupBox.PerformLayout();
			this.MainCOMGroupBox.ResumeLayout(false);
			this.MainCOMGroupBox.PerformLayout();
			this.tabControl1.ResumeLayout(false);
			this.StatusTab.ResumeLayout(false);
			this.StatusTab.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.StatusDisplay)).EndInit();
			this.MainCOMOutputTab.ResumeLayout(false);
			this.MainCOMOutputTab.PerformLayout();
			this.EventLogTab.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.EventLog)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox MainCOMPortSelector;
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
        private System.Windows.Forms.Label ErrorLabel;
        private System.Windows.Forms.DateTimePicker AutoTurnOnTimePicker;
        private System.Windows.Forms.CheckBox AutoTurnOnCheckBox;
        private System.Windows.Forms.Label LogfileTransferTimeLabel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage MainCOMOutputTab;
        private System.Windows.Forms.TextBox COMOut;
        private System.Windows.Forms.TabPage EventLogTab;
        private System.Windows.Forms.DataGridView EventLog;
        private System.Windows.Forms.DataGridViewTextBoxColumn Time;
        private System.Windows.Forms.DataGridViewTextBoxColumn Event;
        private System.Windows.Forms.TabPage StatusTab;
        private System.Windows.Forms.DataGridView StatusDisplay;
        private System.Windows.Forms.DataGridViewTextBoxColumn Property;
        private System.Windows.Forms.DataGridViewTextBoxColumn Current;
        private System.Windows.Forms.DataGridViewTextBoxColumn Reference;
        private System.Windows.Forms.DataGridViewTextBoxColumn Difference;
        private System.Windows.Forms.Label LaserReadyLabel;
        private System.Windows.Forms.Button OpenEventLogFolderButton;
        private System.Windows.Forms.Button EventLogClearButton;
		private System.Windows.Forms.DateTimePicker AutoTurnOnDatePicker;
	}
}

