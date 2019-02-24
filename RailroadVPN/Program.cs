using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
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
            var culture = new CultureInfo(Properties.Settings.Default.locale);

            // Culture for any thread
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            StartAppForm saf = FormManager.Current.CreateForm<StartAppForm>();
            Application.Run(saf);
        }

        static void OnProcessExit(object sender, EventArgs e)
        {
            Utils.killAllMyProcesses();
        }
    }

    class FormManager : ApplicationContext
    {
        //When each form closes, close the application if no other open forms
        private void onFormClosed(object sender, EventArgs e)
        {
            if (Application.OpenForms.Count == 0)
            {
                ExitThread();
            }
        }

        //Any form which might be the last open form in the application should be created with this
        public T CreateForm<T>() where T : Form, new()
        {
            var ret = new T();
            ret.FormClosed += onFormClosed;
            return ret;
        }

        //I'm using Lazy here, because an exception is thrown if any Forms have been
        //created before calling Application.SetCompatibleTextRenderingDefault(false)
        //in the Program class
        private static Lazy<FormManager> _current = new Lazy<FormManager>();
        public static FormManager Current => _current.Value;
    }
}
