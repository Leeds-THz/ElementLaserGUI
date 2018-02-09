using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO.Ports;
using System.Windows.Forms;

namespace ElementCOMGUI
{
    public partial class Form1 : Form
    {
        private string[] COMList;
        private static List<string> MainCOMDataBuffer = new List<string>();
        private static List<string> TempCOMDataBuffer = new List<string>();
        private static List<string> LogFileCOMDataBuffer = new List<string>();
        private bool logFileWriteFlag = false;
        //private Stopwatch LogFileStopwatch = new Stopwatch();

        public Form1()
        {
            InitializeComponent();
            RefreshCOMSelector();
        }

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

        private void UpdateCOMConnectionStatus(SerialPort port, Label statusLabel, Label nameLabel)
        {
            if (port.IsOpen)
            {
                statusLabel.Text = "Connected";
                nameLabel.Text = port.PortName;
            }
            else
            {
                statusLabel.Text = "Not Connected";
                nameLabel.Text = "";
            }
        }

        private void UpdateMainCOMConnectionStatus()
        {
            UpdateCOMConnectionStatus(MainCOMPort, MainCOMConnectionStatusLabel, MainCOMNameLabel);
        }

        private void UpdateLogFileCOMConnectionStatus()
        {
            UpdateCOMConnectionStatus(LogFileCOMPort, LogFileCOMPortConnectionStatusLabel, LogFileCOMNameLabel);
        }

        private void RefreshCOM()
        {
            COMList = SerialPort.GetPortNames();

            // Sort the list
            Array.Sort(COMList);
        }

        private void RefreshCOMSelector()
        {
            // When refresh button clicked, dlete list of COM connections and create a new one

            RefreshCOM();

            MainCOMPortSelector.Items.Clear();

            foreach (var port in COMList)
            {
                MainCOMPortSelector.Items.Add(port);
            }
        }

        private void ConnectCOM(SerialPort port, string portName)
        {
            // Make sure port is closed
            port.Close();

            port.BaudRate = 115200;

            try
            {
                // Set which port to connect to
                port.PortName = portName;

                // ELEMENT SPECIFIC SETTINGS HERE, REMOVE TO DEBUG ON BOARD
                //port.NewLine = "\r";


                // Open a connection to the port
            
                port.Open();
                // Remove data from input/output buffers
                port.DiscardInBuffer();
                port.DiscardOutBuffer();
            }
            catch { }
        }

        private void DisconnectCOM(SerialPort port)
        {
            port.Close();
        }

        private void COMDataRecievedHandler(List<string> dataBufferList, object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = (SerialPort)sender;
            string data = port.ReadExisting(); // Read all existing data in the serial buffer
            //string data = port.ReadLine(); // Read all existing data in the serial buffer

            dataBufferList.Add(data); // Store data to the textbox buffer
        }

        private void MainCOMDataRecievedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            COMDataRecievedHandler(MainCOMDataBuffer, sender, e);
        }

        private void TempCOMDataRecievedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            COMDataRecievedHandler(TempCOMDataBuffer, sender, e);
        }

        private void LogFileCOMDataRecievedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (logFileWriteFlag == true)
            {
                //COMDataRecievedHandler(LogFileCOMDataBuffer, sender, e);
                SerialPort port = (SerialPort)sender;
                string data = port.ReadExisting(); // Read all existing data in the serial buffer

                using (StreamWriter sw = File.AppendText(LogFileSaveDialog.FileName))
                {
                    sw.Write(data);
                }
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

        private void CommandButton_Click(object sender, EventArgs e)
        {
            if (MainCOMPort.IsOpen)
            {
                if (CommandComboBox.Text == "GETLOG=?")
                {
                    if (!LogFileCOMPort.IsOpen)
                    {
                        SetErrorLabelMessage("Log file COM port not connected. Please press 'Auto-Connect'.");
                        return;
                    }

                    if (SetLogFileSavePath() == DialogResult.Cancel)
                    {
                        SetErrorLabelMessage("Save file location not chosen.");
                        return;
                    }

                    StartLogFileRecording();
                }

                MainCOMPort.Write(CommandComboBox.Text + "\r");
            }
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
            // Maybe add a progress bar + execute on separate thread?

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

        
        private void StartLogFileRecording()
        {
            logFileWriteFlag = true;
        }
       

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void COMOut_TextChanged(object sender, EventArgs e)
        {

        }

        

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
