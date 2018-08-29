using Ruaraidheulib;
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
    public partial class LocalSettings : Form
    {
        Slotdata s;
        bool change = false;
        public LocalSettings(Slotdata sd)
        {
            InitializeComponent();
            groupBox1.Controls.Remove(groupBox2);
            this.Controls.Add(groupBox2);
            groupBox2.Location = (Ruaraidheulib.point)groupBox2.Location + (Ruaraidheulib.point)groupBox1.Location;
            s = sd;
            this.Version.Text = VersionInfo.Get().FullVersion;
            change = true;
            Init();
        }
        public void Init()
        {
            if (change)
            {
                change = false;
                comboBox1.DataSource = Enum.GetValues(typeof(Slotdata.SlotType));
                comboBox1.SelectedIndex = Convert.ToInt32(s.Type);
                switch (s.Type)
                {
                    case Slotdata.SlotType.Cam: groupBox2.Show(); groupBox1.Hide(); break;
                    case Slotdata.SlotType.Mqtt: groupBox1.Show(); groupBox2.Hide(); break;
                }
                textBox1.Text = s.Address;
                textBox6.Text = s.Address;
                textBox7.Text = s.Subs;
                checkBox1.Checked = s.Enabled;
                checkBox2.Checked = s.Enabled;
                numericUpDown6.Value = s.Linecount;
                if (Camviewer.sv.Darktheme)
                {
                    BackColor = GeneralSettings.ColorDark;
                    label1.ForeColor = Color.White;
                    label4.ForeColor = Color.White;
                    label7.ForeColor = Color.White;
                    label8.ForeColor = Color.White;
                    groupBox1.ForeColor = Color.White;
                    groupBox2.ForeColor = Color.White;
                    Version.ForeColor = Color.White;
                    checkBox1.ForeColor = Color.White;
                    checkBox2.ForeColor = Color.White;
                }
                else
                {
                    BackColor = GeneralSettings.ColorLight;
                    label1.ForeColor = Color.Black;
                    label4.ForeColor = Color.Black;
                    label7.ForeColor = Color.Black;
                    label8.ForeColor = Color.Black;
                    groupBox1.ForeColor = Color.Black;
                    groupBox2.ForeColor = Color.Black;
                    Version.ForeColor = Color.Black;
                    checkBox1.ForeColor = Color.Black;
                    checkBox2.ForeColor = Color.Black;
                }
                change = true;
            }
        }
        public static Slotdata Show(Slotdata s, Point loc)
        {
            using (LocalSettings l = new LocalSettings(s))
            {
                l.Location = loc;
                l.ShowDialog();
            }
            return (Slotdata)s.Clone();
        }
        public static Slotdata ShowNoEdit(Slotdata s, Point loc)
        {
            Slotdata sd = (Slotdata)s.Clone();
            using (LocalSettings l = new LocalSettings(sd))
            {
                l.Location = loc;
                l.ShowDialog();
            }
            return sd;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (change)
            {
                Slotdata.SlotType st = s.Type;
                Enum.TryParse<Slotdata.SlotType>(comboBox1.SelectedValue.ToString(), out st);
                s.Type = st;
                Init();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            s = new Slotdata();
            Init();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (change)
            {
                s.Enabled = checkBox1.Checked;
                Init();
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (change)
            {
                s.Enabled = checkBox2.Checked;
                Init();
            }
        }

        private void LocalSettings_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (s.Type == Slotdata.SlotType.Cam)
            {
                Uri a;
                if (!(Uri.TryCreate(s.Address, UriKind.Absolute, out a) && (a.Scheme == Uri.UriSchemeHttp || a.Scheme == Uri.UriSchemeHttps)))
                {
                    string ans = Ruaraidheulib.Winforms.MessageBox.Show("Can't parse address. Make sure there are no typos.", "Address invalid", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Two, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, "Ignore", "Try Again");
                    if (ans == "Try Again")
                    {
                        e.Cancel = true;
                    }
                    else
                    {
                    }
                }
            }
        }

        private void LocalSettings_FormClosed(object sender, FormClosedEventArgs e)
        {
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (change)
            {
                s.Address = textBox1.Text;
                Init();
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (change)
            {
                s.Subs = textBox7.Text;
                Init();
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (change)
            {
                s.Address = textBox6.Text;
                Init();
            }
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (change)
            {
                s.Linecount = (int)numericUpDown6.Value;
                Init();
            }
        }
    }
}
