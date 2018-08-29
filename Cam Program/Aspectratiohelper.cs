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
    public partial class Aspectratiohelper : Form
    {
        Size iz;
        Size fz;
        double a0 = 0;
        double a1 = 0;
        Control parent;
        public Aspectratiohelper(Point loc, Control _parent)
        {
            InitializeComponent();
            parent = _parent;
            Iz = new Size(0, 0);
            Location = loc;
            timer1.Start();
            label1.Text = "You want the aspect ratio difference to be as" + Environment.NewLine + "close to zero as possible.";
            Reload();
        }
        public void Reload()
        {
            if (Camviewer.sv.Darktheme)
            {
                BackColor = GeneralSettings.ColorDark;
            }
            else
            {
                BackColor = GeneralSettings.ColorLight;
            }
        }

        public Size Iz { get => iz; set => iz = value; }
        public new Control Parent { get => parent; set => parent = value; }

        public void PushSizes(Size isize, Size fsize)
        {
            iz = isize;
            fz = fsize;
            if (iz.Height == 0 || fz.Height == 0)
            {
                a0 = 0;
                a1 = 0;
            }
            else
            {
                a0 = (float)iz.Width / (float)iz.Height;
                a1 = (float)fz.Width / (float)fz.Height;
            }
            label2.Text = "The current difference in aspect ratio is: " + (a1 - a0).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Camviewer.Tsize = new Size(fz.Width*Camviewer.sv.Wc, (int)Math.Round((fz.Width/a0)*Camviewer.sv.Hc));
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Camviewer.Tsize = new Size((int)Math.Round((fz.Height * a0) * Camviewer.sv.Wc), fz.Height * Camviewer.sv.Hc);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Camviewer.Tsize = new Size(iz.Width * Camviewer.sv.Wc, iz.Height * Camviewer.sv.Hc);
        }

        private void Aspectratiohelper_VisibleChanged(object sender, EventArgs e)
        {
            if (Parent != null && Parent.Parent != null)
            {
                Location = Parent.Parent.Location+(Ruaraidheulib.point)Parent.Location;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Visible)
            {
                if (Type.GetType("Mono.Runtime") != null)
                {
                    TopMost = true;
                }
            }
        }
    }
}
