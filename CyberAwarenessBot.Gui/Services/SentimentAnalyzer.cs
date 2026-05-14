namespace CyberAwarenessBot.Gui.Services
{
    // Delegate declaration – this is the key requirement
    public delegate string ResponseStrategy(string userName, string topic);

    public static class SentimentAnalyzer
    {
        public static string DetectAndRespond(string input, string userName, out bool sentimentDetected, out string detectedTopic)
        {

            if (string.IsNullOrWhiteSpace(input)) //catches nulls to set sentiment as false
            {
                sentimentDetected = false;
                detectedTopic = "";
                return string.Empty;
            }
            sentimentDetected = false;
            detectedTopic = "";

            string lower = input.ToLower();

            // Determine which sentiment the user is expressing
            ResponseStrategy strategy;

            if (lower.Contains("worried") || lower.Contains("scared") || lower.Contains("afraid"))
            {
                sentimentDetected = true;
                strategy = EmpatheticResponse;
            }
            else if (lower.Contains("curious") || lower.Contains("interested"))
            {
                sentimentDetected = true;
                strategy = EncouragingResponse;
            }
            else if (lower.Contains("frustrated") || lower.Contains("annoyed") || lower.Contains("confusing"))
            {
                sentimentDetected = true;
                strategy = SupportiveResponse;
            }
            else
            {
                return string.Empty; // No sentiment detected
            }

            // Figure out which cybersecurity topic the user might be worried about, if any
            if (lower.Contains("password")) detectedTopic = "password";
            else if (lower.Contains("scam")) detectedTopic = "scam";
            else if (lower.Contains("privacy")) detectedTopic = "privacy";
            else if (lower.Contains("phishing")) detectedTopic = "phishing";
            else if (lower.Contains("browsing")) detectedTopic = "browsing";
            else detectedTopic = "general";

            // Execute the delegate
            return strategy(userName, detectedTopic);
        }

        // Different response strategies
        private static string EmpatheticResponse(string userName, string topic)
        {
            string comfort = $"I hear you, {userName}. It's totally normal to feel worried about online threats. ";
            return comfort + GetTipForTopic(topic);
        }

        private static string EncouragingResponse(string userName, string topic)
        {
            string encourage = $"Your curiosity is a great way to stay safe, {userName}. ";
            return encourage + GetTipForTopic(topic);
        }

        private static string SupportiveResponse(string userName, string topic)
        {
            string support = $"I understand it can be frustrating, {userName}. Let's make it simpler. ";
            return support + GetTipForTopic(topic);
        }

        // Helper to give a tip based on topic
        private static string GetTipForTopic(string topic)
        {
            return topic switch
            {
                "password" => "Use a passphrase made of random words – it's both strong and memorable.",
                "scam" => "Never trust a caller who pressures you to act immediately. Hang up and call back on an official number.",
                "privacy" => "Regularly check your app permissions and turn off those you don't need.",
                "phishing" => "Inspect links carefully – phishers often use characters that look similar to letters (e.g., 'g00gle.com').",
                "browsing" => "Stick to well-known websites and avoid clicking pop-ups that say 'You have a virus!'",
                _ => "Stay informed about the latest cybersecurity news to recognise new threats."
            };
        }
    }
}