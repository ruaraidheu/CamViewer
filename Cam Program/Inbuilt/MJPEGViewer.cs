using MjpegProcessor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cam_Program.Inbuilt
{
    class MJPEGViewer : API.V0.SlotObject
    {
        PictureBox p;
        MjpegDecoder m;
        Aspectratiohelper arh;
        public override void Start()
        {
            API.V0.Initial.AddSlotData<MJPEGViewer>();
        }
        public override void Initialize()
        {
            p = new PictureBox();
            p.Dock = System.Windows.Forms.DockStyle.Fill;
            p.Location = new System.Drawing.Point(0, 0);
            p.Margin = new System.Windows.Forms.Padding(0);
            p.Name = "pictureBox1";
            p.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(184)))), ((int)(((byte)(204)))), ((int)(((byte)(232)))));
            //p.Size = this.Size;
            p.Image = Properties.Resources.SplashCamviewer;
            //Psplash = true;
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
            //p.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDown);
            //this.Controls.Add(p);
            arh = new Aspectratiohelper(new Point(0), p);
            arh.Hide();
            base.Initialize();
        }
        public override void Reload()
        {

        }
    }
}
