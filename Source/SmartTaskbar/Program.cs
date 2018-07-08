using System;
using System.Threading;
using System.Windows.Forms;

namespace SmartTaskbar
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //Use a mutex to ensure single instance
            using (Mutex mutex = new Mutex(true, "{32abdf83-53b8-48c4-a689-c65fdd3fcd5a}", out bool createNew))
            {
                if (!createNew)
                    return;
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                //Start a tray instead of a WinForm to reduce memory usage
                new SystemTray();
                Application.Run();
            }
        }
    }
}
