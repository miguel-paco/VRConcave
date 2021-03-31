using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlyVRena2._0.Display;
namespace FlyVRena2._0
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]

        private static void Main(string[] s)
        {
             if (s.Length > 0)
                new MainWindow(s[0], s[1]).Run();
            else
                new MainWindow().Run();
        }
    }
}

