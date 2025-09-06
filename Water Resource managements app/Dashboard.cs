using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace Water_Resource_managements_app
{
    public partial class Dashboard : Form
    {
        public Dashboard()
        {
            InitializeComponent();
            CountConsumer();
            Sumbills();
        }

        private void Dashboard_Load(object sender, EventArgs e)
        {
            // Call the method to display agent count when the dashboard loads
            CountAgrnts();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close(); // Close the dashboard form
        }

        // Database connection string
        SqlConnection Con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\hp\OneDrive\Documents\WaterResourceDb.mdf;Integrated Security=True;Connect Timeout=30");

        private void CountAgrnts()
        {
            try
            {
                // Open the database connection
                Con.Open();

                // Create and execute an SQL query to count the number of agents
                SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM AgentTbl", Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                // Set the count value to the label
                AgNumlbl.Text = dt.Rows[0][0].ToString()+" Agents";
            }
            catch (Exception ex)
            {
                // Handle any exceptions and display an error message
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed even if there is an error
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void CountConsumer()
        {
            try
            {
                // Open the database connection
                Con.Open();

                // Create and execute an SQL query to count the number of agents
                SqlDataAdapter sda = new SqlDataAdapter("SELECT COUNT(*) FROM ConsumerTbl", Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                // Set the count value to the label
                ConsLbl.Text = dt.Rows[0][0].ToString() + " Consumers";
            }
            catch (Exception ex)
            {
                // Handle any exceptions and display an error message
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed even if there is an error
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void Sumbills()
        {
            try
            {
                // Open the database connection
                Con.Open();

                // Create and execute an SQL query to count the number of agents
                SqlDataAdapter sda = new SqlDataAdapter("SELECT Sum(Total) FROM BillTbl", Con);
                DataTable dt = new DataTable();
                sda.Fill(dt);

                // Set the count value to the label
                Billslbl.Text ="Dollar"+ dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                // Handle any exceptions and display an error message
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed even if there is an error
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }



        private void ConsLbl_Click(object sender, EventArgs e)
        {
            // Add any functionality required for this event, if needed
        }

        private void AgNumlbl_Click(object sender, EventArgs e)
        {

        }

        private void Billslbl_Click(object sender, EventArgs e)
        {

        }

        private void BPeriod_ValueChanged(object sender, EventArgs e)
        {
            string BPer = BPeriod.Value.ToString("MM/yyyy"); // Format the month/year correctly

            try
            {
                // Open the database connection
                Con.Open();

                // Create a parameterized SQL query to prevent SQL injection
                SqlDataAdapter sda = new SqlDataAdapter("SELECT SUM(Total) FROM BillTbl WHERE BPeriod = @BPeriod", Con);
                sda.SelectCommand.Parameters.AddWithValue("@BPeriod", BPer);

                // Create a DataTable to hold the result
                DataTable dt = new DataTable();
                sda.Fill(dt);

                // Check if there's any result and set the label text
                if (dt.Rows.Count > 0 && dt.Rows[0][0] != DBNull.Value)
                {
                    BillsMounthlbl.Text = "Dollar  " + dt.Rows[0][0].ToString();
                }
                else
                {
                    BillsMounthlbl.Text = "Dollar 0"; // Handle case where no data is found
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions and display an error message
                MessageBox.Show("Error: " + ex.Message);
            }
            finally
            {
                // Ensure the connection is closed even if there is an error
                if (Con.State == ConnectionState.Open)
                {
                    Con.Close();
                }
            }
        }

        private void label4_Click(object sender, EventArgs e)
        {
            Billings obj = new Billings();
            obj.Show();
            this.Hide();
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {
            Consumer obj = new Consumer();
            obj.Show();
            this.Hide();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Home obj = new Home();
            obj.Show();
            this.Hide();
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }
    }
}
