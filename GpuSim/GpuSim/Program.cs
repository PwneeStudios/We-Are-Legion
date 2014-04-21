namespace GpuSim
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (M3ngineGame game = new M3ngineGame())
            {
                game.Run();
            }
        }
    }
#endif
}

