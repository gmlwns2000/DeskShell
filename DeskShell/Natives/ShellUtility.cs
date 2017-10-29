using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;

namespace DeskShell.Natives
{
    public static class ShellUtility
    {
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
            var handle = new WindowInteropHelper(target).EnsureHandle();
            var defView = GetDefView();

            if (defView != IntPtr.Zero)
            {
                var folderView = GetFolderView(defView);

                WinAPI.SetParent(handle, defView);
                //TODO: this is danger
                //WinAPI.ShowWindow(folderView, WinAPI.ShowWindowCommands.Hide);
                WinAPI.SetWindowLong(handle, WinAPI.GWL_EXSTYLE, WinAPI.GetWindowLong(handle, WinAPI.GWL_EXSTYLE) | WinAPI.WS_EX_NOACTIVATE);

                target.Top = 0;
                target.Left = 0;
                target.Width = SystemParameters.PrimaryScreenWidth;
                target.Height = SystemParameters.PrimaryScreenHeight;
            }
        }

        public static void ShowFolder()
        {
            var def = GetDefView();
            if(def != IntPtr.Zero)
            {
                var hwnd = GetFolderView(def);
                Show(hwnd);
            }
        }

        private static void Show(IntPtr hWnd)
        {
            WinAPI.ShowWindow(hWnd, WinAPI.ShowWindowCommands.Show);
        }

        private static IntPtr GetDefView()
        {
            IntPtr defView = IntPtr.Zero;
            WinAPI.EnumWindows(new WinAPI.EnumWindowsProc((tophandle, topparamhandle) =>
            {
                var tempView = WinAPI.FindWindowEx(tophandle, IntPtr.Zero, "SHELLDLL_DefView", null);

                if (tempView != IntPtr.Zero)
                {
                    defView = tempView;
                }

                return true;
            }), IntPtr.Zero);

            return defView;
        }

        private static IntPtr GetFolderView(IntPtr def)
        {
            var folderView = WinAPI.FindWindowEx(def, IntPtr.Zero, "SysListView32", null);
            return folderView;
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
