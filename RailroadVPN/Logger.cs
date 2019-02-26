using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RailRoadVPN
{
    public sealed class Logger
    {
        private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger(), LazyThreadSafetyMode.ExecutionAndPublication);

        bool initialized = false;
        string logFile;

        public static Logger GetInstance()
        {
            return lazy.Value;
        }

        private Logger()
        {
        }


        public void log(string msg)
        {
            if (!initialized)
            {
                string logsDir = Utils.getLogsDir();
                Directory.CreateDirectory(logsDir);
                this.logFile = logsDir + "\\" + DateTime.UtcNow.Date.ToString("yyyyMMdd") + "_" + Properties.Settings.Default.app_logfile_name;
                this.initialized = true;
            }

            System.IO.StreamWriter sw = null;
            try
           {
                sw = System.IO.File.AppendText(logFile);
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
