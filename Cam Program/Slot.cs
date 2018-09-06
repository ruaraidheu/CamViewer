using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MjpegProcessor;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using System.Threading;

namespace Cam_Program
{
    public partial class Slot : UserControl
    {
        Slotdata s;
        PictureBox p;
        MjpegDecoder m;
        MQTTViewer v;
        MqttClient c;
        Aspectratiohelper arh;
        static bool reload = false;
        bool? lreload = false;
        bool psplash = true;

        public static bool DoReload { get => reload; set => reload = value; }
        public bool Psplash { get { ContextChanged(); return psplash; } set { ContextChanged(); psplash = value; } }

        public Slot(Slotdata sd)
        {
            InitializeComponent();
            s = sd;
            arh = new Aspectratiohelper(new Point(0), this);
            arh.Hide();
            p = new PictureBox();
            p.Dock = System.Windows.Forms.DockStyle.Fill;
            p.Location = new System.Drawing.Point(0, 0);
            p.Margin = new System.Windows.Forms.Padding(0);
            p.Name = "pictureBox1";
            p.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(204)))), ((int)(((byte)(232)))));
            p.Size = this.Size;
            p.Image = Properties.Resources.SplashCamviewer;
            Psplash = true;
            if (Camviewer.sv.LockAspectRatio)
            {
                p.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            }
            else
            {
                p.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            p.TabIndex = 0;
            p.TabStop = false;
            this.Controls.Add(p);
            v = new MQTTViewer();
            v.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(204)))), ((int)(((byte)(232)))));
            v.Dock = System.Windows.Forms.DockStyle.Fill;
            v.Location = new System.Drawing.Point(0, 0);
            v.Name = "mqttViewer1";
            v.Size = this.Size;
            v.TabIndex = 5;
            this.Controls.Add(v);
            v.ContextMenuStrip = contextMenuStrip2;
            if (s.Type == Slotdata.SlotType.Cam)
            {
                v.Hide();
            }
            else if (s.Type == Slotdata.SlotType.Mqtt)
            {
                StartMQTT(false);
                p.Hide();
            }
            timer1.Start();
            timer2.Start();
            Reload();
        }
        public void Reload()
        {
            arh.Reload();
            if (s.Type == Slotdata.SlotType.Cam)
            {
                v.Hide();
                p.Show();
                if (m != null)
                {
                    m.StopStream();
                    m = null;
                }
                m = new MjpegDecoder();
                m.FrameReady += go1;
                try
                {
                    m.ParseStream(new Uri(s.Address));
                }
                catch
                {

                }
            }
            else if (s.Type == Slotdata.SlotType.Mqtt)
            {
                p.Hide();
                v.Show();
                v.LineCount = s.Linecount;
                if (Camviewer.sv.Usemqtt)
                {
                    RestartMQTT();
                }
                else
                {
                    if (c != null)
                    {
                        try
                        {
                            c.Disconnect();
                            v.Print("MQTT Viewer Disconnected", Color.Red);
                        }
                        catch
                        {

                        }
                    }
                    c = null;
                }
            }
            if (!s.Enabled)
            {
                v.Hide();
                p.Show();
                if (m != null)
                {
                    m.StopStream();
                    m = null;
                }
                p.Image = Properties.Resources.SplashCamviewer;
                Psplash = true;
                try
                {
                    c.Disconnect();
                    v.Print("MQTT Viewer Disconnected", Color.Red);
                }
                catch
                {

                }
            }
            if (Camviewer.sv.LockAspectRatio)
            {
                p.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            }
            else
            {
                p.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            BackColor = Camviewer.sv.Color;
            v.BackColor = Camviewer.sv.Color;
            p.BackColor = Camviewer.sv.Color;
            ContextChanged();
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == System.Windows.Forms.MouseButtons.Right)
            //contextMenuStrip1.Show(Cursor.Position);
        }
        public void go1(object sender, FrameReadyEventArgs e)
        {
            p.Image = e.Bitmap;
            Psplash = false;
            if (!s.Enabled)
            {
                if (m != null)
                {
                    m.StopStream();
                    m = null;
                }
                p.Image = Properties.Resources.SplashCamviewer;
                Psplash = true;
            }
            if (arh != null)
            {
                if (e.Bitmap != null)
                {
                    arh.PushSizes(e.Bitmap.Size, ClientSize);
                }
            }
            e.Bitmap = null;
            e.FrameBuffer = null;
        }
        public static string[] Arraysubs(string s)
        {
            s.Replace(" ", String.Empty);
            return s.Split(',');
        }
        public void StartMQTT(bool reload)
        {
            if (Camviewer.sv.Usemqtt)
            {
                try
                {
                    if (reload)
                    {
                        v.Print("MQTT Viewer Reloading.....", Color.DarkOrange);
                    }
                    else
                    {
                        v.Print("MQTT Viewer Connecting.....", Color.DarkOrange);
                    }
                    c = new MqttClient(s.Address);
                    c.MqttMsgPublishReceived += MQTTEvent;
                    string MQTTid = Guid.NewGuid().ToString();
                    c.Connect(MQTTid);
                    string[] sb = Arraysubs(s.Subs);
                    byte[] b = new byte[sb.Length];
                    for (int i = 0; i < b.Length; i++)
                    {
                        b[i] = MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE;
                    }
                    c.Subscribe(sb, b);
                    if (reload)
                    {
                        v.Print("MQTT Viewer Reloaded", Color.Green);
                    }
                    else
                    {
                        v.Print("MQTT Viewer Connected", Color.Green);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    string dr = Ruaraidheulib.Winforms.MessageBox.Show("MQTT could not connect to host.", "Host invalid", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, "OK");
                }
            }
        }
        public void RestartMQTT()
        {
            bool rel = true;
            try
            {
                if (Camviewer.sv.Usemqtt && !c.IsConnected)
                {
                    rel = false;
                }
                c.Disconnect();
                Thread.Sleep(200);
                if (!Camviewer.sv.Usemqtt)
                {
                    v.Print("MQTT Viewer Disconnected", Color.Red);
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
            if (!v.IsDisposed && !v.Disposing)
            {
                if (v.InvokeRequired)
                {
                    try
                    {
                        SetTextCallback d = new SetTextCallback(settext);
                        try
                        {
                            Invoke(d, new object[] { tex });
                        }
                        catch (System.ComponentModel.InvalidAsynchronousStateException)
                        {

                        }
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                }
                else
                {
                    v.Print(tex);
                }
            }
        }
        delegate void SetTextCallbackcol(string text, Color c);
        public void settext(string tex, Color c)
        {
            if (!v.IsDisposed && !v.Disposing)
            {
                if (v.InvokeRequired)
                {
                    try
                    {
                        SetTextCallbackcol d = new SetTextCallbackcol(settext);
                        try
                        {
                            Invoke(d, new object[] { tex, c });
                        }
                        catch (System.ComponentModel.InvalidAsynchronousStateException)
                        {

                        }
                    }
                    catch (ObjectDisposedException)
                    {

                    }
                }
                else
                {
                    v.Print(tex, c);
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
            settext("[" + e.Topic + "]", Color.Gray);
            settext(rec);
        }

        public void Exit()
        {
            try
            {
                c.Disconnect();
                v.Print("MQTT Viewer Disconnected", Color.Red);
            }
            catch
            {

            }
            if (m != null)
            {
                m.StopStream();
            }
            arh.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Camviewer.DoExit();
        }

        private void localSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LocalSettings.Show(s, Parent.Location + (Ruaraidheulib.point)Location);
            Reload();
        }

        private void generalSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneralSettings.Show(Parent.Location + (Ruaraidheulib.point)Location);
        }
        int rcount = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (lreload == null && rcount > 2)
            {
                lreload = false;
                reload = false;
                rcount = 0;
            }
            if (rcount == 0)
            {
                lreload = reload;
            }
            if (reload)
            {
                rcount++;
            }
            else
            {
                rcount = 0;
            }
            if (lreload == true)
            {
                Reload();
                lreload = null;
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Camviewer.sv.Disco)
            {
                Random RNG = new Random();
                BackColor = Color.FromArgb(RNG.Next(0, 256), RNG.Next(0, 256), RNG.Next(0, 256)); ;
                v.BackColor = Color.FromArgb(RNG.Next(0, 256), RNG.Next(0, 256), RNG.Next(0, 256)); ;
                p.BackColor = Color.FromArgb(RNG.Next(0, 256), RNG.Next(0, 256), RNG.Next(0, 256)); ;
            }
        }

        private void aspectRatioHelperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            arh.Location = Location;
            arh.Show();
        }

        private void Slot_Resize(object sender, EventArgs e)
        {
            if (arh != null)
                arh.PushSizes(arh.Iz, ClientSize);
        }
        public void ContextChanged()
        {
            if (v != null)
            {
                if (s.Type == Slotdata.SlotType.Mqtt || s.Enabled == false || psplash)
                {
                    v.ContextMenuStrip = contextMenuStrip2;
                    ContextMenuStrip = contextMenuStrip2;
                }
                else
                {
                    v.ContextMenuStrip = contextMenuStrip1;
                    ContextMenuStrip = contextMenuStrip1;
                }
            }
            else
            {
                if (s.Type == Slotdata.SlotType.Mqtt || s.Enabled == false || psplash)
                {
                    ContextMenuStrip = contextMenuStrip2;
                }
                else
                {
                    ContextMenuStrip = contextMenuStrip1;
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (s.Type == Slotdata.SlotType.Mqtt || s.Enabled == false || psplash)
            {
                contextMenuStrip1.Items[2].Enabled = false;
            }
            else
            {
                contextMenuStrip1.Items[2].Enabled = true;
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            localSettingsToolStripMenuItem_Click(null, null);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            generalSettingsToolStripMenuItem_Click(null, null);
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            exitToolStripMenuItem_Click(null, null);
        }
    }
}
