using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Game
{
    public static class PerfTimer
    {
        class _PerfTimer
        {
            Stopwatch watch;
            int runs;
            double total;
            string name;

            public _PerfTimer(string name)
            {
                watch = new Stopwatch();
                runs = 0;
                total = 0;
                this.name = name;
            }

            public double average()
            {
                return total / runs;
            }

            public void addrun()
            {
                runs++;
                total += watch.ElapsedTicks;
                watch.Reset();
            }

            public void start()
            {
                watch.Start();
            }
        }

        static Dictionary<string, _PerfTimer> Stopwatches = new Dictionary<string, _PerfTimer>();
        static _PerfTimer CurrentWatch;
        static string CurrentName;

        public static void Start(string name)
        {
            if (Stopwatches.ContainsKey(name))
            {
                CurrentWatch = Stopwatches[name];
            }
            else
            {
                CurrentWatch = new _PerfTimer(name);
                Stopwatches.Add(name, CurrentWatch);
            }

            CurrentName = name;
            CurrentWatch.start();
        }

        public static void StopAndWrite()
        {
            var avg = Stop();
            Console.WriteLine("{0}: avg = {1}", CurrentName, avg);
        }

        public static double Stop()
        {
            CurrentWatch.addrun();
            return CurrentWatch.average();
        }
    }
}
