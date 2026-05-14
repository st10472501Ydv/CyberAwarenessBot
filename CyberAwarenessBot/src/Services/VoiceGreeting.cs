using System;
using System.IO;
using System.Linq;
using System.Media;

namespace CyberAwarenessBot.Services
{
    public class VoiceGreeting
    {
        public void PlayGreeting()
        {
            // Try to locate the audio file in several likely locations.
            string audioPath = Path.Combine(AppContext.BaseDirectory, "assets", "greeting.wav");

            // If the project root can be found (a folder containing the .csproj file), prefer the src/Assets location there.
            try
            {
                var dir = new DirectoryInfo(AppContext.BaseDirectory);
                DirectoryInfo? projectRoot = null;
                while (dir != null)
                {
                    if (dir.GetFiles("*.csproj").Any())
                    {
                        projectRoot = dir;
                        break;
                    }
                    dir = dir.Parent;
                }

                if (projectRoot != null)
                {
                    var candidate = Path.GetFullPath(Path.Combine(projectRoot.FullName, "src", "Assets", "greeting.wav"));
                    if (File.Exists(candidate))
                    {
                        audioPath = candidate;
                    }
                }
                else
                {
                    // Fallback: try three levels up from the base directory (typical bin/Debug/net*/ -> project folder)
                    var fallback = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "src", "Assets", "greeting.wav"));
                    if (File.Exists(fallback))
                    {
                        audioPath = fallback;
                    }
                }
            }
            catch
            {
                // ignore and use default audioPath
            }

            if (File.Exists(audioPath))
            {
                try
                {
                    using (SoundPlayer player = new SoundPlayer(audioPath))
                    {
                        player.PlaySync();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to play audio: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Greeting audio not found: " + audioPath);
            }
        }
    }
}