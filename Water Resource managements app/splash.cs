using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Water_Resource_managements_app
{
    public partial class splash : Form
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public static System.Drawing.Region CreateRoundRegion(int width, int height, int radius)
        {
            return System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, width + 1, height + 1, radius, radius));
        }

        public splash()
        {
            InitializeComponent();
            Region = CreateRoundRegion(Width, Height, 25);
            progressbar1.Value = 0;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            progressbar1.Value += 5;
            progressbar1.Text = progressbar1.Value.ToString() + "%";
            if (progressbar1.Value == 100)
            {
                timer1.Enabled = false;
                Login obj = new Login();
                obj.Show();
                this.Hide();
            }
        }

        private void progressbar1_Click(object sender, EventArgs e)
        {

        }
    }
}
