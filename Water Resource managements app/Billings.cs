using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Water_Resource_managements_app
{
    public partial class Billings : Form
    {
        public Billings()
        {
            InitializeComponent();
            CidCb.DropDownStyle = ComboBoxStyle.DropDownList;  // Prevent user from typing
            ShowBillings();
            GetCons();
            Agentslbl.Text = Login.User;
        }

        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\OneDrive\Documents\WaterResourceDb.mdf;Integrated Security=True;Connect Timeout=30");

        // This method retrieves and displays all billing records
        private void ShowBillings()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\OneDrive\Documents\WaterResourceDb.mdf;Integrated Security=True;Connect Timeout=30"))
                {
                    conn.Open();
                    string query = "SELECT * FROM BillTbl";
                    SqlDataAdapter sda = new SqlDataAdapter(query, conn);
                    DataSet ds = new DataSet();
                    sda.Fill(ds);
                    BillingsDGV.DataSource = ds.Tables[0];
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Reset()
        {
            CidCb.SelectedIndex = -1;
            RateTb.Text = "";
            TaxTb.Text = "";
            ConsTb.Text = "";
        }

        private void Savebtn_Click(object sender, EventArgs e)
        {
            if (RateTb.Text == "" || TaxTb.Text == "" || ConsTb.Text == "")
            {
                MessageBox.Show("Missing information!!!");
            }
            else
            {
                try
                {
                    Con.Open();

                    // Check if the CId and BPeriod combination already exists in the database
                    string checkQuery = "SELECT COUNT(*) FROM BillTbl WHERE CId = @CI AND BPeriod = @BP";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, Con);
                    checkCmd.Parameters.AddWithValue("@CI", CidCb.SelectedValue.ToString());
                    checkCmd.Parameters.AddWithValue("@BP", BPeriod.Value.ToString("MM/yyyy"));

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count > 0) // If the record exists, show an error message
                    {
                        MessageBox.Show("A bill for this consumer and billing period already exists!");
                    }
                    else
                    {
                        // Ensure tax is 5%
                        double TaxRate = 10.0;  // Fixed tax rate to 10%

                        // Calculate tax and total with 5% tax rate
                        int R = Convert.ToInt32(RateTb.Text);
                        int Consuption = Convert.ToInt32(ConsTb.Text);
                        double Tax = (R * Consuption) * (TaxRate / 100);  // Calculate tax with fixed rate
                        double Total = (R * Consuption) + Tax;  // Total is consumption plus tax

                        string insertQuery = "INSERT INTO BillTbl (CId, Agent, BPeriod, Consuption, Rate, Tax, Total) VALUES (@CI, @Ag, @BP, @Cons, @Rate, @Tax, @Tot)";
                        SqlCommand insertCmd = new SqlCommand(insertQuery, Con);
                        insertCmd.Parameters.AddWithValue("@CI", CidCb.SelectedValue.ToString());
                        insertCmd.Parameters.AddWithValue("@Ag", Agentslbl.Text);
                        insertCmd.Parameters.AddWithValue("@BP", BPeriod.Value.ToString("MM/yyyy"));
                        insertCmd.Parameters.AddWithValue("@Cons", ConsTb.Text);
                        insertCmd.Parameters.AddWithValue("@Rate", RateTb.Text);
                        insertCmd.Parameters.AddWithValue("@Tax", Tax.ToString());
                        insertCmd.Parameters.AddWithValue("@Tot", Total);

                        insertCmd.ExecuteNonQuery();
                        MessageBox.Show("Bill Added!!!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    if (Con.State == ConnectionState.Open)
                    {
                        Con.Close();
                        ShowBillings();
                    }
                    Reset();
                }
            }
        }

        private void GetCons()
        {
            try
            {
                Con.Open();

                SqlCommand cmd = new SqlCommand("SELECT CId FROM ConsumerTbl", Con);
                SqlDataReader rdr = cmd.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Columns.Add("CId", typeof(int)); // Ensure proper column type
                dt.Load(rdr);

                CidCb.ValueMember = "CId";
                CidCb.DataSource = dt;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void GetConsRate()
        {
            try
            {
                Con.Open();

                string query = "SELECT * FROM ConsumerTbl WHERE CId = @CId";
                SqlCommand cmd = new SqlCommand(query, Con);
                cmd.Parameters.AddWithValue("@CId", CidCb.SelectedValue);

                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                sda.Fill(dt);

                foreach (DataRow dr in dt.Rows)
                {
                    RateTb.Text = dr["CRate"].ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void CidCb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            string selectedCId = CidCb.SelectedValue.ToString();
            CidCb.Enabled = false;
            GetConsRate();
        }

        private void BillingsDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow row = BillingsDGV.Rows[e.RowIndex];

                CidCb.SelectedValue = row.Cells["CId"].Value.ToString();
                RateTb.Text = row.Cells["Rate"].Value.ToString();
                TaxTb.Text = row.Cells["Tax"].Value.ToString();
                ConsTb.Text = row.Cells["Consuption"].Value.ToString();
                Agentslbl.Text = row.Cells["Agent"].Value.ToString();
                BPeriod.Value = DateTime.Parse(row.Cells["BPeriod"].Value.ToString());
            }
        }

        private void deletebtn_Click(object sender, EventArgs e)
        {
            if (CidCb.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a consumer to delete the bill.");
                return;
            }
            if (BPeriod.Value == null)
            {
                MessageBox.Show("Please select a valid billing period.");
                return;
            }

            try
            {
                DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this bill?", "Confirm Deletion", MessageBoxButtons.YesNo);
                if (dialogResult == DialogResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\OneDrive\Documents\WaterResourceDb.mdf;Integrated Security=True;Connect Timeout=30"))
                    {
                        conn.Open();

                        string query = "DELETE FROM BillTbl WHERE CId = @CI AND BPeriod = @BP";
                        SqlCommand cmd = new SqlCommand(query, conn);

                        cmd.Parameters.AddWithValue("@CI", CidCb.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@BP", BPeriod.Value.ToString("MM/yyyy"));

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Bill Deleted Successfully!!!");
                    }

                    ShowBillings();
                    Reset();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Resetbtn_Click(object sender, EventArgs e)
        {
            CidCb.SelectedIndex = -1;
            RateTb.Text = "";
            TaxTb.Text = "";
            ConsTb.Text = "";
            Agentslbl.Text = "";
            BPeriod.Value = DateTime.Now;
            CidCb.Enabled = true;

            MessageBox.Show("Fields have been reset!");
        }

        private void Homebtn_Click(object sender, EventArgs e)
        {
            Home obj = new Home();
            obj.Show();
            this.Hide();
        }

        private void login_Click(object sender, EventArgs e)
        {
            Login obj = new Login();
            obj.Show();
            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            Consumer obj = new Consumer();
            obj.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label4_Click(object sender, EventArgs e)
        {
            Billings obj = new Billings();
            obj.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            Dashboard obj = new Dashboard();
            obj.Show();
            this.Hide();
        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            if (RateTb.Text == "" || TaxTb.Text == "" || ConsTb.Text == "")
            {
                MessageBox.Show("Missing information!!!");
            }
            else
            {
                try
                {
                    Con.Open();

                    // Check if the CId and BPeriod combination exists in the database
                    string checkQuery = "SELECT COUNT(*) FROM BillTbl WHERE CId = @CI AND BPeriod = @BP";
                    SqlCommand checkCmd = new SqlCommand(checkQuery, Con);
                    checkCmd.Parameters.AddWithValue("@CI", CidCb.SelectedValue.ToString());
                    checkCmd.Parameters.AddWithValue("@BP", BPeriod.Value.ToString("MM/yyyy"));

                    int count = Convert.ToInt32(checkCmd.ExecuteScalar());

                    if (count == 0) // If the record does not exist, show an error message
                    {
                        MessageBox.Show("No bill found for this consumer and billing period!");
                    }
                    else
                    {
                        // Ensure tax is 10%
                        double TaxRate = 10.0; // Fixed tax rate to 10%

                        // Calculate tax and total
                        int R = Convert.ToInt32(RateTb.Text);
                        int Consuption = Convert.ToInt32(ConsTb.Text);
                        double Tax = (R * Consuption) * (TaxRate / 100); // Calculate tax with fixed rate
                        double Total = (R * Consuption) + Tax; // Total is consumption plus tax

                        // Update the existing bill in the database
                        string updateQuery = "UPDATE BillTbl SET Agent = @Ag, Consuption = @Cons, Rate = @Rate, Tax = @Tax, Total = @Tot WHERE CId = @CI AND BPeriod = @BP";
                        SqlCommand updateCmd = new SqlCommand(updateQuery, Con);
                        updateCmd.Parameters.AddWithValue("@Ag", Agentslbl.Text);
                        updateCmd.Parameters.AddWithValue("@Cons", ConsTb.Text);
                        updateCmd.Parameters.AddWithValue("@Rate", RateTb.Text);
                        updateCmd.Parameters.AddWithValue("@Tax", Tax.ToString());
                        updateCmd.Parameters.AddWithValue("@Tot", Total);
                        updateCmd.Parameters.AddWithValue("@CI", CidCb.SelectedValue.ToString());
                        updateCmd.Parameters.AddWithValue("@BP", BPeriod.Value.ToString("MM/yyyy"));

                        updateCmd.ExecuteNonQuery();
                        MessageBox.Show("Bill Updated!!!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
                finally
                {
                    if (Con.State == ConnectionState.Open)
                    {
                        Con.Close();
                        ShowBillings();
                    }
                    Reset();
                }
            }

        }
    }
}
