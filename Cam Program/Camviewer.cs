using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ruaraidheulib;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using Ruaraidheulib.Interface.reulib64.Win64.Console;
using System.Security.Permissions;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;
using v = Ruaraidheulib.Interface.reulib64.Win64.Version;

namespace Cam_Program
{
    public partial class Camviewer : Form
    {
        public static string[] cla;
        bool resize = false;
        static bool exit = false;
        static bool reload = false;
        public static Saveable sv;
        List<Slot> ls = new List<Slot>();
        private static Size tsize;

        public static bool Reload { get => reload; set => reload = value; }
        public static Size Tsize { get => tsize; set => tsize = value; }
        public static bool Exit { get => exit; set => exit = value; }

        public static void DoExit()
        {
            Exit = true;
        }
        public Camviewer(Saveable s)
        {
            sv = s;
            
            InitializeComponent();
            Location = sv.Location;
            ClientSize = sv.Size;
            tsize = ClientSize;
            System.Net.ServicePointManager.DefaultConnectionLimit = 100;
            while (sv.Sd.Count < sv.Wc * sv.Hc)
            {
                sv.Sd.Add(new Slotdata());
            }
            for (int i = 0; i < (sv.Wc * sv.Hc); i++)
            {
                ls.Add(new Slot(sv.Sd[i]));
                ls[i].Width = ClientSize.Width / sv.Wc;
                ls[i].Height = ClientSize.Height / sv.Hc;
                ls[i].Location = new Point(Resolveloc(i, ls[i].Size).X, Resolveloc(i, ls[i].Size).Y);
                this.Controls.Add(ls[i]);
            }
            timer1.Start();
        }
        public void FullReload()
        {
            foreach(Slot slot in ls)
            {
                slot.Exit();
                slot.Dispose();
            }
            ls.Clear();
            while (sv.Sd.Count < sv.Wc * sv.Hc)
            {
                sv.Sd.Add(new Slotdata());
            }
            for (int i = 0; i < (sv.Wc * sv.Hc); i++)
            {
                ls.Add(new Slot(sv.Sd[i]));
                ls[i].Width = ClientSize.Width / sv.Wc;
                ls[i].Height = ClientSize.Height / sv.Hc;
                ls[i].Location = new Point(Resolveloc(i, ls[i].Size).X, Resolveloc(i, ls[i].Size).Y);
                this.Controls.Add(ls[i]);
            }
        }
        public TwoT<int> Resolveloc(int a, Size s)
        {
            TwoT<int> b = new TwoT<int>();
            b.X = (int)((float)(a % sv.Wc) * s.Width);
            b.Y = (int)(Math.Floor((double)(a / sv.Wc)) * s.Height);
            return b;
        }

        private void Camviewer_Resize(object sender, EventArgs e)
        {
            Tsize = ClientSize;
            resize = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Tsize != ClientSize)
            {
                ClientSize = Tsize;
                resize = true;
            }
            if (resize)
            {
                for (int i = 0; i < (sv.Wc * sv.Hc); i++)
                {
                    ls[i].Width = ClientSize.Width / sv.Wc;
                    ls[i].Height = ClientSize.Height / sv.Hc;
                    ls[i].Location = new Point(Resolveloc(i, ls[i].Size).X, Resolveloc(i, ls[i].Size).Y);
                }
                sv.Size = ClientSize;
                resize = false;
            }
            if (reload)
            {
                FullReload();
                reload = false;
            }
            if (exit)
            {
                this.Close();
            }
        }

        private void Camviewer_Load(object sender, EventArgs e)
        {
            if (exit)
            {
                this.Close();
            }
        }

