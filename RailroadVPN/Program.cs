using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RailRoadVPN
{
    static class Program
    {
        static Logger logger = Logger.GetInstance();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            StartApp saf = new StartApp();
            Application.Run(saf);
        }
    }
}
