using login.Serial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SqlClient;
using System.Threading;
using NPOI.SS.Util;
using System.Text.RegularExpressions;
using System.Drawing.Printing;

namespace login
{
    public partial class main : Form
    {
        string weight_chk1;
        string weight_chk2;
        public static string b;
        public static string cr_date;
        public static string prn_slipno;
        public static string prn_partyname;
        public static string prn_vehicle_type;
        public static string prn_charges;
        public static string prn_op_name;
        public static string prn_t_weight;
        public static string prn_g_weight;
        public static string prn_n_weight;
        public static string prn_product;
        public static string t_no;
        public static string mdate;
        public static string veh_no;
        public string watermark = null;


        public System.Drawing.Printing.PageSettings PageSettings { get; set; }


        public main()
        {


            InitializeComponent();

            // b = LISTNER.load_data;

            new Thread(crr_weight).Start();
            //weight_box.Text = b;


        }


        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
        public static void SendToMain(string a)
        {

            //b = LISTNER.a;
            //weight_box.Text = b;
            b = LISTNER.load_data;
            b = LISTNER.load_data;



        }
        void main_Load(object sender, EventArgs e)
        {
            ApplyModernTheme();
            get_tyer();
            get_charges();
            User_priv();
            RefreshVehicleAutoComplete();
            RefreshPartyAutoComplete();
            RefreshMaterialAutoComplete();
            this.Vehical_box.Leave += new System.EventHandler(this.Vehical_box_Leave);
            date_box.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            new Thread(crr_weight).Start();
        }

