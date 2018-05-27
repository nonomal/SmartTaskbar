using System;
using System.Threading;
using static SmartTaskbar.Infrastructure.Switcher.SafeNativeMethods;

namespace SmartTaskbar.Infrastructure.Switcher.AutoModeWin10
{
    class DefaultMode : AutoModeBase
    {
        public DefaultMode() : base()
        {
            autothread = new Thread(AutoMode);
        }

        private int windowPID;
        private int uwpPID;

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
                    GetWindowThreadProcessId(h, out windowPID);
                    if (uwpPID == windowPID)
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
                if (uwpPID == 0)
                    if (SetuwpPID(out uwpPID))
                    {
                        maxWindow = IntPtr.Zero;
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
