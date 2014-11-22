using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Terracotta
{
    public static class Program
    {
#if DEBUG
        const int SWP_NOSIZE = 0x0001;

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetConsoleWindow();
#endif

        public static bool
            Server = false,
            Client = false;

        public static bool MultiDebug = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args_)
        {
            List<string> args = new List<string>(args_);

            if      (args.Count > 0 && args.Contains("--server")) { Server = true; Console.WriteLine("Terracotta Server."); }
            else if (args.Count > 0 && args.Contains("--client")) { Client = true; Console.WriteLine("Terracotta Client."); }
            else Client = true;

#if DEBUG
            if (args.Count == 0 || args.Contains("--debug"))
            {
                MultiDebug = true;
            }

            if (Client && args.Count == 0)
            {
                var dir = System.IO.Directory.GetCurrentDirectory();
                System.Diagnostics.Process.Start(System.IO.Path.Combine(dir, "Terracotta.exe"), "--server --debug");
            }

            if (Server && args.Count == 0)
            {
                var dir = System.IO.Directory.GetCurrentDirectory();
                System.Diagnostics.Process.Start(System.IO.Path.Combine(dir, "Terracotta.exe"), "--client --debug");
            }

            if (MultiDebug)
            {
                IntPtr MyConsole = GetConsoleWindow();
                int xpos = 0;
                int ypos = Client ? 0 : 1080 / 2;
                SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);
            }
#endif


            using (GameClass game = new GameClass())
            {
                game.Run();
            }
        }
    }
}

