using Ruaraidheulib;
using Ruaraidheulib.Interface.reulib64.Win64.Console;
using Ruaraidheulib.Interface.reulib64.Win64.Version;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cam_Program
{
    class Launcher : Form
    {
        public static string crashdump = Path.DirectorySeparatorChar + "crashdump.log";
        public static int crashsize = 500;
        public static string logfile = Path.DirectorySeparatorChar + "Log.log";
        public static int logsize = 100000;
        public void Loaded(object sender, EventArgs eventargs)
        {
            Saveable s;
            try
            {
                k.w(VersionInfo.Get().FullVersion);
                bool exit = false;
                CommandLine cl = new CommandLine((t) => { }, (t) => { }, () => { return ""; });
                CommandArgs ca = new CommandArgs("configpath", 1, new CommandArgs.Subcommand("folder", 1), new CommandArgs.Subcommand("filename", 1));
                ca.AliasSubcommand("filename", "name");
                ca.AliasSubcommand("filename", "file");
                ca.AliasSubcommand("folder", "path");
                cl.Add(ca, (c) =>
                {
                    c.GetArg("folder", Saveable.Path);
                    c.GetArg("filename", Saveable.Name);
                    string tmp = "";
                    if (c.GetArgBool("configpath", Saveable.Path + Saveable.Name, out tmp))
                    {
                        Saveable.Name = Path.DirectorySeparatorChar + Path.GetFileName(tmp);
                        Saveable.Path = Path.GetDirectoryName(tmp);
                    }
                    if (c.GetArgBool("folder", Saveable.Path, out tmp))
                    {
                        Saveable.Path = tmp;
                    }
                    if (c.GetArgBool("filename", Saveable.Name, out tmp))
                    {
                        Saveable.Name = Path.DirectorySeparatorChar + tmp;
                    }
                    return Saveable.Path + Saveable.Name;
                });
                cl.AddAlias("configpath", "path");
                cl.AddAlias("configpath", "config");
                cl.Add(new CommandArgs("loadv1"), (c) =>
                {
                    Form1 f = new Form1();
                    f.ShowDialog();
                    exit = true;
                    return "Loaded";
                });
                cl.Add(new CommandArgs("loadv2"), (c) =>
                {
                    s = Saveable.DeserializeItem();
                    Camviewer f = new Camviewer(s);
                    f.ShowDialog();
                    exit = true;
                    return "Loaded";
                });
                cl.Add(new CommandArgs("crash"), (c) =>
                {
                    try
                    {
                        ErrorTest0();
                    }
                    catch (Exception e)
                    {
                        throw new TestException("This is a test exception.", e);
                    }
                    return "Crashed";
                });
                string tmp0 = "";
                string tmp1 = "";
                foreach (string tmp2 in Cam_Program.Camviewer.cla)
                {
                    tmp0 += tmp2 + " ";
                }
                cl.Read(tmp0, out tmp1);
                if (!exit)
                {
                    s = Saveable.DeserializeItem();
                    if (File.Exists(Saveable.Path + crashdump))
                    {
                        if (s.Askfiledeletecrash)
                        {
                            if (new FileInfo(Saveable.Path + crashdump).Length > crashsize)
                            {
                                string str = Ruaraidheulib.Winforms.MessageBox.Show("Crash dump file is getting big do you want to clear it?", "Large File", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Four, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Four, "Yes", "No", "Always", "Never");
                                switch (str)
                                {
                                    case "Yes":
                                        File.Delete(Saveable.Path + crashdump);
                                        break;
                                    case "No":
                                        break;
                                    case "Always":
                                        File.Delete(Saveable.Path + crashdump);
                                        s.Askfiledeletecrash = false;
                                        s.Alwaysdeletecrash = true;
                                        break;
                                    case "Never":
                                        s.Askfiledeletecrash = false;
                                        s.Alwaysdeletecrash = false;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (s.Alwaysdeletecrash && new FileInfo(Saveable.Path + crashdump).Length > crashsize)
                            {
                                File.Delete(Saveable.Path + crashdump);
                            }
                        }
                    }
                    if (File.Exists(Saveable.Path + logfile))
                    {
                        if (s.Askfiledeletelog)
                        {
                            if (new FileInfo(Saveable.Path + logfile).Length > logsize)
                            {
                                string str = Ruaraidheulib.Winforms.MessageBox.Show("Log file is getting big do you want to clear it?", "Large File", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.Two, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, "Yes", "No", "Always", "Never");
                                switch (str)
                                {
                                    case "Yes":
                                        File.Delete(Saveable.Path + logfile);
                                        break;
                                    case "No":
                                        break;
                                    case "Always":
                                        File.Delete(Saveable.Path + logfile);
                                        s.Askfiledeletelog = false;
                                        s.Alwaysdeletelog = true;
                                        break;
                                    case "Never":
                                        s.Askfiledeletelog = false;
                                        s.Alwaysdeletelog = false;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (s.Alwaysdeletelog && new FileInfo(Saveable.Path + logfile).Length > logsize)
                            {
                                File.Delete(Saveable.Path + logfile);
                            }
                        }
                    }
                    using (Camviewer cv = new Camviewer(s))
                        cv.ShowDialog();
                }
            }
            catch (Exception e)
            {
                Ruaraidheulib.Winforms.MessageBox.Show("This program has been closed due to a fatal " + Ruaraidheulib.Winforms.MessageBox.ErrorWrite(e), "Fatal Exception", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, "OK");

                try
                {
                    if (Camviewer.sv != null)
                        Saveable.SerializeItem(Camviewer.sv);
                }
                catch (Exception ex)
                {
                    Ruaraidheulib.Winforms.MessageBox.Show("Could not save data." + Environment.NewLine + Environment.NewLine + Ruaraidheulib.Winforms.MessageBox.ErrorWrite(ex), "Exception", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, "OK");
                }
                try
                {
                    File.AppendAllText(Saveable.Path + crashdump, Environment.NewLine + "--------------------------------------------------" + Environment.NewLine + Ruaraidheulib.Winforms.MessageBox.ErrorWrite(e));
                }
                catch (Exception ex)
                {
                    Ruaraidheulib.Winforms.MessageBox.Show("Could not save error dump." + Environment.NewLine + Environment.NewLine + Ruaraidheulib.Winforms.MessageBox.ErrorWrite(ex), "Exception", Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, Ruaraidheulib.Winforms.MessageBox.ButtonLayout.One, "OK");
                }
            }
            this.Close();
        }
        public Launcher(string[] args)
        {
            Hide();
            this.Load += new System.EventHandler(this.Loaded);
            Cam_Program.Camviewer.cla = args;
        }
        public static void ErrorTest2()
        {
            throw new Exception("Inner Inner Inner Exception");
        }
        public static void ErrorTest1()
        {
            try
            {
                ErrorTest2();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Inner Inner Exception", e);
            }
        }
        public static void ErrorTest0()
        {
            try
            {
                ErrorTest1();
            }
            catch (Exception e)
            {
                throw new TestException("Inner Exception", e);
            }
        }
    }
    public class TestException : Exception
    {
        public TestException()
        {
        }

        public TestException(string message) : base(message)
        {
        }

        public TestException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TestException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}

