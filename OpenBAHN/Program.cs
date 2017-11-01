using System;
using System.Collections.Generic;

namespace OpenBAHN
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Main game = new Main())
            {
                game.Run();
            }
            using (test test = new test())
            {
                //test.Run();
            }
        }
    }
#endif
}

