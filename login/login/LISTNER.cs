using System;
using System.Data;
using System.Data.SqlClient;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace login.Serial
{
    public partial class LISTNER : Form
    {
        public static string a;
        public static string ld_data = "0";
        public static string active_mech;
        private SerialPortManager _spManager;

        public LISTNER()
        {
            InitializeComponent();
            UserInitialization();
        }

        private void UserInitialization()
        {
            _spManager = new SerialPortManager();
            SerialSettings mySerialSettings = _spManager.CurrentSerialSettings;
            serialSettingsBindingSource.DataSource = mySerialSettings;

            try
            {
                using (SqlConnection db_connect = new SqlConnection(db_Connection.connection_string()))
                {
                    db_connect.Open();
                    using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM machine_master WHERE m_status = 'Active'", db_connect))
                    {
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string port = dt.Rows[0][1]?.ToString().Trim();
                            string baud = dt.Rows[0][2]?.ToString().Trim();
                            string data = dt.Rows[0][3]?.ToString().Trim();
                            string parity = dt.Rows[0][4]?.ToString().Trim();
                            string stop = dt.Rows[0][5]?.ToString().Trim();

                            if (!string.IsNullOrEmpty(port))
                            {
                                mySerialSettings.PortName = port;
                                portNameComboBox.Text = port;
                            }
                            if (int.TryParse(baud, out int bVal))
                            {
                                mySerialSettings.BaudRate = bVal;
                                baudRateComboBox.Text = baud;
                            }
                            if (int.TryParse(data, out int dVal))
                            {
                                mySerialSettings.DataBits = dVal;
                                dataBitsComboBox.Text = data;
                            }
                            if (!string.IsNullOrEmpty(parity) && Enum.TryParse(parity, true, out Parity pVal))
                            {
                                parityComboBox.Text = parity;
                                mySerialSettings.Parity = pVal;
                            }
                            if (!string.IsNullOrEmpty(stop) && Enum.TryParse(stop, true, out StopBits sVal))
                            {
                                stopBitsComboBox.Text = stop;
                                mySerialSettings.StopBits = sVal;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("UserInitialization serial error: " + ex.Message);
            }

            _spManager.NewSerialDataRecieved += _spManager_NewSerialDataRecieved;
            this.FormClosing += MainForm_FormClosing;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _spManager?.Dispose();
        }

        private void _spManager_NewSerialDataRecieved(object sender, SerialDataEventArgs e)
        {
            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new EventHandler<SerialDataEventArgs>(_spManager_NewSerialDataRecieved), sender, e);
                    return;
                }

                if (e.Data == null || e.Data.Length == 0)
                    return;

                const int maxTextLength = 1000;
                if (tbData1.TextLength > maxTextLength)
                {
                    tbData1.Text = tbData1.Text.Remove(0, tbData1.TextLength - maxTextLength);
                }

                // Mask 8th bit (parity bit) so 7-bit ASCII with parity does not convert to '?'
                byte[] cleanBytes = new byte[e.Data.Length];
                for (int i = 0; i < e.Data.Length; i++)
                {
                    cleanBytes[i] = (byte)(e.Data[i] & 0x7F);
                }

                string str = Encoding.ASCII.GetString(cleanBytes);
                string newstr;

                textBox1.Text = str;

                if (str.ToLower().Contains(" "))
                {
                    int spaceIndex = str.IndexOf(" ");
                    string afterSpace = str.Substring(spaceIndex + 1);
                    newstr = afterSpace.Length > 6 ? afterSpace.Substring(0, 6).Trim() : afterSpace.Trim();
                }
                else
                {
                    newstr = str.Trim();
                }

                // Extract numeric digits to prevent displaying question marks or garbage characters
                string numericOnly = Regex.Match(newstr, @"\d+(\.\d+)?").Value;
                ld_data = !string.IsNullOrEmpty(numericOnly) ? numericOnly : newstr;
                tbData1.Text = ld_data;
            }
            catch (Exception EX)
            {
                MessageBox.Show("exception: " + EX.Message);
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _spManager?.StopListening();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {
        }

        public void LISTNER_Load(object sender, EventArgs e)
        {
            try
            {
                const string m_status = "Active";
                using (SqlConnection db_connect = new SqlConnection(db_Connection.connection_string()))
                {
                    db_connect.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM machine_master WHERE m_status = @status", db_connect))
                    {
                        cmd.Parameters.AddWithValue("@status", m_status);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                active_mech = dt.Rows[0][0]?.ToString();
                                string port = dt.Rows[0][1]?.ToString().Trim();
                                if (!string.IsNullOrEmpty(port) && _spManager?.CurrentSerialSettings != null)
                                {
                                    _spManager.CurrentSerialSettings.PortName = port;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception EX)
            {
                Console.WriteLine("LISTNER_Load error: " + EX.Message);
            }

            try
            {
                _spManager?.StartListening();
            }
            catch (Exception ex)
            {
                Console.WriteLine("StartListening error: " + ex.Message);
            }
        }

        private void serialSettingsBindingSource_CurrentChanged(object sender, EventArgs e)
        {
        }

        private void tbData_KeyUp(object sender, KeyEventArgs e)
        {
        }

        private void portNameComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_spManager?.CurrentSerialSettings != null && !string.IsNullOrWhiteSpace(portNameComboBox.Text))
            {
                _spManager.CurrentSerialSettings.PortName = portNameComboBox.Text.Trim();
            }
        }
    }
}
