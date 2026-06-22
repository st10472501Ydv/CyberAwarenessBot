namespace CyberAwarenessBot.Gui.Services
{
    /// <summary>
    /// Defines a response strategy delegate used by <see cref="SentimentAnalyzer"/>
    /// to generate sentiment-appropriate replies based on the user's name and topic.
    /// </summary>
    /// <param name="userName">The name of the user.</param>
    /// <param name="topic">The detected cybersecurity topic.</param>
    /// <returns>A response string tailored to the sentiment.</returns>
    public delegate string ResponseStrategy(string userName, string topic);

    /// <summary>
    /// Analyses user input for emotional sentiment (worried, curious, frustrated)
    /// and returns an appropriate response using delegate-based strategies.
    /// </summary>
    public static class SentimentAnalyzer
    {
        /// <summary>
        /// Detects sentiment in the user's input and returns a strategy-based response.
        /// </summary>
        /// <param name="input">The raw user input.</param>
        /// <param name="userName">The user's name for personalisation.</param>
        /// <param name="sentimentDetected">True if a known sentiment was matched.</param>
        /// <param name="detectedTopic">The cybersecurity topic inferred from the input.</param>
        /// <returns>
        /// A sentiment-tailored response string, or empty string if no sentiment was detected.
        /// </returns>
        public static string DetectAndRespond(string input, string userName, out bool sentimentDetected, out string detectedTopic)
        {

            if (string.IsNullOrWhiteSpace(input))
            {
                sentimentDetected = false;
                detectedTopic = "";
                return string.Empty;
            }
            sentimentDetected = false;
            detectedTopic = "";

            string lower = input.ToLower();

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
                return string.Empty;
            }

            if (lower.Contains("password")) detectedTopic = "password";
            else if (lower.Contains("scam")) detectedTopic = "scam";
            else if (lower.Contains("privacy")) detectedTopic = "privacy";
            else if (lower.Contains("phishing")) detectedTopic = "phishing";
            else if (lower.Contains("browsing")) detectedTopic = "browsing";
            else detectedTopic = "general";

            return strategy(userName, detectedTopic);
        }

        /// <summary>Responds with empathy to worried or scared users.</summary>
        private static string EmpatheticResponse(string userName, string topic)
        {
            string comfort = $"I hear you, {userName}. It's totally normal to feel worried about online threats. ";
            return comfort + GetTipForTopic(topic);
        }

        /// <summary>Responds with encouragement to curious or interested users.</summary>
        private static string EncouragingResponse(string userName, string topic)
        {
            string encourage = $"Your curiosity is a great way to stay safe, {userName}. ";
            return encourage + GetTipForTopic(topic);
        }

        /// <summary>Responds supportively to frustrated or confused users.</summary>
        private static string SupportiveResponse(string userName, string topic)
        {
            string support = $"I understand it can be frustrating, {userName}. Let's make it simpler. ";
            return support + GetTipForTopic(topic);
        }

        /// <summary>Returns a brief cybersecurity tip for the given topic.</summary>
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