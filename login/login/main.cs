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

            // Console.WriteLine("value of nullees@!@!@" + dt.Rows[0][0]);

            //if (String.IsNullOrEmpty(weight_box.Text) && String.IsNullOrEmpty(tire_weight_box.Text))
            //{
            //    MessageBox.Show("Tare Weight Or Gross Weight Should Not Be Empty");
            //}
            if (String.IsNullOrEmpty(Vehical_box.Text))
            {
                MessageBox.Show("Please Enter Vehical No");
            }

            if (String.IsNullOrEmpty(slipno_box.Text))
            {
                Console.WriteLine("value of vechlbox%^&*" + Vehical_box.Text);
                SqlConnection db_connect1 = new SqlConnection(db_Connection.connection_string());
                SqlDataAdapter sda = new SqlDataAdapter("Select net_weight,slip_no from measure where vehical_no ='" + Vehical_box.Text.Trim() + "' and net_weight = '' or net_weight = 'NULL' ", db_connect1);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    MessageBox.Show("Pending Slip against vehiclenom " + Vehical_box.Text + " slip no " + dt.Rows[0][1]);
                }
                else
                {
                    SqlConnection db_connect = new SqlConnection(db_Connection.connection_string());
                    db_connect.Open();
                    string qry = null;
                    if (String.IsNullOrEmpty(slipno_box.Text))
                    {

                        // slip no gen
                        string mrg = " ";
                        String active_m = LISTNER.active_mech.Trim();
                        string dt_chk = get_slipseq().Substring(2, 6);
                        Console.WriteLine("valueo f dtchk##" + dt_chk);
                        string kk = get_slipseq().Substring(8);
                        string gen_slipno = "";
                        Console.WriteLine("KK VALUE@@@@" + kk);
                        int slipseq = int.Parse(kk);
                        slipseq++;
                        var slipseqdate = DateTime.Now.ToString("yyMMdd");
                        // compair db date and current date
                        if (int.Parse(dt_chk) != int.Parse(slipseqdate))
                        {
                            slipseq = 1;
                            kk = "0";
                            mrg = "000" + slipseq.ToString();
                            gen_slipno = active_m + slipseqdate + mrg;
                        }
                        else
                        {

                            if (int.Parse(kk) < 10)
                            {
                                mrg = "000" + slipseq.ToString();

                                gen_slipno = active_m + slipseqdate + mrg;
                            }
                            if (int.Parse(kk) > 9 && int.Parse(kk) < 99)
                            {
                                mrg = "00" + slipseq.ToString();

                                gen_slipno = active_m + slipseqdate + mrg;
                            }
                            if (int.Parse(kk) > 99 && int.Parse(kk) < 999)
                            {
                                mrg = "0" + slipseq.ToString();

                                gen_slipno = active_m + slipseqdate + mrg;
                            }
                            if (int.Parse(kk) > 999 && int.Parse(kk) < 9999)
                            {
                                gen_slipno = active_m + slipseqdate + slipseq;

                            }
                        }
                        //halt after 99999
                        if (slipseq > 9999)
                        {

                            MessageBox.Show("Max Slip No is 9999");
                            this.Close();

                        }
            ;






                        MessageBox.Show(gen_slipno);

                        //int? gross_weight = int.Parse(weight_box.Text.ToString());
                        //int? tare_weight = int.Parse(tire_weight_box.Text.ToString());

                        string get_session = Form1.User_sessson.ToString();

                        try
                        {


                            //string slip_no = slipno_box.Text.ToString();
                            //string weigh = weight_box.Text.ToString();
                            int? net_weight = null;



                            DateTime curr_date = DateTime.Now;
                            string StringFormat = "yyy-MM-dd HH:MM:ss";

                            string ss = curr_date.ToString(StringFormat);
                            date_box.Text = ss;

                            Console.WriteLine("Date*&*&**" + ss);

                            slipno_box.Text = gen_slipno.ToString();
                            qry = "insert into measure(slip_no, weight, tire_weight, net_weight, vehical_no, party, meterial, vehical_type, charges, date,remarks, user_name,token_no) values('" + gen_slipno + "','" + weight_box.Text + "', '" + tire_weight_box.Text + "', '" + net_weight + "',  '" + Vehical_box.Text + "', '" + Party_box.Text + "', '" + meterial_box.Text + "','" + vechaltype_drop.Text + "','" + charges_drop.Text + "','" + curr_date.ToString(StringFormat) + "', '" + remarks_box.Text + "','" + get_session + "','" + tokenNo_box.Text + "')";
                            String slipchkqry = ("Select net_weight from measure where  ='" + Vehical_box.Text + "' and net_weight = '' or net_weight = 'NULL'; ");

                            SqlCommand cmd = new SqlCommand(qry, db_connect);
                            cmd.ExecuteNonQuery();



                            // MessageBox.Show("data save");

                            db_connect.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("exception" + ex);
                        }
                        finally
                        {
                            db_connect.Close();
                        }

                    }

                }

            }

            else
            {


                //try
                //{

                //to update slip no 
                DateTime curr_date = DateTime.Now;
                string StringFormat = "yyy-MM-dd HH:MM:ss";

                string ss = curr_date.ToString(StringFormat);
                mod_date.Text = ss.ToString();
                Console.WriteLine("net%%#%#%#" + ss);
                mod_date.Text = ss.ToString();

                int net_weight = int.Parse(weight_box.Text) - int.Parse(tire_weight_box.Text);

                net_weight_box.Text = net_weight.ToString();

                SqlConnection db_connect_1 = new SqlConnection(db_Connection.connection_string());


                db_connect_1.Open();

                SqlCommand SqlComm = new SqlCommand("UPDATE measure SET weight=@weight , tire_weight=@tire_weight, net_weight=@net_weight ,  party=@party , meterial=@meterial , vehical_type=@vehical_type , charges=@charges , date=@date , remarks=@remarks, mod_date=@mod_date , mod_by=@mod_by  where slip_no=@slip_no", db_connect_1);
                Console.WriteLine("DB CONNECT " + db_connect_1);

                SqlComm.Parameters.AddWithValue("@weight", weight_box.Text);
                SqlComm.Parameters.AddWithValue("@tire_weight", tire_weight_box.Text);
                SqlComm.Parameters.AddWithValue("@net_weight", net_weight.ToString());
                // SqlComm.Parameters.AddWithValue("@vehical_no", Vehical_box.Text);
                SqlComm.Parameters.AddWithValue("@party", Party_box.Text);
                SqlComm.Parameters.AddWithValue("@meterial", meterial_box.Text);
                SqlComm.Parameters.AddWithValue("@vehical_type", vechaltype_drop.Text);
                SqlComm.Parameters.AddWithValue("@charges", charges_drop.Text);
                SqlComm.Parameters.AddWithValue("@date", date_box.Text);

                SqlComm.Parameters.AddWithValue("@remarks", remarks_box.Text);

                SqlComm.Parameters.AddWithValue("@mod_date", ss.ToString());
                SqlComm.Parameters.AddWithValue("@mod_by", Form1.User_sessson.ToString());
                SqlComm.Parameters.AddWithValue("@slip_no", slipno_box.Text);

                SqlComm.ExecuteNonQuery();
                db_connect_1.Close();
                MessageBox.Show("Slip no Updated");



            }
        }


        private void save_main_Click(object sender, EventArgs e)

        {

            // Console.WriteLine("value of nullees@!@!@" + dt.Rows[0][0]);

            //if (String.IsNullOrEmpty(weight_box.Text) && String.IsNullOrEmpty(tire_weight_box.Text))
            //{
            //    MessageBox.Show("Tare Weight Or Gross Weight Should Not Be Empty");
            //}
            if (String.IsNullOrEmpty(Vehical_box.Text))
            {
                MessageBox.Show("Please Enter Vehical No");
            }

            if (String.IsNullOrEmpty(slipno_box.Text))
            {
                Console.WriteLine("value of vechlbox%^&*" + Vehical_box.Text);
                SqlConnection db_connect1 = new SqlConnection(db_Connection.connection_string());
                SqlDataAdapter sda = new SqlDataAdapter("Select net_weight,slip_no from measure where vehical_no ='" + Vehical_box.Text.Trim() + "' and net_weight = '' or net_weight = 'NULL' ", db_connect1);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                if (dt.Rows.Count == 1)
                {
                    MessageBox.Show("Pending Slip against vehiclenom " + Vehical_box.Text + " slip no " + dt.Rows[0][1]);
                }
                else
                {
                    SqlConnection db_connect = new SqlConnection(db_Connection.connection_string());
                    db_connect.Open();
                    string qry = null;
                    if (String.IsNullOrEmpty(slipno_box.Text))
                    {

                        // slip no gen
                        string mrg = " ";
                        String active_m = LISTNER.active_mech.Trim();
                        string dt_chk = get_slipseq().Substring(2, 6);
                        Console.WriteLine("valueo f dtchk##" + dt_chk);
                        string kk = get_slipseq().Substring(8);
                        string gen_slipno = "";
                        Console.WriteLine("KK VALUE@@@@" + kk);
                        int slipseq = int.Parse(kk);
                        slipseq++;
                        var slipseqdate = DateTime.Now.ToString("yyMMdd");
                        // compair db date and current date
                        if (int.Parse(dt_chk) != int.Parse(slipseqdate))
                        {
                            slipseq = 1;
                            kk = "0";
                            mrg = "000" + slipseq.ToString();
                            gen_slipno = active_m + slipseqdate + mrg;
                        }
                        else
                        {

                            if (slipseq < 10)
                            {
                                mrg = "000" + slipseq.ToString();

                                gen_slipno = active_m + slipseqdate + mrg;
                            }
                            if (slipseq > 9 && slipseq < 100)
                            {
                                mrg = "00" + slipseq.ToString();

                                gen_slipno = active_m + slipseqdate + mrg;
                            }
                            if (slipseq > 99 && slipseq < 1000)
                            {
                                mrg = "0" + slipseq.ToString();

                                gen_slipno = active_m + slipseqdate + mrg;
                            }
                            if (slipseq > 999 && slipseq < 9999)
                            {
                                gen_slipno = active_m + slipseqdate + slipseq;

                            }
                        }
                        //halt after 99999
                        if (slipseq > 9999)
                        {

                            MessageBox.Show("Max Slip No is 9999");
                            this.Close();

                        }
            ;






                        MessageBox.Show(gen_slipno);

                        //int? gross_weight = int.Parse(weight_box.Text.ToString());
                        //int? tare_weight = int.Parse(tire_weight_box.Text.ToString());

                        string get_session = Form1.User_sessson.ToString();

                        try
                        {


                            //string slip_no = slipno_box.Text.ToString();
                            //string weigh = weight_box.Text.ToString();
                            int? net_weight = null;



                            DateTime curr_date = DateTime.Now;
                            string StringFormat = "yyy-MM-dd HH:MM:ss";

                            string ss = curr_date.ToString(StringFormat);
                            date_box.Text = ss;

                            Console.WriteLine("Date*&*&**" + ss);

                            slipno_box.Text = gen_slipno.ToString();
                            qry = "insert into measure(slip_no, weight, tire_weight, net_weight, vehical_no, party, meterial, vehical_type, charges, date,remarks, user_name,token_no) values('" + gen_slipno + "','" + weight_box.Text + "', '" + tire_weight_box.Text + "', '" + net_weight + "',  '" + Vehical_box.Text + "', '" + Party_box.Text + "', '" + meterial_box.Text + "','" + vechaltype_drop.Text + "','" + charges_drop.Text + "','" + curr_date.ToString(StringFormat) + "', '" + remarks_box.Text + "','" + get_session + "','" + tokenNo_box.Text + "')";
                            String slipchkqry = ("Select net_weight from measure where  ='" + Vehical_box.Text + "' and net_weight = '' or net_weight = 'NULL'; ");

                            SqlCommand cmd = new SqlCommand(qry, db_connect);
                            cmd.ExecuteNonQuery();



                            // MessageBox.Show("data save");

                            db_connect.Close();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("exception" + ex);
                        }
                        finally
                        {
                            db_connect.Close();
                        }

                    }

                }

            }

            else
            {


                //try
                //{

                //to update slip no 
                DateTime curr_date = DateTime.Now;
                string StringFormat = "yyy-MM-dd HH:MM:ss";

                string ss = curr_date.ToString(StringFormat);
                mod_date.Text = ss.ToString();
                Console.WriteLine("net%%#%#%#" + ss);
                mod_date.Text = ss.ToString();

                int net_weight = int.Parse(weight_box.Text) - int.Parse(tire_weight_box.Text);

                net_weight_box.Text = net_weight.ToString();

                SqlConnection db_connect_1 = new SqlConnection(db_Connection.connection_string());


                db_connect_1.Open();

                SqlCommand SqlComm = new SqlCommand("UPDATE measure SET weight=@weight , tire_weight=@tire_weight, net_weight=@net_weight ,  party=@party , meterial=@meterial , vehical_type=@vehical_type , charges=@charges , date=@date , remarks=@remarks, mod_date=@mod_date , mod_by=@mod_by  where slip_no=@slip_no", db_connect_1);
                Console.WriteLine("DB CONNECT " + db_connect_1);

                SqlComm.Parameters.AddWithValue("@weight", weight_box.Text);
                SqlComm.Parameters.AddWithValue("@tire_weight", tire_weight_box.Text);
                SqlComm.Parameters.AddWithValue("@net_weight", net_weight.ToString());
                // SqlComm.Parameters.AddWithValue("@vehical_no", Vehical_box.Text);
                SqlComm.Parameters.AddWithValue("@party", Party_box.Text);
                SqlComm.Parameters.AddWithValue("@meterial", meterial_box.Text);
                SqlComm.Parameters.AddWithValue("@vehical_type", vechaltype_drop.Text);
                SqlComm.Parameters.AddWithValue("@charges", charges_drop.Text);
                SqlComm.Parameters.AddWithValue("@date", date_box.Text);

                SqlComm.Parameters.AddWithValue("@remarks", remarks_box.Text);

                SqlComm.Parameters.AddWithValue("@mod_date", ss.ToString());
                SqlComm.Parameters.AddWithValue("@mod_by", Form1.User_sessson.ToString());
                SqlComm.Parameters.AddWithValue("@slip_no", slipno_box.Text);

                SqlComm.ExecuteNonQuery();
                db_connect_1.Close();
                MessageBox.Show("Slip no Updated");



            }
        }


        private void populate_Click(object sender, EventArgs e)
        {

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
                string searchSlip = slipno_box.Text.Trim();
                if (string.IsNullOrEmpty(searchSlip))
                {
                    MessageBox.Show("Please enter a Slip No to search.", "Search Slip", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                try
                {
                    using (SqlConnection db_connect = new SqlConnection(db_Connection.connection_string()))
                    {
                        db_connect.Open();
                        SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM measure WHERE slip_no = @slip_no", db_connect);
                        sda.SelectCommand.Parameters.AddWithValue("@slip_no", searchSlip);
                        DataTable dt = new DataTable();
                        sda.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            DataRow row = dt.Rows[0];
                            slipno_box.Text = row["slip_no"].ToString().Trim();
                            weight_box.Text = row["weight"] != DBNull.Value ? row["weight"].ToString().Trim() : "";
                            tire_weight_box.Text = row["tire_weight"] != DBNull.Value ? row["tire_weight"].ToString().Trim() : "";
                            net_weight_box.Text = row["net_weight"] != DBNull.Value ? row["net_weight"].ToString().Trim() : "";
                            Vehical_box.Text = row["vehical_no"] != DBNull.Value ? row["vehical_no"].ToString().Trim() : "";
                            Party_box.Text = row["party"] != DBNull.Value ? row["party"].ToString().Trim() : "";
                            meterial_box.Text = row["meterial"] != DBNull.Value ? row["meterial"].ToString().Trim() : "";
                            vechaltype_drop.Text = row["vehical_type"] != DBNull.Value ? row["vehical_type"].ToString().Trim() : "";
                            charges_drop.Text = row["charges"] != DBNull.Value ? row["charges"].ToString().Trim() : "";
                            date_box.Text = row["date"] != DBNull.Value ? row["date"].ToString().Trim() : "";
                            remarks_box.Text = row["remarks"] != DBNull.Value ? row["remarks"].ToString().Trim() : "";
                            mod_date.Text = row["mod_date"] != DBNull.Value ? row["mod_date"].ToString().Trim() : "";
                            tokenNo_box.Text = row["token_no"] != DBNull.Value ? row["token_no"].ToString().Trim() : "";

                            // Calculate Net Weight
                            CalculateNetWeight();

                            // Automatically suggest the next weighment mode for 2nd weight:
                            if (!string.IsNullOrEmpty(tire_weight_box.Text) && string.IsNullOrEmpty(weight_box.Text))
                            {
                                radioButton2.Checked = true; // Set to Gross Weight
                            }
                            else if (!string.IsNullOrEmpty(weight_box.Text) && string.IsNullOrEmpty(tire_weight_box.Text))
                            {
                                radioButton1.Checked = true; // Set to Tare Weight
                            }

                            MessageBox.Show("Record loaded successfully for Slip No: " + searchSlip, "Slip Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("No record found for Slip No: " + searchSlip, "Record Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error searching Slip No: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private int calculate_netweight()
        {
            int weight = int.Parse(weight_box.Text);
            int tire_weight = int.Parse(tire_weight_box.Text);
            int net_weight = weight - tire_weight;
            return net_weight;

        }

        private void tire_weight_box_TextChanged(object sender, EventArgs e)
        {
            CalculateNetWeight();
        }
        public string get_slipseq()
        {

            SqlConnection db_connect = new SqlConnection(db_Connection.connection_string());
            db_connect.Open();
            String mk = LISTNER.active_mech.ToString().Replace(" ", ""); ;
            Console.WriteLine("value of mk33" + mk);
            SqlDataAdapter sda = new SqlDataAdapter("Select max(slip_no) from measure where slip_no like '" + mk + "%' ", db_connect);

            DataTable dt = new DataTable();

            sda.Fill(dt);
            Console.WriteLine("value of dt &&##&@&#@&" + dt.Rows[0][0]);
            if (dt.Rows[0][0].ToString() == "")
            {
                String active_m = LISTNER.active_mech.Replace(" ", "");
                var slipseqdate = DateTime.Now.ToString("yyMMdd");

                String gen_slipno = "0000";
                gen_slipno = active_m + slipseqdate + gen_slipno;
                Console.WriteLine("value of sq!!!!" + gen_slipno);
                return gen_slipno;

            }
            else
            {
                db_connect.Close();
                return dt.Rows[0][0].ToString();

            }
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
            pending_Slips.Show();
        }

        private void Vehical_box_TextChanged(object sender, EventArgs e)
        {

        }

        private void Vehical_box_KeyPress(object sender, KeyPressEventArgs e)
        {
            //var regex = new Regex(@"[^a-zA-Z0-9\s]");
            //if (regex.IsMatch(e.KeyChar.ToString()))
            //{
            //    e.Handled = true;
            //}

            e.Handled = Char.IsPunctuation(e.KeyChar) ||
                       Char.IsSeparator(e.KeyChar) ||
                       Char.IsSymbol(e.KeyChar);

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
                SqlConnection db_connect = new SqlConnection(db_Connection.connection_string());
                SqlDataAdapter sda = new SqlDataAdapter("Select * from measure where vehical_no='" + Vehical_box.Text + "' and (net_weight = '' or net_weight = 'NULL' ) ", db_connect);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                try
                {
                    if (dt.Rows.Count > 0)
                    {

                        slipno_box.Text = dt.Rows[0][0].ToString().Trim();
                        weight_box.Text = dt.Rows[0][1].ToString().Trim();
                        tire_weight_box.Text = dt.Rows[0][2].ToString().Trim();
                        net_weight_box.Text = dt.Rows[0][3].ToString().Trim();
                        Party_box.Text = dt.Rows[0][5].ToString().Trim();

                        meterial_box.Text = dt.Rows[0][6].ToString().Trim();
                        vechaltype_drop.Text = dt.Rows[0][7].ToString().Trim();
                        charges_drop.Text = dt.Rows[0][8].ToString().Trim();
                        date_box.Text = dt.Rows[0][9].ToString().Trim();
                        remarks_box.Text = dt.Rows[0][10].ToString().Trim();
                        mod_date.Text = dt.Rows[0][13].ToString().Trim();
                        Console.WriteLine("value of challan@@#@" + dt.Rows[0][13].ToString().Trim());
                        tokenNo_box.Text = dt.Rows[0][14].ToString().Trim();

                    }

                    else
                    {
                        MessageBox.Show("Record not Found");
                    }
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

            // slpprintPreviewDialog1.ShowDialog();

            System.Drawing.Printing.PrintDocument printDocument = new PrintDocument();
            // Set custom page size (Width x Height in inches)
            PaperSize customPaperSize = new PaperSize("Custom Size", 750, 550); // Width = 850px, Height = 1100px
            printDocument.DefaultPageSettings.PaperSize = customPaperSize;
            // You can also set other page properties, like margins, if needed
            printDocument.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);
            // Attach the PrintPage event
            printDocument.PrintPage += slpprintDocument1_PrintPage;
            // Create and configure the PrintPreviewDialog
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;
            // Set the size of the print preview dialog
            printPreviewDialog.Width = 800;  // Set the width of the dialog (in pixels)
            printPreviewDialog.Height = 700; // Set the height of the dialog (in pixels)
            // Optionally, you can set the dialog's location on the screen
            printPreviewDialog.StartPosition = FormStartPosition.CenterScreen;
            // Show the PrintPreviewDialog
            printPreviewDialog.ShowDialog();

        }

        private void slpprintDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // e.Graphics.DrawString(label1.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(250, 25));
            //e.Graphics.DrawString(addressalble.Text, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(225, 50));
            string m_status = "Active";
            SqlConnection db_connect = new SqlConnection(db_Connection.connection_string());
            db_connect.Open();
            SqlDataAdapter sda = new SqlDataAdapter("Select plant_name,address from machine_master where m_status='" + m_status + "' ", db_connect);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            Console.WriteLine("value of plant name" + dt.Rows[0][0]);
            Console.WriteLine("value of plant name" + dt.Rows[0][1]);
            // string watermark = "Orignal Print";
            // Image Image = Image.FromFile("D:/logo-main.png");
            // System.Drawing.Graphics gpr = Graphics.FromImage(Image);
            System.Drawing.Brush brush = new SolidBrush(System.Drawing.Color.Red);
            Font font = new System.Drawing.Font("Arial", 55, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);

            SizeF size = e.Graphics.MeasureString(watermark, font);

            float y = 10;
            float x = 50;

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;

            SizeF txt = e.Graphics.MeasureString(Text, this.Font);
            SizeF sz = e.Graphics.VisibleClipBounds.Size;
            RectangleF printArea = new RectangleF(x, y, size.Width, size.Height);
            e.Graphics.RotateTransform(45);
            e.Graphics.DrawString(watermark, font, Brushes.LightGray, new RectangleF(0, 0, sz.Height, sz.Width), format);

            // e.Graphics.DrawString(watermark, font, Brushes.LightGray, printArea);
            e.Graphics.ResetTransform();
            //e.PageSettings.PaperSize.Width = 214;
            //e.PageSettings.PaperSize.Height= 105;
            //  e.PageSettings.PrinterSettings.DefaultPageSettings.PaperSize = ;
            e.Graphics.DrawString("Vehical No:  " + veh_no, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(350, 250));
            e.Graphics.DrawString("Create Date:  " + cr_date, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(350, 300));
            e.Graphics.DrawString("Final Date:  " + mdate, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(350, 350));
            e.Graphics.DrawString(dt.Rows[0][0].ToString(), new Font("Arial", 12, FontStyle.Bold), Brushes.Black, new Point(150, 25));
            // e.Graphics.DrawString(dt.Rows[0][1].ToString(), new Font("Arial", 9, FontStyle.Bold), Brushes.Black, new Point(90, 50));
            // Define the font and header text
            string headerText = dt.Rows[0][1].ToString();
            Font headerFont = new Font("Arial", 9, FontStyle.Bold);
            // Set the starting point for the header
            PointF headerPoint = new PointF(70, 50);
            // Draw the header text
            e.Graphics.DrawString(headerText, headerFont, Brushes.Black, headerPoint);
            // Measure the size of the header text to calculate the underline position
            SizeF headerSize = e.Graphics.MeasureString(headerText, headerFont);
            // Draw a line underneath the header
            float underlineY = headerPoint.Y + headerSize.Height + 5;  // 5 pixels below the header
            e.Graphics.DrawLine(Pens.Black, headerPoint.X, underlineY, headerPoint.X + headerSize.Width, underlineY);
            // Optional: You can add more content below the header if needed
            e.Graphics.DrawString("Slip No:  " + main.prn_slipno, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 100));
            e.Graphics.DrawString("Party Name:  " + main.prn_partyname, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(350, 100));
            e.Graphics.DrawString("Challan No:  " + main.t_no, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 150));
            e.Graphics.DrawString("Product:  " + main.prn_product, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(350, 150));
            e.Graphics.DrawString("Vehicle Type:  " + main.prn_vehicle_type, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 200));
            e.Graphics.DrawString("Charges:  " + main.prn_charges, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(350, 200));
            e.Graphics.DrawString("Operator Name:  " + main.prn_op_name, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(350, 400));
            e.Graphics.DrawString("Operator Sign:  ", new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 400));
            e.Graphics.DrawString("Tare Weight:  " + main.prn_t_weight, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 250));
            e.Graphics.DrawString("Gross Weight:  " + main.prn_g_weight, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 300));
            e.Graphics.DrawString("Net Weight:  " + main.prn_n_weight, new Font("Arial", 12, FontStyle.Regular), Brushes.Black, new Point(25, 350));
            //refresh----------------
            slipno_box.Clear();
            weight_box.Clear();
            tire_weight_box.Clear();
            Vehical_box.Clear();
            charges_drop.Text = "";
            date_box.Clear();
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
                this.SuspendLayout();
                this.Text = "BSPL Weighbridge Automation System - V2.0";
                this.BackColor = Color.FromArgb(240, 244, 248);
                this.BackgroundImage = null;
                this.KeyPreview = true;
                this.KeyDown += main_Modern_KeyDown;

                // Ensure initial radio button state
                if (!radioButton1.Checked && !radioButton2.Checked)
                {
                    radioButton2.Checked = true;
                    weight_chk2 = "True";
                }

                // 1. ToolStrip / Menu Bar styling
                toolStrip1.BackColor = Color.FromArgb(15, 23, 42); // Dark slate
                toolStrip1.ForeColor = Color.White;
                toolStrip1.GripStyle = ToolStripGripStyle.Hidden;
                toolStrip1.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
                toolStrip1.Padding = new Padding(8, 4, 8, 4);

                foreach (ToolStripItem item in toolStrip1.Items)
                {
                    item.ForeColor = Color.White;
                    if (item is ToolStripDropDownItem dropDown)
                    {
                        dropDown.DropDown.BackColor = Color.FromArgb(30, 41, 59);
                        dropDown.DropDown.ForeColor = Color.White;
                        foreach (ToolStripItem subItem in dropDown.DropDownItems)
                        {
                            subItem.BackColor = Color.FromArgb(30, 41, 59);
                            subItem.ForeColor = Color.White;
                        }
                    }
                }

                // Detach existing controls from direct form
                this.Controls.Remove(tableLayoutPanel1);
                this.Controls.Remove(tableLayoutPanel2);
                this.Controls.Remove(cuttent_weight_box);
                this.Controls.Remove(button1);
                this.Controls.Remove(radioButton1);
                this.Controls.Remove(radioButton2);
                this.Controls.Remove(save_main);
                this.Controls.Remove(print_btn);
                this.Controls.Remove(button2);
                this.Controls.Remove(ref_dateSlip_no);
                this.Controls.Remove(label14);
                this.Controls.Remove(ref_slipNo_box);

                // 2. Header Banner
                Panel headerPanel = new Panel();
                headerPanel.Dock = DockStyle.Top;
                headerPanel.Height = 62;
                headerPanel.BackColor = Color.FromArgb(15, 23, 42);
                headerPanel.Padding = new Padding(15, 8, 15, 8);

                Label lblHeaderTitle = new Label();
                lblHeaderTitle.Text = "⚖️  WEIGHBRIDGE AUTOMATION SYSTEM";
                lblHeaderTitle.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
                lblHeaderTitle.ForeColor = Color.White;
                lblHeaderTitle.AutoSize = true;
                lblHeaderTitle.Location = new Point(15, 10);

                Label lblHeaderSubtitle = new Label();
                lblHeaderSubtitle.Text = "Aarogya Dairy Products Private Limited  |  Electronic Pitless Weighbridge";
                lblHeaderSubtitle.Font = new Font("Segoe UI", 8.5f, FontStyle.Regular);
                lblHeaderSubtitle.ForeColor = Color.FromArgb(148, 163, 184);
                lblHeaderSubtitle.AutoSize = true;
                lblHeaderSubtitle.Location = new Point(18, 36);

                Label lblLiveClock = new Label();
                lblLiveClock.Text = DateTime.Now.ToString("dd-MMM-yyyy  HH:mm:ss");
                lblLiveClock.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
                lblLiveClock.ForeColor = Color.FromArgb(56, 189, 248);
                lblLiveClock.AutoSize = true;
                lblLiveClock.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                lblLiveClock.Location = new Point(headerPanel.Width - 250, 10);

                Label lblOperatorInfo = new Label();
                lblOperatorInfo.Text = "👤 Operator: " + (string.IsNullOrEmpty(Form1.User_sessson) ? "Admin" : Form1.User_sessson) + " | Shift: General";
                lblOperatorInfo.Font = new Font("Segoe UI", 8.5f, FontStyle.Regular);
                lblOperatorInfo.ForeColor = Color.FromArgb(203, 213, 225);
                lblOperatorInfo.AutoSize = true;
                lblOperatorInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                lblOperatorInfo.Location = new Point(headerPanel.Width - 250, 36);

                headerPanel.Controls.Add(lblHeaderTitle);
                headerPanel.Controls.Add(lblHeaderSubtitle);
                headerPanel.Controls.Add(lblLiveClock);
                headerPanel.Controls.Add(lblOperatorInfo);

                _clockTimer = new System.Windows.Forms.Timer();
                _clockTimer.Interval = 1000;
                _clockTimer.Tick += (s, ev) =>
                {
                    lblLiveClock.Text = DateTime.Now.ToString("dd-MMM-yyyy  HH:mm:ss");
                    lblLiveClock.Location = new Point(headerPanel.Width - lblLiveClock.Width - 25, 10);
                    lblOperatorInfo.Location = new Point(headerPanel.Width - lblOperatorInfo.Width - 25, 36);
                };
                _clockTimer.Start();

                // 3. Bottom Action Bar
                Panel bottomBar = new Panel();
                bottomBar.Dock = DockStyle.Bottom;
                bottomBar.Height = 65;
                bottomBar.BackColor = Color.White;
                bottomBar.Padding = new Padding(20, 10, 20, 10);
                bottomBar.Paint += (s, pe) =>
                {
                    pe.Graphics.DrawLine(new Pen(Color.FromArgb(226, 232, 240), 1), 0, 0, bottomBar.Width, 0);
                };

                Label lblShortcuts = new Label();
                lblShortcuts.Text = "Shortcuts: [F1] Capture Weight  |  [F2] Save Slip  |  [F3] Print  |  [F4] Tare Mode  |  [F5] Gross Mode  |  [Esc] Exit";
                lblShortcuts.Font = new Font("Segoe UI", 9f, FontStyle.Italic);
                lblShortcuts.ForeColor = Color.FromArgb(100, 116, 139);
                lblShortcuts.AutoSize = true;
                lblShortcuts.Location = new Point(20, 22);
                bottomBar.Controls.Add(lblShortcuts);

                FlowLayoutPanel actionButtonsPanel = new FlowLayoutPanel();
                actionButtonsPanel.Dock = DockStyle.Right;
                actionButtonsPanel.FlowDirection = FlowDirection.LeftToRight;
                actionButtonsPanel.AutoSize = true;
                actionButtonsPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                actionButtonsPanel.WrapContents = false;
                actionButtonsPanel.BackColor = Color.Transparent;
                actionButtonsPanel.Padding = new Padding(0, 10, 20, 0);

                StyleButton(save_main, "💾  SAVE SLIP (F2)", Color.FromArgb(37, 99, 235), Color.White, new Size(150, 42));
                StyleButton(print_btn, "🖨️  PRINT SLIP (F3)", Color.FromArgb(13, 148, 136), Color.White, new Size(150, 42));
                StyleButton(button2, "🚪  EXIT (Esc)", Color.FromArgb(220, 38, 38), Color.White, new Size(130, 42));

                save_main.Margin = new Padding(0, 0, 10, 0);
                print_btn.Margin = new Padding(0, 0, 10, 0);
                button2.Margin = new Padding(0, 0, 0, 0);

                actionButtonsPanel.Controls.Add(save_main);
                actionButtonsPanel.Controls.Add(print_btn);
                actionButtonsPanel.Controls.Add(button2);
                bottomBar.Controls.Add(actionButtonsPanel);

                // 4. Main Scrollable Container
                Panel mainContainer = new Panel();
                mainContainer.Dock = DockStyle.Fill;
                mainContainer.AutoScroll = true;
                mainContainer.Padding = new Padding(20, 15, 20, 15);
                mainContainer.BackColor = Color.FromArgb(240, 244, 248);

                // 5. Hero Live Scale Console Card
                Panel heroScaleCard = new Panel();
                heroScaleCard.Dock = DockStyle.Top;
                heroScaleCard.Height = 120;
                heroScaleCard.BackColor = Color.FromArgb(30, 41, 59); // Slate-800
                heroScaleCard.Padding = new Padding(15, 12, 15, 12);
                heroScaleCard.Margin = new Padding(0, 0, 0, 15);

                // Digital LED Display Bezel
                Panel digitalBezel = new Panel();
                digitalBezel.BackColor = Color.FromArgb(2, 6, 23); // Jet Black
                digitalBezel.Size = new Size(350, 96);
                digitalBezel.Location = new Point(15, 12);
                digitalBezel.BorderStyle = BorderStyle.FixedSingle;

                Label lblScaleStatus = new Label();
                lblScaleStatus.Text = "● SCALE LIVE INDICATOR";
                lblScaleStatus.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
                lblScaleStatus.ForeColor = Color.FromArgb(16, 185, 129); // Emerald-500
                lblScaleStatus.Location = new Point(12, 6);
                lblScaleStatus.AutoSize = true;

                Label lblUnitKg = new Label();
                lblUnitKg.Text = "KG";
                lblUnitKg.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
                lblUnitKg.ForeColor = Color.FromArgb(100, 116, 139);
                lblUnitKg.Location = new Point(295, 36);
                lblUnitKg.AutoSize = true;

                cuttent_weight_box.BackColor = Color.FromArgb(2, 6, 23);
                cuttent_weight_box.ForeColor = Color.FromArgb(0, 255, 157); // Electric Neon Green
                cuttent_weight_box.Font = new Font("Consolas", 36f, FontStyle.Bold);
                cuttent_weight_box.BorderStyle = BorderStyle.None;
                cuttent_weight_box.Location = new Point(8, 26);
                cuttent_weight_box.Size = new Size(280, 60);
                cuttent_weight_box.ReadOnly = true;

                digitalBezel.Controls.Add(lblScaleStatus);
                digitalBezel.Controls.Add(lblUnitKg);
                digitalBezel.Controls.Add(cuttent_weight_box);
                heroScaleCard.Controls.Add(digitalBezel);

                // Mode Selection & Capture Section
                Panel capturePanel = new Panel();
                capturePanel.BackColor = Color.Transparent;
                capturePanel.Location = new Point(385, 12);
                capturePanel.Size = new Size(550, 96);

                Label lblModeHeader = new Label();
                lblModeHeader.Text = "ACTIVE WEIGHING MODE";
                lblModeHeader.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
                lblModeHeader.ForeColor = Color.FromArgb(203, 213, 225);
                lblModeHeader.Location = new Point(5, 5);
                lblModeHeader.AutoSize = true;

                radioButton1.Text = "Tare Weight (F4)";
                radioButton1.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                radioButton1.ForeColor = Color.White;
                radioButton1.Location = new Point(5, 28);
                radioButton1.AutoSize = true;

                radioButton2.Text = "Gross Weight (F5)";
                radioButton2.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
                radioButton2.ForeColor = Color.White;
                radioButton2.Location = new Point(175, 28);
                radioButton2.AutoSize = true;

                StyleButton(button1, "⚡  CAPTURE WEIGHT (F1)", Color.FromArgb(16, 185, 129), Color.White, new Size(270, 36));
                button1.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
                button1.Location = new Point(5, 55);

                capturePanel.Controls.Add(lblModeHeader);
                capturePanel.Controls.Add(radioButton1);
                capturePanel.Controls.Add(radioButton2);
                capturePanel.Controls.Add(button1);
                heroScaleCard.Controls.Add(capturePanel);

                // Spacer
                Panel spacer1 = new Panel { Dock = DockStyle.Top, Height = 15, BackColor = Color.Transparent };

                // 6. Two-Column Card Layout for Data Entry
                TableLayoutPanel twoColumnCards = new TableLayoutPanel();
                twoColumnCards.Dock = DockStyle.Fill;
                twoColumnCards.ColumnCount = 2;
                twoColumnCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
                twoColumnCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50f));
                twoColumnCards.RowCount = 1;
                twoColumnCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
                twoColumnCards.BackColor = Color.Transparent;

                // Card Left: Vehicle & Material
                Panel cardLeft = new Panel();
                cardLeft.Dock = DockStyle.Fill;
                cardLeft.BackColor = Color.White;
                cardLeft.Padding = new Padding(16, 12, 16, 16);
                cardLeft.Margin = new Padding(0, 0, 10, 0);

                Panel pnlLeftHeader = new Panel();
                pnlLeftHeader.Dock = DockStyle.Top;
                pnlLeftHeader.Height = 36;
                pnlLeftHeader.BackColor = Color.White;

                Label lblCardLeftTitle = new Label();
                lblCardLeftTitle.Text = "🚛  VEHICLE & WEIGHMENT DETAILS";
                lblCardLeftTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
                lblCardLeftTitle.ForeColor = Color.FromArgb(30, 58, 138); // Blue-900
                lblCardLeftTitle.Dock = DockStyle.Fill;
                lblCardLeftTitle.TextAlign = ContentAlignment.MiddleLeft;
                pnlLeftHeader.Controls.Add(lblCardLeftTitle);

                cardLeft.Controls.Add(tableLayoutPanel1);
                cardLeft.Controls.Add(pnlLeftHeader);
                pnlLeftHeader.BringToFront();
                tableLayoutPanel1.Dock = DockStyle.Fill;

                // Card Right: Slip & Transaction
                Panel cardRight = new Panel();
                cardRight.Dock = DockStyle.Fill;
                cardRight.BackColor = Color.White;
                cardRight.Padding = new Padding(16, 12, 16, 16);
                cardRight.Margin = new Padding(10, 0, 0, 0);

                Panel pnlRightHeader = new Panel();
                pnlRightHeader.Dock = DockStyle.Top;
                pnlRightHeader.Height = 36;
                pnlRightHeader.BackColor = Color.White;

                Label lblCardRightTitle = new Label();
                lblCardRightTitle.Text = "📋  SLIP & CHARGES DETAILS";
                lblCardRightTitle.Font = new Font("Segoe UI", 11f, FontStyle.Bold);
                lblCardRightTitle.ForeColor = Color.FromArgb(79, 70, 229); // Indigo-700
                lblCardRightTitle.Dock = DockStyle.Fill;
                lblCardRightTitle.TextAlign = ContentAlignment.MiddleLeft;
                pnlRightHeader.Controls.Add(lblCardRightTitle);

                cardRight.Controls.Add(tableLayoutPanel2);
                cardRight.Controls.Add(pnlRightHeader);
                pnlRightHeader.BringToFront();
                tableLayoutPanel2.Dock = DockStyle.Fill;

                twoColumnCards.Controls.Add(cardLeft, 0, 0);
                twoColumnCards.Controls.Add(cardRight, 1, 0);

                // Assemble controls into mainContainer
                mainContainer.Controls.Add(twoColumnCards);
                mainContainer.Controls.Add(spacer1);
                mainContainer.Controls.Add(heroScaleCard);

                // Add to Form
                this.Controls.Add(mainContainer);
                this.Controls.Add(bottomBar);
                this.Controls.Add(headerPanel);
                this.Controls.Add(toolStrip1);

                // 7. Polish all input fields and labels in tableLayoutPanel1
                StyleInputsInTableLayout1();

                // 8. Polish all input fields and labels in tableLayoutPanel2
                StyleInputsInTableLayout2();

                this.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ApplyModernTheme error: " + ex.Message);
            }
        }

        private void StyleInputsInTableLayout1()
        {
            tableLayoutPanel1.SuspendLayout();
            tableLayoutPanel1.Controls.Clear();
            tableLayoutPanel1.BackColor = Color.White;

            tableLayoutPanel1.ColumnCount = 2;
            tableLayoutPanel1.ColumnStyles.Clear();
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150f));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            tableLayoutPanel1.RowCount = 7;
            tableLayoutPanel1.RowStyles.Clear();
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 85f));

            label1.Text = "Gross Wt (Kg)";
            label2.Text = "Tare Wt (Kg)";
            label3.Text = "Net Wt (Kg)";
            label4.Text = "Vehicle No *";
            label5.Text = "Party Name";
            label6.Text = "Material / Goods";
            label7.Text = "Remarks / Notes";

            Label[] labels = { label1, label2, label3, label4, label5, label6, label7 };
            foreach (var lbl in labels)
            {
                lbl.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                lbl.ForeColor = Color.FromArgb(51, 65, 85);
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Margin = new Padding(4, 3, 4, 3);
                lbl.AutoSize = false;
            }

            weight_box.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            weight_box.BackColor = Color.FromArgb(239, 246, 255);
            weight_box.ForeColor = Color.FromArgb(30, 58, 138);
            weight_box.BorderStyle = BorderStyle.FixedSingle;
            weight_box.TextAlign = HorizontalAlignment.Right;
            weight_box.Dock = DockStyle.Fill;
            weight_box.Margin = new Padding(4, 5, 8, 5);

            tire_weight_box.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            tire_weight_box.BackColor = Color.FromArgb(254, 243, 199);
            tire_weight_box.ForeColor = Color.FromArgb(146, 64, 14);
            tire_weight_box.BorderStyle = BorderStyle.FixedSingle;
            tire_weight_box.TextAlign = HorizontalAlignment.Right;
            tire_weight_box.Dock = DockStyle.Fill;
            tire_weight_box.Margin = new Padding(4, 5, 8, 5);

            net_weight_box.Font = new Font("Segoe UI", 13f, FontStyle.Bold);
            net_weight_box.BackColor = Color.FromArgb(236, 253, 245);
            net_weight_box.ForeColor = Color.FromArgb(6, 95, 70);
            net_weight_box.BorderStyle = BorderStyle.FixedSingle;
            net_weight_box.TextAlign = HorizontalAlignment.Right;
            net_weight_box.Dock = DockStyle.Fill;
            net_weight_box.Margin = new Padding(4, 5, 8, 5);

            TextBox[] textboxes = { Vehical_box, Party_box, meterial_box };
            foreach (var tb in textboxes)
            {
                tb.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
                tb.BackColor = Color.White;
                tb.ForeColor = Color.FromArgb(15, 23, 42);
                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.Dock = DockStyle.Fill;
                tb.Margin = new Padding(4, 5, 8, 5);
            }
            Vehical_box.CharacterCasing = CharacterCasing.Upper;

            remarks_box.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            remarks_box.BorderStyle = BorderStyle.FixedSingle;
            remarks_box.Dock = DockStyle.Fill;
            remarks_box.Margin = new Padding(4, 4, 8, 4);

            // Explicitly place controls into exact (col, row) cells
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(weight_box, 1, 0);

            tableLayoutPanel1.Controls.Add(label2, 0, 1);
            tableLayoutPanel1.Controls.Add(tire_weight_box, 1, 1);

            tableLayoutPanel1.Controls.Add(label3, 0, 2);
            tableLayoutPanel1.Controls.Add(net_weight_box, 1, 2);

            tableLayoutPanel1.Controls.Add(label4, 0, 3);
            tableLayoutPanel1.Controls.Add(Vehical_box, 1, 3);

            tableLayoutPanel1.Controls.Add(label5, 0, 4);
            tableLayoutPanel1.Controls.Add(Party_box, 1, 4);

            tableLayoutPanel1.Controls.Add(label6, 0, 5);
            tableLayoutPanel1.Controls.Add(meterial_box, 1, 5);

            tableLayoutPanel1.Controls.Add(label7, 0, 6);
            tableLayoutPanel1.Controls.Add(remarks_box, 1, 6);

            tableLayoutPanel1.ResumeLayout(true);
        }

        private void StyleInputsInTableLayout2()
        {
            tableLayoutPanel2.SuspendLayout();
            tableLayoutPanel2.Controls.Clear();
            tableLayoutPanel2.BackColor = Color.White;

            tableLayoutPanel2.ColumnCount = 2;
            tableLayoutPanel2.ColumnStyles.Clear();
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150f));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            tableLayoutPanel2.RowCount = 7;
            tableLayoutPanel2.RowStyles.Clear();
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 40f));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 85f));

            label10.Text = "Slip No (Enter ↵)";
            label13.Text = "Challan / Token";
            label11.Text = "1st Wt Date";
            label12.Text = "2nd Wt Date";
            label8.Text = "Vehicle Type";
            label9.Text = "Charges (Rs)";

            Label[] labels = { label10, label13, label11, label12, label8, label9 };
            foreach (var lbl in labels)
            {
                lbl.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
                lbl.ForeColor = Color.FromArgb(51, 65, 85);
                lbl.Dock = DockStyle.Fill;
                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.Margin = new Padding(4, 3, 4, 3);
                lbl.AutoSize = false;
            }

            slipno_box.Font = new Font("Segoe UI", 12f, FontStyle.Bold);
            slipno_box.BackColor = Color.FromArgb(243, 232, 255);
            slipno_box.ForeColor = Color.FromArgb(107, 33, 168);
            slipno_box.BorderStyle = BorderStyle.FixedSingle;
            slipno_box.Dock = DockStyle.Fill;
            slipno_box.Margin = new Padding(4, 5, 8, 5);
            slipno_box.ReadOnly = false;
            slipno_box.TabIndex = 20;

            TextBox[] textboxes = { tokenNo_box, date_box, mod_date };
            foreach (var tb in textboxes)
            {
                tb.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
                tb.BackColor = Color.White;
                tb.ForeColor = Color.FromArgb(15, 23, 42);
                tb.BorderStyle = BorderStyle.FixedSingle;
                tb.Dock = DockStyle.Fill;
                tb.Margin = new Padding(4, 5, 8, 5);
            }

            vechaltype_drop.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
            vechaltype_drop.Dock = DockStyle.Fill;
            vechaltype_drop.Margin = new Padding(4, 5, 8, 5);

            charges_drop.Font = new Font("Segoe UI", 10.5f, FontStyle.Regular);
            charges_drop.Dock = DockStyle.Fill;
            charges_drop.Margin = new Padding(4, 5, 8, 5);

            // Explicitly place controls into exact (col, row) cells
            tableLayoutPanel2.Controls.Add(label10, 0, 0);
            tableLayoutPanel2.Controls.Add(slipno_box, 1, 0);

            tableLayoutPanel2.Controls.Add(label13, 0, 1);
            tableLayoutPanel2.Controls.Add(tokenNo_box, 1, 1);

            tableLayoutPanel2.Controls.Add(label11, 0, 2);
            tableLayoutPanel2.Controls.Add(date_box, 1, 2);

            tableLayoutPanel2.Controls.Add(label12, 0, 3);
            tableLayoutPanel2.Controls.Add(mod_date, 1, 3);

            tableLayoutPanel2.Controls.Add(label8, 0, 4);
            tableLayoutPanel2.Controls.Add(vechaltype_drop, 1, 4);

            tableLayoutPanel2.Controls.Add(label9, 0, 5);
            tableLayoutPanel2.Controls.Add(charges_drop, 1, 5);

            tableLayoutPanel2.ResumeLayout(true);
        }


        private void StyleButton(Button btn, string text, Color backColor, Color foreColor, Size size)
        {
            btn.Text = text;
            btn.BackColor = backColor;
            btn.ForeColor = foreColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
            btn.Size = size;
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
        }

        private void CalculateNetWeight()
        {
            try
            {
                int gross = 0;
                int tare = 0;
                int.TryParse(weight_box.Text.Trim(), out gross);
                int.TryParse(tire_weight_box.Text.Trim(), out tare);
                if (gross > 0 && tare > 0)
                {
                    net_weight_box.Text = (gross - tare).ToString();
                }
                else if (gross > 0 && tare == 0)
                {
                    net_weight_box.Text = gross.ToString();
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

