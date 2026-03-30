using System;
using System.IO;
using System.Media;

namespace CyberAwarenessBot.Services
{
    public class VoiceGreeting
    {
        public void PlayGreeting()
        {
            string audioPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "assets", "greeting.wav");

            if (File.Exists(audioPath))
            {
                using (SoundPlayer player = new SoundPlayer(audioPath))
                {
                    player.PlaySync();
                }
            }
            else
            {
                Console.WriteLine("Voice greeting file not found.");
            }
        }
    }
}