using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadVPN
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());

        public static Logger GetInstance()
        { return lazy.Value; }

        private Logger()
        {
        }

        public void log(string msg)
        {
            System.IO.StreamWriter sw = null;
            try
           {
                sw = System.IO.File.AppendText(Utils.getLocalAppDirPath() + "//" + DateTime.UtcNow.Date.ToString("yyyyMMdd") + "_" + Properties.Settings.Default.app_logfile_name);
                string logLine = System.String.Format("{0:G}: {1}.", System.DateTime.Now, msg);
                sw.WriteLine(logLine);
                sw.Close();
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
