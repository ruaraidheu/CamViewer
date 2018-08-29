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
    public partial class Preferences : Form
    {
        public bool reset = false;
        public bool reload = false;
        Form1 f;
        public Preferences(Point l, Form1 f1)
        {
            f = f1;
            InitializeComponent();
            Location = l;
            if (Properties.Settings.Default.Disco)
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }
            if (Properties.Settings.Default.UseMQTT)
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }
            Version.Text = Form1.Version;
            textBox1.Text = Properties.Settings.Default.URILT;
            textBox2.Text = Properties.Settings.Default.URIRT;
            textBox3.Text = Properties.Settings.Default.URILM;
            textBox4.Text = Properties.Settings.Default.URIRM;
            textBox5.Text = Properties.Settings.Default.URIRB;
            textBox6.Text = Properties.Settings.Default.MQTT;
            textBox7.Text = Properties.Settings.Default.MQTTSubs;
            numericUpDown1.Value = (decimal)Properties.Settings.Default.b1;
            numericUpDown2.Value = (decimal)Properties.Settings.Default.b2;
            numericUpDown3.Value = (decimal)Properties.Settings.Default.b3;
            numericUpDown4.Value = (decimal)Properties.Settings.Default.b4;
            numericUpDown5.Value = (decimal)Properties.Settings.Default.b5;
            numericUpDown6.Value = (decimal)Properties.Settings.Default.b6;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.Disco = checkBox1.Checked;
            Properties.Settings.Default.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure you want to reset all settings?", "Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dr == System.Windows.Forms.DialogResult.Yes)
            {
                Properties.Settings.Default.Reset();
                if (Properties.Settings.Default.Disco)
                {
                    checkBox1.Checked = true;
                }
                else
                {
                    checkBox1.Checked = false;
                }
                if (Properties.Settings.Default.UseMQTT)
                {
                    checkBox2.Checked = true;
                }
                else
                {
                    checkBox2.Checked = false;
                }
                textBox1.Text = Properties.Settings.Default.URILT;
                textBox2.Text = Properties.Settings.Default.URIRT;
                textBox3.Text = Properties.Settings.Default.URILM;
                textBox4.Text = Properties.Settings.Default.URIRM;
                textBox5.Text = Properties.Settings.Default.URIRB;
                textBox6.Text = Properties.Settings.Default.MQTT;
                textBox7.Text = Properties.Settings.Default.MQTTSubs;
                reset = true;
                button2.PerformClick();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.URILT = textBox1.Text;
            reload = true;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.URIRT = textBox2.Text;
            reload = true;
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.URILM = textBox3.Text;
            reload = true;
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.URIRM = textBox4.Text;
            reload = true;
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.URIRB = textBox5.Text;
            reload = true;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MQTT = textBox6.Text;
            reload = true;
        }

        private void Preferences_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
            DialogResult = System.Windows.Forms.DialogResult.OK;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.UseMQTT = checkBox2.Checked;
            Properties.Settings.Default.Save();
            groupBox1.Visible = checkBox2.Checked;
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.MQTTSubs = textBox7.Text;
            reload = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.b1 = (int)numericUpDown1.Value;
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.b2 = (int)numericUpDown2.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.b3 = (int)numericUpDown3.Value;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.b4 = (int)numericUpDown4.Value;
        }

        private void numericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.b5 = (int)numericUpDown5.Value;
        }

        private void numericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default.b6 = (int)numericUpDown6.Value;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Backingup is not implemented.");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Restoring is not implemented.");
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.Experimental)
            {
                DialogResult dr = MessageBox.Show("Would you like to activate 'Experimental Mode', to test upcoming features.", "Activate Experimental Mode", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    Properties.Settings.Default.Experimental = true;
                    ExperimentPanel ep = new ExperimentPanel(f);
                }
            }
            else
            {
                ExperimentPanel ep = new ExperimentPanel(f);
            }
        }
    }
}
