using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cam_Program
{
    public partial class Startup : Form
    {
        Form1 f1;
        public Startup(Form1 f)
        {
            InitializeComponent();
            f1 = f;
            Show();
            timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            Close();
        }
    }
}
