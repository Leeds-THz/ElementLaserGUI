using System;
using System.IO;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;

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
        /// Serial data recieved event
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
        /// Serial data recieved event
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
        /// Serial data recieved event
        /// </param>
        private void TempCOMDataRecievedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            COMDataRecievedHandler(TempCOMDataBuffer, sender, e);
        }

        /// <summary>
        /// Event handler used by the log file com port to write recieved data to the log file 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            LogFileSaveDialog.Filter = "Comma Separated Value File|*.csv";
            LogFileSaveDialog.Title = "Log File Save Location";
            var result = LogFileSaveDialog.ShowDialog();

            if (result == DialogResult.Cancel)
            {
                LogFileSaveDialog.FileName = "";
            }

            UpdateSaveLocationLabel();

            return result;
        }

        private void StartLogFileRecording()
        {
            logFileWriteFlag = true;
        }

        private void StopLogFileRecording()
        {
            logFileWriteFlag = false;
        }

        #endregion

        #region GUI_INTERFACE_FUNCTIONS

        private void Update_Tick(object sender, EventArgs e)
        {
            UpdateMainCOMConnectionStatus();
            UpdateLogFileCOMConnectionStatus();

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
        }

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



        private void COMRefreshButton_Click(object sender, EventArgs e)
        {
            RefreshCOMSelector();
        }

        private void MainCOMConnectButton_Click(object sender, EventArgs e)
        {
            ConnectCOM(MainCOMPort, MainCOMPortSelector.Text);
        }



        private void CommandButton_Click(object sender, EventArgs e)
        {
            if (MainCOMPort.IsOpen)
            {
                ClearErrorLabelMessage();

                if (CommandComboBox.Text == "GETLOG=?")
                {
                    if (!LogFileCOMPort.IsOpen)
                    {
                        StopLogFileRecording();
                        SetErrorLabelMessage("Log file COM port not connected. Please press 'Auto-Connect'");
                        return;
                    }

                    if (SetLogFileSavePath() == DialogResult.Cancel)
                    {
                        StopLogFileRecording();
                        SetErrorLabelMessage("Save file location not chosen");
                        return;
                    }

                    StartLogFileRecording();
                }

                MainCOMPort.Write(CommandComboBox.Text + "\r");
            }
        }

        private void ClearErrorLabelMessage()
        {
            ErrorLabel.Text = "";
        }

        private void SetErrorLabelMessage(string message)
        {
            ErrorLabel.Text = "Error: " + message;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            COMOut.Clear();
        }


        private void LogFileCOMPortAutoConnect_Click(object sender, EventArgs e)
        {
            // Maybe execute on separate thread?
            ClearErrorLabelMessage();
            TempCOMDataBuffer.Clear();
            LogFileCOMPort.Close();
            RefreshCOM();

            AutoConnectProgressBar.Value = 0;
            AutoConnectProgressBar.Maximum = COMList.Length;
            AutoConnectProgressBar.Visible = true;

            // Iterate through all COM ports
            foreach (var com in COMList)
            {
                // Temporarily connect to the COM port
                ConnectCOM(TempCOMPort, com);

                if (TempCOMPort.IsOpen)
                {
                    // Send empty data
                    TempCOMPort.Write("\r");

                    int whileCounter = 0;

                    // Read recieved data
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
                        AutoConnectProgressBar.Value = AutoConnectProgressBar.Maximum;
                        break;
                    }

                    // Clear temp data buffer
                    TempCOMDataBuffer.Clear();
                }

                if (AutoConnectProgressBar.Value < AutoConnectProgressBar.Maximum)
                {
                    AutoConnectProgressBar.Value++;
                }
            }

            if (!LogFileCOMPort.IsOpen)
            {
                SetErrorLabelMessage("Could not find the log file COM port");
            }

            AutoConnectProgressBar.Visible = false;
        }

        private void MainDisconnectButton_Click(object sender, EventArgs e)
        {
            DisconnectCOM(MainCOMPort);
        }

        private void LogDisconnectButton_Click(object sender, EventArgs e)
        {
            DisconnectCOM(LogFileCOMPort);
        }

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

    }
}
