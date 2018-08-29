using Ruaraidheulib.Interface.reulib64.Win64.Version;
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
    public partial class GeneralSettings : Form
    {
        bool doreload = false;
        public GeneralSettings()
        {
            InitializeComponent();
            doreload = true;
            Init(null);
        }
        public static void Show(Point location)
        {
            using (GeneralSettings l = new GeneralSettings())
            {
                l.Location = location;
                l.ShowDialog();
            }
        }
        public void Init(bool? reload)
        {
            if (doreload)
            {
                doreload = false;
                checkBox3.Checked = Camviewer.sv.Usemqtt;
                checkBox2.Checked = Camviewer.sv.Darktheme;
                checkBox1.Checked = Camviewer.sv.Disco;
                checkBox4.Checked = Camviewer.sv.LockAspectRatio;
                numericUpDown6.Value = Camviewer.sv.Wc;
                numericUpDown1.Value = Camviewer.sv.Hc;
                this.Version.Text = VersionInfo.Get().FullVersion;
                if (Camviewer.sv.Darktheme)
                {
                    BackColor = ColorDark;
                    Version.ForeColor = Color.White;
                    checkBox1.ForeColor = Color.White;
                    checkBox2.ForeColor = Color.White;
                    checkBox3.ForeColor = Color.White;
                    checkBox4.ForeColor = Color.White;
                    label1.ForeColor = Color.White;
                    label8.ForeColor = Color.White;
                }
                else
                {
                    BackColor = ColorLight;
                    Version.ForeColor = Color.Black;
                    checkBox1.ForeColor = Color.Black;
                    checkBox2.ForeColor = Color.Black;
                    checkBox3.ForeColor = Color.Black;
                    checkBox4.ForeColor = Color.Black;
                    label1.ForeColor = Color.Black;
                    label8.ForeColor = Color.Black;
                }
                if (reload == false)
                {
                    Slot.DoReload = true;
                }
                if (reload == true)
                {
                    Camviewer.Reload = true;
                }
                doreload = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to reset all settings.", "Reset settings", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                Camviewer.sv = new Saveable();
            }
            Init(false);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Saveable.SerializeItem(Camviewer.sv);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            cd.Color = Camviewer.sv.Color;
            cd.ShowDialog();
            Camviewer.sv.Color = cd.Color;
            Init(false);
            cd.Dispose();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (doreload)
            {
                Camviewer.sv.Usemqtt = checkBox3.Checked;
                Init(false);
            }
        }

        public static Color ColorDark
        {
            get { return Color.FromArgb(64, 64, 64); }
        }
        public static Color ColorLight
        {
            get { return Color.FromArgb(200, 200, 200); }
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (doreload)
            {
                Camviewer.sv.Darktheme = checkBox2.Checked;
                Init(null);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (doreload)
            {
                Camviewer.sv.Disco = checkBox1.Checked;
                if (Camviewer.sv.Disco)
                {
                    Init(null);
                }
                else
                {
                    Init(false);
                }
            }
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (doreload)
            {
                Camviewer.sv.Wc = (int)numericUpDown6.Value;
                Camviewer.Reload = true;
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (doreload)
            {
                Camviewer.sv.Hc = (int)numericUpDown1.Value;
                Camviewer.Reload = true;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (doreload)
            {
                Camviewer.sv.LockAspectRatio = checkBox4.Checked;
                Init(false);
            }
        }
    }
}
