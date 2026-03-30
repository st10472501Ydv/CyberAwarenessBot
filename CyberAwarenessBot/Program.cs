using CyberAwarenessBot.UI;
using CyberAwarenessBot.Services;

namespace CyberAwarenessBot
{
    internal class Program
    {
        static void Main(string[] args)
        {
            VoiceGreeting greeting = new VoiceGreeting();
            greeting.PlayGreeting();

            AsciiArt.DisplayLogo();
        }
    }
}