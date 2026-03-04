using Infrastructure.DataContext;
using Infrastructure.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace i_freeze.Utilities
{
    public class GlobalKeyboardHook
    {
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private int taskbarHandle;


        // This method sets the low-level keyboard hook by calling SetWindowsHookEx. 
        // It hooks the keyboard inputs globally and directs them to the HookCallback method for processing.
        public static void SetHook()
        {
            _hookID = SetHook(_proc);
        }

        // This method unhooks the keyboard by calling UnhookWindowsHookEx, 
        // removing the low-level hook set by SetHook to stop monitoring keyboard inputs.
        public static void Unhook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        // This private method sets the hook for keyboard inputs using SetWindowsHookEx.
        // It uses the current process and module to get the handle needed for setting the hook.
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 5;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int VK_LWIN = 0x5B;
        private const int VK_RWIN = 0x5C;
        private const int WM_SYSKEYDOWN = 0x0104;

        // This method processes the keyboard events intercepted by the hook. 
        // It checks for specific key combinations like Alt+Tab, Alt+F4, Escape, and the Windows keys (Left and Right).
        // If these keys are pressed, the method blocks them by returning a non-zero value, 
        // preventing further processing by the system.
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0 && (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN))
            {
                int vkCode = Marshal.ReadInt32(lParam);

                if ((Keys)vkCode == Keys.Tab && Control.ModifierKeys == Keys.Alt)
                {
                    return (IntPtr)1; // Block Alt+Tab
                }
                else if ((Keys)vkCode == Keys.F4 && Control.ModifierKeys == Keys.Alt)
                {
                    return (IntPtr)1; // Block Alt+F4
                }
                else if ((Keys)vkCode == Keys.Escape)
                {
                    return (IntPtr)1; // Block Escape
                }
            }

            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == VK_LWIN || vkCode == VK_RWIN)
                {
                    return (IntPtr)1; // Suppress the key event
                }
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        // This method hides the Windows taskbar by finding the taskbar window handle and calling ShowWindow with SW_HIDE.
        public void HideTaskbar()
        {
            taskbarHandle = FindWindow("Shell_TrayWnd", "");
            ShowWindow(taskbarHandle, SW_HIDE);
        }

        // This method shows the Windows taskbar by finding the taskbar window handle and calling ShowWindow with SW_SHOW.
        public void ShowTaskbar()
        {
            taskbarHandle = FindWindow("Shell_TrayWnd", "");
            ShowWindow(taskbarHandle, SW_SHOW);
        }


        // This imported method from user32.dll finds a window by class name and window text.
        // It is used in HideTaskbar and ShowTaskbar to get the handle of the taskbar window.
        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);

        // This imported method from user32.dll shows or hides a window, depending on the command passed (SW_HIDE or SW_SHOW).
        // It is used to control the visibility of the taskbar.
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        // This imported method from user32.dll sets a system-wide hook to monitor specific events like keyboard inputs.
        // It is used in SetHook to hook into the keyboard events.

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        // This imported method from user32.dll removes a hook set by SetWindowsHookEx.
        // It is used in Unhook to stop monitoring keyboard events.

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        // This imported method from user32.dll passes the hook information to the next hook in the chain.
        // It is used in HookCallback when the key event is not blocked, allowing the event to be processed normally.

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        // This imported method from kernel32.dll retrieves the handle of the module (DLL or EXE) in the current process.
        // It is used in SetHook to get the handle of the current process's module.

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        //[DllImport("user32.dll")]
        //private static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        //[DllImport("user32.dll")]
        //private static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);
    }

}
