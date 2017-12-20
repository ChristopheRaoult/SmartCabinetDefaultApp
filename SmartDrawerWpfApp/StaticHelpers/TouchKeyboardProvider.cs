using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SmartDrawerWpfApp.StaticHelpers
{
    public class TouchKeyboardProvider
    {
        #region Private: Fields

        private readonly string _virtualKeyboardPath;
        private readonly bool _hasTouchScreen;

        #endregion

        #region ITouchKeyboardProvider Methods

        public TouchKeyboardProvider()
        {
            _hasTouchScreen = HasTouchInput();
            if (Environment.Is64BitOperatingSystem)
                _virtualKeyboardPath = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles),
                @"Common Files\Microsoft Shared\ink\TabTip.exe");
            else
                _virtualKeyboardPath = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles),
                    @"Microsoft Shared\ink\TabTip.exe");

        }

        public void ShowTouchKeyboard(/*bool numericKeyboard*/)
        {
            if (!_hasTouchScreen) return;

            try
            {

                /*const string keyName = "HKEY_CURRENT_USER\\Software\\Microsoft\\TabletTip\\1.7";

                var regValue = (int)Registry.GetValue(keyName, "KeyboardLayoutPreference", 0);
                var regShowNumericKeyboard = regValue == 1;

                if (numericKeyboard && regShowNumericKeyboard == false)
                {
                    // Set the registry so it will show the number pad via the standard keyboard.
                    Registry.SetValue(keyName, "KeyboardLayoutPreference", 4, RegistryValueKind.DWord);

                    // Kill the previous process so the registry change will take effect.
                    KillTabTip();
                }
                else if (numericKeyboard == false && regShowNumericKeyboard)
                {
                    // Set the registry so it will NOT show the number pad via the thumb keyboard.
                    Registry.SetValue(keyName, "KeyboardLayoutPreference", 0, RegistryValueKind.DWord);

                    // Kill the previous process so the registry change will take effect.
                    KillTabTip();
                }*/

                Process.Start(_virtualKeyboardPath);
            }
            catch (Exception ex)
            {
                // TODO: Log error.
                Debug.WriteLine(ex.ToString());
            }
        }

        public void HideTouchKeyboard()
        {
            if (!_hasTouchScreen) return;

            var nullIntPtr = new IntPtr(0);
            const uint wmSyscommand = 0x0112;
            var scClose = new IntPtr(0xF060);

            var keyboardWnd = FindWindow("IPTip_Main_Window", null);
            if (keyboardWnd != nullIntPtr)
            {
                SendMessage(keyboardWnd, wmSyscommand, scClose, nullIntPtr);
            }
        }


        #endregion

        private static void KillTabTip()
        {
            // Kill the previous process so the registry change will take effect.
            var processlist = Process.GetProcesses();

            foreach (var process in processlist.Where(process => process.ProcessName == "TabTip"))
            {
                process.Kill();
                break;
            }
        }

        #region Private: Win32 API Methods

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string sClassName, string sAppName);

        [DllImport("user32.dll", EntryPoint = "SendMessage", SetLastError = true)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

        #endregion

        #region Private: Methods

        private static bool HasTouchInput()
        {
            return Tablet.TabletDevices.Cast<TabletDevice>().Any(
                tabletDevice => tabletDevice.Type == TabletDeviceType.Touch);
        }

        #endregion
    }
}
