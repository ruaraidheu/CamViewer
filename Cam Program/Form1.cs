using MjpegProcessor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace Cam_Program
{
    public partial class Form1 : Form
    {
        public static string Version = "V1.0.1.0";
        MjpegDecoder d1;
        MjpegDecoder d2;
        MjpegDecoder d3;
        MjpegDecoder d4;
        MjpegDecoder d5;
        MqttClient MQTT;
        Random RNG = new Random();
        public Form1()
        {
            InitializeComponent();
            //Startup s = new Startup(this);
            timer1.Start();
            Setpos();
            Location = Properties.Settings.Default.location;
            Size = Properties.Settings.Default.size;
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            d1 = new MjpegDecoder();
            d1.FrameReady += go1;
            try
            {
                d1.ParseStream(new Uri(Properties.Settings.Default.URILT));
            }
            catch
            {

            }
            d2 = new MjpegDecoder();
            d2.FrameReady += go2;
            try
            {
                d2.ParseStream(new Uri(Properties.Settings.Default.URIRT));
            }
            catch
            {

            }
            d3 = new MjpegDecoder();
            d3.FrameReady += go3;
            try
            {
                d3.ParseStream(new Uri(Properties.Settings.Default.URILM));
            }
            catch
            {

            }
            d4 = new MjpegDecoder();
            d4.FrameReady += go4;
            try
            {
                d4.ParseStream(new Uri(Properties.Settings.Default.URIRM));
            }
            catch
            {

            }
            d5 = new MjpegDecoder();
            d5.FrameReady += go5;
            try
            {
                d5.ParseStream(new Uri(Properties.Settings.Default.URIRB));
            }
            catch
            {

            }
            if (Properties.Settings.Default.Usecolor)
            {
                BackColor = Properties.Settings.Default.BGColor;
                mqttViewer1.BackColor = Properties.Settings.Default.BGColor;
            }
            else
            {
                BackColor = Color.FromArgb(184, 204, 232);
                mqttViewer1.BackColor = Color.FromArgb(184, 204, 232);
            }
            StartMQTT(false);
        }
        public static string[] Arraysubs(string s)
        {
            s.Replace(" ", String.Empty);
            return s.Split(',');
        }
        public void StartMQTT(bool reload)
        {
            if (Properties.Settings.Default.UseMQTT)
            {
                try
                {
                    if (reload)
                    {
                        mqttViewer1.Print("MQTT Viewer Reloading.....", Color.DarkOrange);
                    }
                    else
                    {
                        mqttViewer1.Print("MQTT Viewer Connecting.....", Color.DarkOrange);
                    }
                    MQTT = new MqttClient(Properties.Settings.Default.MQTT);
                    MQTT.MqttMsgPublishReceived += MQTTEvent;
                    string MQTTid = Guid.NewGuid().ToString();
                    MQTT.Connect(MQTTid);
                    string[] s = Arraysubs(Properties.Settings.Default.MQTTSubs);
                    byte[] b = new byte[s.Length];
                    for (int i = 0; i < b.Length; i++)
                    {
                        b[i] = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE;
                    }
                    MQTT.Subscribe(s, b);
                    if (reload)
                    {
                        mqttViewer1.Print("MQTT Viewer Reloaded", Color.Green);
                    }
                    else
                    {
                        mqttViewer1.Print("MQTT Viewer Connected", Color.Green);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    DialogResult dr = MessageBox.Show("MQTT could not connect to host.", "Host invalid", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
                    if (dr == System.Windows.Forms.DialogResult.Ignore)
                    {

                    }
                    else if (dr == System.Windows.Forms.DialogResult.Retry)
                    {
                        StartMQTT(false);
                    }
                    else
                    {
                        dr = MessageBox.Show("Would you like to exit the application? Pressing no will allow you to enter a new address.", "Abort", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                        if (dr == System.Windows.Forms.DialogResult.Yes)
                        {
                            Application.Exit();
                        }
                        else
                        {
                            InputBox ib = new InputBox("Enter a valid MQTT URL", "Enter URL", "");
                            dr = ib.ShowDialog();
                            if (dr == System.Windows.Forms.DialogResult.OK)
                            {
                                Properties.Settings.Default.MQTT = ib.textBox1.Text;
                                Properties.Settings.Default.Save();
                                StartMQTT(false);
                            }
                        }
                    }
                }
            }
        }
        public void RestartMQTT()
        {
            bool rel = true;
            try
            {
                if (Properties.Settings.Default.UseMQTT && !MQTT.IsConnected)
                {
                    rel = false;
                }
                MQTT.Disconnect();
                Thread.Sleep(200);
                if (!Properties.Settings.Default.UseMQTT)
                {
                    mqttViewer1.Print("MQTT Viewer Disconnected", Color.Red);
                }
            }
            catch
            {
                rel = false;
            }
            StartMQTT(rel);
        }
        delegate void SetTextCallback(string text);
        public void settext(string tex)
        {
            if (!mqttViewer1.IsDisposed & !mqttViewer1.Disposing)
            {
                if (mqttViewer1.InvokeRequired)
                {
                    SetTextCallback d = new SetTextCallback(settext);
                    Invoke(d, new object[] { tex });
                }
                else
                {
                    mqttViewer1.Print(tex);
                }
            }
        }
        public void MQTTEvent(object sender, MqttMsgPublishEventArgs e)
        {
            byte[] b = e.Message;
            int k = e.Message.Length;
            string rec = "";
            for (int i = 0; i < k; i++)
            {
                rec = rec + Convert.ToChar(b[i]);
            }
            settext(rec);
        }
        public void Setpos()
        {
            int[] a = Resolveid(Properties.Settings.Default.b1);
            int[] b = Resolveid(Properties.Settings.Default.b2);
            int[] c = Resolveid(Properties.Settings.Default.b3);
            int[] d = Resolveid(Properties.Settings.Default.b4);
            int[] e = Resolveid(Properties.Settings.Default.b5);
            int[] f = Resolveid(Properties.Settings.Default.b6);
            tableLayoutPanel1.SetCellPosition(pictureBox1, new TableLayoutPanelCellPosition(a[0], a[1]));
            tableLayoutPanel1.SetCellPosition(pictureBox2, new TableLayoutPanelCellPosition(b[0], b[1]));
            tableLayoutPanel1.SetCellPosition(pictureBox3, new TableLayoutPanelCellPosition(c[0], c[1]));
            tableLayoutPanel1.SetCellPosition(pictureBox4, new TableLayoutPanelCellPosition(d[0], d[1]));
            tableLayoutPanel1.SetCellPosition(pictureBox5, new TableLayoutPanelCellPosition(e[0], e[1]));
            tableLayoutPanel1.SetCellPosition(mqttViewer1, new TableLayoutPanelCellPosition(f[0], f[1]));
        }
        public int[] Resolveid(int a)
        {
            int[] b = new int[2];
            if (a % 2 == 0)
                b[0] = 1;
            else
                b[0] = 0;
            b[1] = ((a - 1) / 2);
            return b;
        }
        public void go1(object sender, FrameReadyEventArgs e)
        {
            pictureBox1.Image = e.Bitmap;
            e.Bitmap = null;
            e.FrameBuffer = null;
        }
        public void go2(object sender, FrameReadyEventArgs e)
        {
            pictureBox2.Image = e.Bitmap;
            e.Bitmap = null;
            e.FrameBuffer = null;
        }
        public void go3(object sender, FrameReadyEventArgs e)
        {
            pictureBox3.Image = e.Bitmap;
            e.Bitmap = null;
            e.FrameBuffer = null;
        }
        public void go4(object sender, FrameReadyEventArgs e)
        {
            pictureBox4.Image = e.Bitmap;
            e.Bitmap = null;
            e.FrameBuffer = null;
        }
        public void go5(object sender, FrameReadyEventArgs e)
        {
            pictureBox5.Image = e.Bitmap;
            e.Bitmap = null;
            e.FrameBuffer = null;
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Properties.Settings.Default.Disco)
            {
                BackColor = Color.FromArgb(RNG.Next(0, 256), RNG.Next(0, 256), RNG.Next(0, 256));
                mqttViewer1.BackColor = Color.FromArgb(RNG.Next(0, 256), RNG.Next(0, 256), RNG.Next(0, 256));
            }
        }
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Preferences p = new Preferences(Location, this))
            {
                p.ShowDialog();
                if (p.reset == true)
                {
                    Location = Properties.Settings.Default.location;
                    Size = Properties.Settings.Default.size;
                    p.reload = true;
                }
                if (p.reload == true)
                {
                    Setpos();
                    d1.StopStream();
                    d2.StopStream();
                    d3.StopStream();
                    d4.StopStream();
                    d5.StopStream();
                    d1 = null;
                    d2 = null;
                    d3 = null;
                    d4 = null;
                    d5 = null;
                    Thread.Sleep(200);
                    d1 = new MjpegDecoder();
                    d1.FrameReady += go1;
                    try
                    {
                        d1.ParseStream(new Uri(Properties.Settings.Default.URILT));
                    }
                    catch
                    {

                    }
                    d2 = new MjpegDecoder();
                    d2.FrameReady += go2;
                    try
                    {
                        d2.ParseStream(new Uri(Properties.Settings.Default.URIRT));
                    }
                    catch
                    {

                    }
                    d3 = new MjpegDecoder();
                    d3.FrameReady += go3;
                    try
                    {
                        d3.ParseStream(new Uri(Properties.Settings.Default.URILM));
                    }
                    catch
                    {

                    }
                    d4 = new MjpegDecoder();
                    d4.FrameReady += go4;
                    try
                    {
                        d4.ParseStream(new Uri(Properties.Settings.Default.URIRM));
                    }
                    catch
                    {

                    }
                    d5 = new MjpegDecoder();
                    d5.FrameReady += go5;
                    try
                    {
                        d5.ParseStream(new Uri(Properties.Settings.Default.URIRB));
                    }
                    catch
                    {

                    }
                    RestartMQTT();
                    p.reload = false;
                }
                if (Properties.Settings.Default.Usecolor)
                {
                    BackColor = Properties.Settings.Default.BGColor;
                    mqttViewer1.BackColor = Properties.Settings.Default.BGColor;
                }
                else
                {
                    BackColor = Color.FromArgb(184, 204, 232);
                    mqttViewer1.BackColor = Color.FromArgb(184, 204, 232);
                }
                p.Close();
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                MQTT.Disconnect();
                mqttViewer1.Print("MQTT Viewer Disconnected", Color.Red);
            }
            catch
            {

            }
            Properties.Settings.Default.location = Location;
            Properties.Settings.Default.size = Size;
            Properties.Settings.Default.Save();
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
                contextMenuStrip1.Show(Cursor.Position);
        }
    }
}