        private void Camviewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            Saveable.SerializeItem(sv);
            foreach(Slot sl in ls)
            { sl.Exit(); }
        }

        private void Camviewer_LocationChanged(object sender, EventArgs e)
        {
            sv.Location = Location;
        }
    }
    public class IncorrectVersionException : Exception
    {
        public IncorrectVersionException(ModuleVersion v0, ModuleVersion v1)
        {
            Console.WriteLine("The Current version is " + v0.ToString() + " it must be at least " + v1.ToString());
        }
    }
    [Serializable()]
    public class Saveable : ISerializable
    {
        private static string path = Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "Camviewer";
        private static string name = System.IO.Path.DirectorySeparatorChar + "config.cvsf";
        ModuleVersion version = v.VersionInfo.Get().ProductVersion;
        bool disco = false;
        bool usemqtt = true;
        bool darktheme = true;
        Point location = new Point(100, 100);
        Size size = new Size(800, 950);
        Color color = Color.FromArgb(32, 32, 32);
        int wc = 2;
        int hc = 3;
        bool lockaspectratio = true;
        bool readOnly = false;
        bool askfiledeletecrash = true;
        bool alwaysdeletecrash = false;
        bool askfiledeletelog = true;
        bool alwaysdeletelog = false;
        List<Slotdata> sd = new List<Slotdata>();

        public static string Path { get => path; set => path = value; }
        public static string Pathc { get; set; } = System.IO.Path.DirectorySeparatorChar + "Config";
        public static string Pathl { get; set; } = System.IO.Path.DirectorySeparatorChar + "Log";
        public static string Pathp { get; set; } = System.IO.Path.DirectorySeparatorChar + "Plugins";
        public static string Name { get => name; set => name = value; }
        public ModuleVersion Version { get => version; set { } }
        public bool Disco { get => disco; set => disco = value; }
        public bool Usemqtt { get => usemqtt; set => usemqtt = value; }
        public bool Darktheme { get => darktheme; set => darktheme = value; }
        public Point Location { get => location; set => location = value; }
        public Size Size { get => size; set => size = value; }
        [XmlIgnore]
        public Color Color { get => color; set => color = value; }
        [XmlElement(Namespace = "REULIB")]
        public Ruaraidheulib.Interface.reulib64.GameObjects.Color ColorRGBA { get => (Ruaraidheulib.Interface.reulib64.GameObjects.Color)color; set => color = (Color)value; }
        public int Wc { get => wc; set => wc = value; }
        public int Hc { get => hc; set => hc = value; }
        public bool LockAspectRatio { get => lockaspectratio; set => lockaspectratio = value; }
        public bool PluginsEnabled { get; set; } = false;
        public List<Slotdata> Sd { get => sd; set => sd = value; }
        public bool ReadOnly { get => readOnly; set => readOnly = value; }
        public bool Askfiledeletecrash { get => askfiledeletecrash; set => askfiledeletecrash = value; }
        public bool Alwaysdeletecrash { get => alwaysdeletecrash; set => alwaysdeletecrash = value; }
        public bool Askfiledeletelog { get => askfiledeletelog; set => askfiledeletelog = value; }
        public bool Alwaysdeletelog { get => alwaysdeletelog; set => alwaysdeletelog = value; }

        public Saveable()
        {
            Kcon();
        }
        public void Kcon()
        {
        }


        
        public Saveable(SerializationInfo info, StreamingContext context)
        {
            Kcon();
            ModuleVersion v = (ModuleVersion)info.GetValue("Version", typeof(ModuleVersion));
            if (v < "2.0")
            {
                throw new IncorrectVersionException(v, "2.0");
            }
            path = (string)info.GetValue("Path", typeof(string));
            disco = (bool)info.GetValue("Disco Mode", typeof(bool));
            usemqtt = (bool)info.GetValue("Use MQTT", typeof(bool));
            darktheme = (bool)info.GetValue("Dark Theme", typeof(bool));
            location = (Point)info.GetValue("Location", typeof(Point));
            size = (Size)info.GetValue("Size", typeof(Size));
            color = (Ruaraidheulib.Interface.reulib64.GameObjects.Color)info.GetValue("Color", typeof(Ruaraidheulib.Interface.reulib64.GameObjects.Color));
            wc = (int)info.GetValue("Width Count", typeof(int));
            hc = (int)info.GetValue("Height Count", typeof(int));
            sd = (List<Slotdata>)info.GetValue("Slotdata", typeof(List<Slotdata>));
        }
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Path", path);
            info.AddValue("Version", version);
            info.AddValue("Disco Mode", disco);
            info.AddValue("Use MQTT", usemqtt);
            info.AddValue("Dark Theme", darktheme);
            info.AddValue("Location", location);
            info.AddValue("Size", size);
            info.AddValue("Color", (Ruaraidheulib.Interface.reulib64.GameObjects.Color)color);
            info.AddValue("Width Count", wc);
            info.AddValue("Height Count", hc);
            info.AddValue("Slotdata", sd);
        }



        public static void SerializeItem(Saveable t)
        {
            SerializeItem(t, path, name);
        }
        public static void SerializeItem(Saveable t, string _path, string _name)
        {
            // Create an instance of the type and serialize it.
            if (!t.readOnly)
            {
                bool? exit = true;
                if (!Directory.Exists(_path))
                {
                    while (exit == true)
                    {
                        exit = null;
                        try
                        {
                            Directory.CreateDirectory(_path);
                            exit = false;
                        }
                        catch (UnauthorizedAccessException)
                        {
                            string dr = Ruaraidheulib.Winforms.MessageBox.Show("Don't have permission to write file to " + _path + _name, "Can't save file", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Two, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Two, "Retry", "Ignore");
                            if (dr == "Retry")
                            {
                                exit = true;
                            }
                            else if (dr == "Ignore")
                            {
                                exit = null;
                            }
                        }
                    }
                }
                else
                {
                    exit = false;
                }


                if (exit == false)
                {
                    if (Directory.Exists(_path))
                    {
                        if (File.Exists(_path + _name + ".bak2"))
                        {
                            File.Copy(_path + _name + ".bak2", _path + _name + ".bak3", true);
                        }
                        if (File.Exists(_path + _name + ".bak1"))
                        {
                            File.Copy(_path + _name + ".bak1", _path + _name + ".bak2", true);
                        }
                        if (File.Exists(_path + _name))
                        {
                            File.Copy(_path + _name, _path + _name + ".bak1", true);
                        }

                        FileStream s = null;
                        try
                        {
                            s = new FileStream(_path + _name, FileMode.Create);
                        }
                        catch (Exception e)
                        {
                            Ruaraidheulib.Winforms.MessageBox.Show(Ruaraidheulib.Winforms.MessageBox.ErrorWrite(e));
                        }
                        bool retry = true;
                        if (s != null)
                        {
                            while (retry)
                            {
                                retry = false;
                                try
                                {
                                    new System.Xml.Serialization.XmlSerializer(typeof(Saveable)).Serialize(s, t);
                                }
                                catch (Exception e)
                                {
                                    string dr = "";
                                    if (e.InnerException != null)
                                    {
                                        dr = Ruaraidheulib.Winforms.MessageBox.Show("Can't save file: " + _path + _name + Environment.NewLine + e.Message + Environment.NewLine + e.InnerException.Message + Environment.NewLine + "Retry will try to save file again. Cancel will exit without saving.", "Error", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Two, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Two, "Retry", "Don't Save");
                                    }
                                    else
                                    {
                                        dr = Ruaraidheulib.Winforms.MessageBox.Show("Can't save file: " + _path + _name + Environment.NewLine + e.Message, "Error", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Two, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Two, "Retry", "Don't Save");
                                    }
                                    if (dr == "Retry")
                                    {
                                        retry = true;
                                    }
                                    else
                                    {
                                        retry = false;
                                    }
                                }
                            }
                            s.Close();
                            s.Dispose();
                        }
                    }
                }
            }
        }
        public static Saveable DeserializeItem()
        {
            return DeserializeItem(path, name);
        }
        public static Saveable DeserializeItem(string _path, string _name, int backup = 1, bool filesexist = false)
        {
            if (File.Exists(_path + _name))
            {
                filesexist = true;
                Saveable t = new Saveable();
                try
                {
                    using (FileStream s = new FileStream(_path + _name, FileMode.Open))
                    {
                        t = (Saveable)new System.Xml.Serialization.XmlSerializer(typeof(Saveable)).Deserialize(s);
                    }
                }
                catch (Exception e)
                {
                    string dr;
                    if (e.InnerException != null)
                    {
                        dr = Ruaraidheulib.Winforms.MessageBox.Show("Can't load save file: " + _path + _name + Environment.NewLine + e.Message + Environment.NewLine + e.InnerException.Message + Environment.NewLine, "Error", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Four, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Three, "Abort", "Retry", "Load Backup", "Defaults");
                    }
                    else
                    {
                        dr = Ruaraidheulib.Winforms.MessageBox.Show("Can't load save file: " + _path + _name + Environment.NewLine + e.Message + Environment.NewLine, "Error", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Four, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Three, "Abort", "Retry", "Load Backup", "Defaults");
                    }
                    if (dr == "Abort")
                    {
                        t.ReadOnly = true;
                        Application.Exit();
                    }
                    else if (dr == "Retry")
                    {
                        t = DeserializeItem(_path, _name, backup, filesexist);
                    }
                    else if (dr == "Load Backup")
                    {
                        if (backup > 3)
                        {
                            Ruaraidheulib.Winforms.MessageBox.Show("No valid backup files.");
                        }
                        else
                        {
                            t = DeserializeItem(_path, _name + ".bak" + backup.ToString(), backup + 1, filesexist);
                        }
                    }
                }
                return t;
            }
            else
            {
                if (backup > 3)
                {
                    if (filesexist)
                        Ruaraidheulib.Winforms.MessageBox.Show("No valid backup files.");
                }
                else
                {
                    return DeserializeItem(_path, _name + ".bak" + backup.ToString(), backup + 1, filesexist);
                }
                return new Saveable();
            }
        }

        //public void WriteXml(XmlWriter writer)
        //{
        //    XmlDesigner.Write(Writer, writer);
        //}
        public void Writer(XmlDocument x)
        {
            //XmlNode xversion = XmlDesigner.XmlDesign("Version", Version, x);
            //XmlNode xdisco = XmlDesigner.XmlDesign("Disco", Disco, x);
            //XmlNode xmqtt = XmlDesigner.XmlDesign("Usemqtt", Usemqtt, x);
            //XmlNode xdark = XmlDesigner.XmlDesign("Darktheme", Darktheme, x);
            //XmlNode xlocation = XmlDesigner.XmlDesign("Location", Location, x);
            //XmlNode xsize = XmlDesigner.XmlDesign("Size", Size, x);
            //XmlNode xcolor = XmlDesigner.XmlDesign("Color", Color, x);
            //XmlNode xwc = XmlDesigner.XmlDesign("Wc", Wc, x);
            //XmlNode xhc = XmlDesigner.XmlDesign("Hc", Hc, x);
            //XmlNode xsd = XmlDesigner.XmlDesign("Sd", Sd, x);
        }
    }
    public class Xmlconverter : IXmlObject
    {
        public bool XmlObject => throw new NotImplementedException();

        public void FromString(string s)
        {
            throw new NotImplementedException();
        }

        public void FromXml(XmlNode x)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void ToXml(XmlNode x)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
    public interface IXmlObject : ISerializable, IXmlSerializable
    {
        string ToString();
        void FromString(string s);
        void ToXml(XmlNode x);
        void FromXml(XmlNode x);
        bool XmlObject { get; }
    }
    public static class XmlDesigner
    {
        public delegate void Writer(XmlDocument x);
        public static void Write(Writer w, XmlWriter xw)
        {
            XmlDocument x0 = new XmlDocument();
            w(x0);
            x0.WriteTo(xw);
        }
        public static XmlNode XmlDesign(string name, IXmlObject o, XmlDocument x)
        {
            XmlNode xn = x.CreateElement(name);
            if (o.XmlObject)
            {
                o.ToXml(xn);
            }
            else
            {
                xn.InnerText = o.ToString();
            }
            x.AppendChild(xn);
            return xn;
        }
        public static XmlNode XmlDesign(string name, IXmlObject o, XmlNode x)
        {
            return null;
        }
    }
    [Serializable()]
    public class Slotdata : ICloneable
    {
        bool enabled = true;
        string address = "";
        string subs = "#";
        int linecount = 500;
        public enum SlotType
        {
            Cam, Mqtt
        }
        SlotType type = SlotType.Cam;
        public SlotType Type
        {
            get { return type; }
            set { type = value; }
        }
        public string Address
        {
            get { return address; }
            set { address = value; }
        }
        public string Subs
        {
            get { return subs; }
            set { subs = value; }
        }
        public bool Enabled { get => enabled; set => enabled = value; }
        public int Linecount { get => linecount; set => linecount = value; }

        public object Clone()
        {
            Slotdata sld = new Slotdata();
            sld.Address = Address;
            sld.Subs = Subs;
            sld.Type = Type;
            sld.Enabled = Enabled;
            return sld;
        }
    }
    [Serializable]
    public class ModuleVersion : ICloneable, IComparable
    {
        private int major;
        private int minor;
        private int build;
        private int revision;
        /// <summary>
        /// Gets the major.
        /// </summary>
        /// <value></value>
        public int Major
        {
            get
            {
                return major;
            }
            set
            {
                major = value;
            }
        }
        /// <summary>
        /// Gets the minor.
        /// </summary>
        /// <value></value>
        public int Minor
        {
            get
            {
                return minor;
            }
            set
            {
                minor = value;
            }
        }
        /// <summary>
        /// Gets the build.
        /// </summary>
        /// <value></value>
        public int Build
        {
            get
            {
                return build;
            }
            set
            {
                build = value;
            }
        }
        /// <summary>
        /// Gets the revision.
        /// </summary>
        /// <value></value>
        public int Revision
        {
            get
            {
                return revision;
            }
            set
            {
                revision = value;
            }
        }

        public bool XmlObject => false;

        /// <summary>
        /// Creates a new <see cref="ModuleVersion"/> instance.
        /// </summary>
        public ModuleVersion()
        {
            this.build = -1;
            this.revision = -1;
            this.major = 0;
            this.minor = 0;
        }
        /// <summary>
        /// Creates a new <see cref="ModuleVersion"/> instance.
        /// </summary>
        /// <param name="version">Version.</param>
        public ModuleVersion(string version)
        {
            this.build = -1;
            this.revision = -1;
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            char[] chArray1 = new char[1] { '.' };
            string[] textArray1 = version.Split(chArray1);
            int num1 = textArray1.Length;
            if ((num1 < 2) || (num1 > 4))
            {
                throw new ArgumentException("Arg_VersionString");
            }
            this.major = int.Parse(textArray1[0], CultureInfo.InvariantCulture);
            if (this.major < 0)
            {
                throw new ArgumentOutOfRangeException("version", "ArgumentOutOfRange_Version");
            }
            this.minor = int.Parse(textArray1[1], CultureInfo.InvariantCulture);
            if (this.minor < 0)
            {
                throw new ArgumentOutOfRangeException("version", "ArgumentOutOfRange_Version");
            }
            num1 -= 2;
            if (num1 > 0)
            {
                this.build = int.Parse(textArray1[2], CultureInfo.InvariantCulture);
                if (this.build < 0)
                {
                    throw new ArgumentOutOfRangeException("build", "ArgumentOutOfRange_Version");
                }
                num1--;
                if (num1 > 0)
                {
                    this.revision = int.Parse(textArray1[3], CultureInfo.InvariantCulture);
                    if (this.revision < 0)
                    {
                        throw new ArgumentOutOfRangeException("revision", "ArgumentOutOfRange_Version");
                    }
                }
            }
        }
        public static ModuleVersion GetCurrentVersion()
        {
            return Application.ProductVersion;
        }
        /// <summary>
        /// Creates a new <see cref="ModuleVersion"/> instance.
        /// </summary>
        /// <param name="major">Major.</param>
        /// <param name="minor">Minor.</param>
        public ModuleVersion(int major, int minor)
        {
            this.build = -1;
            this.revision = -1;
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", "ArgumentOutOfRange_Version");
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", "ArgumentOutOfRange_Version");
            }
            this.major = major;
            this.minor = minor;
            this.major = major;
        }
        /// <summary>
        /// Creates a new <see cref="ModuleVersion"/> instance.
        /// </summary>
        /// <param name="major">Major.</param>
        /// <param name="minor">Minor.</param>
        /// <param name="build">Build.</param>
        public ModuleVersion(int major, int minor, int build)
        {
            this.build = -1;
            this.revision = -1;
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", "ArgumentOutOfRange_Version");
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", "ArgumentOutOfRange_Version");
            }
            if (build < 0)
            {
                throw new ArgumentOutOfRangeException("build", "ArgumentOutOfRange_Version");
            }
            this.major = major;
            this.minor = minor;
            this.build = build;
        }
        /// <summary>
        /// Creates a new <see cref="ModuleVersion"/> instance.
        /// </summary>
        /// <param name="major">Major.</param>
        /// <param name="minor">Minor.</param>
        /// <param name="build">Build.</param>
        /// <param name="revision">Revision.</param>
        public ModuleVersion(int major, int minor, int build, int revision)
        {
            this.build = -1;
            this.revision = -1;
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", "ArgumentOutOfRange_Version");
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", "ArgumentOutOfRange_Version");
            }
            if (build < 0)
            {
                throw new ArgumentOutOfRangeException("build", "ArgumentOutOfRange_Version");
            }
            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException("revision", "ArgumentOutOfRange_Version");
            }
            this.major = major;
            this.minor = minor;
            this.build = build;
            this.revision = revision;
        }
