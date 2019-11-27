using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UniversityTask
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var form = (Form)null;

            if (args.Length > 0 && int.TryParse(args[0], out var seed))
            {
                form = new MainForm(seed);
            }
            else
            {
                form = new MainForm();
            }

            Application.Run(form);
        }
    }
}
