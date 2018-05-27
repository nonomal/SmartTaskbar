using System;
using System.Threading;
using static SmartTaskbar.Infrastructure.Switcher.SafeNativeMethods;

namespace SmartTaskbar.Infrastructure.Switcher.AutoMode
{
    class DefaultMode : AutoModeBase
    {
        public DefaultMode() : base()
        {
            autothread = new Thread(AutoMode);
        }

        private void AutoMode()
        {
            bool tryShowBar = true;
            while (true)
            {
                while (IsCursorOverTaskbar(ref cursor, ref msgData))
                    Thread.Sleep(250);
                EnumWindows((h, l) =>
                {
                    if (!IsWindowVisible(h))
                        return true;
                    GetWindowPlacement(h, ref placement);
                    if (placement.showCmd != SW_MAXIMIZE)
                        return true;
                    maxWindow = h;
                    return false;
                }, IntPtr.Zero);
                if (maxWindow == IntPtr.Zero)
                {
                    if (tryShowBar == false)
                    {
                        Thread.Sleep(375);
                        continue;
                    }
                    tryShowBar = false;
                    ShowTaskbar(ref msgData);
                    Thread.Sleep(500);
                    continue;
                }
                HideTaskbar(ref msgData);
                do
                {
                    Thread.Sleep(500);
                    if (!IsWindowVisible(maxWindow))
                        break;
                    GetWindowPlacement(maxWindow, ref placement);
                } while (placement.showCmd == SW_MAXIMIZE);
                tryShowBar = true;
                maxWindow = IntPtr.Zero;
            }
        }
    }
}
