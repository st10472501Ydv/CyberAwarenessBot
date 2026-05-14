using System;
using System.IO;
using System.Media;
using System.Threading.Tasks;

namespace CyberAwarenessBot.Gui.Services
{
    public class VoiceGreeting
    {
        public async Task PlayAsync()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "greeting.wav");

            if (!File.Exists(path))
                return;

            // Play on a background thread to avoid freezing the UI
            await Task.Run(() =>
            {
                using (SoundPlayer player = new SoundPlayer(path))
                {
                    player.PlaySync();
                }
            });
        }
    }
}