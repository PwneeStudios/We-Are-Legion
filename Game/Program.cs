using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Newtonsoft.Json;

namespace Game
{
    public static class Log
    {
        public static bool
            Debug = true,

            //SpeedMods = false,
            //Errors = true,
            //Receive = false,
            //Send = false,
            //Outbox = false,
            //Processing = true,
            //Do = true,
            //UpdateSim = true,
            //Delays = true,
            //Draws = false,
            //DoUpdates = true,
            //Hash = false;

            SpeedMods = false,
            Errors = true,
            Receive = false,
            Send = false,
            Outbox = false,
            Processing = false,
            Do = true,
            UpdateSim = false,
            Delays = false,
            Draws = false,
            DoUpdates = false,
            Hash = false;
    }

    public static class ConsoleHelper
    {
        public static void CreateConsole()
        {
            return; // This is only needed for leet console debugging sessions.

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
            Client = false,
            SteamNetworking = false,
            RemoveComputerDragonLords = false;

        public static UInt64 SteamServer = 0;
        public static UInt64[] SteamUsers = { 0, 0, 0, 0 };
        public static List<UInt64> SteamSpectators;

        public static int Port = 13000;
        public static string IpAddress = "127.0.0.1";

        public static int
            NumPlayers = 1,
            StartupPlayerNumber = 1,
            Width = -1,
            Height = -1,
            PosX = -1,
            PosY = -1;

        public const int
            MaxPlayers = 4,
            MaxTeams = 4,
            SpectatorIndex = MaxPlayers + 1;

        public static string
            StartupMap = null;

        public static GameParameters
            StartupGameParams = null;

        public static int[]
            Teams = new int[] { -1, 1, 2, 3, 4 },
            Kingdoms = new int[] { 1, 2, 3, 4 };

        public static UInt64[]
            PlayersSteamUser = new UInt64[] { 0, 0, 0, 0, 0 };

        public static bool
            GameStarted = false,
            WorldLoaded = false,
            Spectate = false,

            AlwaysActive = false,
            DisableScreenEdge = false,
            LogShortHash = false,
            LogLongHash = false,
            Headless = false,
            MaxFps = false,
            HasConsole = false;

        static void Start(string options)
        {
            var dir = Directory.GetCurrentDirectory();
            Process.Start(Path.Combine(dir, "WeAreLegion.exe"), options);            
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args_)
        {
            Console.WriteLine("Starting up application.");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            string args = null;

#if DEBUG
            if (args_.Length == 0)
            {
                // Demo debug
                args = "--server                --port 13000 --p 1 --t 1234 --n 1 --debug --w 1280 --h 720";

                // Demo release
                //args = "--w 1920 --h 1080";

                // Client
                //args = "--client --ip 173.174.83.72 --port 13000 --p 1 --t 1234 --n 2 --map Beset.m3n   --w 1280 --h 720 --debug";

                // Local client
                //args = "--client --ip 127.0.0.1 --port 13000 --p 1 --t 1234 --n 2 --map Beset.m3n   --debug --double";

                // Single player windowed
                //args = "--server                --port 13000 --p 2 --t 1234 --n 1 --map Beset.m3n   --debug --w 1280 --h 720";
                //args = "--server                --port 13000 --p 2 --t 1234 --n 1 --map Beset.m3n   --debug --w 1920 --h 1080";

                // Single player debug
                //args = "--server                --port 13000 --p 2 --t 1234 --n 1 --map Beset.m3n   --debug --double";

                // Single player
                //args = "--server                --port 13000 --p 2 --t 1234 --n 1 --map Beset.m3n";

                // Single player with client-server debug
                //args = "--server                --port 13000 --p 2 --t 1234 --n 1 --map Beset.m3n   --debug --double --logshorthash";

                // Two player debug
                //var clientArgs = "--client --ip 127.0.0.1 --port 13000 --p 1 --t 1234 --n 2 --map Beset.m3n   --debug --double --logshorthash --logperiod 10";
                //var serverArgs = "--server                --port 13000 --p 2 --t 1234 --n 2 --map Beset.m3n   --debug --double --logshorthash --logperiod 10";
                //args = serverArgs;
                //Start(clientArgs);

                // Four player debug
                //args = "--server                --port 13000 --p 1 --t 1234 --n 4 --map Beset.m3n   --debug --quad";
                //Start("  --client --ip 127.0.0.1 --port 13000 --p 2 --t 1234 --n 4 --map Beset.m3n   --debug --quad");
                //Start("  --client --ip 127.0.0.1 --port 13000 --p 3 --t 1234 --n 4 --map Beset.m3n   --debug --quad");
                //Start("  --client --ip 127.0.0.1 --port 13000 --p 4 --t 1234 --n 4 --map Beset.m3n   --debug --quad");
            }
#else
            // Release
            args = "";

            //args = "--server                --port 13000 --p 1 --t 1234 --n 1 --w -1";
            //args = "--server                --port 13000 --p 1 --t 1234 --n 1 --w 1280 --h 720";

            //args = "--server                --port 13000 --p 1 --t 1234 --n 1 --map Beset.m3n";
            //args = "--server                --port 13000 --p 2 --t 1234 --n 1 --map Beset.m3n   --debug --double";
#endif

            if (args != null)
            {
                ParseOptions(args);
            }
            else
            {
                ParseOptions(new List<string>(args_));
            }

            using (GameClass game = new GameClass())
            {
                game.Run();
            }
        }

        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogDump(string.Format("{0}\nExceptionObject:\n{1}\nsender:\n{2}", "Exception on MainClass.CurrentDomain_UnhandledException",
                e.ExceptionObject == null ? "" : e.ExceptionObject.ToString(),
                sender == null ? "" : sender.ToString()));
        }

