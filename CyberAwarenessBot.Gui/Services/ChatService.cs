using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberAwarenessBot.Gui.Services
{
    public class ChatService
    {
        private readonly string _userName;
        private readonly Dictionary<string, List<string>> _keywordResponses;
        private readonly Random _rng = new Random();

        // Context for follow-ups
        private string _lastTopic = "";
        private string _favouriteTopic = "";

        public ChatService(string userName)
        {
            _userName = userName;
            _keywordResponses = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "password", new List<string>
                    {
                        "Use a mix of letters, numbers, and symbols. Avoid common words.",
                        "Never share your password with anyone. Use a password manager!",
                        "Change your passwords every 3-6 months for better security."
                    }
                },
                { "scam", new List<string>
                    {
                        "Be wary of unsolicited emails promising large sums of money.",
                        "Scammers often impersonate banks. Always verify directly.",
                        "If it sounds too good to be true, it probably is."
                    }
                },
                { "privacy", new List<string>
                    {
                        "Review app permissions regularly. Limit data sharing.",
                        "Use a VPN when on public Wi-Fi to protect your privacy.",
                        "Enable two-factor authentication wherever possible."
                    }
                },
                { "phishing", new List<string>
                    {
                        "Phishing emails often have urgent language and suspicious links.",
                        "Check the sender's email address carefully. Fake domains are common.",
                        "Hover over links to see the real URL before clicking."
                    }
                },
                { "browsing", new List<string>
                    {
                        "Look for 'https' and the padlock icon before entering sensitive info.",
                        "Avoid downloading files from untrusted websites.",
                        "Keep your browser updated to patch security holes."
                    }
                }
            };
        }

        public string GetResponse(string userInput)
        {
            if (string.IsNullOrWhiteSpace(userInput))
                return "I didn't quite catch that. Could you repeat?";

            // ---- Sentiment detection (before keywords) ----
            string sentimentResponse = SentimentAnalyzer.DetectAndRespond(
                userInput, _userName,
                out bool sentimentDetected, out string sentimentTopic);

            if (sentimentDetected)
            {
                _lastTopic = sentimentTopic; // so follow‑ups like "tell me more" work
                return sentimentResponse;
            }

            // ---- General / built‑in commands ----
            string lower = userInput.ToLower();

            if (lower.Contains("help") || lower.Contains("what can i ask"))
            {
                return "I can help with password safety, phishing, scams, privacy, and safe browsing. " +
                       "You can also say 'another tip' or 'tell me more'.";
            }
            if (lower.Contains("purpose") || lower.Contains("what do you do"))
            {
                return "My purpose is to educate South African citizens about cybersecurity threats and how to stay safe online.";
            }
            if (lower.Contains("how are you"))
            {
                return "I'm just a bot, but I'm ready to help you stay cyber-safe!";
            }

            // ---- Follow‑up requests ----
            if (IsFollowUp(lower, "another tip", "another", "more tips"))
            {
                if (!string.IsNullOrEmpty(_lastTopic) && _keywordResponses.ContainsKey(_lastTopic))
                {
                    var tips = _keywordResponses[_lastTopic];
                    return tips[_rng.Next(tips.Count)];
                }
                return "What topic would you like another tip on?";
            }

            if (lower.Contains("tell me more") || lower.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                {
                    return GetDetailedExplanation(_lastTopic);
                }
                return "Which topic would you like me to explain further?";
            }

            // ---- Keyword matching ----
            foreach (var kvp in _keywordResponses)
            {
                if (lower.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _lastTopic = kvp.Key;

                    // Check if user is expressing interest → remember favourite
                    if (lower.Contains("interest") || lower.Contains("like") || lower.Contains("favourite"))
                    {
                        _favouriteTopic = kvp.Key;
                        return $"Great! I'll remember that you're interested in {kvp.Key}. " +
                               kvp.Value[_rng.Next(kvp.Value.Count)];
                    }

                    return kvp.Value[_rng.Next(kvp.Value.Count)];
                }
            }

            // ---- Memory recall – if no keyword, but favouriteTopic is stored, mention it occasionally ----
            if (!string.IsNullOrEmpty(_favouriteTopic) && _rng.Next(4) == 0) // 25% chance
            {
                return $"As someone interested in {_favouriteTopic}, you might want to know: " +
                       _keywordResponses[_favouriteTopic][_rng.Next(_keywordResponses[_favouriteTopic].Count)];
            }

            // ---- Default fallback ----
            return "I'm not sure I understand. Could you rephrase? Type 'help' for options.";
        }

        private bool IsFollowUp(string input, params string[] phrases)
        {
            return phrases.Any(p => input.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private string GetDetailedExplanation(string topic)
        {
            return topic switch
            {
                "password" => "A strong password is at least 12 characters long, includes uppercase, lowercase, numbers, and symbols. Avoid using personal info like birthdays.",
                "scam" => "Scams can come via email, phone, or social media. Always verify the source independently before acting.",
                "privacy" => "Online privacy means controlling who can see your data. Use privacy settings on social media, and avoid oversharing.",
                "phishing" => "Phishing is a type of social engineering where attackers trick you into revealing sensitive information. Always double-check before clicking links.",
                "browsing" => "Safe browsing involves checking for 'https', avoiding suspicious downloads, and keeping your browser updated.",
                _ => "I can explain password safety, scams, privacy, phishing, or safe browsing in more detail."
            };
        }

        // Expose for future use (e.g., by sentiment analyser)
        public string LastTopic => _lastTopic;
        public string FavouriteTopic => _favouriteTopic;
    }
}