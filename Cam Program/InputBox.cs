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
    public partial class InputBox : Form
    {
        public InputBox(string s, string t, string u)
        {
            InitializeComponent();
            label1.Text = s;
            Text = t;
            textBox1.Text = u;
            label1.Location = new Point(Size.Width/2 - label1.Size.Width/2, label1.Location.Y);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
        }
    }
}
