using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ElementCOMGUI
{
    public partial class Form1 : Form
    {
        #region VARIABLES

        private string[] COMList; // Array to store names of all COM ports discovered
        private static List<string> MainCOMDataBuffer = new List<string>(); // List to store data recieved from the main COM connection
        private static List<string> TempCOMDataBuffer = new List<string>(); // List to store data recieved from the temp COM connection
        //private static List<string> LogFileCOMDataBuffer = new List<string>(); // List to store data recieved from the log file COM connection
        private bool logFileWriteFlag = false; // Flag set high when log file is being written to
        private bool autoTurnOnSentFlag = false; // Flag set high when auto turn on has been requested
        private bool supressAutoTurnOnCheckBoxMessage = false; // Flag set high when the program is altering the state of the auto turn on check box
        private DateTime prevTime = DateTime.Now;

        #endregion

        #region CONSTRUCTOR

        /// <summary>
        /// Form Constructor
        /// </summary>
        public Form1()
        {
            InitializeComponent(); // Initialised the application
            RefreshCOMSelector(); // Get the discoverable COM ports on start-up
        }

        #endregion

        #region COM_CONNECTION_FUNCTIONS

        /// <summary>
        /// Retrieves the discoverable COM ports, storing the names to 'COMList'
        /// </summary>
        private void RefreshCOM()
        {
            COMList = SerialPort.GetPortNames(); // Acquire string array of the names of all discoverable COM ports

            Array.Sort(COMList); // Sort the list
        }

        /// <summary>
        /// Connects 'SerialPort' object 'port' to the port with name 'portName' if possible, using a baud rate of 'baudRate'.
        /// </summary>
        /// <param name="port">
        /// SerialPort object to be opened
        /// </param>
        /// <param name="portName">
        /// Name of the port to be connected to
        /// </param>
        /// <param name="baudRate">
        /// Baud rate of the port (Default value = 115200)
        /// </param>
        private void ConnectCOM(SerialPort port, string portName, int baudRate = 115200)
        {
            port.Close(); // Make sure port is closed

            port.BaudRate = baudRate; // Set the baud rate

            try
            {
                port.PortName = portName; // Set which port to connect to

                port.Open(); // Open a connection to the port

                // Remove data from input/output buffers
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
            }
            catch { }
        }

        /// <summary>
        /// Closes the connection of serial port 'port'
        /// </summary>
        /// <param name="port">
        /// Serial port to be closed
        /// </param>
        private void DisconnectCOM(SerialPort port)
        {
            port.Close(); // Close connection of port
        }

        /// <summary>
        /// Event handler to store serial data recieved to be stored to a buffer list
        /// </summary>
        /// <param name="dataBufferList">
        /// List where the recieved data will be stored
        /// </param>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void COMDataRecievedHandler(List<string> dataBufferList, object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender; // Get the 'SerialPort' object which raised the event
            string data = port.ReadExisting(); // Read all existing data in a string

            dataBufferList.Add(data); // Store data to the buffer
        }

        /// <summary>
        /// Event handler used by the main com port to buffer recieved data to 'MainCOMDataBuffer'
        /// </summary>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void MainCOMDataRecievedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            COMDataRecievedHandler(MainCOMDataBuffer, sender, e);
        }

        /// <summary>
        /// Event handler used by the temp com port to buffer recieved data to 'TempCOMDataBuffer'
        /// </summary>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void TempCOMDataRecievedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            COMDataRecievedHandler(TempCOMDataBuffer, sender, e);
        }

        /// <summary>
        /// Event handler used by the log file com port to write recieved data to the log file 
        /// </summary>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void LogFileCOMDataRecievedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            // If log file writing is enabled
            if (logFileWriteFlag)
            {
                SerialPort port = (SerialPort)sender; // Get the 'SerialPort' object which raised the event
                string data = port.ReadExisting(); // Read all existing data into a string

                // Append the data to the file 'LogFileSaveDialog.FileName'
                using (StreamWriter sw = File.AppendText(LogFileSaveDialog.FileName))
                {
                    sw.Write(data);
                }
            }
        }

        /// <summary>
        /// Sets the given status label and name label text depending on the state of the serial port 'port' connection
        /// </summary>
        /// <param name="port">
        /// 'SerialPort' object that will be checked
        /// </param>
        /// <param name="statusLabel">
        /// GUI label which displays the status of the connection
        /// </param>
        /// <param name="nameLabel">
        /// GUI label which displays the name of the connected COM port
        /// </param>
        private void UpdateCOMConnectionStatus(SerialPort port, Label statusLabel, Label nameLabel)
        {
            // If port is open (connected)
            if (port.IsOpen)
            {
                statusLabel.Text = "Connected"; // Set connection status label text to 'Connected'
                nameLabel.Text = port.PortName; // Set COM name label text to the name of the connected port
            }
            else
            {
                statusLabel.Text = "Not Connected"; // Set connection status label text to 'Not Connected'
                nameLabel.Text = ""; // Hide the COM name label text
            }
        }

        /// <summary>
        /// Updates the main COM status and name labels
        /// </summary>
        private void UpdateMainCOMConnectionStatus()
        {
            UpdateCOMConnectionStatus(MainCOMPort, MainCOMConnectionStatusLabel, MainCOMNameLabel);
        }

        /// <summary>
        /// Updates the log file COM status and name labels
        /// </summary>
        private void UpdateLogFileCOMConnectionStatus()
        {
            UpdateCOMConnectionStatus(LogFileCOMPort, LogFileCOMPortConnectionStatusLabel, LogFileCOMNameLabel);
        }

        #endregion

        #region LOG_FILE_SAVE_DIALOG_FUNCTIONS

        /// <summary>
        /// Brings up the save file dialog for the log file. Returns the end state of the dialog (i.e. if it was cancelled)
        /// </summary>
        /// <returns></returns>
        private DialogResult SetLogFileSavePath()
        {
            LogFileSaveDialog.Filter = "CSV File|*.csv"; // Set the save file delimiter filter
            LogFileSaveDialog.Title = "Log File Save Location"; // Set the title of the save dialog window
            var result = LogFileSaveDialog.ShowDialog(); // Show the save file dialog and store the result of the dialog

            // If the save file dialog result is a cancel
            if (result == DialogResult.Cancel)
            {
                LogFileSaveDialog.FileName = ""; // Set the save file path string to an empty string
            }

            UpdateSaveLocationLabel(); // Update the save location label

            return result; // Return the result of the save dialog
        }

        /// <summary>
        /// Sets 'logFileWriteFlag' to true
        /// </summary>
        private void StartLogFileRecording()
        {
            logFileWriteFlag = true;
        }

        /// <summary>
        /// Sets 'logFileWriteFlag' to false
        /// </summary>
        private void StopLogFileRecording()
        {
            logFileWriteFlag = false;
        }

        #endregion

        #region GUI_INTERFACE_FUNCTIONS

        /// <summary>
        /// Function called on every update call of the 'Update' timer
        /// </summary>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void Update_Tick(object sender, EventArgs e)
        {
            UpdateMainCOMConnectionStatus(); // Check if the main COM port is connected
            UpdateLogFileCOMConnectionStatus(); // Check if log file COM port is connected

            // Check if there is data in the textbox buffer
            if (MainCOMDataBuffer.Count > 0)
            {
                foreach (char c in MainCOMDataBuffer[0])
                {
                    if (c == '\n')
                    {
                        // Write newline to the textbox when '\n' char recieved
                        COMOut.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        // Write char to the textbox
                        COMOut.AppendText(c.ToString());
                    }
                }

                // Remove the most recent data from the buffer
                MainCOMDataBuffer.RemoveAt(0);
            }

            
            // Check if a minute has elapsed
            if (prevTime.Minute != DateTime.Now.Minute && !(DateTime.Now == AutoTurnOnTimePicker.Value && AutoTurnOnCheckBox.Checked))
            {
                // If main port is open, send a staus request
                if (MainCOMPort.IsOpen && !LogfileTransferTimeLabel.Visible)
                {
                    SendMainCOMCommand("STATUS=?");
                }

                // Set prevTime to current time
                prevTime = DateTime.Now;
            }

            // At the start of each day, clear the COM screen and the event log
            if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
            {
                ClearButton_Click(null, null);
                EventLogClearButton_Click(null, null);
            }

            ParseLastLine();

            DisplayLaserReady();

            AutoTurnOn();
        }

        /// <summary>
        /// Updates the list of available COMs and displays the list on the main COM port selector
        /// </summary>
        private void RefreshCOMSelector()
        {
            // When refresh button clicked, delete list of COM connections and create a new one

            RefreshCOM();

            MainCOMPortSelector.Items.Clear();

            foreach (var port in COMList)
            {
                MainCOMPortSelector.Items.Add(port);
            }
        }


        /// <summary>
        /// Function to be called when the COM refresh button is clicked
        /// </summary>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void COMRefreshButton_Click(object sender, EventArgs e)
        {
            RefreshCOMSelector();
        }

        /// <summary>
        /// Function to be called when the main COM connection button is pressed
        /// </summary>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void MainCOMConnectButton_Click(object sender, EventArgs e)
        {
            ConnectCOM(MainCOMPort, MainCOMPortSelector.Text);
        }

        private void SendMainCOMCommand(string command)
        {
            if (MainCOMPort.IsOpen)
            {
                // Send the command over the main COM port
                MainCOMPort.Write(command + "\r");

                LogCommandSent(command);
            }
        }

        /// <summary>
        /// Function to be called when the send command button is clicked
        /// </summary>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void CommandButton_Click(object sender, EventArgs e)
        {
            // Check if the main COM port is connected
            if (MainCOMPort.IsOpen)
            {
                // Clear any error messages
                ClearErrorLabelMessage();

                // If the "GETLOG=?" command is sent
                if (CommandComboBox.Text == "GETLOG=?")
                {
                    // If the log file COM port is not connected
                    if (!LogFileCOMPort.IsOpen)
                    {
                        // Stop the log file from recording
                        StopLogFileRecording();
                        // Display error message about log file COM port not being connected
                        SetErrorLabelMessage("Log file COM port not connected. Please press 'Auto-Connect'");
                        return;
                    }

                    // Bring up the save file dialog and if the result of the dialog is a failure
                    if (SetLogFileSavePath() == DialogResult.Cancel)
                    {
                        // Stop the log file from recording
                        StopLogFileRecording();
                        // Display error message about the save location not being chosen
                        SetErrorLabelMessage("Save file location not chosen");
                        return;
                    }

                    // Begin recording the log file COM input to the log file
                    StartLogFileRecording();
                }

                // Send the command over the main COM port
                SendMainCOMCommand(CommandComboBox.Text);
            }
            else
            {
                SetErrorLabelMessage("Main COM port not connected");
            }
        }

        /// <summary>
        /// Sets the error message text to be blank
        /// </summary>
        private void ClearErrorLabelMessage()
        {
            ErrorLabel.Text = "";
        }

        /// <summary>
        /// Display the given error message on the error message label. Displays in the format of "Error: 'message'"
        /// </summary>
        /// <param name="message">
        /// Error message to be displayed
        /// </param>
        private void SetErrorLabelMessage(string message)
        {
            ErrorLabel.Text = "Error: " + message;
        }

        /// <summary>
        /// Function called when the clear button is clicked
        /// </summary>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void ClearButton_Click(object sender, EventArgs e)
        {
            COMOut.Clear();
        }

        /// <summary>
        /// Function called when the log file auto-connect button is clicked. Iterates through all visible COM ports in an attempt to find the port the log file is sort over
        /// </summary>
        /// <param name="sender">
        /// Object which raised the event
        /// </param>
        /// <param name="e">
        /// Event which was raised
        /// </param>
        private void LogFileCOMPortAutoConnect_Click(object sender, EventArgs e)
        {
            // Clear any error messages
            ClearErrorLabelMessage();

            // Clear the temp COM buffer
            TempCOMDataBuffer.Clear();

            // Close any current connections to the log file COM port
            LogFileCOMPort.Close();

            // Refresh the list of visible COM ports
            RefreshCOM();

            // Initilise the auto-connect progress bar
            AutoConnectProgressBar.Value = 0; // Set initial bar value to 0
            AutoConnectProgressBar.Maximum = COMList.Length; // Set the max length of the auto-connect bar to be the number of visible COM ports
            AutoConnectProgressBar.Visible = true; // Set the progress bar to be visible

            // Iterate through all COM ports
            foreach (var com in COMList)
            {
                // Temporarily connect to the COM port
                ConnectCOM(TempCOMPort, com);

                // If the temporary COM port connected correctly
                if (TempCOMPort.IsOpen)
                {
                    // Send empty data
                    TempCOMPort.Write("\r");

                    int whileCounter = 0;

                    // Read recieved data

                    // Delay for a bit to allow for data to be sent and recieved
                    while (TempCOMDataBuffer.Count == 0 && whileCounter < Int32.MaxValue / 2)
                    {
                        whileCounter++;
                    }

                    // Close connection
                    TempCOMPort.Close();

                    // Check if ACK bit was recieved
                    if (TempCOMDataBuffer.Count > 0 && TempCOMDataBuffer[0][0] == 0x06)
                    {
                        // Connect the LogFileCOM port to the port
                        ConnectCOM(LogFileCOMPort, com);
                        // Update the auto-connect progress bar
                        AutoConnectProgressBar.Value = AutoConnectProgressBar.Maximum;
                        break;
                    }

                    // Clear temp data buffer
                    TempCOMDataBuffer.Clear();
                }

                // If the auto-connect progress bar's current value is less than its max value
                if (AutoConnectProgressBar.Value < AutoConnectProgressBar.Maximum)
                {
                    // Increase the auto-connect progress value
                    AutoConnectProgressBar.Value++;
                }
            }

            // If a connection to the log file COM port was unsuccessful
            if (!LogFileCOMPort.IsOpen)
            {
                // Display error message about the log file COM port not being found
                SetErrorLabelMessage("Could not find the log file COM port");
            }

            // Hide the auto-connect progress bar
            AutoConnectProgressBar.Visible = false;
        }

        /// <summary>
        /// Function to be called when the main COM disconnect button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainDisconnectButton_Click(object sender, EventArgs e)
        {
            DisconnectCOM(MainCOMPort);
        }

        /// <summary>
        ///  Function to be called when the log file COM disconnect button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogDisconnectButton_Click(object sender, EventArgs e)
        {
            DisconnectCOM(LogFileCOMPort);
        }

        /// <summary>
        /// Updates the text in the save location label. Displays the path of the current save file location if the save dialog worked and displays nothing otherwise
        /// </summary>
        private void UpdateSaveLocationLabel()
        {
            if (LogFileSaveDialog.FileName != "")
            {
                LogFileSavePathLabel.Text = "Log file will be saved to: '" + LogFileSaveDialog.FileName + "'";
            }
            else
            {
                LogFileSavePathLabel.Text = "";
            }
        }

        #endregion

        #region MESSAGE_PARSING_FUNCTIONS

        string GetTextBoxLastLine(TextBox textBox)
        {
            var lines = TakeLastLines(textBox.Text, 2);

            if (lines.Count == 0)
            {
                return "";
            }
            else
            {
                return lines[0];
            }
        }

        List<string> GetTextBoxLastLines(TextBox textBox, int lineCount)
        {
            var lines = TakeLastLines(textBox.Text, lineCount + 1);

            if (lines.Count == 0)
            {
                return null;
            }
            else
            {
                return lines;
            }
        }

        /// <summary>
        /// Retrieves the last n lines from a multiline string. Code taken from "https://stackoverflow.com/questions/11942885/take-the-last-n-lines-of-a-string-c-sharp"
        /// </summary>
        /// <param name="text"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        private static List<string> TakeLastLines(string text, int count)
        {
            List<string> lines = new List<string>();
            Match match = Regex.Match(text, "^.*$", RegexOptions.Multiline | RegexOptions.RightToLeft);

            while (match.Success && lines.Count < count)
            {
                lines.Insert(0, match.Value);
                match = match.NextMatch();
            }

            return lines;
        }

        void ParseLastLine()
        {
            // Read last line of COMOUT
            string lastLine = GetTextBoxLastLine(COMOut);

            DisplayLogFileTimeRemaining(lastLine);

            CheckStatus();
        }

        #region LOGFILE_TIME_ESTIMATOR

        int LogfileTransferSecondsRemaining(string logLine)
        {
            // Logline in format "LOGFILE N BYTES REMAINING"
            string pattern = @"\d+(?= BYTES REMAINING)";
            Match match = Regex.Match(logLine, pattern);

            int secondsRemaining = 0;

            if (match.Captures.Count == 1 && int.TryParse(match.Value, out int bytesRemaining))
            {
                secondsRemaining = bytesRemaining / 10000;
            }


            return secondsRemaining;
        }

        void DisplayLogFileTimeRemaining(string lastLine)
        {
            int secondsRemaining = LogfileTransferSecondsRemaining(lastLine);

            if (secondsRemaining == 0)
            {
                LogfileTransferTimeLabel.Hide();
            }
            else
            {
                LogfileTransferTimeLabel.Show();

                LogfileTransferTimeLabel.Text = String.Format("File Transfer Time Remaining:\n{0} Second(s)", secondsRemaining);
            }
        }

        #endregion

        #region STATUS_CHECKING

        #region LASER_READY_CHECKING

        void DisplayLaserReady()
        {
            var prevState = PrevLaserReady();

            if (IsLaserReady())
            {
                LaserReadyLabel.Text = "Laser Ready!";
                LaserReadyLabel.ForeColor = System.Drawing.Color.Green;

                if (!prevState)
                {
                    LogEvent("Laser ready");
                }
            }
            else
            {
                LaserReadyLabel.Text = "Laser Not Ready!";
                LaserReadyLabel.ForeColor = System.Drawing.Color.Red;

                if (prevState)
                {
                    LogEvent("Laser not ready");
                }
            }
        }

        bool PrevLaserReady()
        {
            if (LaserReadyLabel.Text == "Laser Ready!")
            {
                return true;
            }

            return false;
        }

        bool IsLaserReady()
        {
            // If |Power diff| < 10 and |CWL| < 10 and |FWHM| < 10 and Warm Up == NO
            var warmUpString = GetStatusCell("Warming Up", 1);
            var powerDiffString = GetStatusCell("Power", 3);
            var CWLDiffString = GetStatusCell("Center WL", 3);
            var FWHMDiffString = GetStatusCell("FWHM", 3);

            if (warmUpString == "NO" && StringAbsValueInLimits(powerDiffString, 10) && StringAbsValueInLimits(CWLDiffString, 10) && StringAbsValueInLimits(FWHMDiffString, 10))
            {
                return true;
            }

            return false;
        }

        bool StringAbsValueInLimits(string value, int threshold)
        {
            var numString = RegexSingleGroupMatch(value, @"(\d+)");
            if (numString != "")
            {
                int intValue = int.Parse(numString);

                if (intValue <= threshold)
                {
                    return true;
                }
            }

            return false;

        }

        string GetStatusCell(string property, int column)
        {
            // Get index
            var propertyIndex = GetStatusRowPropertyIndex(property);

            if (propertyIndex == -1)
            {
                return "";
            }

            return (string)StatusDisplay.Rows[propertyIndex].Cells[column].Value;
        }

        #endregion

        void CheckStatus()
        {
            var lastLines = GetTextBoxLastLines(COMOut, 50);

            foreach (var line in lastLines)
            {
                CheckStatusLine(line);
            }
        }

        void CheckStatusLine(string logLine)
        {
            WarmUpDisplay(logLine);
            ShutterDisplay(logLine);
            PowerDisplay(logLine);
            CenterWLDisplay(logLine);
            FWHMDisplay(logLine);
            QD1SUMDisplay(logLine);
            QD1XDisplay(logLine);
            QD1YDisplay(logLine);
            QD3SUMDisplay(logLine);
            QD3XDisplay(logLine);
            QD3YDisplay(logLine);
            USRITempDisplay(logLine);
            CAVITempDisplay(logLine);
            PUMPTempDisplay(logLine);
            DIAGTempDisplay(logLine);
        }

        int GetStatusRowPropertyIndex(string propertyString)
        {
            for (int i = 0; i < StatusDisplay.Rows.Count; i++)
            {
                if (StatusDisplay.Rows[i].Cells[0].Value == propertyString)
                {
                    return i;
                }
            }
            return -1;
        }

        void SetStatus(string property, string status)
        {
            if (status != "")
            {
                // Get index
                var propertyIndex = GetStatusRowPropertyIndex(property);

                if (propertyIndex == -1)
                {
                    return;
                }

                // Set status
                StatusDisplay.Rows[propertyIndex].Cells[1].Value = status;
            }
        }

        void SetStatus(string property, List<string> status)
        {
            if (status != null)
            {
                // Get index
                var propertyIndex = GetStatusRowPropertyIndex(property);

                if (propertyIndex == -1)
                {
                    return;
                }

                // Set status
                for (int i = 0; i < status.Count; i++)
                {
                    StatusDisplay.Rows[propertyIndex].Cells[i+1].Value = status[i];
                }
                
            }
        }

        string RegexSingleGroupMatch(string line, string regex)
        {
            if (line != null)
            {
                Match match = Regex.Match(line, regex);

                if (match.Groups.Count == 2)
                {
                    return match.Groups[1].Value;
                }
            }
            return "";
        }

        List<string> RegexMultiGroupMatch(string line, string regex, int groupCount)
        {
            if (line != null)
            {
                Match match = Regex.Match(line, regex);

                if (match.Groups.Count == groupCount + 1)
                {
                    var outStringList = new List<string>();

                    for (int i = 0; i < groupCount; i++)
                    {
                        outStringList.Add(match.Groups[i + 1].Value);
                    }

                    return outStringList;
                }
            }
            return null;
        }

        void WarmUpDisplay(string logLine)
        {
            var status = RegexSingleGroupMatch(logLine, @"WARM UP;(\w+);");

            SetStatus("Warming Up", status);
        }

        void ShutterDisplay(string logLine)
        {
            var status = RegexSingleGroupMatch(logLine, @"SHUTTER;(\w+);");

            SetStatus("Shutter", status);
        }

        void PowerDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"P 800 CAL;\s*(-?\d+)\s*mW;\s*(-?\d+)\s*mW;\s*(-?\d+)\s*mW;", 3);

            SetStatus("Power", status);
        }

        void CenterWLDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"CENTER WL;\s*(-?\d+)\s*nm;\s*(-?\d+)\s*nm;\s*(-?\d+)\s*nm;", 3);

            SetStatus("Center WL", status);
        }

        void FWHMDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"FWHM  800;\s*(-?\d+)\s*nm;\s*(-?\d+)\s*nm;\s*(-?\d+)\s*nm;", 3);

            SetStatus("FWHM", status);
        }

        void QD1SUMDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  1  SUM;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (532 nm) SUM", status);
        }

        void QD1XDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  1  X;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (532 nm) X", status);
        }

        void QD1YDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  1  Y;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (532 nm) Y", status);
        }

        void QD3SUMDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  3  SUM;\s*(-?\d+)\s*;\s*(-?\d+)\s*;\s*(-?\d+)\s*;", 3);

            SetStatus("4QD (800 nm) SUM", status);
        }

        void QD3XDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  3  X;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (800 nm) X", status);
        }

        void QD3YDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  3  Y;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (800 nm) Y", status);
        }

        void USRITempDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"TEMP USRI;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;", 3);

            SetStatus("User Interface Temp", status);
        }

        void CAVITempDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"TEMP CAVI;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;", 3);

            SetStatus("Cavity Temp", status);
        }

        void PUMPTempDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"TEMP PUMP;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;", 3);

            SetStatus("Pump Laser Temp", status);
        }

        void DIAGTempDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"TEMP DIAG;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;", 3);

            SetStatus("Diagnostics Temp", status);
        }

        #endregion

        #endregion

        #region AUTO_TURN_ON_FUNCTIONS

        private void AutoTurnOnTimePicker_ValueChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(AutoTurnOnTimePicker.Value.TimeOfDay.ToString());
            //AutoTurnOnTimePicker.Value.Second = 0;
        }

        private void AutoTurnOnCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (!supressAutoTurnOnCheckBoxMessage)
            {
                // Clear any error messages
                ClearErrorLabelMessage();

                if (AutoTurnOnCheckBox.Checked)
                {
                    
                    if (!MainCOMPort.IsOpen)
                    {
                        SetErrorLabelMessage("Main COM port not connected");
                        supressAutoTurnOnCheckBoxMessage = true;
                        AutoTurnOnCheckBox.Checked = false;
                        return;
                    }
                    

                    LogEvent("Element set to turn on at " + AutoTurnOnTimePicker.Value.ToShortTimeString());
                    //AutoTurnOnTimePicker.Hide();
                    AutoTurnOnTimePicker.Enabled = false;
                    MessageBox.Show("The Element will automatically be turned on at " + AutoTurnOnTimePicker.Value.ToShortTimeString());
                }
                else
                {
                    LogEvent("Element auto turn on unset");
                    //AutoTurnOnTimePicker.Show();
                    AutoTurnOnTimePicker.Enabled = true;
                    MessageBox.Show("The Element will not automatically be turned on");
                }
            }
            else
            {
                supressAutoTurnOnCheckBoxMessage = false;
            }

            autoTurnOnSentFlag = false;
        }

        private bool AutoTurnOnChecker()
        {
            if (AutoTurnOnCheckBox.Checked)
            {
                var curTime = DateTime.Now.ToShortTimeString();
                var autoTurnOnTime = AutoTurnOnTimePicker.Value.ToShortTimeString();

                if (curTime == autoTurnOnTime)
                {
                    AutoTurnOnTimePicker.Enabled = true;
                    //AutoTurnOnTimePicker.Show();
                    return true;
                }
            }

            return false;
        }

        private void SendTurnOnCommand()
        {
            if (MainCOMPort.IsOpen)
            {
                // Send the command over the main COM port
                MainCOMPort.Write("STARTLSR=1\r");
            }
        }

        private void AutoTurnOn()
        {
            var turnOn = AutoTurnOnChecker();

            if (!autoTurnOnSentFlag && turnOn)
            {
                supressAutoTurnOnCheckBoxMessage = true;

                if (MainCOMPort.IsOpen)
                {
                    SendTurnOnCommand();

                    autoTurnOnSentFlag = true;

                    AutoTurnOnCheckBox.Checked = false;

                    LogEvent("Auto turn on request sent");
                    MessageBox.Show("The Element was automatically turned on at " + DateTime.Now.ToShortTimeString());
                }
                else
                {
                    SetErrorLabelMessage("Main COM port not connected");
                    LogEvent("Auto turn on failed (Main COM port not connected)");
                    AutoTurnOnCheckBox.Checked = false;
                }
            }
        }

        #endregion

        #region EVENT_LOG_FUNCTIONS

        #region EVENT_LOG_FILE_OUT

        void AppendLogToFile(string eventString)
        {
            // Get current filename (based on current date)
            string filename = "./EventLogFiles/ElementCOMGUILog-" + DateTime.Now.ToString("yy_MM_dd") + ".csv";

            // Check if log file directory exists
            if (!Directory.Exists("./EventLogFiles"))
            {
                // Create LogFiles directory
                Directory.CreateDirectory("./EventLogFiles");
            }

            // Check if file exists
            if (!File.Exists(filename))
            {
                // Create new file and close file
                File.Create(filename).Dispose();
            }


            // Try to append to file
            try
            {
                using (StreamWriter sw = File.AppendText(filename))
                {
                    sw.WriteLine(DateTime.Now.ToString("HH:mm:ss") + "," + eventString);
                }
            }
            catch { }
            
        }

        private void OpenEventLogFolderButton_Click(object sender, EventArgs e)
        {
            // Check if log file directory exists
            if (!Directory.Exists("./EventLogFiles"))
            {
                // Create LogFiles directory
                Directory.CreateDirectory("./EventLogFiles");
            }
            
            // Open folder in file explorer
            Process.Start(@".\EventLogFiles");
        }

        #endregion

        void LogCommandSent(string commandString)
        {
            switch (commandString)
            {
                case "STATUS=?":
                    LogEvent("Status request sent");
                    break;
                case "STARTLSR=1":
                    LogEvent("Laser turn on request sent");
                    break;
                case "SHUTTER=1":
                    LogEvent("Shutter opened");
                    break;
                case "SHUTTER=0":
                    LogEvent("Shutter closed");
                    break;
                case "STOPLSR=1":
                    LogEvent("Laser turn off request sent");
                    break;
                case "GETLOG=?":
                    LogEvent("Log file requested");
                    break;
                case "RESETLOG=1":
                    LogEvent("Log reset request sent");
                    break;
                default:
                    break;
            }
        }

        void LogEvent(string eventString)
        {
            // Add event to GUI event log
            EventLog.Rows.Add(String.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), eventString);
            // Add event to file event log
            AppendLogToFile(eventString);
        }


        private void EventLogClearButton_Click(object sender, EventArgs e)
        {
            // Clear the event log table
            EventLog.Rows.Clear();
        }

        #endregion


    }
}
