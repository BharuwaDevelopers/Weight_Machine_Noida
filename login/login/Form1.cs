using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using login.Serial;

namespace login
{
    public partial class Form1 : Form
    {
        public static String User_sessson;
        public static String Session_priv;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ApplyModernTheme();

            try
            {
                LISTNER lis = new LISTNER();
                lis.Show();
                lis.Hide();
            }
            catch (Exception ex)
            {
                Console.WriteLine("LISTNER initialization notice: " + ex.Message);
            }
        }

        private void ApplyModernTheme()
        {
            this.AcceptButton = button1;
            this.CancelButton = button2;

            // Card custom border drawing
            pnlCard.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(203, 213, 225), 1))
                {
                    e.Graphics.DrawRectangle(p, 0, 0, pnlCard.Width - 1, pnlCard.Height - 1);
                }
            };

            // Button 1 (Login) hover effects
            button1.MouseEnter += (s, e) =>
            {
                button1.BackColor = Color.FromArgb(29, 78, 216); // Darker blue
            };
            button1.MouseLeave += (s, e) =>
            {
                button1.BackColor = Color.FromArgb(37, 99, 235); // Primary blue
            };

            // Button 2 (Exit) hover effects
            button2.MouseEnter += (s, e) =>
            {
                button2.BackColor = Color.FromArgb(226, 232, 240); // Darker slate
            };
            button2.MouseLeave += (s, e) =>
            {
                button2.BackColor = Color.FromArgb(241, 245, 249); // Base slate
            };

            // TextBox focus highlights
            username_box.GotFocus += (s, e) => username_box.BackColor = Color.FromArgb(248, 250, 252);
            username_box.LostFocus += (s, e) => username_box.BackColor = Color.White;
            password_box.GotFocus += (s, e) => password_box.BackColor = Color.FromArgb(248, 250, 252);
            password_box.LostFocus += (s, e) => password_box.BackColor = Color.White;

            this.ActiveControl = username_box;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string userName = username_box.Text.Trim();
            string passWord = password_box.Text;

            if (string.IsNullOrEmpty(userName))
            {
                MessageBox.Show("Please enter your Username / Operator ID.", "Authentication Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                username_box.Focus();
                return;
            }

            if (string.IsNullOrEmpty(passWord))
            {
                MessageBox.Show("Please enter your Password.", "Authentication Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                password_box.Focus();
                return;
            }

            try
            {
                using (SqlConnection db_connect = new SqlConnection(db_Connection.connection_string()))
                {
                    db_connect.Open();

                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM user_master WHERE user_name=@user AND password=@pass", db_connect))
                    {
                        cmd.Parameters.AddWithValue("@user", userName);
                        cmd.Parameters.AddWithValue("@pass", passWord);

                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        if (count > 0)
                        {
                            string authType = "";
                            using (SqlCommand authCmd = new SqlCommand("SELECT TOP 1 auth_type FROM user_master WHERE user_name=@user", db_connect))
                            {
                                authCmd.Parameters.AddWithValue("@user", userName);
                                object authObj = authCmd.ExecuteScalar();
                                if (authObj != null)
                                {
                                    authType = authObj.ToString();
                                }
                            }

                            User_sessson = userName;
                            Session_priv = authType;

                            this.Hide();
                            main main_form = new main();
                            main_form.Show();
                        }
                        else
                        {
                            MessageBox.Show("Invalid Username or Password.\nPlease check your credentials and try again.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            password_box.Clear();
                            password_box.Focus();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection error:\n" + ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
            Application.Exit();
        }
    }
}
