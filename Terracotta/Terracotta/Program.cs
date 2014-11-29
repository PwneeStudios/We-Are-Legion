using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Terracotta
{
    public static class Log
    {
        public static bool
            SpeedMods = false,
            Errors = true,
            Receive = false,
            Send = false,
            Outbox = false,
            Processing = false,
            Do = true,
            UpdateSim = false,
            Delays = true,
            Draws = false,
            DoUpdates = false;
    }

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

        public static int Port = 13000;
        public static string IpAddress = "127.0.0.1";
                                    
        public static int
            StartupPlayerNumber = 1;

        public static bool MultiDebug = false;
        public static bool LogHash = false;
        public static bool Headless = false;
        public static bool MaxFps = false;

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

            if (args.Contains("--ip")) { int i = args.IndexOf("--ip"); IpAddress = args[i + 1]; }
            if (args.Contains("--port")) { int i = args.IndexOf("--port"); Port = int.Parse(args[i + 1]); }

            if (args.Contains("--loghash")) { LogHash = true; }
            if (args.Contains("--headless")) { Headless = true; }
            if (args.Contains("--maxfps")) { MaxFps = true; }

            // Log settings
            Console.WriteLine("ip set to {0}", IpAddress);
            Console.WriteLine("port set to {0}", Port);

            if (Server) Console.WriteLine("Terracotta Server. Player {0}", StartupPlayerNumber);
            if (Client) Console.WriteLine("Terracotta Client. Player {0}", StartupPlayerNumber);

            if (LogHash) Console.WriteLine("Logging hashes");
            if (Headless) Console.WriteLine("Headless");
            if (MaxFps) Console.WriteLine("Max fps");

#if DEBUG
            if (args.Count == 0 || args.Contains("--debug"))
            {
                MultiDebug = true;
            }

            if (MultiDebug && Client && args.Count == 0)
            {
                var dir = System.IO.Directory.GetCurrentDirectory();
                System.Diagnostics.Process.Start(System.IO.Path.Combine(dir, "Terracotta.exe"), "--server --debug --p2 --headless");
            }

            if (MultiDebug && Server && args.Count == 0)
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

