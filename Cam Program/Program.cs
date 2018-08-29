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
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Launcher(args));
        }
    }
}
