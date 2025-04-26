using Planets.Classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Planets
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            THyperNavigation.Activate();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            THyperNavigation.Enabled = false;
        }
    }
}
