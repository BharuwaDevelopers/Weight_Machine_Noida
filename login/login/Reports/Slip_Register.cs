using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace login.Reports
{
    public partial class Slip_Register : Form
    {
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSummaryBadge;
        private Button btnExportExcel;
        private Button btnRefreshData;

        private Panel pnlFilterBar;
        private TextBox txtSearch;
        private TextBox txtPartyFilter;
        private TextBox txtVehicleFilter;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private CheckBox chkUseDate;
        private Button btnResetFilter;

        private Panel pnlFooter;
        private Label lblStatusText;

        public Slip_Register()
        {
            InitializeComponent();
        }

        private void Slip_Register_Load(object sender, EventArgs e)
        {
            BuildModernUI();
            ApplyDataGridStyling();
            LoadData();
        }

        private void LoadData()
        {
            try
            {
                this.measureTableAdapter.Fill(this.weight_bridgeDataSet1.measure);
                UpdateSummaryMetrics();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("Error loading slip register: " + ex.Message, "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BuildModernUI()
        {
            this.Text = "Weighment Slip Register Report";
            this.Size = new Size(1150, 650);
            this.MinimumSize = new Size(950, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(241, 245, 249); // slate-100

            if (this.menuStrip1 != null)
            {
                this.menuStrip1.Visible = false;
            }

            // 1. Header Panel
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(15, 23, 42) // slate-900
            };

            lblTitle = new Label
            {
                Text = "📋 Weighment Slip Register",
                Font = new Font("Segoe UI", 13.5f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(16, 14),
                AutoSize = true
            };

            lblSummaryBadge = new Label
            {
                Text = "Total Slips: 0",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(226, 232, 240),
                BackColor = Color.FromArgb(30, 41, 59),
                Padding = new Padding(8, 4, 8, 4),
                Location = new Point(330, 16),
                AutoSize = true
            };

            btnExportExcel = new Button
            {
                Text = "📥 Export Excel",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(13, 148, 136), // teal-600
                FlatStyle = FlatStyle.Flat,
                Size = new Size(125, 34),
                Location = new Point(this.ClientSize.Width - 250, 13),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnExportExcel.FlatAppearance.BorderSize = 0;
            btnExportExcel.Click += (s, ev) => ExportToExcelOrCsv();

            btnRefreshData = new Button
            {
                Text = "🔄 Refresh",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(71, 85, 105), // slate-600
                FlatStyle = FlatStyle.Flat,
                Size = new Size(100, 34),
                Location = new Point(this.ClientSize.Width - 115, 13),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnRefreshData.FlatAppearance.BorderSize = 0;
            btnRefreshData.Click += (s, ev) => { LoadData(); ResetFilters(); };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSummaryBadge);
            pnlHeader.Controls.Add(btnExportExcel);
            pnlHeader.Controls.Add(btnRefreshData);

            // 2. Filter Bar Panel
            pnlFilterBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 52,
                BackColor = Color.White,
                Padding = new Padding(8)
            };

            Label lblSearch = new Label { Text = "🔍 Search:", Location = new Point(12, 16), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85) };
            txtSearch = new TextBox { Location = new Point(85, 13), Width = 150, Font = new Font("Segoe UI", 9.5f) };
            txtSearch.TextChanged += (s, ev) => ApplyFilter();

            Label lblParty = new Label { Text = "Party:", Location = new Point(248, 16), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85) };
            txtPartyFilter = new TextBox { Location = new Point(292, 13), Width = 130, Font = new Font("Segoe UI", 9.5f) };
            txtPartyFilter.TextChanged += (s, ev) => ApplyFilter();

            Label lblVehicle = new Label { Text = "Vehicle:", Location = new Point(435, 16), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85) };
            txtVehicleFilter = new TextBox { Location = new Point(495, 13), Width = 110, Font = new Font("Segoe UI", 9.5f) };
            txtVehicleFilter.TextChanged += (s, ev) => ApplyFilter();

            chkUseDate = new CheckBox { Text = "Date Filter:", Location = new Point(620, 15), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold), ForeColor = Color.FromArgb(51, 65, 85) };
            chkUseDate.CheckedChanged += (s, ev) => ApplyFilter();

            dtpFrom = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(710, 13), Width = 100, Font = new Font("Segoe UI", 9f) };
            dtpTo = new DateTimePicker { Format = DateTimePickerFormat.Short, Location = new Point(818, 13), Width = 100, Font = new Font("Segoe UI", 9f) };
            dtpFrom.ValueChanged += (s, ev) => { if (chkUseDate.Checked) ApplyFilter(); };
            dtpTo.ValueChanged += (s, ev) => { if (chkUseDate.Checked) ApplyFilter(); };

            btnResetFilter = new Button
            {
                Text = "🧹 Clear",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                BackColor = Color.FromArgb(241, 245, 249),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(75, 28),
                Location = new Point(930, 12),
                Cursor = Cursors.Hand
            };
            btnResetFilter.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnResetFilter.Click += (s, ev) => ResetFilters();

            pnlFilterBar.Controls.Add(lblSearch);
            pnlFilterBar.Controls.Add(txtSearch);
            pnlFilterBar.Controls.Add(lblParty);
            pnlFilterBar.Controls.Add(txtPartyFilter);
            pnlFilterBar.Controls.Add(lblVehicle);
            pnlFilterBar.Controls.Add(txtVehicleFilter);
            pnlFilterBar.Controls.Add(chkUseDate);
            pnlFilterBar.Controls.Add(dtpFrom);
            pnlFilterBar.Controls.Add(dtpTo);
            pnlFilterBar.Controls.Add(btnResetFilter);

            // 3. Footer Status Panel
            pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                BackColor = Color.FromArgb(241, 245, 249)
            };
            lblStatusText = new Label
            {
                Text = "Ready",
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(12, 7),
                AutoSize = true
            };
            pnlFooter.Controls.Add(lblStatusText);

            // Add panels to form & dock grid
            this.Controls.Add(pnlFilterBar);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlFooter);

            dataGridView1.Dock = DockStyle.Fill;
            dataGridView1.BringToFront();
        }

        private void ApplyDataGridStyling()
        {
            try
            {
                typeof(DataGridView).InvokeMember("DoubleBuffered",
                    BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                    null, dataGridView1, new object[] { true });
            }
            catch { }

            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(15, 23, 42); // slate-900
            dataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridView1.ColumnHeadersHeight = 36;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            dataGridView1.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
            dataGridView1.DefaultCellStyle.SelectionBackColor = Color.FromArgb(2, 132, 199); // sky-600
            dataGridView1.DefaultCellStyle.SelectionForeColor = Color.White;
            dataGridView1.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dataGridView1.RowTemplate.Height = 32;
            dataGridView1.GridColor = Color.FromArgb(226, 232, 240);
            dataGridView1.BorderStyle = BorderStyle.None;
            dataGridView1.BackgroundColor = Color.White;

            MapColumnHeader("slipnoDataGridViewTextBoxColumn", "Slip No");
            MapColumnHeader("weightDataGridViewTextBoxColumn", "Gross Wt (kg)");
            MapColumnHeader("tireweightDataGridViewTextBoxColumn", "Tare Wt (kg)");
            MapColumnHeader("netweightDataGridViewTextBoxColumn", "Net Wt (kg)");
            MapColumnHeader("vehicalnoDataGridViewTextBoxColumn", "Vehicle No");
            MapColumnHeader("partyDataGridViewTextBoxColumn", "Party Name");
            MapColumnHeader("meterialDataGridViewTextBoxColumn", "Material / Product");
            MapColumnHeader("vehicaltypeDataGridViewTextBoxColumn", "Vehicle Type");
            MapColumnHeader("chargesDataGridViewTextBoxColumn", "Charges (₹)");
            MapColumnHeader("dateDataGridViewTextBoxColumn", "Date & Time");
            MapColumnHeader("remarksDataGridViewTextBoxColumn", "Remarks");
            MapColumnHeader("usernameDataGridViewTextBoxColumn", "Operator");
            MapColumnHeader("moddateDataGridViewTextBoxColumn", "Mod Date");
            MapColumnHeader("modbyDataGridViewTextBoxColumn", "Mod By");
            MapColumnHeader("tokennoDataGridViewTextBoxColumn", "Challan / Token");

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
        }

        private void MapColumnHeader(string colName, string headerText)
        {
            if (dataGridView1.Columns[colName] != null)
            {
                dataGridView1.Columns[colName].HeaderText = headerText;
            }
        }

        private void ApplyFilter()
        {
            try
            {
                List<string> filters = new List<string>();

                if (!string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    string search = txtSearch.Text.Trim().Replace("'", "''");
                    filters.Add($"(slip_no LIKE '%{search}%' OR vehical_no LIKE '%{search}%' OR party LIKE '%{search}%' OR meterial LIKE '%{search}%')");
                }

                if (!string.IsNullOrWhiteSpace(txtPartyFilter.Text))
                {
                    string party = txtPartyFilter.Text.Trim().Replace("'", "''");
                    filters.Add($"party LIKE '%{party}%'");
                }

                if (!string.IsNullOrWhiteSpace(txtVehicleFilter.Text))
                {
                    string veh = txtVehicleFilter.Text.Trim().Replace("'", "''");
                    filters.Add($"vehical_no LIKE '%{veh}%'");
                }

                if (chkUseDate.Checked)
                {
                    string fromStr = dtpFrom.Value.ToString("yyyy-MM-dd 00:00:00");
                    string toStr = dtpTo.Value.ToString("yyyy-MM-dd 23:59:59");
                    filters.Add($"[date] >= '{fromStr}' AND [date] <= '{toStr}'");
                }

                measureBindingSource.Filter = string.Join(" AND ", filters);
                UpdateSummaryMetrics();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Filter error: " + ex.Message);
            }
        }

        private void ResetFilters()
        {
            txtSearch.Clear();
            txtPartyFilter.Clear();
            txtVehicleFilter.Clear();
            chkUseDate.Checked = false;
            dtpFrom.Value = DateTime.Now;
            dtpTo.Value = DateTime.Now;
            measureBindingSource.Filter = "";
            UpdateSummaryMetrics();
        }

        private void UpdateSummaryMetrics()
        {
            int count = measureBindingSource.Count;
            lblSummaryBadge.Text = $"Total Slips: {count}";
            lblStatusText.Text = $"Filtered Results: {count} weighment records loaded.";
        }

        private void ExportToExcelOrCsv()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog
                {
                    FileName = "Slip_Register_Report_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Filter = "Excel Workbook (*.xlsx)|*.xlsx|CSV File (*.csv)|*.csv"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();

                    // Headers
                    List<string> headers = new List<string>();
                    foreach (DataGridViewColumn col in dataGridView1.Columns)
                    {
                        if (col.Visible)
                            headers.Add("\"" + col.HeaderText.Replace("\"", "\"\"") + "\"");
                    }
                    sb.AppendLine(string.Join(",", headers));

                    // Rows
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        if (!row.IsNewRow)
                        {
                            List<string> cells = new List<string>();
                            foreach (DataGridViewColumn col in dataGridView1.Columns)
                            {
                                if (col.Visible)
                                {
                                    object val = row.Cells[col.Index].Value;
                                    string cellText = val != null ? val.ToString().Replace("\"", "\"\"") : "";
                                    cells.Add("\"" + cellText + "\"");
                                }
                            }
                            sb.AppendLine(string.Join(",", cells));
                        }
                    }

                    File.WriteAllText(sfd.FileName, sb.ToString(), Encoding.UTF8);
                    MessageBox.Show("Slip register report exported successfully!", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting report: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void excleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportToExcelOrCsv();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e) { }
        private void fillByToolStripButton_Click(object sender, EventArgs e) { }
        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e) { }
    }
}
