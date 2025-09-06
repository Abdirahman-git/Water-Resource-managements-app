using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Water_Resource_managements_app
{
    public partial class Agents : Form
    {
        public Agents()
        {
            InitializeComponent();
            ShowAgents();
        }

        private void Agents_Load(object sender, EventArgs e)
        {

        }

        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\OneDrive\Documents\WaterResourceDb.mdf;Integrated Security=True;Connect Timeout=30");

        public object AgIDTb { get; private set; }

        private void ShowAgents()
        {
            try
            {
                Con.Open();
                string Query = "SELECT AgNum as Code,AgName as Name,agPhone as Phone,AgAdd as Address,AgPass as Password FROM AgentTbl"; // Ensure table name matches the database
                SqlDataAdapter sda = new SqlDataAdapter(Query, Con);
                SqlCommandBuilder builder = new SqlCommandBuilder(sda);
                var ds = new DataSet();
                sda.Fill(ds);
                AgentsDGV.DataSource = ds.Tables[0];
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
                }
            }
        }

        private void savebtn_Click(object sender, EventArgs e)
        {
            if (AgNameTb.Text == "" || AgPassTb.Text == "" || AggPhoneTb.Text == "" || AgAddTb.Text == "")
            {
                MessageBox.Show("Missing Information");
            }
            else
            {
                try
                {
                    // Make sure the connection is closed before opening
                    if (Con.State == ConnectionState.Open)
                        Con.Close();

                    // Open the connection within the using statement
                    using (SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\OneDrive\Documents\WaterResourceDb.mdf;Integrated Security=True;Connect Timeout=30"))
                    {
                        con.Open();

                        // Check if the agent already exists
                        string checkQuery = "SELECT COUNT(*) FROM AgentTbl WHERE AgName=@AN AND AgPhone=@AP AND AgAdd=@AA AND AgPass=@APa";
                        using (SqlCommand checkCmd = new SqlCommand(checkQuery, con))
                        {
                            checkCmd.Parameters.AddWithValue("@AN", AgNameTb.Text);
                            checkCmd.Parameters.AddWithValue("@AP", AggPhoneTb.Text);
                            checkCmd.Parameters.AddWithValue("@AA", AgAddTb.Text);
                            checkCmd.Parameters.AddWithValue("@APa", AgPassTb.Text);

                            int count = (int)checkCmd.ExecuteScalar();

                            if (count > 0)
                            {
                                MessageBox.Show("This agent has already been saved before!");
                            }
                            else
                            {
                                // Insert the new agent into the database
                                string insertQuery = "INSERT INTO AgentTbl (AgName, AgPhone, AgAdd, AgPass) VALUES (@AN, @AP, @AA, @APa)";
                                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                                {
                                    cmd.Parameters.AddWithValue("@AN", AgNameTb.Text);
                                    cmd.Parameters.AddWithValue("@AP", AggPhoneTb.Text);
                                    cmd.Parameters.AddWithValue("@AA", AgAddTb.Text);
                                    cmd.Parameters.AddWithValue("@APa", AgPassTb.Text);
                                    cmd.ExecuteNonQuery();
                                }

                                MessageBox.Show("Agent Added!!!");
                                ShowAgents(); // Refresh the DataGridView
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }




        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Editbtn_Click(object sender, EventArgs e)
        {
            if (key == 0)
            {
                MessageBox.Show("Select the Agent to Update");
            }
            else if (AgNameTb.Text == "" || AgPassTb.Text == "" || AggPhoneTb.Text == "" || AgAddTb.Text == "")
            {
                MessageBox.Show("Missing Information");
            }
            else
            {
                try
                {
                    Con.Open();
                    string query = "UPDATE AgentTbl SET AgName=@AN, AgPhone=@AP, AgAdd=@AA, AgPass=@APa WHERE AgNum=@Key";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@AN", AgNameTb.Text);
                    cmd.Parameters.AddWithValue("@AP", AggPhoneTb.Text);
                    cmd.Parameters.AddWithValue("@AA", AgAddTb.Text);
                    cmd.Parameters.AddWithValue("@APa", AgPassTb.Text);
                    cmd.Parameters.AddWithValue("@Key", key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Agent Updated Successfully!");
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
                        ShowAgents(); // Refresh the DataGridView after the update
                    }
                }
            }
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        int key = 0;
        private void AgentsDGV_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex >= 0) // Ensure a valid row index is clicked
                {
                    DataGridViewRow row = AgentsDGV.Rows[e.RowIndex];
                    AgNameTb.Text = row.Cells["Name"].Value.ToString(); // Use column names or indexes appropriately
                    AggPhoneTb.Text = row.Cells["Phone"].Value.ToString();
                    AgAddTb.Text = row.Cells["Address"].Value.ToString();
                    AgPassTb.Text = row.Cells["Password"].Value.ToString();

                    // Optionally set the key if required for editing/deleting
                    if (int.TryParse(row.Cells["Code"].Value.ToString(), out int id))
                    {
                        key = id; // Store the key for further operations
                    }
                    else
                    {
                        key = 0; // Reset key if parsing fails
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void AgentsDGV_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void deletebtn_Click(object sender, EventArgs e)
        {
            if (key == 0)
            {
                MessageBox.Show("Select the Agent to Delete");
            }
            else
            {
                try
                {
                    Con.Open();
                    string query = "DELETE FROM AgentTbl WHERE AgNum=@Key";
                    SqlCommand cmd = new SqlCommand(query, Con);
                    cmd.Parameters.AddWithValue("@Key", key);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Agent Deleted Successfully!");
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
                        ShowAgents(); // Refresh the DataGridView after deletion
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Clear all TextBox fields
            AgNameTb.Text = "";
            AggPhoneTb.Text = "";
            AgAddTb.Text = "";
            AgPassTb.Text = "";

            // Reset the key to indicate no selection
            key = 0;

            // Optional: Show a confirmation message (can be removed if not needed)
            MessageBox.Show("Fields have been reset.");
        }

        private void label2_Click(object sender, EventArgs e)
        {
     
        }

        private void label3_Click(object sender, EventArgs e)
        {
         
        }

        private void label4_Click(object sender, EventArgs e)
        {
        
        }

        private void label5_Click(object sender, EventArgs e)
        {
        
        }

        private void Homebtn_Click(object sender, EventArgs e)
        {

        }

        private void login_Click(object sender, EventArgs e)
        {
            Login obj = new Login();
            obj.Show();
            this.Hide();
        }
    }
}