        private void weight_box_TextChanged(object sender, EventArgs e)
        {
            CalculateNetWeight();
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void save_main_Click_old(object sender, EventArgs e)
        {
            save_main_Click(sender, e);
        }

        private void save_main_Click(object sender, EventArgs e)
        {
            string rawVeh = Vehical_box.Text != null ? Vehical_box.Text.Trim().ToUpper() : "";
            if (string.IsNullOrEmpty(rawVeh))
            {
                MessageBox.Show("Please Enter Vehicle No.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Vehical_box.Focus();
                return;
            }

            string rawParty = Party_box.Text != null ? Party_box.Text.Trim() : "";
            if (string.IsNullOrEmpty(rawParty))
            {
                MessageBox.Show("Please Enter Party Name.", "Party Name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Party_box.Focus();
                return;
            }

            string rawMaterial = meterial_box.Text != null ? meterial_box.Text.Trim() : "";
            if (string.IsNullOrEmpty(rawMaterial))
            {
                MessageBox.Show("Please Enter Product / Material Name.", "Product Name Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                meterial_box.Focus();
                return;
            }

            string cleanVeh = rawVeh.Replace("-", "").Replace(" ", "");
            string currentSlipNo = slipno_box.Text != null ? slipno_box.Text.Trim() : "";

            // If slipno_box is empty, this is a NEW slip (1st weighment)
            if (string.IsNullOrEmpty(currentSlipNo))
            {
                // 1st weighment: Gross Weight is mandatory
                string grossRaw1 = weight_box.Text != null ? weight_box.Text.Trim() : "";
                if (string.IsNullOrEmpty(grossRaw1))
                {
                    //MessageBox.Show("Please Enter Gross Weight before saving.", "Gross Weight Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //weight_box.Focus();
                    //return;
                }
                int grossCheck;
                if (!int.TryParse(grossRaw1, out grossCheck))
                {
                    //MessageBox.Show("Gross Weight must be a valid number.", "Invalid Gross Weight", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //weight_box.Focus();
                    //return;
                }
                string tareRaw1 = tire_weight_box.Text != null ? tire_weight_box.Text.Trim() : "";
                if (string.IsNullOrEmpty(tareRaw1))
                {
                    //MessageBox.Show("Please Enter Gross Weight before saving.", "Gross Weight Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //weight_box.Focus();
                    //return;
                }
                int tareCheck;
                if (!int.TryParse(tareRaw1, out tareCheck))
                {
                    //MessageBox.Show("Gross Weight must be a valid number.", "Invalid Gross Weight", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //weight_box.Focus();
                    //return;
                }
                
                if (tareCheck <= 0 && grossCheck<=0)
                {
                    MessageBox.Show("Tare Weight cannot be Zero or negative. Please enter a valid weight.", "Invalid Gross Weight", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    weight_box.Focus();
                    return;
                }
                if (grossCheck <= 0 && tareCheck<=0)
                {
                    MessageBox.Show("Gross Weight cannot be Zero or negative. Please enter a valid weight.", "Invalid Gross Weight", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    weight_box.Focus();
                    return;
                }

                // Check if this vehicle already has a pending slip
                try
                {
                    using (SqlConnection db_connect1 = new SqlConnection(db_Connection.connection_string()))
                    {
                        db_connect1.Open();
                        string chkQry = @"SELECT TOP 1 slip_no, vehical_no FROM measure 
                                         WHERE (vehical_no = @rawVeh OR REPLACE(REPLACE(vehical_no, '-', ''), ' ', '') = @cleanVeh)
                                           AND (net_weight IS NULL OR LTRIM(RTRIM(net_weight)) = '' OR net_weight = 'NULL')
                                         ORDER BY slip_no DESC";
                        using (SqlCommand chkCmd = new SqlCommand(chkQry, db_connect1))
                        {
                            chkCmd.Parameters.AddWithValue("@rawVeh", rawVeh);
                            chkCmd.Parameters.AddWithValue("@cleanVeh", cleanVeh);
                            using (SqlDataAdapter sda = new SqlDataAdapter(chkCmd))
                            {
                                DataTable dt = new DataTable();
                                sda.Fill(dt);
                                if (dt.Rows.Count >= 1)
                                {
                                    string pendingSlipNo = dt.Rows[0]["slip_no"].ToString().Trim();
                                    DialogResult dr = MessageBox.Show(
                                        "Pending Slip (" + pendingSlipNo + ") already exists for Vehicle " + rawVeh + ".\n\nDo you want to load this pending slip now?",
                                        "Pending Slip Found",
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question);

                                    if (dr == DialogResult.Yes)
                                    {
                                        LoadSlipBySlipNo(pendingSlipNo, false);
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error checking pending slips: " + ex.Message);
                }

                // Generate new slip number
                string active_m = (LISTNER.active_mech ?? "M").Trim();
                if (string.IsNullOrEmpty(active_m)) active_m = "M";

                string slipseqdate = DateTime.Now.ToString("yyMMdd");
                int slipseq = 1;

                string lastSlip = get_slipseq();
                if (!string.IsNullOrEmpty(lastSlip) && lastSlip.Length >= active_m.Length + 10)
                {
                    int prefixLen = active_m.Length;
                    if (lastSlip.StartsWith(active_m, StringComparison.OrdinalIgnoreCase))
                    {
                        string lastDate = lastSlip.Substring(prefixLen, 6);
                        string lastSeqStr = lastSlip.Substring(prefixLen + 6);
                        int parsedSeq;
                        if (int.TryParse(lastSeqStr, out parsedSeq))
                        {
                            if (lastDate == slipseqdate)
                            {
                                slipseq = parsedSeq + 1;
                            }
                            else
                            {
                                slipseq = 1;
                            }
                        }
                    }
                }

                if (slipseq > 9999)
                {
                    MessageBox.Show("Maximum Slip sequence reached for today.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string gen_slipno = string.Format("{0}{1}{2:D4}", active_m, slipseqdate, slipseq);

                try
                {
                    DateTime curr_date = DateTime.Now;
                    string StringFormat = "yyyy-MM-dd HH:mm:ss";
                    string dateStr = curr_date.ToString(StringFormat);
                    date_box.Text = dateStr;

                    string get_session = Form1.User_sessson != null ? Form1.User_sessson.ToString() : "Admin";

                    using (SqlConnection db_connect = new SqlConnection(db_Connection.connection_string()))
                    {
                        db_connect.Open();
                        string insertQry = @"INSERT INTO measure(slip_no, weight, tire_weight, net_weight, vehical_no, party, meterial, vehical_type, charges, date, remarks, user_name, token_no) 
                                             VALUES(@slip_no, @weight, @tire_weight, NULL, @vehical_no, @party, @meterial, @vehical_type, @charges, @date, @remarks, @user_name, @token_no)";
                        using (SqlCommand cmd = new SqlCommand(insertQry, db_connect))
                        {
                            cmd.Parameters.AddWithValue("@slip_no", gen_slipno);
                            cmd.Parameters.AddWithValue("@weight", weight_box.Text.Trim());
                            cmd.Parameters.AddWithValue("@tire_weight", tire_weight_box.Text.Trim());
                            cmd.Parameters.AddWithValue("@vehical_no", rawVeh);
                            cmd.Parameters.AddWithValue("@party", Party_box.Text.Trim());
                            cmd.Parameters.AddWithValue("@meterial", meterial_box.Text.Trim());
                            cmd.Parameters.AddWithValue("@vehical_type", vechaltype_drop.Text.Trim());
                            cmd.Parameters.AddWithValue("@charges", charges_drop.Text.Trim());
                            cmd.Parameters.AddWithValue("@date", dateStr);
                            cmd.Parameters.AddWithValue("@remarks", remarks_box.Text.Trim());
                            cmd.Parameters.AddWithValue("@user_name", get_session);
                            cmd.Parameters.AddWithValue("@token_no", tokenNo_box.Text.Trim());

                            cmd.ExecuteNonQuery();
                        }
                    }

                    slipno_box.Text = gen_slipno;
                    RefreshVehicleAutoComplete();
                    RefreshPartyAutoComplete();
                    RefreshMaterialAutoComplete();
                    MessageBox.Show("Slip No: " + gen_slipno + " Saved Successfully", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error saving slip: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                // 2nd weighment: Update existing slip — both weights required
                string grossRaw2 = weight_box.Text != null ? weight_box.Text.Trim() : "";
                string tareRaw2 = tire_weight_box.Text != null ? tire_weight_box.Text.Trim() : "";

                if (string.IsNullOrEmpty(grossRaw2))
                {
                    MessageBox.Show("Please Enter Gross Weight before updating.", "Gross Weight Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    weight_box.Focus();
                    return;
                }
                if (string.IsNullOrEmpty(tareRaw2))
                {
                    MessageBox.Show("Please Enter Tare Weight before updating.", "Tare Weight Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tire_weight_box.Focus();
                    return;
                }

                int grossVal = 0;
                int tareVal = 0;
                if (!int.TryParse(grossRaw2, out grossVal))
                {
                    MessageBox.Show("Gross Weight must be a valid number.", "Invalid Gross Weight", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    weight_box.Focus();
                    return;
                }
                if (grossVal <= 0)
                {
                    MessageBox.Show("Gross Weight cannot be Zero or negative. Please enter a valid weight.", "Invalid Gross Weight", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    weight_box.Focus();
                    return;
                }
                if (!int.TryParse(tareRaw2, out tareVal))
                {
                    MessageBox.Show("Tare Weight must be a valid number.", "Invalid Tare Weight", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tire_weight_box.Focus();
                    return;
                }
                if (tareVal <= 0)
                {
                    MessageBox.Show("Tare Weight cannot be Zero or negative. Please enter a valid weight.", "Invalid Tare Weight", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tire_weight_box.Focus();
                    return;
                }
                if (tareVal > grossVal)
                {
                    MessageBox.Show("Tare Weight (" + tareVal + ") cannot be greater than Gross Weight (" + grossVal + ").",
                        "Weight Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tire_weight_box.Focus();
                    return;
                }

                int net_weight = grossVal - tareVal;
                net_weight_box.Text = net_weight.ToString();

                DateTime curr_date = DateTime.Now;
                string StringFormat = "yyyy-MM-dd HH:mm:ss";
                string ss = curr_date.ToString(StringFormat);
                mod_date.Text = ss;

                try
                {
                    using (SqlConnection db_connect_1 = new SqlConnection(db_Connection.connection_string()))
                    {
                        db_connect_1.Open();
                        string updateQry = @"UPDATE measure SET 
                                                weight = @weight, 
                                                tire_weight = @tire_weight, 
                                                net_weight = @net_weight, 
                                                party = @party, 
                                                meterial = @meterial, 
                                                vehical_type = @vehical_type, 
                                                charges = @charges, 
                                                remarks = @remarks, 
                                                mod_date = @mod_date, 
                                                mod_by = @mod_by 
                                             WHERE slip_no = @slip_no";
                        using (SqlCommand SqlComm = new SqlCommand(updateQry, db_connect_1))
                        {
                            SqlComm.Parameters.AddWithValue("@weight", grossVal.ToString());
                            SqlComm.Parameters.AddWithValue("@tire_weight", tareVal.ToString());
                            SqlComm.Parameters.AddWithValue("@net_weight", net_weight.ToString());
                            SqlComm.Parameters.AddWithValue("@party", Party_box.Text.Trim());
                            SqlComm.Parameters.AddWithValue("@meterial", meterial_box.Text.Trim());
                            SqlComm.Parameters.AddWithValue("@vehical_type", vechaltype_drop.Text.Trim());
                            SqlComm.Parameters.AddWithValue("@charges", charges_drop.Text.Trim());
                            SqlComm.Parameters.AddWithValue("@remarks", remarks_box.Text.Trim());
                            SqlComm.Parameters.AddWithValue("@mod_date", ss);
                            SqlComm.Parameters.AddWithValue("@mod_by", Form1.User_sessson != null ? Form1.User_sessson.ToString() : "Admin");
                            SqlComm.Parameters.AddWithValue("@slip_no", currentSlipNo);

                            SqlComm.ExecuteNonQuery();
                        }
                    }

                    RefreshVehicleAutoComplete();
                    RefreshPartyAutoComplete();
                    RefreshMaterialAutoComplete();
                    MessageBox.Show("Slip No: " + currentSlipNo + " Updated Successfully", "Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating slip: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }


        private void populate_Click(object sender, EventArgs e)
        {

        }

        private void btn_refresh_Click(object sender, EventArgs e)
        {
            // Clear all input fields for a fresh new slip
            slipno_box.Text = "";
            Vehical_box.Text = "";
            Party_box.Text = "";
            meterial_box.Text = "";
            weight_box.Text = "";
            tire_weight_box.Text = "";
            net_weight_box.Text = "";
            remarks_box.Text = "";
            tokenNo_box.Text = "";
            date_box.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            mod_date.Text = "";
            vechaltype_drop.SelectedIndex = -1;
            charges_drop.SelectedIndex = -1;

            // Clear ref fields if they exist
            if (ref_slipNo_box != null) ref_slipNo_box.Text = "";

            RefreshVehicleAutoComplete();
            RefreshPartyAutoComplete();
            RefreshMaterialAutoComplete();

            // Reset focus to vehicle field
            Vehical_box.Focus();
        }

        private void weight_box_KeyUp(object sender, KeyEventArgs e)
        {


        }

        private void button1_Click(object sender, EventArgs e)
        {
            b = LISTNER.ld_data.ToString();

            int parsedVal;
            if (!int.TryParse(b, out parsedVal))
            {
                parsedVal = 0;
            }

            if (weight_chk1 == "True" || radioButton1.Checked)
            {
                tire_weight_box.Text = parsedVal.ToString();
            }
            if (weight_chk2 == "True" || radioButton2.Checked)
            {
                weight_box.Text = parsedVal.ToString();
            }

            // Set current date time
            string nowStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (string.IsNullOrEmpty(slipno_box.Text))
            {
                date_box.Text = nowStr;
            }
            else
            {
                mod_date.Text = nowStr;
            }

            CalculateNetWeight();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
            //new Thread(crr_weight).Abort();
            //this.Close();
            Application.Exit();

        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

        }

        private void toolStripSplitButton1_ButtonClick(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tireMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

            weight_chk1 = radioButton1.Checked.ToString();
            //MessageBox.Show("radiobutton" + weight_chk1);
            //Console.WriteLine("######value of radio########"+weight_chk1);

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

            weight_chk2 = radioButton2.Checked.ToString();
            // MessageBox.Show("radiobutton2" + weight_chk2);
        }

        private void slipno_box_TextChanged(object sender, EventArgs e)
        {

        }

        private void slipno_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LoadSlipBySlipNo(slipno_box.Text, true);
            }
        }
        private int calculate_netweight()
        {
            int weight = 0;
            int tire_weight = 0;
            int.TryParse(weight_box.Text.Trim(), out weight);
            int.TryParse(tire_weight_box.Text.Trim(), out tire_weight);
            return weight - tire_weight;
        }

        private void tire_weight_box_TextChanged(object sender, EventArgs e)
        {
            CalculateNetWeight();
        }
        public string get_slipseq()
        {
            try
            {
                using (SqlConnection db_connect = new SqlConnection(db_Connection.connection_string()))
                {
                    db_connect.Open();
                    string mk = (LISTNER.active_mech ?? "M").ToString().Trim();
                    using (SqlDataAdapter sda = new SqlDataAdapter("SELECT MAX(slip_no) FROM measure WHERE slip_no LIKE @mk", db_connect))
                    {
                        sda.SelectCommand.Parameters.AddWithValue("@mk", mk + "%");
                        DataTable dt = new DataTable();
                        sda.Fill(dt);
                        if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value && !string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                        {
                            return dt.Rows[0][0].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("get_slipseq error: " + ex.Message);
            }

            string active_m = (LISTNER.active_mech ?? "M").ToString().Trim();
            string slipseqdate = DateTime.Now.ToString("yyMMdd");
            return active_m + slipseqdate + "0000";
        }

        private void addDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Machine_master machine_Master = new Machine_master();
            machine_Master.Show();
        }

        private void tareMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Charges_Master tare_Master = new Charges_Master();
            //tare_Master.Show();
            vehical_master vehical_Master = new vehical_master();
            vehical_Master.Show();
        }

        private void userMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            User_Master user_Master = new User_Master();
            user_Master.Show();
        }

        private void chargesMasterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Charges_Master charges_Master = new Charges_Master();
            charges_Master.Show();
        }

        public void get_tyer()
        {
            SqlConnection db_connect = new SqlConnection(db_Connection.connection_string());
            db_connect.Open();
            SqlDataAdapter sda = new SqlDataAdapter("select tyre from charge_master", db_connect);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                vechaltype_drop.Items.Add(dr["tyre"].ToString());
            }
            db_connect.Close();



        }

        public void get_charges()
        {
            SqlConnection db_connect = new SqlConnection(db_Connection.connection_string());
            db_connect.Open();
            SqlDataAdapter sda = new SqlDataAdapter("select charges from charge_master", db_connect);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            foreach (DataRow dr in dt.Rows)
            {
                charges_drop.Items.Add(dr["charges"].ToString());
            }
            db_connect.Close();



        }

        private void vechaltype_drop_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void toolStripButton1_Click_1(object sender, EventArgs e)
        {

        }

        private void pendingSlipsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reports.Pending_slips pending_Slips = new Reports.Pending_slips();
            pending_Slips.Show(this);
        }

        private void Vehical_box_TextChanged(object sender, EventArgs e)
        {

        }

        private void Vehical_box_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Allow control keys (backspace, enter, tab)
            if (char.IsControl(e.KeyChar))
                return;

            // Allow letters, digits, hyphen (-), and space ( )
            if (char.IsLetterOrDigit(e.KeyChar) || e.KeyChar == '-' || e.KeyChar == ' ')
                return;

            // Block other symbols
            e.Handled = true;
        }
        public void User_priv()
        {
            String get_priv = Form1.Session_priv;
            // master_Strip = new  as master_strip();
            Console.WriteLine("value og get session priv" + get_priv);
            if (get_priv.CompareTo("user") == 1)
            {

                master_strip.Enabled = false;
            }

        }

        private void slipRegisterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reports.Slip_Register slip_Register = new Reports.Slip_Register();
            slip_Register.Show();
        }

        private void charges_drop_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        public void cuttent_weight_box_TextChanged(object sender, EventArgs e)
        {



        }

        void crr_weight()
        {
            try
            {
                if (InvokeRequired)
                {
                    for (int i = 1; i > 0; i++)
                    {
                        this.BeginInvoke((MethodInvoker)delegate ()
                        {
                            string currentVal = string.IsNullOrEmpty(LISTNER.ld_data) ? "0" : LISTNER.ld_data.ToString();
                            cuttent_weight_box.Text = currentVal;
                            cuttent_weight_box.SelectAll();
                            cuttent_weight_box.SelectionAlignment = HorizontalAlignment.Center;
                            cuttent_weight_box.DeselectAll();
                        });
                        Thread.Sleep(500);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ex" + ex);
            }




        }

        private void Vehical_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                LoadPendingSlipForVehicle(Vehical_box.Text, true);
            }
        }

        private void Vehical_box_Leave(object sender, EventArgs e)
        {
            // If slipno_box is empty and user entered a vehicle no, auto-load any pending slip quietly
            if (string.IsNullOrEmpty(slipno_box.Text.Trim()) && !string.IsNullOrEmpty(Vehical_box.Text.Trim()))
            {
                LoadPendingSlipForVehicle(Vehical_box.Text, false);
            }
        }

        public void PopulateSlipData(DataRow row)
        {
            if (row == null) return;

            slipno_box.Text = row["slip_no"] != DBNull.Value ? row["slip_no"].ToString().Trim() : "";

            string wtVal = row["weight"] != DBNull.Value ? row["weight"].ToString().Trim() : "";
            string tareVal = row["tire_weight"] != DBNull.Value ? row["tire_weight"].ToString().Trim() : "";

            weight_box.Text = wtVal;
            tire_weight_box.Text = tareVal;
            net_weight_box.Text = row["net_weight"] != DBNull.Value ? row["net_weight"].ToString().Trim() : "";

            if (row["vehical_no"] != DBNull.Value && !string.IsNullOrEmpty(row["vehical_no"].ToString()))
            {
                Vehical_box.Text = row["vehical_no"].ToString().Trim();
            }

            Party_box.Text = row["party"] != DBNull.Value ? row["party"].ToString().Trim() : "";
            meterial_box.Text = row["meterial"] != DBNull.Value ? row["meterial"].ToString().Trim() : "";
            vechaltype_drop.Text = row["vehical_type"] != DBNull.Value ? row["vehical_type"].ToString().Trim() : "";
            charges_drop.Text = row["charges"] != DBNull.Value ? row["charges"].ToString().Trim() : "";
            string dVal = row["date"] != DBNull.Value ? row["date"].ToString().Trim() : "";
            date_box.Text = !string.IsNullOrEmpty(dVal) ? dVal : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            remarks_box.Text = row["remarks"] != DBNull.Value ? row["remarks"].ToString().Trim() : "";
            string modVal = row["mod_date"] != DBNull.Value ? row["mod_date"].ToString().Trim() : "";
            mod_date.Text = modVal;
            tokenNo_box.Text = row["token_no"] != DBNull.Value ? row["token_no"].ToString().Trim() : "";

            // Automatically suggest the next weighment mode for 2nd weight:
            if (!string.IsNullOrEmpty(tareVal) && string.IsNullOrEmpty(wtVal))
            {
                radioButton2.Checked = true; // Tare already captured, now capture Gross
                weight_chk2 = "True";
            }
            else if (!string.IsNullOrEmpty(wtVal) && string.IsNullOrEmpty(tareVal))
            {
                radioButton1.Checked = true; // Gross already captured, now capture Tare
                weight_chk2 = "False";
            }

            CalculateNetWeight();
        }

        public bool LoadSlipBySlipNo(string searchSlip, bool showMessages = true)
        {
            searchSlip = searchSlip != null ? searchSlip.Trim() : "";
            if (string.IsNullOrEmpty(searchSlip))
            {
                if (showMessages)
                    MessageBox.Show("Please enter a Slip No to search.", "Search Slip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            try
            {
                using (SqlConnection db_connect = new SqlConnection(db_Connection.connection_string()))
                {
                    db_connect.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT TOP 1 * FROM measure WHERE slip_no = @slip_no", db_connect))
                    {
                        cmd.Parameters.AddWithValue("@slip_no", searchSlip);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                PopulateSlipData(dt.Rows[0]);
                                if (showMessages)
                                    MessageBox.Show("Record loaded successfully for Slip No: " + searchSlip, "Slip Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return true;
                            }
                            else
                            {
                                if (showMessages)
                                    MessageBox.Show("No record found for Slip No: " + searchSlip, "Record Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (showMessages)
                    MessageBox.Show("Error searching Slip No: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool LoadPendingSlipForVehicle(string vehicleNo, bool showMessages = false)
        {
            string rawVeh = vehicleNo != null ? vehicleNo.Trim().ToUpper() : "";
            if (string.IsNullOrEmpty(rawVeh))
                return false;

            string cleanVeh = rawVeh.Replace("-", "").Replace(" ", "");

            try
            {
                using (SqlConnection db_connect = new SqlConnection(db_Connection.connection_string()))
                {
                    db_connect.Open();
                    string qry = @"SELECT TOP 1 * FROM measure 
                                   WHERE (vehical_no = @rawVeh 
                                          OR REPLACE(REPLACE(vehical_no, '-', ''), ' ', '') = @cleanVeh)
                                     AND (net_weight IS NULL OR LTRIM(RTRIM(net_weight)) = '' OR net_weight = 'NULL')
                                   ORDER BY slip_no DESC";
                    using (SqlCommand cmd = new SqlCommand(qry, db_connect))
                    {
                        cmd.Parameters.AddWithValue("@rawVeh", rawVeh);
                        cmd.Parameters.AddWithValue("@cleanVeh", cleanVeh);
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            sda.Fill(dt);

                            if (dt.Rows.Count > 0)
                            {
                                PopulateSlipData(dt.Rows[0]);
                                if (showMessages)
                                {
                                    MessageBox.Show("Pending Slip (" + slipno_box.Text + ") loaded for Vehicle: " + rawVeh, 
                                                    "Pending Slip Loaded", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                return true;
                            }
                            else
                            {
                                if (showMessages)
                                {
                                    MessageBox.Show("No pending slip found for Vehicle: " + rawVeh, 
                                                    "Record Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (showMessages)
                    MessageBox.Show("Error searching Vehicle No: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public void RefreshVehicleAutoComplete()
        {
            try
            {
                AutoCompleteStringCollection ac = new AutoCompleteStringCollection();
                using (SqlConnection con = new SqlConnection(db_Connection.connection_string()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT vehical_no FROM measure WHERE (net_weight IS NULL OR LTRIM(RTRIM(net_weight)) = '' OR net_weight = 'NULL') AND vehical_no IS NOT NULL AND LTRIM(RTRIM(vehical_no)) <> ''", con))
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string v = dr[0] != null ? dr[0].ToString().Trim() : "";
                            if (!string.IsNullOrEmpty(v))
                            {
                                ac.Add(v);
                            }
                        }
                    }
                }
                if (Vehical_box.InvokeRequired)
                {
                    Vehical_box.BeginInvoke((MethodInvoker)delegate
                    {
                        Vehical_box.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        Vehical_box.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        Vehical_box.AutoCompleteCustomSource = ac;
                    });
                }
                else
                {
                    Vehical_box.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    Vehical_box.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    Vehical_box.AutoCompleteCustomSource = ac;
                }
            }
            catch { }
        }

        public void RefreshPartyAutoComplete()
        {
            try
            {
                AutoCompleteStringCollection ac = new AutoCompleteStringCollection();
                using (SqlConnection con = new SqlConnection(db_Connection.connection_string()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT party FROM measure WHERE party IS NOT NULL AND LTRIM(RTRIM(party)) <> '' ORDER BY party", con))
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string p = dr[0] != null ? dr[0].ToString().Trim() : "";
                            if (!string.IsNullOrEmpty(p))
                            {
                                ac.Add(p);
                            }
                        }
                    }
                }
                if (Party_box.InvokeRequired)
                {
                    Party_box.BeginInvoke((MethodInvoker)delegate
                    {
                        Party_box.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        Party_box.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        Party_box.AutoCompleteCustomSource = ac;
                    });
                }
                else
                {
                    Party_box.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    Party_box.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    Party_box.AutoCompleteCustomSource = ac;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RefreshPartyAutoComplete error: " + ex.Message);
            }
        }

        public void RefreshMaterialAutoComplete()
        {
            try
            {
                AutoCompleteStringCollection ac = new AutoCompleteStringCollection();
                using (SqlConnection con = new SqlConnection(db_Connection.connection_string()))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT meterial FROM measure WHERE meterial IS NOT NULL AND LTRIM(RTRIM(meterial)) <> '' ORDER BY meterial", con))
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            string m = dr[0] != null ? dr[0].ToString().Trim() : "";
                            if (!string.IsNullOrEmpty(m))
                            {
                                ac.Add(m);
                            }
                        }
                    }
                }
                if (meterial_box.InvokeRequired)
                {
                    meterial_box.BeginInvoke((MethodInvoker)delegate
                    {
                        meterial_box.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                        meterial_box.AutoCompleteSource = AutoCompleteSource.CustomSource;
                        meterial_box.AutoCompleteCustomSource = ac;
                    });
                }
                else
                {
                    meterial_box.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    meterial_box.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    meterial_box.AutoCompleteCustomSource = ac;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("RefreshMaterialAutoComplete error: " + ex.Message);
            }
        }

        private void ref_slipNo_box_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SqlConnection db_connect = new SqlConnection(db_Connection.connection_string());
                SqlDataAdapter sda = new SqlDataAdapter("Select vehical_no,date from measure where slip_no='" + ref_slipNo_box.Text + "' ", db_connect);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                //Console.WriteLine("value of dt))))" + dt.Rows[0][0]); ;
                try
                {
                    charges_drop.Text = "0";
                    Vehical_box.Text = dt.Rows[0][0].ToString();
                    ref_dateSlip_no.Text = dt.Rows[0][1].ToString();







                }
                catch (Exception ex)
                {
                    MessageBox.Show("value enterd was not currect " + ex);

                }
                finally
                {
                    db_connect.Close();
                }
            }
        }
        public void get_printvalue()
        {


        }

        private void print_btn_Click(object sender, EventArgs e)
        {
            //    pageSetupDialog1.PageSettings =
            //     new System.Drawing.Printing.PageSettings();
            //    pageSetupDialog1.PrinterSettings =
            //new System.Drawing.Printing.PrinterSettings();
            //    pageSetupDialog1.ShowNetwork = false;

            //    //Show the dialog storing the result.
            //    DialogResult result = pageSetupDialog1.ShowDialog();
            //    if (result == DialogResult.OK)
            //    {
            //        object[] results = new object[]{
            //    pageSetupDialog1.PageSettings.Margins,
            //    pageSetupDialog1.PageSettings.PaperSize,
            //    pageSetupDialog1.PageSettings.Landscape,
            //    pageSetupDialog1.PrinterSettings.PrinterName,
            //    pageSetupDialog1.PrinterSettings.PrintRange};
            //        //ListBox1.Items.AddRange(results);
            //    }
            prn_slipno = slipno_box.Text;
            prn_partyname = Party_box.Text;
            prn_vehicle_type = vechaltype_drop.Text;
            prn_charges = charges_drop.Text;
            prn_op_name = Form1.User_sessson;
            prn_t_weight = tire_weight_box.Text;
            prn_g_weight = weight_box.Text;
            prn_n_weight = net_weight_box.Text;
            prn_product = meterial_box.Text;
            t_no = tokenNo_box.Text;
            cr_date = date_box.Text;
            mdate = mod_date.Text;
            veh_no = Vehical_box.Text;
            //Print.slip_print_frm slip_Print_Frm = new Print.slip_print_frm();
            //slip_Print_Frm.Show();
            //slpprintPreviewDialog1.Document = slpprintDocument1;
            //slpprintPreviewDialog1.Height = 1;
            //slpprintPreviewDialog1.Width = 1;

            SqlConnection db_connect = new SqlConnection(db_Connection.connection_string());
            db_connect.Open();
            SqlDataAdapter sda1 = new SqlDataAdapter("select slip_no, print_count from measure  where slip_no='" + slipno_box.Text + "' ", db_connect);
            DataTable dt1 = new DataTable();
            sda1.Fill(dt1);
            // Console.WriteLine("value of plant name" + dt1.Rows[0][0]);
            if (prn_n_weight.Length > 0 && prn_n_weight != "0")
            {
                if (dt1.Rows[0][1].ToString().Length == 0)
                {
                    String qry = "update   measure set print_count='1'  where slip_no = '" + slipno_box.Text + "'";
                    SqlCommand cmd = new SqlCommand(qry, db_connect);
                    cmd.ExecuteNonQuery();
                    db_connect.Close();
                    watermark = "Original";
                }
                else
                {
                    Int32 count = (int)dt1.Rows[0][1];
                    Int32 printCount = count + 1;
                    String qry = "update   measure set print_count='" + printCount + "'  where slip_no = '" + slipno_box.Text + "'";
                    SqlCommand cmd = new SqlCommand(qry, db_connect);
                    cmd.ExecuteNonQuery();
                    db_connect.Close();
                    watermark = "Duplicate";
                }
            }

            System.Drawing.Printing.PrintDocument printDocument = new PrintDocument();
            
            // Force 1 single copy (prevents printer driver defaulting to 2 copies)
            printDocument.PrinterSettings.Copies = 1;
            
            // 1. Zero out page margins so tractor feed paper starts at exact content boundary
            printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);
            printDocument.OriginAtMargins = false;

            // 2. Select standard 6-inch continuous tractor feed paper size
            // Continuous computer stationery in India is 6" tall (or 12" perforated at 6").
            // Using 6" (600 hundredths of an inch) ensures the printer does a form feed
            // to advance the slip completely to the tear-off bar/perforation upon completion.
            PaperSize bestPaperSize = null;
            try
            {
                // 1. First match the user's custom form (WeightbridgeSlip) configured in Print Server Properties
                foreach (PaperSize ps in printDocument.PrinterSettings.PaperSizes)
                {
                    if (ps.PaperName.IndexOf("Weightbridge", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        ps.PaperName.IndexOf("Weighbridge", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        bestPaperSize = ps;
                        break;
                    }
                }

                // 2. If not found, match native 6-inch continuous tractor forms (e.g. "8 x 6 in", "10 x 6 in")
                if (bestPaperSize == null)
                {
                    foreach (PaperSize ps in printDocument.PrinterSettings.PaperSizes)
                    {
                        if (ps.PaperName.IndexOf("8 x 6 in", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            ps.PaperName.IndexOf("10 x 6 in", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            (ps.Height >= 580 && ps.Height <= 620 && ps.Width >= 700))
                        {
                            bestPaperSize = ps;
                            break;
                        }
                    }
                }
            }
            catch { }

            if (bestPaperSize != null)
            {
                printDocument.DefaultPageSettings.PaperSize = bestPaperSize;
                try
                {
                    printDocument.PrinterSettings.DefaultPageSettings.PaperSize = bestPaperSize;
                }
                catch { }
            }
            else
            {
                PaperSize customPaperSize = new PaperSize("DotMatrix_6Inch", 780, 600);
                printDocument.DefaultPageSettings.PaperSize = customPaperSize;
                try
                {
                    printDocument.PrinterSettings.DefaultPageSettings.PaperSize = customPaperSize;
                }
                catch { }
            }
            
            // 4. Attach the PrintPage event
            printDocument.PrintPage += slpprintDocument1_PrintPage;
            
            // Create and configure the PrintPreviewDialog
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.Width = 900;
            printPreviewDialog.Height = 600;
            printPreviewDialog.StartPosition = FormStartPosition.CenterScreen;
            printPreviewDialog.ShowDialog();

        }

        private void slpprintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            string m_status = "Active";
            string plantName = "WEIGHBRIDGE";
            string plantAddress = "";

            try
            {
                using (SqlConnection db_connect = new SqlConnection(db_Connection.connection_string()))
                {
                    db_connect.Open();
                    SqlDataAdapter sda = new SqlDataAdapter("Select plant_name,address from machine_master where m_status='" + m_status + "' ", db_connect);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        plantName = dt.Rows[0][0] != DBNull.Value ? dt.Rows[0][0].ToString() : "WEIGHBRIDGE";
                        plantAddress = dt.Rows[0][1] != DBNull.Value ? dt.Rows[0][1].ToString() : "";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading machine master: " + ex.Message);
            }

            Graphics g = e.Graphics;

            // 100% Crisp Dot-Matrix Text Rendering (Disables blurry anti-aliasing)
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;

            // Background Watermark (Light Gray rotated watermark)
            if (!string.IsNullOrEmpty(watermark))
            {
                Font wmFont = new Font("Arial", 32, FontStyle.Bold);
                g.RotateTransform(-20);
                g.DrawString(watermark.ToUpper(), wmFont, new SolidBrush(Color.FromArgb(230, 230, 230)), new PointF(10, 200));
                g.ResetTransform();
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None;
            }

            // Pens & Fonts optimized for Dot Matrix sharpness without text overlap
            Pen borderPen = new Pen(Color.Black, 1.5f);
            Pen thinPen = new Pen(Color.Black, 1.0f);
            Pen dashPen = new Pen(Color.Black, 1.0f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };

            Font plantFont = new Font("Arial", 11f, FontStyle.Bold);
            Font addrFont = new Font("Arial", 8.5f, FontStyle.Bold);
            Font labelFont = new Font("Arial", 9f, FontStyle.Bold);
            Font valueFont = new Font("Arial", 9f, FontStyle.Bold);
            Font weightLabelFont = new Font("Arial", 9.5f, FontStyle.Bold);
            Font weightValueFont = new Font("Arial", 12.5f, FontStyle.Bold);

            // Outer Frame dimensions (width 740px starting at X: 20 -> right border at X: 760)
            // Perfectly fits 8-inch continuous tractor paper (780px) with clean 20px margins
            float startX = 20;
            float frameWidth = 740;
            float rightX = startX + frameWidth; // 760
            float frameHeight = 325;

            // Outer Frame
            g.DrawRectangle(borderPen, startX, 2, frameWidth, frameHeight);

            // Header Separator Line
            g.DrawLine(borderPen, startX, 50, rightX, 50);

            // Plant Header & Address
            g.DrawString(plantName.ToUpper(), plantFont, Brushes.Black, new PointF(startX + 10, 8));
            if (!string.IsNullOrEmpty(plantAddress))
            {
                g.DrawString(plantAddress, addrFont, Brushes.Black, new PointF(startX + 10, 28));
            }

            // Right-aligned Copy Type Badge (ORIGINAL / DUPLICATE SLIP)
            string badgeText = (watermark ?? "ORIGINAL").ToUpper() + " SLIP";
            Font badgeFont = new Font("Arial", 9f, FontStyle.Bold);
            SizeF badgeSize = g.MeasureString(badgeText, badgeFont);
            float badgeBoxW = badgeSize.Width + 8;
            float badgeX = (rightX - 10) - badgeBoxW;
            g.DrawRectangle(borderPen, badgeX, 8, badgeBoxW, badgeSize.Height + 4);
            g.DrawString(badgeText, badgeFont, Brushes.Black, badgeX + 4, 10);

            // 4-Row Compact Details Grid (Party Name and Product on separate rows to completely avoid overlap)
            // Row 1 (Y: 58)
            g.DrawString("Slip No:", labelFont, Brushes.Black, startX + 10, 58);
            g.DrawString(main.prn_slipno ?? "", valueFont, Brushes.Black, startX + 85, 58);

            g.DrawString("Vehicle No:", labelFont, Brushes.Black, startX + 265, 58);
            g.DrawString(veh_no ?? "", valueFont, Brushes.Black, startX + 360, 58);

            g.DrawString("Date/Time:", labelFont, Brushes.Black, startX + 505, 58);
            g.DrawString(cr_date ?? "", valueFont, Brushes.Black, startX + 590, 58);

            // Row 2 (Y: 80): Party Name (Left - wide 395px area) & Final Date (Right)
            g.DrawString("Party Name:", labelFont, Brushes.Black, startX + 10, 80);
            g.DrawString(main.prn_partyname ?? "", valueFont, Brushes.Black, new RectangleF(startX + 100, 80, 395, 20));

            g.DrawString("Final Date:", labelFont, Brushes.Black, startX + 505, 80);

            string gStr = main.prn_g_weight != null ? main.prn_g_weight.Trim() : "";
            string tStr = main.prn_t_weight != null ? main.prn_t_weight.Trim() : "";

            bool isGrossNull = string.IsNullOrEmpty(gStr) || gStr.Equals("NULL", StringComparison.OrdinalIgnoreCase);
            bool isTareNull = string.IsNullOrEmpty(tStr) || tStr.Equals("NULL", StringComparison.OrdinalIgnoreCase);

            // In case of zero, print final date (only if any weight value is null/empty, do not print)
            bool shouldPrintFinalDate = !isGrossNull && !isTareNull;

            if (shouldPrintFinalDate)
            {
                string finalDateToPrint = !string.IsNullOrWhiteSpace(mdate) ? mdate : (!string.IsNullOrWhiteSpace(cr_date) ? cr_date : DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                g.DrawString(finalDateToPrint, valueFont, Brushes.Black, startX + 590, 80);
            }

            // Row 3 (Y: 102): Product (Left - wide 395px area) & Vehicle Type (Right)
            g.DrawString("Product:", labelFont, Brushes.Black, startX + 10, 102);
            g.DrawString(main.prn_product ?? "", valueFont, Brushes.Black, new RectangleF(startX + 100, 102, 395, 20));

            g.DrawString("Vehicle Type:", labelFont, Brushes.Black, startX + 505, 102);
            g.DrawString(main.prn_vehicle_type ?? "", valueFont, Brushes.Black, startX + 605, 102);

            // Row 4 (Y: 124): Challan No (Left) & Charges (Right)
            g.DrawString("Challan No:", labelFont, Brushes.Black, startX + 10, 124);
            g.DrawString(main.t_no ?? "", valueFont, Brushes.Black, startX + 100, 124);

            g.DrawString("Charges:", labelFont, Brushes.Black, startX + 505, 124);
            g.DrawString(main.prn_charges ?? "", valueFont, Brushes.Black, startX + 575, 124);

            // Divider Line before Weight Table
            g.DrawLine(thinPen, startX, 146, rightX, 146);

            // Weight Table Box (Width: 720px, Height: 68px, Y: 152 to 220)
            float tableX = startX + 10;
            float tableW = frameWidth - 20; // 720
            float tableRight = tableX + tableW; // 750
            float colW = tableW / 3f; // 240
            float tableY = 152;
            float tableH = 68;

            g.DrawRectangle(thinPen, tableX, tableY, tableW, tableH);
            g.DrawLine(thinPen, tableX + colW, tableY, tableX + colW, tableY + tableH);
            g.DrawLine(thinPen, tableX + (colW * 2), tableY, tableX + (colW * 2), tableY + tableH);
            g.DrawLine(dashPen, tableX, tableY + 22, tableRight, tableY + 22);

            // Centered Weight Table Headers & Values
            using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                // Weight Table Headers
                g.DrawString("GROSS WEIGHT", weightLabelFont, Brushes.Black, new RectangleF(tableX, tableY, colW, 22), sfCenter);
                g.DrawString("TARE WEIGHT", weightLabelFont, Brushes.Black, new RectangleF(tableX + colW, tableY, colW, 22), sfCenter);
                g.DrawString("NET WEIGHT", weightLabelFont, Brushes.Black, new RectangleF(tableX + (colW * 2), tableY, colW, 22), sfCenter);

                // Weight Table Values
                g.DrawString((main.prn_g_weight ?? "0") + " Kg", weightValueFont, Brushes.Black, new RectangleF(tableX, tableY + 22, colW, 46), sfCenter);
                g.DrawString((main.prn_t_weight ?? "0") + " Kg", weightValueFont, Brushes.Black, new RectangleF(tableX + colW, tableY + 22, colW, 46), sfCenter);
                g.DrawString((main.prn_n_weight ?? "0") + " Kg", weightValueFont, Brushes.Black, new RectangleF(tableX + (colW * 2), tableY + 22, colW, 46), sfCenter);
            }

            // Bottom Section Lines & Signatures
            g.DrawLine(thinPen, startX, 228, rightX, 228);

            g.DrawString("Operator: " + (main.prn_op_name ?? ""), labelFont, Brushes.Black, startX + 10, 236);

            // Signatures (Driver & Operator)
            float sigLineW = 180;
            float sigLineY = 290;
            float driverSigX = startX + 30;
            g.DrawLine(thinPen, driverSigX, sigLineY, driverSigX + sigLineW, sigLineY);
            g.DrawString("Driver Signature", addrFont, Brushes.Black, driverSigX + 35, sigLineY + 5);

            float opSigX = rightX - 30 - sigLineW;
            g.DrawLine(thinPen, opSigX, sigLineY, opSigX + sigLineW, sigLineY);
            g.DrawString("Operator Signature", addrFont, Brushes.Black, opSigX + 25, sigLineY + 5);

            // Signal single-page output (prevents trailing blank pages on dot matrix / PDF printers)
            e.HasMorePages = false;

            // Refresh / Clear form fields after print
            slipno_box.Clear();
            weight_box.Clear();
            tire_weight_box.Clear();
            Vehical_box.Clear();
            charges_drop.Text = "";
            date_box.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            mod_date.Clear();
            vechaltype_drop.Text = "";
            remarks_box.Clear();
            meterial_box.Clear();
            Party_box.Clear();
            net_weight_box.Clear();
            tokenNo_box.Clear();
        }

        private void label11_Click(object sender, EventArgs e)
        {

        }

        private void dataChangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Reports.Change_Data change_Data = new Reports.Change_Data();
            change_Data.Show();

        }

        private void slpprintPreviewDialog1_Load(object sender, EventArgs e)
        {

        }

        private System.Windows.Forms.Timer _clockTimer;

        private void ApplyModernTheme()
        {
            try
            {
                // Ensure initial radio button state
                if (!radioButton1.Checked && !radioButton2.Checked)
                {
                    radioButton2.Checked = true;
                    weight_chk2 = "True";
                }

                // Initialize operator session label
                lblOperatorInfo.Text = "👤 Operator: " + (string.IsNullOrEmpty(Form1.User_sessson) ? "Admin" : Form1.User_sessson) + " | Shift: General";

                // Initialize live clock
                lblLiveClock.Text = DateTime.Now.ToString("dd-MMM-yyyy  HH:mm:ss");
                if (_clockTimer == null)
                {
                    _clockTimer = new System.Windows.Forms.Timer();
                    _clockTimer.Interval = 1000;
                    _clockTimer.Tick += (s, ev) =>
                    {
                        string currentNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        lblLiveClock.Text = DateTime.Now.ToString("dd-MMM-yyyy  HH:mm:ss");

                        // If brand new slip (1st weight in progress), show current date time in 1st Wt Date
                        if (string.IsNullOrEmpty(slipno_box.Text))
                        {
                            date_box.Text = currentNow;
                        }
                    };
                    _clockTimer.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ApplyModernTheme error: " + ex.Message);
            }
        }


        private void CalculateNetWeight()
        {
            try
            {
                string gText = weight_box.Text != null ? weight_box.Text.Trim() : "";
                string tText = tire_weight_box.Text != null ? tire_weight_box.Text.Trim() : "";

                if (!string.IsNullOrEmpty(gText) && !string.IsNullOrEmpty(tText))
                {
                    int gross = 0;
                    int tare = 0;
                    int.TryParse(gText, out gross);
                    int.TryParse(tText, out tare);

                    int net = gross - tare;
                    if (net < 0) net = 0;
                    net_weight_box.Text = net.ToString();
                }
                else
                {
                    net_weight_box.Text = "";
                }
            }
            catch { }
        }

        private void main_Modern_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                button1_Click(button1, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F2)
            {
                save_main_Click(save_main, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F3)
            {
                print_btn_Click(print_btn, EventArgs.Empty);
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F4)
            {
                radioButton1.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.F5)
            {
                radioButton2.Checked = true;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                if (MessageBox.Show("Are you sure you want to exit?", "Exit Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    button2_Click(button2, EventArgs.Empty);
                }
                e.Handled = true;
            }
        }

        private void main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_clockTimer != null)
            {
                _clockTimer.Stop();
                _clockTimer.Dispose();
            }
            Environment.Exit(0);
            Application.Exit();
        }
    }
}