        public static void LogDump(Exception e)
        {
            LogDump(e.ToString() + '\n' + e.StackTrace);
        }

        public static void LogDump(string dump)
        {
            dump = string.Format("Dump report at {0}\n----------------------------------\n\n{1}", DateTime.Now, dump);

            var stream = File.Open("dump", FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
            var writer = new StreamWriter(stream);
            writer.Write(dump);
            writer.Close();
            stream.Close();
        }

        public static void ParseOptions(string args)
        {
            var parts = args.Split(MessageStr.Seperator);

            var argList = new List<string>();
            for (int i = 0; i < parts.Length; i += 2)
            {
                argList.AddRange(parts[i].Split(' '));
                
                if (i + 1 < parts.Length)
                {
                    argList.Add(parts[i + 1]);
                }
            }

            argList.RemoveAll(match => match == "");

            ParseOptions(argList);
        }

        public static void ParseOptions(List<string> args)
        {
            Spectate = false;

            GameStarted = false;
            WorldLoaded = false;
            Server = false;
            Client = false;
            SteamNetworking = false;

            if (args.Contains("--p"))
            { 
                int i = args.IndexOf("--p");
                StartupPlayerNumber = int.Parse(args[i + 1]);

                if (StartupPlayerNumber == 0)
                {
                    Spectate = true;
                }
            }

            if (args.Contains("--t"))
            {
                string teams = args[args.IndexOf("--t") + 1];

                for (int i = 0; i < 4; i++)
                {
                    Teams[i + 1] = int.Parse(teams[i].ToString());
                }
            }

            if (args.Contains("--k"))
            {
                string kingdoms = args[args.IndexOf("--k") + 1];

                for (int i = 0; i < 4; i++)
                {
                    Kingdoms[i] = int.Parse(kingdoms[i].ToString());
                }
            }

            if (args.Contains("--n")) { int i = args.IndexOf("--n"); NumPlayers = int.Parse(args[i + 1]); }

            if (args.Contains("--map"))
            {
                int i = args.IndexOf("--map");
                StartupMap = args[i + 1];
            }

            if (args.Contains("--params"))
            {
                int i = args.IndexOf("--params");
                string game_params = args[i + 1];
                StartupGameParams = (GameParameters)JsonConvert.DeserializeObject(game_params, typeof(GameParameters));
                StartupGameParams.Setup();
            }

            if (args.Contains("--spells"))
            {
                int i = args.IndexOf("--spells");
                string spells = args[i + 1];
                Spells.SpellInfoDict = (Dictionary<string, SpellInfo>)
                    JsonConvert.DeserializeObject(spells, typeof(Dictionary<string, SpellInfo>));
            }

            if (args.Contains("--server")) Server = true;
            else if (args.Contains("--client")) Client = true;

            if (args.Contains("--remove-computer-dragonlords")) RemoveComputerDragonLords = true;
            else RemoveComputerDragonLords = false;

            if (args.Contains("--steam-networking")) SteamNetworking = true;

            if (args.Contains("--steam-users"))
            {
                int index = args.IndexOf("--steam-users");
                int count = int.Parse(args[index + 1]);

                for (int i = 0; i < count; i++)
                {
                    string user = args[index + i + 2];
                    SteamUsers[i] = UInt64.Parse(user);
                }
            }

            SteamSpectators = new List<ulong>();
            if (args.Contains("--steam-spectators"))
            {
                int index = args.IndexOf("--steam-spectators");
                int count = int.Parse(args[index + 1]);

                for (int i = 0; i < count; i++)
                {
                    string user = args[index + i + 2];
                    SteamSpectators.Add(UInt64.Parse(user));
                }
            }

            if (args.Contains("--steam-server"))
            {
                int index = args.IndexOf("--steam-server");

                string user = args[index + 1];
                SteamServer = UInt64.Parse(user);
            }

            if (args.Contains("--ip")) { int i = args.IndexOf("--ip"); IpAddress = args[i + 1]; }
            if (args.Contains("--port")) { int i = args.IndexOf("--port"); Port = int.Parse(args[i + 1]); }

            if (args.Contains("--always-active")) { AlwaysActive = true; }
            if (args.Contains("--disable-edge")) { DisableScreenEdge = true; }
            if (args.Contains("--headless")) { Headless = true; }
            if (args.Contains("--maxfps")) { MaxFps = true; }
            if (args.Contains("--console")) { HasConsole = true; }

            if (args.Contains("--w")) { int i = args.IndexOf("--w"); Width = int.Parse(args[i + 1]); }
            if (args.Contains("--h")) { int i = args.IndexOf("--h"); Height = int.Parse(args[i + 1]); }

            if (Log.Debug || args.Contains("--debug")) { HasConsole = true; AlwaysActive = true; }

            // Log settings
            Console.WriteLine("ip set to {0}", IpAddress);
            Console.WriteLine("port set to {0}", Port);

            if (Server) Console.WriteLine("We Are Legion Server. Player {0}", StartupPlayerNumber);
            if (Client) Console.WriteLine("We Are Legion Client. Player {0}", StartupPlayerNumber);

            if (Headless) Console.WriteLine("Headless enabled");
            if (MaxFps) Console.WriteLine("Max fps enabled");
            if (HasConsole) { Console.WriteLine("Console enabled"); ConsoleHelper.CreateConsole(); }

            if (args.Contains("--double")) SetDouble(args);
            if (args.Contains("--quad")) SetQuad(args);

            PlayersSteamUser = new UInt64[] { 0, 0, 0, 0, 0 };
            for (int player = 1; player <= 4; player++)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (Program.Kingdoms[i] == player)
                    {
                        PlayersSteamUser[player] = Program.SteamUsers[i];
                    }
                }
            }
        }

