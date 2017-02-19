using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DeskShell.Natives
{
    public static class ShellUtility
    {
        private static IntPtr _shellHandle = IntPtr.Zero;
        public static IntPtr ShellHandle
        {
            get
            {
                if(_shellHandle == IntPtr.Zero)
                {
                    //try get old shell handle
                    _shellHandle = GetProgmanShellHandle();
                    //if failed get new shell handle
                    if (_shellHandle == null || _shellHandle == IntPtr.Zero)
                    {
                        _shellHandle = GetWorkerWShellHandle();
                    }
                }

                return _shellHandle;
            }
        }

        public static void SetShell(Window target)
        {
            if (target.Owner != null)
            {
                SetDesktop(target);

                Screen screen = Screen.PrimaryScreen;
                target.Top = screen.WorkingArea.Top;
                target.Left = screen.WorkingArea.Left;
                target.Width = screen.WorkingArea.Width;
                target.Height = screen.WorkingArea.Height;
            }
        }

        public static void SetDesktop(Window target)
        {
            IntPtr handle = new WindowInteropHelper(target).EnsureHandle();
            
            WinAPI.SetParent(handle, ShellHandle);

            WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, WinAPI.GetWindowLong(handle, WinAPI.GWL_EXSTYLE) | WinAPI.WS_EX_NOACTIVATE);

            Screen screen = Screen.PrimaryScreen;
            target.Top = screen.Bounds.Top;
            target.Left = screen.Bounds.Left;
            target.Width = screen.Bounds.Width;
            target.Height = screen.Bounds.Height;
        }

        private static IntPtr GetProgmanShellHandle()
        {
            IntPtr progman = WinAPI.FindWindow("Progman", null);
            IntPtr defView = WinAPI.FindWindowEx(progman, IntPtr.Zero, "SHELLDLL_DefView", null);

            return defView;
        }

        private static IntPtr GetWorkerWShellHandle()
        {
            IntPtr hDestop = WinAPI.GetDesktopWindow();
            IntPtr hWorkerW = IntPtr.Zero;
            IntPtr hShellViewWin = IntPtr.Zero;

            do
            {
                hWorkerW = WinAPI.FindWindowEx(hDestop, hWorkerW, "WorkerW", null);
                hShellViewWin = WinAPI.FindWindowEx(hWorkerW, IntPtr.Zero, "SHELLDLL_DefView", null);
            }
            while (hShellViewWin == IntPtr.Zero && hWorkerW != IntPtr.Zero);

            return hShellViewWin;
        }

        public static void SetTranspernt(Window target, bool value)
        {
            var handle = new WindowInteropHelper(target).EnsureHandle();
            var extendedStyle = WinAPI.GetWindowLong(handle, WinAPI.GWL_EXSTYLE);

            if (value)
            {
                WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, extendedStyle | WinAPI.WS_EX_TRANSPARENT);
            }
            else
            {
                WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, extendedStyle & ~WinAPI.WS_EX_TRANSPARENT);
            }
        }
    }
}
