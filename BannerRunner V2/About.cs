using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace BannerRunner_V3
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void About_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=bulutk%40gmail%2ecom&item_name=BannerRunner&no_shipping=0&no_note=1&tax=0&currency_code=USD&lc=TR&bn=PP%2dDonationsBF&charset=UTF%2d8");
            }
            catch (Exception)
            { }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start("http://www.bulutk.com");
            }
            catch (Exception)
            { }
        }
    }
}