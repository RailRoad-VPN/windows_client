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
            Form form = null;

            string user_uuid = null;
            logger.LogMessageToFile("get user_uuid variable from properties");
            try
            {
                user_uuid = (string) Properties.Settings.Default["user_uuid"];
            } catch (System.Configuration.ConfigurationException e)
            {
                logger.LogMessageToFile("Configuration Exception");
                logger.LogMessageToFile(e.BareMessage);
                return;
            }

            logger.LogMessageToFile("user_uuid: " + user_uuid);

            if (user_uuid != null && user_uuid != "")
            {
                logger.LogMessageToFile("Loading MainForm");
                form = new MainForm();
            } else
            {
                logger.LogMessageToFile("Loading InpuntPinForm");
                form = new InputPinForm();
            }
            Application.Run(form);
        }
    }
}