#region ICloneable Members
        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            ModuleVersion version1 = new ModuleVersion();
            version1.major = this.major;
            version1.minor = this.minor;
            version1.build = this.build;
            version1.revision = this.revision;
            return version1;
        }
#endregion
#region IComparable Members
        /// <summary>
        /// Compares to.
        /// </summary>
        /// <param name="obj">Obj.</param>
        /// <returns></returns>
        public int CompareTo(object version)
        {
            if (version == null)
            {
                return 1;
            }
            if (!(version is ModuleVersion))
            {
                throw new ArgumentException("Arg_MustBeVersion");
            }
            ModuleVersion version1 = (ModuleVersion)version;
            if (this.major != version1.Major)
            {
                if (this.major > version1.Major)
                {
                    return 1;
                }
                return -1;
            }
            if (this.minor != version1.Minor)
            {
                if (this.minor > version1.Minor)
                {
                    return 1;
                }
                return -1;
            }
            if (this.build != version1.Build)
            {
                if (this.build > version1.Build)
                {
                    return 1;
                }
                return -1;
            }
            if (this.revision == version1.Revision)
            {
                return 0;
            }
            if (this.revision > version1.Revision)
            {
                return 1;
            }
            return -1;
        }
#endregion
        /// <summary>
        /// Equalss the specified obj.
        /// </summary>
        /// <param name="obj">Obj.</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if ((obj == null) || !(obj is ModuleVersion))
            {
                return false;
            }
            ModuleVersion version1 = (ModuleVersion)obj;
            if (((this.major == version1.Major) && (this.minor == version1.Minor)) && (this.build == version1.Build) && (this.revision == version1.Revision))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// Gets the hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            int num1 = 0;
            num1 |= ((this.major & 15) << 0x1c);
            num1 |= ((this.minor & 0xff) << 20);
            num1 |= ((this.build & 0xff) << 12);
            return (num1 | this.revision & 0xfff);
        }
        /// <summary>
        /// Operator ==s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator ==(ModuleVersion v1, ModuleVersion v2)
        {
            return v1.Equals(v2);
        }
        /// <summary>
        /// Operator &gt;s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator >(ModuleVersion v1, ModuleVersion v2)
        {
            return (v2 < v1);
        }
        /// <summary>
        /// Operator &gt;=s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator >=(ModuleVersion v1, ModuleVersion v2)
        {
            return (v2 <= v1);
        }
        /// <summary>
        /// Operator !=s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator !=(ModuleVersion v1, ModuleVersion v2)
        {
            return (v1 != v2);
        }
        /// <summary>
        /// Operator &lt;s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator <(ModuleVersion v1, ModuleVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) < 0);
        }
        /// <summary>
        /// Operator &lt;=s the specified v1.
        /// </summary>
        /// <param name="v1">V1.</param>
        /// <param name="v2">V2.</param>
        /// <returns></returns>
        public static bool operator <=(ModuleVersion v1, ModuleVersion v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) <= 0);
        }
        public static implicit operator ModuleVersion(Version v)
        {
            return new ModuleVersion(v.Major, v.Minor, v.Build, v.Revision);
        }
        public static implicit operator Version(ModuleVersion v)
        {
            return new Version(v.Major, v.Minor, v.Build, v.Revision);
        }
        public static implicit operator ModuleVersion(string v)
        {
            return new ModuleVersion(v);
        }
        public static implicit operator string(ModuleVersion v)
        {
            return v.ToString();
        }
        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.build == -1)
            {
                return this.ToString(2);
            }
            if (this.revision == -1)
            {
                return this.ToString(3);
            }
            return this.ToString(4);
        }
        /// <summary>
        /// Toes the string.
        /// </summary>
        /// <param name="fieldCount">Field count.</param>
        /// <returns></returns>
        public string ToString(int fieldCount)
        {
            object[] objArray1;
            switch (fieldCount)
            {
                case 0:
                    {
                        return string.Empty;
                    }
                case 1:
                    {
                        return (this.major.ToString());
                    }
                case 2:
                    {
                        return (this.major.ToString() + "." + this.minor.ToString());
                    }
            }
            if (this.build == -1)
            {
                throw new ArgumentException(string.Format("ArgumentOutOfRange_Bounds_Lower_Upper {0},{1}", "0", "2"), "fieldCount");
            }
            if (fieldCount == 3)
            {
                objArray1 = new object[5] { this.major, ".", this.minor, ".", this.build };
                return string.Concat(objArray1);
            }
            if (this.revision == -1)
            {
                throw new ArgumentException(string.Format("ArgumentOutOfRange_Bounds_Lower_Upper {0},{1}", "0", "3"), "fieldCount");
            }
            if (fieldCount == 4)
            {
                objArray1 = new object[7] { this.major, ".", this.minor, ".", this.build, ".", this.revision };
                return string.Concat(objArray1);
            }
            throw new ArgumentException(string.Format("ArgumentOutOfRange_Bounds_Lower_Upper {0},{1}", "0", "4"), "fieldCount");
        }

        public void FromString(string s)
        {
            ModuleVersion v = s;
            Major = v.Major;
            Minor = v.Minor;
            Build = v.Build;
            Revision = v.Revision;
        }

        public void ToXml(XmlNode x)
        {
            x.InnerText = this.ToString();
        }

        public void FromXml(XmlNode x)
        {
            throw new NotImplementedException();
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            throw new NotImplementedException();
        }

        public XmlSchema GetSchema()
        {
            throw new NotImplementedException();
        }

        public void ReadXml(XmlReader reader)
        {
            throw new NotImplementedException();
        }

        public void WriteXml(XmlWriter writer)
        {
            throw new NotImplementedException();
        }
    }
}
