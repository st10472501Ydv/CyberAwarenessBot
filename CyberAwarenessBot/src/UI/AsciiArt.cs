using System;
using System.Collections.Generic;
using System.Text;

namespace CyberAwarenessBot.UI
{
    public static class AsciiArt
    {
        public static void DisplayLogo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(@"   ___     _                               ");
            Console.WriteLine(@"  / __|  _| |__  ___ _ _                  ");
            Console.WriteLine(@" | (_| || | '_ \/ -_) '_|                 ");
            Console.WriteLine(@"  \___\_, |_.__/\___|_|                   ");
            Console.WriteLine(@"   /_\|__/_ ____ _ _ _ ___ _ _  ___ ______");
            Console.WriteLine(@"  / _ \ V  V / _` | '_/ -_) ' \/ -_|_-<_-<");
            Console.WriteLine(@" /_/_\_\_/\_/\__,_|_| \___|_||_\___/__/__/ ");
            Console.WriteLine(@" | _ ) ___| |_                             ");
            Console.WriteLine(@" | _ \/ _ \  _|                            ");
            Console.WriteLine(@" |___/\___/\__|                            ");

            Console.ResetColor();
        }
    }
}
