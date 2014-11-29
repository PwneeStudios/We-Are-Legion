using System;
using System.IO;
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

    public static class ConsoleHelper
    {
        public static void CreateConsole()
        {
            AllocConsole();

            // stdout's handle seems to always be equal to 7
            IntPtr defaultStdout = new IntPtr(7);
            IntPtr currentStdout = GetStdHandle(StdOutputHandle);

            if (currentStdout != defaultStdout)
                // reset stdout
                SetStdHandle(StdOutputHandle, defaultStdout);

            // reopen stdout
            TextWriter writer = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(writer);
        }

        // P/Invoke required:
        private const UInt32 StdOutputHandle = 0xFFFFFFF5;
        [DllImport("kernel32.dll")]
        private static extern IntPtr GetStdHandle(UInt32 nStdHandle);
        [DllImport("kernel32.dll")]
        private static extern void SetStdHandle(UInt32 nStdHandle, IntPtr handle);
        [DllImport("kernel32")]
        static extern bool AllocConsole();
    }

    public static class Program
    {
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


        public static bool
            Server = false,
            Client = false;

        public static int Port = 13000;
        public static string IpAddress = "127.0.0.1";
                                    
        public static int
            StartupPlayerNumber = 1,
            Width = -1,
            Height = -1;

        public static bool
            MultiDebug = false,
            LogHash = false,
            Headless = false,
            MaxFps = false,
            HasConsole = false;

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

            if (args.Contains("--console")) { HasConsole = true; }

            if (args.Contains("--w")) { int i = args.IndexOf("--w"); Width = int.Parse(args[i + 1]); }
            if (args.Contains("--h")) { int i = args.IndexOf("--h"); Height = int.Parse(args[i + 1]); }

#if DEBUG
            if (args.Count == 0 || args.Contains("--multidebug"))
            {
                MultiDebug = true;
                HasConsole = true;
                Program.Width = Program.Height = 512;
            }
#endif

            // Log settings
            Console.WriteLine("ip set to {0}", IpAddress);
            Console.WriteLine("port set to {0}", Port);

            if (Server) Console.WriteLine("Terracotta Server. Player {0}", StartupPlayerNumber);
            if (Client) Console.WriteLine("Terracotta Client. Player {0}", StartupPlayerNumber);

            if (LogHash) Console.WriteLine("Logging hashes enabled");
            if (Headless) Console.WriteLine("Headless enabled");
            if (MaxFps) Console.WriteLine("Max fps enabled");
            if (HasConsole) { Console.WriteLine("Console enabled"); ConsoleHelper.CreateConsole(); }

            if (MultiDebug)
            {
                Console.WriteLine("MultiDebug enabled");

                if (MultiDebug && Client && args.Count == 0)
                {
                    var dir = System.IO.Directory.GetCurrentDirectory();
                    System.Diagnostics.Process.Start(System.IO.Path.Combine(dir, "Terracotta.exe"), "--server --console --multidebug --p2 --headless");
                }

                if (MultiDebug && Server && args.Count == 0)
                {
                    var dir = System.IO.Directory.GetCurrentDirectory();
                    System.Diagnostics.Process.Start(System.IO.Path.Combine(dir, "Terracotta.exe"), "--client --console --multidebug --p2");
                }

                IntPtr MyConsole = GetConsoleWindow();
                int xpos = 0;
                int ypos = Client ? 0 : 1080 / 2;
                SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);
                Console.BufferWidth = 169;
                Console.WindowWidth = 169;
                Console.BufferHeight = Int16.MaxValue - 1;
                Console.WindowHeight = 37;
            }


            using (GameClass game = new GameClass())
            {
                game.Run();
            }
        }
    }
}