        private static void SetDouble(List<string> args)
        {
            try
            {
                int w = 1920 / 2, h = 1080 / 2;

                if (!args.Contains("--w")) Width = 512;
                if (!args.Contains("--h")) Height = 512;

                IntPtr MyConsole = GetConsoleWindow();
                int xpos = 0;
                int ypos = 0;

                switch (StartupPlayerNumber)
                {
                    case 1: xpos = 0; ypos = 0; PosX = 2*w - 512; PosY = 0; break;
                    case 2: xpos = 0; ypos = h; PosX = 2*w - 512; PosY = h; break;
                }

                SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);
                Console.BufferWidth = 169;
                Console.WindowWidth = 169;
                Console.BufferHeight = Int16.MaxValue - 1;
                Console.WindowHeight = 37;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error setting console size/position:");
                Console.WriteLine(e);
            }
        }

        private static void SetQuad(List<string> args)
        {
            int w = 1920 / 2, h = 1080 / 2;

            try
            {
                if (!args.Contains("--w")) Width = 512;
                if (!args.Contains("--h")) Height = 512;

                IntPtr MyConsole = GetConsoleWindow();
                int xpos = 0;
                int ypos = 0;
                switch (StartupPlayerNumber)
                {
                    case 1: xpos = 0; ypos = 0; PosX = w   - 512; PosY = 0; break;
                    case 2: xpos = 0; ypos = h; PosX = w   - 512; PosY = h; break;
                    case 3: xpos = w; ypos = 0; PosX = 2*w - 512; PosY = 0; break;
                    case 4: xpos = w; ypos = h; PosX = 2*w - 512; PosY = h; break;
                }
                    
                SetWindowPos(MyConsole, 0, xpos, ypos, 0, 0, SWP_NOSIZE);
                Console.BufferWidth = 169;
                Console.WindowWidth = 60;
                Console.BufferHeight = Int16.MaxValue - 1;
                Console.WindowHeight = 37;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error setting console size/position:");
                Console.WriteLine(e);
            }
        }
    }
}

