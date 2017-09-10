using SirMestreBlackCat.Model;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Model.Libraries.KeyBoardHooking
{
    public class KeyBoardHooking
    {
        MemoryFunctions MemoryFunctions = new MemoryFunctions("GTA5", "GTA5.exe");

        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(Key vKey);
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        public KeyBoardHooking()
        {
            // Keyboard hooking (F9).
            Thread Thread = new Thread(() =>
            {
                while (true)
                {
                    if (GetAsyncKeyState(120) == -32767)
                    {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
                        {
                            if (Application.Current.MainWindow.Topmost == true)
                            {
                                Application.Current.MainWindow.Hide();
                                Application.Current.MainWindow.Topmost = false;
                            }
                            else
                            {
                                Application.Current.MainWindow.Show();
                                Application.Current.MainWindow.Topmost = true;
                            }
                        }));
                    }
                }
            });
            Thread.IsBackground = true;
            Thread.SetApartmentState(ApartmentState.STA);
            Thread.Start();
        }
    }
}
