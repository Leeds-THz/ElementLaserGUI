using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace ElementLaserGUI
{
    public partial class Form1 : Form
    {
		#region VARIABLES

		/// <summary>
		/// Array to store names of all COM ports discovered
		/// </summary>
		private string[] COMList;

		/// <summary>
		/// List to store data recieved from the main COM connection
		/// </summary>
		private static List<string> MainCOMDataBuffer = new List<string>();

		/// <summary>
		/// List to store data recieved from the temp COM connection
		/// </summary>
		private static List<string> TempCOMDataBuffer = new List<string>();

		/// <summary>
		/// Stores the previous time the 'UpdateTick' function was called
		/// </summary>
        private DateTime prevTime = DateTime.Now;

		#region FLAGS

		/// <summary>
		/// Flag set high when log file is being written to
		/// </summary>
		private bool logFileWriteFlag = false;

		/// <summary>
		/// Flag set high when auto turn on has been requested
		/// </summary>
		private bool autoTurnOnSentFlag = false;

		/// <summary>
		/// Flag set high when the program is altering the state of the auto turn on check box
		/// </summary>
		private bool supressAutoTurnOnCheckBoxMessage = false;

		/// <summary>
		/// Flag set high when an auto turn on is performed. Flag set to low when auto turn on dialog is closed.
		/// Used as a safety feature to automatically turn the laser off if there is no user input within a set amount of time.
		/// </summary>
		private bool AutoTurnOffFlag = false;

		#endregion

		#endregion

		#region CONSTRUCTOR

		/// <summary>
		/// Form Constructor
		/// </summary>
		public Form1()
        {
            InitializeComponent(); // Initialised the application
            RefreshCOMSelector(); // Get the discoverable COM ports on start-up
			StatusDataGridInit(); // Inits the field shown in the status data grid
		}

		#endregion

		#region STATUS_DATAGRID_INIT_FUNCTIONS

		/// <summary>
		/// Sets the intial values of each row in the status data grid
		/// NOTE: In previous versions this code was in 'Form1.Designer.cs' in auto-generated code, but it kept getting deleted.
		/// </summary>
		void StatusDataGridInit()
		{
			///
			/// Status rows
			/// 
			StatusDisplay.Rows.Add("Warming Up");
			StatusDisplay.Rows.Add("Shutter");
			StatusDisplay.Rows.Add("Power");
			StatusDisplay.Rows.Add("Center WL");
			StatusDisplay.Rows.Add("FWHM");
			StatusDisplay.Rows.Add("User Interface Temp");
			StatusDisplay.Rows.Add("Cavity Temp");
			StatusDisplay.Rows.Add("Pump Laser Temp");
			StatusDisplay.Rows.Add("Diagnostics Temp");
			StatusDisplay.Rows.Add("4QD (532 nm) SUM");
			StatusDisplay.Rows.Add("4QD (532 nm) X");
			StatusDisplay.Rows.Add("4QD (532 nm) Y");
			StatusDisplay.Rows.Add("4QD (800 nm) SUM");
			StatusDisplay.Rows.Add("4QD (800 nm) X");
			StatusDisplay.Rows.Add("4QD (800 nm) Y");
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
            if (DateTime.Now.Second == 5 && prevTime.Minute != DateTime.Now.Minute && !(DateTime.Now == AutoTurnOnTimePicker.Value && AutoTurnOnCheckBox.Checked))
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
			
			// Read
            ParseLastLine();

            DisplayLaserReady();

            AutoTurnOn();

			AutoTurnOffChecker();

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

		/// <summary>
		/// Acquires the string of the last line of a given 'TextBox'
		/// </summary>
		/// <param name="textBox">
		/// 'TextBox' to acquire the last line from
		/// </param>
		/// <returns>
		/// String containing the last line of the text box
		/// If the text box is empty, an empty string is returned
		/// </returns>
        string GetTextBoxLastLine(TextBox textBox)
        {
            var lines = TakeLastLines(textBox.Text, 2);

			// If the text box is empty, return an empty string
            if (lines.Count == 0)
            {
                return "";
            }
			// Else return the last line
            else
            {
                return lines[0];
            }
        }

		/// <summary>
		/// Acquires the last 'lineCount' number of lines from the given 'TextBox'
		/// </summary>
		/// <param name="textBox">
		/// 'TextBox' to acquire lines from
		/// </param>
		/// <param name="lineCount">
		/// Number of lines (from the end) to acquire
		/// </param>
		/// <returns>
		/// A list of strings. Each item in the list is a different line from the text box
		/// If the text box is empty, 'null' is returned
		/// </returns>
        List<string> GetTextBoxLastLines(TextBox textBox, int lineCount)
        {
            var lines = TakeLastLines(textBox.Text, lineCount + 1);

			// If the text box is empty, null is returned
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
        /// <param name="text">
		/// Multiline string to be parsed
		/// </param>
        /// <param name="count">
		/// Number of lines to acquire (begining at the end of the string)
		/// </param>
        /// <returns>
		/// A list of strings containing the last 'count' number of lines from the end of the string. Each element of the list is a separate line.
		/// If the text is empty, an empty list is returned.
		/// </returns>
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
		
		/// <summary>
		/// Reads the last line of the COM output and update the display based on the text
		/// </summary>
        void ParseLastLine()
        {
            // Read last line of the COM output textbox
            string lastLine = GetTextBoxLastLine(COMOut);

			// Display the logfile acquisition time remaining (will only display if correct text is received)
			DisplayLogFileTimeRemaining(lastLine);

            CheckStatus();
        }

        #region LOGFILE_TIME_ESTIMATOR
		
		/// <summary>
		/// Parses the given string 'logLine' and finds out how many seconds remain of the given request.
		/// Used when laser log file is acquired.
		/// </summary>
		/// <param name="logLine">
		/// String to be parsed
		/// </param>
		/// <returns>
		/// Number of seconds remaining, as parsed by 'logLine'
		/// </returns>
        int LogfileTransferSecondsRemaining(string logLine)
        {
            // Logline in format "LOGFILE N BYTES REMAINING"
            string pattern = @"\d+(?= BYTES REMAINING)";
            Match match = Regex.Match(logLine, pattern); // Regex using the pattern above to extract the number of bytes remaining

            int secondsRemaining = 0;
			
			// Make sure only 1 number was extracted and then convert the number string into an int
            if (match.Captures.Count == 1 && int.TryParse(match.Value, out int bytesRemaining))
            {
				// Rough calculation to convert bytes remaining into seconds (based on the baud rate I think?)
                secondsRemaining = bytesRemaining / 10000;
            }

			// Return the number of seconds left
            return secondsRemaining;
        }
		
		/// <summary>
		/// Display the text showing the time remaining for the log file acquisition
		/// </summary>
		/// <param name="lastLine">
		/// String of the last line from the COM output
		/// </param>
        void DisplayLogFileTimeRemaining(string lastLine)
        {
			// Convert the "LOGFILE N BYTES REMAINING" string into seconds remaining
			int secondsRemaining = LogfileTransferSecondsRemaining(lastLine);
			
			// If the action has finished (0 seconds remaining), hide the text
            if (secondsRemaining == 0)
            {
                LogfileTransferTimeLabel.Hide();
            }
			// Else show the time remaining
            else
            {
                LogfileTransferTimeLabel.Show(); // Unhide text

                LogfileTransferTimeLabel.Text = String.Format("File Transfer Time Remaining:\n{0} Second(s)", secondsRemaining); // Set text to be displayed
            }
        }

        #endregion

        #region STATUS_CHECKING

        #region LASER_READY_CHECKING

		/// <summary>
		/// Updates the text label indicating if the laser is ready or not.
		/// </summary>
        void DisplayLaserReady()
        {
			// Check the state of the laser from the last check
            var prevState = PrevLaserReady();

			// If the laser is ready
            if (IsLaserReady())
            {
				// Update the label text
                LaserReadyLabel.Text = "Laser Ready!";
				// Update the label text colour
                LaserReadyLabel.ForeColor = System.Drawing.Color.Green;

				// If there has been a change of state since the last check
                if (!prevState)
                {
					// Log the event
                    LogEvent("Laser ready");
                }
            }
			// Laser is not ready
            else
            {
				// Update the label text
				LaserReadyLabel.Text = "Laser Not Ready!";
				// Update the label text colour
				LaserReadyLabel.ForeColor = System.Drawing.Color.Red;

				// If there has been a change of state since the last check
				if (prevState)
                {
					// Log the event
					LogEvent("Laser not ready");
                }
            }
        }

		/// <summary>
		/// Checks the state of the laser as acquired by the last check
		/// This is done by reading the text of the 'LaserReadyLabel'
		/// </summary>
		/// <returns>
		/// True if the laser was ready during the last check
		/// False if the laser was not ready during the last check
		/// </returns>
        bool PrevLaserReady()
        {
			return (LaserReadyLabel.Text == "Laser Ready!");
        }

		/// <summary>
		/// Checks if the laser is ready
		/// All information is acquired through the element status get command
		/// The laser is defined to be ready under the following conditions:
		///		- Warming Up = NO
		///		- Power is +/- 10mW off the target power
		///		- Center Wavelength is +/- 10nm off the target wavelength
		///		- FWHM is +/- 10nm off the target FWHM
		/// </summary>
		/// <returns>
		/// True if the laser is determined to be ready, false otherwise
		/// </returns>
        bool IsLaserReady()
        {
            // If |Power diff| < 10 and |CWL| < 10 and |FWHM| < 10 and Warm Up == NO
            var warmUpString = GetStatusCell("Warming Up", 1);
            //var powerDiffString = GetStatusCell("Power", 3);
			//var powerString = GetStatusCell("Power", 1);
			var CWLDiffString = GetStatusCell("Center WL", 3);
            var FWHMDiffString = GetStatusCell("FWHM", 3);

			int powerVal = 0;

			try
			{
				powerVal = Int32.Parse(GetStatusCell("Power", 1));
			}
			catch (Exception)
			{
			}

			/*
            if (warmUpString == "NO" && StringAbsValueInLimits(powerDiffString, 10) && StringAbsValueInLimits(CWLDiffString, 10) && StringAbsValueInLimits(FWHMDiffString, 10))
            {
                return true;
            }

            return false;
			*/

			return (warmUpString == "NO" && powerVal >= LaserReadyPowerThreshNumUpDown.Value && StringAbsValueInLimits(CWLDiffString, 10) && StringAbsValueInLimits(FWHMDiffString, 10));
        }

		/// <summary>
		/// Acquires a number from a string and checks its absolute value is lower than the given 'threshold'
		/// Absolute value acquired by ignoring any sign infront of the number string
		/// </summary>
		/// <param name="value">
		/// String containing the number to be checked
		/// </param>
		/// <param name="threshold">
		/// Maximum value |'value'| can be for the function to return true
		/// </param>
		/// <returns>
		/// True if threshold >= |'value'| 
		/// </returns>
		bool StringAbsValueInLimits(string value, int threshold)
        {
			// Acquire the number from the string, ignoring the sign
            var numString = RegexSingleGroupMatch(value, @"(\d+)");

			// If a number was acquired
            if (numString != "")
            {
				// Convert it to an integer
                int intValue = int.Parse(numString);


				/*
                if (intValue <= threshold)
                {
                    return true;
                }
				*/

				// Check that the value is less than or equal to the threshold
				return (intValue <= threshold);
            }

            return false;
        }
		
		/// <summary>
		/// Gets the status of the given property and the given column
		/// </summary>
		/// <param name="property">
		/// Property to get the value from
		/// </param>
		/// <param name="column">
		/// Column to get the value from
		/// </param>
		/// <returns>
		/// Gets the string from the laser status table from the given column of the given property
		/// </returns>
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
		
		/// <summary>
		/// Read the last 50 COM output lines recieved and update the laser status accordingly
		/// </summary>
        void CheckStatus()
        {
			// Get the last 50 lines from the COM output
            var lastLines = GetTextBoxLastLines(COMOut, 50);
			
			// Iterate through each line
            foreach (var line in lastLines)
            {
				// Parse the line and update the relevant field
                CheckStatusLine(line);
            }
        }
		
		/// <summary>
		/// Parse the given line and update the relevant field
		/// </summary>
		/// <param name="logLine">
		/// String to be parsed
		/// </param>
        void CheckStatusLine(string logLine)
        {
			// NOTE: Could probably speed things up here be returning from the function if a Regex was successful. 
			// Would require all functions to return a bool representing if the field was updated or not and a few if statements.
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
		
		/// <summary>
		/// Get the index of the property named 'propertyString' in 'StatusDisplay' (the table showing the laser status).
		/// Used when setting the value of a property in the table.
		/// </summary>
		/// <param name="propertyString">
		/// Property to find the index of
		/// </param>
		/// <returns>
		/// Index of the property 'propertyString' in the table 'StatusDisplays'. If the property is not found, -1 is returned.
		/// </returns>
        int GetStatusRowPropertyIndex(string propertyString)
        {
			// Iterate through the table
            for (int i = 0; i < StatusDisplay.Rows.Count; i++)
            {
				// If the property matches the property we are looking for
                if (StatusDisplay.Rows[i].Cells[0].Value == propertyString)
                {
					// Return the index of the property (current index)
                    return i;
                }
            }

			// Property was not found, return -1
            return -1;
        }

		/// <summary>
		/// Sets the status of the given property in the laser status table
		/// </summary>
		/// <param name="property">
		/// Property to be updated (e.g. element power)
		/// </param>
		/// <param name="status">
		/// Value to assign to the property (e.g. 720mW)
		/// </param>
        void SetStatus(string property, string status)
        {
			// If a valid status string is given
            if (status != "")
            {
                // Get index
                var propertyIndex = GetStatusRowPropertyIndex(property);
				
				// If property could not be found, return
                if (propertyIndex == -1)
                {
                    return;
                }

                // Set status
                StatusDisplay.Rows[propertyIndex].Cells[1].Value = status;
            }
        }
		
		/// <summary>
		/// Sets a list of statuses to the given property
		/// </summary>
		/// <param name="property">
		/// Property to be updated
		/// </param>
		/// <param name="status">
		/// Values to be assigned to the property
		/// </param>
        void SetStatus(string property, List<string> status)
        {
			// Check if a valid list of status have been given
            if (status != null)
            {
                // Get index
                var propertyIndex = GetStatusRowPropertyIndex(property);
				
				// If property could not be found, return
				if (propertyIndex == -1)
                {
                    return;
                }

                // Set each status in the status list
                for (int i = 0; i < status.Count; i++)
                {
                    StatusDisplay.Rows[propertyIndex].Cells[i+1].Value = status[i];
                }
                
            }
        }
		
		/// <summary>
		/// Acquires a single group from a regex match
		/// </summary>
		/// <param name="line">
		/// Line to be parsed
		/// </param>
		/// <param name="regex">
		/// Regex equation to apply to the line
		/// </param>
		/// <returns>
		/// A string of the required regex group
		/// </returns>
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
		
		/// <summary>
		/// Acquires 'groupCount' groups from a regex match
		/// </summary>
		/// <param name="line">
		/// Line to be parsed
		/// </param>
		/// <param name="regex">
		/// Regex equation to apply to the line
		/// </param>
		/// <param name="groupCount">
		/// Number of groups to acquire
		/// </param>
		/// <returns>
		/// List of strings, each index being a separate group acquired by the regex function
		/// </returns>
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
		
		/// <summary>
		/// Attempt to parse the 'WARM UP' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
        void WarmUpDisplay(string logLine)
        {
            var status = RegexSingleGroupMatch(logLine, @"WARM UP;(\w+);");

            SetStatus("Warming Up", status);
        }

		/// <summary>
		/// Attempt to parse the 'SHUTTER' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>		
		void ShutterDisplay(string logLine)
        {
            var status = RegexSingleGroupMatch(logLine, @"SHUTTER;(\w+);");

            SetStatus("Shutter", status);
        }

		/// <summary>
		/// Attempt to parse the 'P 800 CAL' status (element power)
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void PowerDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"P 800 CAL;\s*(-?\d+)\s*mW;\s*(-?\d+)\s*mW;\s*(-?\d+)\s*mW;", 3);

            SetStatus("Power", status);
        }

		/// <summary>
		/// Attempt to parse the 'CENTER WL' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void CenterWLDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"CENTER WL;\s*(-?\d+)\s*nm;\s*(-?\d+)\s*nm;\s*(-?\d+)\s*nm;", 3);

            SetStatus("Center WL", status);
        }

		/// <summary>
		/// Attempt to parse the 'FWHM' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void FWHMDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"FWHM  800;\s*(-?\d+)\s*nm;\s*(-?\d+)\s*nm;\s*(-?\d+)\s*nm;", 3);

            SetStatus("FWHM", status);
        }

		/// <summary>
		/// Attempt to parse the '4QD 1 SUM' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void QD1SUMDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  1  SUM;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (532 nm) SUM", status);
        }

		/// <summary>
		/// Attempt to parse the '4 QD 1 X' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void QD1XDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  1  X;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (532 nm) X", status);
        }

		/// <summary>
		/// Attempt to parse the '4 QD 1 Y' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void QD1YDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  1  Y;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (532 nm) Y", status);
        }

		/// <summary>
		/// Attempt to parse the '4 QD 3 SUM' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void QD3SUMDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  3  SUM;\s*(-?\d+)\s*;\s*(-?\d+)\s*;\s*(-?\d+)\s*;", 3);

            SetStatus("4QD (800 nm) SUM", status);
        }

		/// <summary>
		/// Attempt to parse the '4 QD 3 X' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void QD3XDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  3  X;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (800 nm) X", status);
        }

		/// <summary>
		/// Attempt to parse the '4 QD 3 Y' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void QD3YDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"4QD  3  Y;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;\s*(-?\+?\d+)\s*;", 3);

            SetStatus("4QD (800 nm) Y", status);
        }

		/// <summary>
		/// Attempt to parse the 'TEMP USRI' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void USRITempDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"TEMP USRI;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;", 3);

            SetStatus("User Interface Temp", status);
        }

		/// <summary>
		/// Attempt to parse the 'TEMP CAVI' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void CAVITempDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"TEMP CAVI;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;", 3);

            SetStatus("Cavity Temp", status);
        }

		/// <summary>
		/// Attempt to parse the 'TEMP PUMP' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void PUMPTempDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"TEMP PUMP;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;", 3);

            SetStatus("Pump Laser Temp", status);
        }

		/// <summary>
		/// Attempt to parse the 'TEMP DIAG' status
		/// </summary>
		/// <param name="logLine">
		/// Log line to be parsed
		/// </param>
		void DIAGTempDisplay(string logLine)
        {
            var status = RegexMultiGroupMatch(logLine, @"TEMP DIAG;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;\s*(-?\+?\d+.?\d*)\s*;", 3);

            SetStatus("Diagnostics Temp", status);
        }

		#endregion

		#endregion

		#region AUTO_TURN_OFF_FUNCTIONS

		private void AutoTurnOffChecker()
		{
			// Check if the turn off flag is set high
			if (AutoTurnOffFlag)
			{
				// Get the current time and date strings
				var curTimeStr = DateTime.Now.ToShortTimeString();
				var curDateStr = DateTime.Now.ToShortDateString();

				// Get the auto turn on date and time
				var autoTurnOnTime = AutoTurnOnTimePicker.Value;
				//var autoTurnOffTimeStr = autoTurnOnTime.AddMinutes((double)AutoTurnOffWaitNumericUpDown.Value).ToShortTimeString();
				var autoTurnOffTimeStr = autoTurnOnTime.AddHours((double)AutoTurnOffWaitNumericUpDown.Value).ToShortTimeString();
				var autoTurnOffDateStr = AutoTurnOnDatePicker.Value.ToShortDateString();

				// Check if the current DateTime is the same as the the turn off DateTime
				if (curDateStr == autoTurnOffDateStr && curTimeStr == autoTurnOffTimeStr)
				{
					// Check if the main com port is open
					if (!MainCOMPort.IsOpen)
					{
						// Display an error message
						SetErrorLabelMessage("Main COM port not connected");
						return;
					}

					// Reset the turn off flag
					AutoTurnOffFlag = false;

					// Send a turn off command
					MainCOMPort.Write("STOPLSR=1\r");

					// Log that an auto turn off request has been sent
					LogEvent("Auto turn off request sent (Auto turn on timed out)");

					// Display a message stating the laser was automatically turned off
					MessageBox.Show("The Element was automatically turned off at " + curTimeStr + " " + curDateStr + "\n(Auto turn on timed out)", "Auto Turn-Off Occurred", MessageBoxButtons.OK, MessageBoxIcon.Stop);
				}
			}
		}

		#endregion

		#region AUTO_TURN_ON_FUNCTIONS

		/// <summary>
		/// Function called when the value of the auto turn on picker changes
		/// Function currently does nothing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void AutoTurnOnTimePicker_ValueChanged(object sender, EventArgs e)
        {
            //MessageBox.Show(AutoTurnOnTimePicker.Value.TimeOfDay.ToString());
            //AutoTurnOnTimePicker.Value.Second = 0;
        }

		/// <summary>
		/// Function called when the auto turn on check box value is changed
		/// If the box is ticked, the auto turn on time picker will become locked and the laser will be set to automatically turn on at the given time
		/// If the box is unticked, the laser auto turn on will be disabled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void AutoTurnOnCheckBox_CheckedChanged(object sender, EventArgs e)
        {
			// If the user is interacting with the check box
			//		(Check is perfomed using the 'supressAutoTurnOnCheckBoxMessage' flag. It prevents the auto turn on enabled/disabled popups from appearing when the program is automatically enabling/disabling the check box)
			if (!supressAutoTurnOnCheckBoxMessage)
            {
                // Clear any error messages
                ClearErrorLabelMessage();

				// If the box is checked
                if (AutoTurnOnCheckBox.Checked)
                {
                    // If there isn't a COM connection to the laser
                    if (!MainCOMPort.IsOpen)
                    {
						// Display an error message
                        SetErrorLabelMessage("Main COM port not connected");
						// Disable auto turn on pop up
                        supressAutoTurnOnCheckBoxMessage = true;
						// Disable the check box
                        AutoTurnOnCheckBox.Checked = false;
                        return;
                    }

                    // Check if the time seleced is in the past
                    // Construct DateTime object for the auto turn on time
                    string autoTurnOnTimeString = AutoTurnOnTimePicker.Value.ToShortTimeString() + " " + AutoTurnOnDatePicker.Value.ToShortDateString();
                    DateTime autoTurnOnDateTime = DateTime.Parse(autoTurnOnTimeString);

                    if (DateTime.Now.CompareTo(autoTurnOnDateTime) > 0)
                    {
                        // Display an error message
                        SetErrorLabelMessage("Cannot select an auto-turn on time in the past");
                        // Disable auto turn on pop up
                        supressAutoTurnOnCheckBoxMessage = true;
                        // Disable the check box
                        AutoTurnOnCheckBox.Checked = false;
                        return;
                    }

                    // Log the auto turn on being set as an event
                    LogEvent("Element set to turn on at " + AutoTurnOnTimePicker.Value.ToShortTimeString() + " " + AutoTurnOnDatePicker.Value.ToShortDateString());
					// Disable user control to the time and date pickers (done to prevent accidently changing the auto turn on time after it is set)
                    AutoTurnOnTimePicker.Enabled = false;
					AutoTurnOnDatePicker.Enabled = false;
					AutoTurnOffWaitNumericUpDown.Enabled = false;
					// Show a pop up box stating the auto turn on time
					MessageBox.Show("The Element will automatically be turned on at " + AutoTurnOnTimePicker.Value.ToShortTimeString() + " " + AutoTurnOnDatePicker.Value.ToShortDateString(), "Auto Turn-On Enabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
				// Box is unchecked
                else
                {
					// Log the auto turn on being unset as an event
                    LogEvent("Element auto turn on unset");
					// Allow for the user to change the auto turn on time and date using the pickers
                    AutoTurnOnTimePicker.Enabled = true;
					AutoTurnOnDatePicker.Enabled = true;
					AutoTurnOffWaitNumericUpDown.Enabled = true;
					// Show a pop up box stating that ther auto turn on has been unset
					MessageBox.Show("The Element will not automatically be turned on", "Auto Turn-On Disabled", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
			// If the code reaches here, the program has automatically change the check box state
            else
            {
				// Disable the popup flag
                supressAutoTurnOnCheckBoxMessage = false;
            }

			// Reset the auto turn on flag
            autoTurnOnSentFlag = false;
        }

		/// <summary>
		/// Checks if an auto turn on should be performed
		///	Auto turn on is performed if the auto turn box is set and the current 'DateTime' matches the 'DateTime' of the auto turn on picker
		/// </summary>
		/// <returns>
		/// True if an auto turn on should be performed, false otherwise
		/// </returns>
        private bool AutoTurnOnChecker()
        {
			// If the auto turn on is enabled
            if (AutoTurnOnCheckBox.Checked)
            {
				// Get the current time and date strings
                var curTime = DateTime.Now.ToShortTimeString();
				var curDate = DateTime.Now.ToShortDateString();

				// Get the auto turn on date and time
                var autoTurnOnTime = AutoTurnOnTimePicker.Value.ToShortTimeString();
				var autoTurnOnDate = AutoTurnOnDatePicker.Value.ToShortDateString();

                if (curDate == autoTurnOnDate && curTime == autoTurnOnTime)
                {
                    AutoTurnOnTimePicker.Enabled = true;
					AutoTurnOnDatePicker.Enabled = true;
					AutoTurnOffWaitNumericUpDown.Enabled = true;
                    return true;
                }
            }

            return false;
        }

		/// <summary>
		/// Attempts to send a turn on command to the laser
		/// </summary>
        private void SendTurnOnCommand()
        {
			// If there is a COM connection to the laser
            if (MainCOMPort.IsOpen)
            {
                // Send the command over the main COM port
                MainCOMPort.Write("STARTLSR=1\r");
            }
        }

		/// <summary>
		/// Performs an auto turn on if one should be performed 
		/// </summary>
        private void AutoTurnOn()
        {
			// Check if a auto turn on should be performed
            var turnOn = AutoTurnOnChecker();

			// If an auto turn on has not already been requested and an auto turn on should be perfomed
            if (!autoTurnOnSentFlag && turnOn)
            {
				// Supress check box change messages
                supressAutoTurnOnCheckBoxMessage = true;

				// If there is a COM connection to the laser
                if (MainCOMPort.IsOpen)
                {
					// Send a turn on command
                    SendTurnOnCommand();

					// Set the auto turn on flag to high, as a auto turn on has been requested
                    autoTurnOnSentFlag = true;

					// Disable the auto turn on check box
                    AutoTurnOnCheckBox.Checked = false;

					// Set the laser turn off flag to high
					AutoTurnOffFlag = true;

					// Log that an auto turn on request has been sent
					LogEvent("Auto turn on request sent");

					// Display a text box stating that the element has been automatically turned on
					//MessageBox.Show("The Element was automatically turned on at " + DateTime.Now.ToShortTimeString());
					var result = MessageBox.Show("The Element was automatically turned on at " + DateTime.Now.ToShortTimeString() + "\nPress 'OK' to prevent auto turn off before " + DateTime.Now.AddHours((double)AutoTurnOffWaitNumericUpDown.Value).ToShortTimeString(), "Auto Turn-On Success", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

					if (result == DialogResult.OK)
					{
						AutoTurnOffFlag = false;
					}

				}
				// There is no COM connection to the laser
                else
                {
					// Display an error message
                    SetErrorLabelMessage("Main COM port not connected");
					// Log that the auto turn on failed
					LogEvent("Auto turn on failed (Main COM port not connected)");
					// Disable the check box
					AutoTurnOnCheckBox.Checked = false;
                }
            }
        }

        #endregion

        #region EVENT_LOG_FUNCTIONS

        #region EVENT_LOG_FILE_OUT

		/// <summary>
		/// Appends the event to the current log file
		/// New log file generated each day
		/// </summary>
		/// <param name="eventString">
		/// Text to be added to the log file
		/// </param>
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

		/// <summary>
		/// Opens the directory containing the log files in windows file explorer when the 'OpenEventLogFolder' button is pressed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
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

		/// <summary>
		/// Logs the given event to the log file and to the GUI
		/// Status acquisition commands are hidden in the GUI. This is so it doesn't get clogged up by the automatic status requests sent every minute
		/// </summary>
		/// <param name="commandString">
		/// Command to be logged
		/// </param>
        void LogCommandSent(string commandString)
        {
            switch (commandString)
            {
                case "STATUS=?":
                    LogEvent("Status request sent", false); // Hides the event in the GUI to prevent visual clogging
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

		/// <summary>
		/// Logs the given event string in the log file and (if 'showInGUI' is true) in the GUI event log
		/// </summary>
		/// <param name="eventString">
		/// String to be logged
		/// </param>
		/// <param name="showInGUI">
		/// When 'true', the string will be logged in the GUI and the log file
		/// When 'false', the string will only by logged in the log file
		/// Default value of 'true'
		/// </param>
		void LogEvent(string eventString, bool showInGUI = true)
        {
			// Add event to GUI event log
			if (showInGUI)
			{
				EventLog.Rows.Add(String.Format("{0} {1}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString()), eventString);
			}
            // Add event to file event log
            AppendLogToFile(eventString);
        }

		/// <summary>
		/// Clears the event log when the 'EventLogClear' button is pressed
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
        private void EventLogClearButton_Click(object sender, EventArgs e)
        {
            // Clear the event log table
            EventLog.Rows.Clear();
        }

        #endregion


    }
}
