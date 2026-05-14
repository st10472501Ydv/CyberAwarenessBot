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
            {
                // If the file is missing, just return – we'll handle the fallback in the UI.
                return;
            }

            // Run the audio playback on a background thread so the UI stays responsive
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