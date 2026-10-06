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

            System.Drawing.Printing.PrintDocument printDocument = new PrintDocument();
            // Custom compact paper size for Dot Matrix printer paper saving (Width: 7.2" / 720, Height: 3.6" / 360)
            PaperSize customPaperSize = new PaperSize("DotMatrix_PaperSave", 720, 360);
            printDocument.DefaultPageSettings.PaperSize = customPaperSize;
            printDocument.DefaultPageSettings.Margins = new Margins(10, 10, 10, 10);
            
            // Attach the PrintPage event
            printDocument.PrintPage += slpprintDocument1_PrintPage;
            
            // Create and configure the PrintPreviewDialog
            PrintPreviewDialog printPreviewDialog = new PrintPreviewDialog();
            printPreviewDialog.Document = printDocument;
            printPreviewDialog.Width = 850;
            printPreviewDialog.Height = 550;
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

            // Background Watermark (Light Gray rotated watermark)
            if (!string.IsNullOrEmpty(watermark))
            {
                Font wmFont = new Font("Arial", 36, FontStyle.Bold);
                g.RotateTransform(-20);
                g.DrawString(watermark.ToUpper(), wmFont, new SolidBrush(Color.FromArgb(230, 230, 230)), new PointF(10, 200));
                g.ResetTransform();
            }

            // Pens & Fonts optimized for Dot Matrix clarity
            Pen borderPen = new Pen(Color.Black, 1.5f);
            Pen thinPen = new Pen(Color.Black, 1.0f);
            Pen dashPen = new Pen(Color.Black, 1.0f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };

            Font plantFont = new Font("Arial", 11, FontStyle.Bold);
            Font addrFont = new Font("Arial", 8.5f, FontStyle.Regular);
            Font labelFont = new Font("Arial", 9, FontStyle.Bold);
            Font valueFont = new Font("Arial", 9, FontStyle.Regular);
            Font weightLabelFont = new Font("Arial", 9, FontStyle.Bold);
            Font weightValueFont = new Font("Arial", 12, FontStyle.Bold);

            // Outer Frame (Width: 690px, Height: 335px)
            g.DrawRectangle(borderPen, 15, 10, 690, 335);

            // Header Separator Line
            g.DrawLine(borderPen, 15, 58, 705, 58);

            // Plant Header & Address
            g.DrawString(plantName.ToUpper(), plantFont, Brushes.Black, new PointF(25, 14));
            if (!string.IsNullOrEmpty(plantAddress))
            {
                g.DrawString(plantAddress, addrFont, Brushes.Black, new PointF(25, 34));
            }

            // Right-aligned Copy Type Badge (ORIGINAL / DUPLICATE SLIP)
            string badgeText = (watermark ?? "ORIGINAL").ToUpper() + " SLIP";
            Font badgeFont = new Font("Arial", 9, FontStyle.Bold);
            SizeF badgeSize = g.MeasureString(badgeText, badgeFont);
            float badgeX = 695 - badgeSize.Width - 10;
            g.DrawRectangle(borderPen, badgeX, 14, badgeSize.Width + 8, badgeSize.Height + 4);
            g.DrawString(badgeText, badgeFont, Brushes.Black, badgeX + 4, 16);

            // 3-Column Compact Details Grid (Y: 65, 90, 115)
            // Column 1 (X: 25), Column 2 (X: 250), Column 3 (X: 480)

            // Row 1
            g.DrawString("Slip No:", labelFont, Brushes.Black, 25, 65);
            g.DrawString(main.prn_slipno ?? "", valueFont, Brushes.Black, 95, 65);

            g.DrawString("Vehicle No:", labelFont, Brushes.Black, 250, 65);
            g.DrawString(veh_no ?? "", valueFont, Brushes.Black, 335, 65);

            g.DrawString("Date/Time:", labelFont, Brushes.Black, 480, 65);
            g.DrawString(cr_date ?? "", valueFont, Brushes.Black, 555, 65);

            // Row 2
            g.DrawString("Party Name:", labelFont, Brushes.Black, 25, 90);
            g.DrawString(main.prn_partyname ?? "", valueFont, Brushes.Black, 105, 90);

            g.DrawString("Product:", labelFont, Brushes.Black, 250, 90);
            g.DrawString(main.prn_product ?? "", valueFont, Brushes.Black, 315, 90);

            g.DrawString("Final Date:", labelFont, Brushes.Black, 480, 90);
            g.DrawString(mdate ?? "", valueFont, Brushes.Black, 555, 90);

            // Row 3
            g.DrawString("Challan No:", labelFont, Brushes.Black, 25, 115);
            g.DrawString(main.t_no ?? "", valueFont, Brushes.Black, 105, 115);

            g.DrawString("Vehicle Type:", labelFont, Brushes.Black, 250, 115);
            g.DrawString(main.prn_vehicle_type ?? "", valueFont, Brushes.Black, 340, 115);

            g.DrawString("Charges:", labelFont, Brushes.Black, 480, 115);
            g.DrawString(main.prn_charges ?? "", valueFont, Brushes.Black, 545, 115);

            // Divider Line before Weight Table
            g.DrawLine(thinPen, 15, 140, 705, 140);

            // Weight Table Box (Y: 146 to 220, Height: 74px)
            g.DrawRectangle(thinPen, 25, 146, 670, 74);
            g.DrawLine(thinPen, 248, 146, 248, 220);
            g.DrawLine(thinPen, 471, 146, 471, 220);
            g.DrawLine(dashPen, 25, 170, 695, 170);

            // Weight Table Headers
            g.DrawString("GROSS WEIGHT", weightLabelFont, Brushes.Black, 80, 150);
            g.DrawString("TARE WEIGHT", weightLabelFont, Brushes.Black, 305, 150);
            g.DrawString("NET WEIGHT", weightLabelFont, Brushes.Black, 530, 150);

            // Weight Table Values
            g.DrawString((main.prn_g_weight ?? "0") + " Kg", weightValueFont, Brushes.Black, 75, 182);
            g.DrawString((main.prn_t_weight ?? "0") + " Kg", weightValueFont, Brushes.Black, 300, 182);
            g.DrawString((main.prn_n_weight ?? "0") + " Kg", weightValueFont, Brushes.Black, 525, 182);

            // Bottom Section Lines & Signatures
            g.DrawLine(thinPen, 15, 230, 705, 230);

            g.DrawString("Operator: " + (main.prn_op_name ?? ""), labelFont, Brushes.Black, 25, 240);

            // Signatures
            g.DrawLine(thinPen, 50, 305, 210, 305);
            g.DrawString("Driver Signature", addrFont, Brushes.Black, 80, 310);

            g.DrawLine(thinPen, 500, 305, 660, 305);
            g.DrawString("Operator Signature", addrFont, Brushes.Black, 525, 310);

            // Refresh / Clear form fields after print
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
                        lblLiveClock.Text = DateTime.Now.ToString("dd-MMM-yyyy  HH:mm:ss");
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

