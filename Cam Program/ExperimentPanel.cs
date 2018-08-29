using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cam_Program
{
    public partial class ExperimentPanel : Form
    {
        Form1 f;
        public ExperimentPanel(Form1 f1)
        {
            f = f1;
            InitializeComponent();
            Show();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog c = new ColorDialog();
            c.ShowDialog();
            Properties.Settings.Default.BGColor = c.Color;
            Properties.Settings.Default.Save();
            c.Dispose();
        }
    }
}
