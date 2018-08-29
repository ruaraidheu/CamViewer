using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Cam_Program
{
    public partial class MQTTViewer : UserControl
    {
        int maxline = 500;
        public MQTTViewer()
        {
            InitializeComponent();
            string s = "MQTT Viewer Console Version 1.0-b.";
            int len = richTextBox1.Text.Length;
            richTextBox1.AppendText(s);
            richTextBox1.ScrollToCaret();
            richTextBox1.SelectionStart = len;
            richTextBox1.SelectionLength = s.Length;
            richTextBox1.SelectionColor = Color.Indigo;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            timer1.Start();
        }
        public int LineCount { get { return maxline; } set { maxline = value; } }
        List<string> lst = new List<string>();
        List<Color> lco = new List<Color>();
        public void Print(string s, Color col)
        {
            lst.Add(s);
            lco.Add(col);
        }
        /// <summary>
        /// Adds new line to rich text box and adds input string.
        /// </summary>
        /// <param name="s">Input string</param>
        public void PrintT(string s, Color col)
        {
            if (richTextBox1.Lines.Length >= maxline)
            {
                richTextBox1.Select(0, richTextBox1.GetFirstCharIndexFromLine(richTextBox1.Lines.Length - (maxline-1)));
                richTextBox1.ReadOnly = false;
                richTextBox1.SelectedText = "";
                richTextBox1.ReadOnly = true;
                richTextBox1.SelectionStart = richTextBox1.Text.Length;
                richTextBox1.SelectionLength = 0;
                //richTextBox1.Lines = richTextBox1.Lines.Skip(richTextBox1.Lines.Length - maxline).ToArray();
            }
            int len = richTextBox1.Text.Length;
            string t = Environment.NewLine + s;
            richTextBox1.AppendText(t);
            richTextBox1.ScrollToCaret();
            richTextBox1.SelectionStart = len;
            richTextBox1.SelectionLength = t.Length;
            richTextBox1.SelectionColor = col;
            richTextBox1.SelectionStart = richTextBox1.Text.Length;
            richTextBox1.SelectionLength = 0;
        }
        public void Print(string s)
        {
            if ((BackColor.R * 0.299 + BackColor.G * 0.587 + BackColor.B * 0.114) > 186)
            {
                Print(s, Color.Black);
            }
            else
            {
                Print(s, Color.White);
            }
        }
        public void Print(string s, params object[] args)
        {
            s = String.Format(s, args);
            Print(s);
        }
        public void Print(string s, Color col, params object[] args)
        {
            s = String.Format(s, args);
            Print(s, col);
        }

        private void MQTTViewer_BackColorChanged(object sender, EventArgs e)
        {
            richTextBox1.BackColor = BackColor;
        }

        private void MQTTViewer_ContextMenuStripChanged(object sender, EventArgs e)
        {
            richTextBox1.ContextMenuStrip = this.ContextMenuStrip;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            while (lst.Count > 0)
            {
                try
                {
                    if (lst.First()!=null&&lco.First()!=null)
                        PrintT(lst.First(), lco.First());
                }
                catch (NullReferenceException ex)
                {
                    Ruaraidheulib.Winforms.MessageBox.Show("NullReferenceException in PrintT has been caught, please report this" + Environment.NewLine
                        + Environment.NewLine
                        + "is RTB null: " + (richTextBox1 == null).ToString() + Environment.NewLine
                        + "LST: " + lst.ToString() + Environment.NewLine
                        + "LST0: " + lst[0] + Environment.NewLine
                        + "LCO: "+lco.ToString() + Environment.NewLine
                        + "LCO0: " + lco[0].ToString() + Environment.NewLine
                        + "RTB: "+richTextBox1.ToString() + Environment.NewLine
                        + "Target Site: "+ex.TargetSite + Environment.NewLine
                        + "Stack Trace: "+ex.StackTrace + Environment.NewLine
                        + "Message: "+ex.Message + Environment.NewLine
                        + "Selection Start: "+richTextBox1.SelectionStart+Environment.NewLine
                        + "Selection Length: " +richTextBox1.SelectionLength+ Environment.NewLine
                        );
                }
                lst.RemoveAt(0);
                lco.RemoveAt(0);
            }
        }
    }
}
