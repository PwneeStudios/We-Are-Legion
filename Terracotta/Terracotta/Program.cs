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

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;
#endif

        public static bool
            Server = false,
            Client = false;

        public static int
            StartupPlayerNumber = 1;

        public static bool MultiDebug = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args_)
        {
            List<string> args = new List<string>(args_);

            if (args.Contains("--p1")) StartupPlayerNumber = 1;
            if (args.Contains("--p2")) StartupPlayerNumber = 2;
            if (args.Contains("--p3")) StartupPlayerNumber = 3;
            if (args.Contains("--p4")) StartupPlayerNumber = 4;

            if      (args.Contains("--server")) Server = true;
            else if (args.Contains("--client")) Client = true;
            else Client = true;
            //else Server = true;

            if (Server) Console.WriteLine("Terracotta Server. Player {0}", StartupPlayerNumber);
            if (Client) Console.WriteLine("Terracotta Client. Player {0}", StartupPlayerNumber);

#if DEBUG
            if (args.Count == 0 || args.Contains("--debug"))
            {
                MultiDebug = true;
            }

            if (Client && args.Count == 0)
            {
                var dir = System.IO.Directory.GetCurrentDirectory();
                System.Diagnostics.Process.Start(System.IO.Path.Combine(dir, "Terracotta.exe"), "--server --debug --p2");
            }

            if (Server && args.Count == 0)
            {
                var dir = System.IO.Directory.GetCurrentDirectory();
                System.Diagnostics.Process.Start(System.IO.Path.Combine(dir, "Terracotta.exe"), "--client --debug --p2");
            }

            if (MultiDebug)
            {
                IntPtr MyConsole = GetConsoleWindow();
                int xpos = 0;
                int ypos = Client ? 0 : 1080 / 2;
                SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);
                Console.BufferWidth = 169;
                Console.WindowWidth = 169;
                Console.BufferHeight = Int16.MaxValue - 1;
                Console.WindowHeight = 37;

                //ShowWindow(MyConsole, SW_HIDE);
            }
#endif


            using (GameClass game = new GameClass())
            {
                game.Run();
            }
        }
    }
}

