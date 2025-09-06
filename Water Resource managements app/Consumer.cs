using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Water_Resource_managements_app
{
    public partial class Consumer : Form
    {
        public Consumer()
        {
            InitializeComponent();
        }

        private void Consumer_Load(object sender, EventArgs e)
        {
            ShowAgents();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\OneDrive\Documents\WaterResourceDb.mdf;Integrated Security=True;Connect Timeout=30");

        public object AgIDTb { get; private set; }

        private void ShowAgents()
        {
            try
            {
                Con.Open();
                string Query = "SELECT CId as Number, CName as Name, CAddress as Address, CPhone as Phone, CCategory as Category, CJDate as JoinDate, CRate as Rate FROM ConsumerTbl";
                SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                DataSet ds = new DataSet();
                sda.Fill(ds);
                ConsumerDGV.DataSource = ds.Tables[0];
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
        private void Reset()
        {
            CNameTb.Text = "";
            CPhoneTb.Text = "";
            CAddTb.Text = "";
            CRateTb.Text = "";
            CCatTb.SelectedIndex = -1;


        }
        private void savebtn_Click(object sender, EventArgs e)
        {
            if (CNameTb.Text == "" || CAddTb.Text == "" || CPhoneTb.Text == "" || CCatTb.SelectedIndex == -1)
            {
                MessageBox.Show("Missing Information");
            }
            else
            {
                try
                {
                    Con.Open();
                    SqlCommand cmd = new SqlCommand("INSERT INTO ConsumerTbl (CName, CAddress, CPhone, CCategory, CJDate, CRate) VALUES (@CN, @CA, @CP, @CCa, @CJD, @CR)", Con);
                    cmd.Parameters.AddWithValue("@CN", CNameTb.Text);
                    cmd.Parameters.AddWithValue("@CA", CAddTb.Text);
                    cmd.Parameters.AddWithValue("@CP", CPhoneTb.Text);
                    cmd.Parameters.AddWithValue("@CCa", CCatTb.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@CJD", CJDate.Value.Date);
                    cmd.Parameters.AddWithValue("@CR", CRateTb.Text);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Consumer Added!!!");
                    Reset();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    if (Con.State == ConnectionState.Open)
                    {
                        Con.Close();
                        ShowAgents(); // Refresh the DataGridView
                    }
                }
            }
        }
        private void GetRate()
        {
            if(CCatTb.SelectedIndex==0)
            {
                CRateTb.Text = "70";
            }else if(CCatTb.SelectedIndex == 1)
            {
                CRateTb.Text = "95";
            }
            else
            {
                CRateTb.Text = "120";
            }
        }
        private void CCatTb_SelectionChangeCommitted(object sender, EventArgs e)
        {
            GetRate();
        }

        private void ConsumerDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                // Ensure the user clicks on a valid row (cell click, not column header)
                if (e.RowIndex >= 0 && e.RowIndex < ConsumerDGV.Rows.Count)
                {
                    // Select the row
                    DataGridViewRow selectedRow = ConsumerDGV.Rows[e.RowIndex];

                    // Populate textboxes with selected row values
                    CNameTb.Text = selectedRow.Cells["Name"].Value.ToString();
                    CAddTb.Text = selectedRow.Cells["Address"].Value.ToString();
                    CPhoneTb.Text = selectedRow.Cells["Phone"].Value.ToString();
                    CCatTb.SelectedItem = selectedRow.Cells["Category"].Value.ToString();

                    // Ensure the date value is valid before setting it
                    if (DateTime.TryParse(selectedRow.Cells["JoinDate"].Value.ToString(), out DateTime joinDate))
                    {
                        CJDate.Value = joinDate;
                    }
                    else
                    {
                        MessageBox.Show("Invalid date format in the selected row.");
                    }

                    CRateTb.Text = selectedRow.Cells["Rate"].Value.ToString();
                }
                else
                {
                    MessageBox.Show("Please select a valid row.");
                }
            }
            catch (Exception ex)
            {
                // Display error message if something goes wrong
                MessageBox.Show("An error occurred: " + ex.Message);
            }
        }

        private void editbtn_Click(object sender, EventArgs e)
        {
            // Check if required fields are filled in
            if (CNameTb.Text == "" || CAddTb.Text == "" || CPhoneTb.Text == "" || CCatTb.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a consumer and fill in all fields before updating.");
                return; // Exit the method early if validation fails
            }

            // Ensure a row is selected from the DataGridView
            if (ConsumerDGV.CurrentRow == null || ConsumerDGV.CurrentRow.Index < 0)
            {
                MessageBox.Show("Please select a valid consumer from the list.");
                return;
            }

            try
            {
                // Retrieve the Consumer ID from the selected row
                int consumerId = Convert.ToInt32(ConsumerDGV.CurrentRow.Cells["Number"].Value);

                // Open a new connection using a `using` statement
                using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\OneDrive\Documents\WaterResourceDb.mdf;Integrated Security=True;Connect Timeout=30"))
                {
                    con.Open();

                    // Define the update query
                    string query = "UPDATE ConsumerTbl SET CName=@CN, CAddress=@CA, CPhone=@CP, CCategory=@CCa, CJDate=@CJD, CRate=@CR WHERE CId=@CID";
                    SqlCommand cmd = new SqlCommand(query, con);

                    // Add parameters to the command to avoid SQL injection
                    cmd.Parameters.AddWithValue("@CN", CNameTb.Text);
                    cmd.Parameters.AddWithValue("@CA", CAddTb.Text);
                    cmd.Parameters.AddWithValue("@CP", CPhoneTb.Text);
                    cmd.Parameters.AddWithValue("@CCa", CCatTb.SelectedItem.ToString());
                    cmd.Parameters.AddWithValue("@CJD", CJDate.Value.Date);
                    cmd.Parameters.AddWithValue("@CR", CRateTb.Text);
                    cmd.Parameters.AddWithValue("@CID", consumerId);

                    // Execute the query
                    int rowsAffected = cmd.ExecuteNonQuery();

                    // Show appropriate message
                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Consumer details updated successfully!");
                        Reset();     // Clear input fields
                        ShowAgents(); // Refresh the DataGridView
                    }
                    else
                    {
                        MessageBox.Show("No changes were made. Please try again.");
                    }
                } // Connection is automatically closed here
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        private void Deletebtn_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (ConsumerDGV.CurrentRow == null || ConsumerDGV.CurrentRow.Index < 0)
            {
                MessageBox.Show("Please select a valid consumer to delete.");
                return;
            }

            // Retrieve the Consumer ID from the selected row
            int consumerId = Convert.ToInt32(ConsumerDGV.CurrentRow.Cells["Number"].Value);

            // Confirm deletion
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this consumer?", "Confirm Deletion", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    // Open a connection to the database
                    using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\OneDrive\Documents\WaterResourceDb.mdf;Integrated Security=True;Connect Timeout=30"))
                    {
                        con.Open();

                        // Define the delete query
                        string query = "DELETE FROM ConsumerTbl WHERE CId=@CID";
                        SqlCommand cmd = new SqlCommand(query, con);

                        // Add parameters to the command
                        cmd.Parameters.AddWithValue("@CID", consumerId);

                        // Execute the query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        // Check if the deletion was successful
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Consumer deleted successfully!");
                            ShowAgents(); // Refresh the DataGridView
                            Reset(); // Reset the input fields
                        }
                        else
                        {
                            MessageBox.Show("Failed to delete consumer. Please try again.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}");
                }
            }
        }

        private void Resetbtn_Click(object sender, EventArgs e)
        {
            // Clear all input fields
            CNameTb.Text = "";
            CAddTb.Text = "";
            CPhoneTb.Text = "";
            CRateTb.Text = "";
            CCatTb.SelectedIndex = -1;

            // Optionally reset the date picker to the current date
            CJDate.Value = DateTime.Now;

            // Clear any selected row in the DataGridView
            ConsumerDGV.ClearSelection();

            // Optionally, show a confirmation message
            MessageBox.Show("Fields have been reset!");
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
    }
}
