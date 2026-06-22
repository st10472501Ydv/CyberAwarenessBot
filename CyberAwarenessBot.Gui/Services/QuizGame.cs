using System;
using System.Collections.Generic;
using CyberAwarenessBot.Gui.Models;

namespace CyberAwarenessBot.Gui.Services
{
    public class QuizGame
    {
        private readonly List<QuizQuestion> _questions;
        private int _currentIndex;
        private int _score;

        public bool IsActive { get; private set; }
        public int Score => _score;
        public int TotalQuestions => _questions.Count;
        public int CurrentQuestionNumber => _currentIndex + 1;
        public QuizQuestion? CurrentQuestion => IsActive && _currentIndex < _questions.Count ? _questions[_currentIndex] : null;

        public QuizGame()
        {
            _questions = new List<QuizQuestion>
            {
                new()
                {
                    Question = "What is the safest response when you receive an unexpected email asking for your password?",
                    Options = new List<string> { "Reply with your password", "Click the link to check", "Report it as phishing", "Forward it to friends" },
                    CorrectOptionIndex = 2,
                    Explanation = "Phishing emails should be reported, not replied to. Legitimate organisations never ask for passwords via email."
                },
                new()
                {
                    Question = "Which of the following is a strong password?",
                    Options = new List<string> { "password123", "Ilovecats", "M7#k9p@2!xQ", "Admin2024" },
                    CorrectOptionIndex = 2,
                    Explanation = "A strong password uses a mix of uppercase, lowercase, numbers, and special characters. M7#k9p@2!xQ contains all four."
                },
                new()
                {
                    Question = "What does HTTPS stand for?",
                    Options = new List<string> { "Hyper Text Transfer Protocol Secure", "High Tech Transfer System", "Hyper Transfer Text Protocol Secure", "None of the above" },
                    CorrectOptionIndex = 0,
                    Explanation = "HTTPS (Hyper Text Transfer Protocol Secure) encrypts data between your browser and the website, protecting your information."
                },
                new()
                {
                    Question = "True or False: Using public Wi-Fi without a VPN is safe for online banking.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Public Wi-Fi networks are often unencrypted, making it easy for attackers to intercept your data. Always use a VPN on public Wi-Fi."
                },
                new()
                {
                    Question = "What is social engineering in cybersecurity?",
                    Options = new List<string> { "Building social networks online", "Manipulating people to reveal confidential information", "Creating social media profiles", "Engineering social media algorithms" },
                    CorrectOptionIndex = 1,
                    Explanation = "Social engineering tricks people into breaking security procedures, often by impersonating trusted entities or creating urgency."
                },
                new()
                {
                    Question = "How often should you update your passwords for sensitive accounts?",
                    Options = new List<string> { "Never", "Every 3-6 months", "Once a year", "Only when you get hacked" },
                    CorrectOptionIndex = 1,
                    Explanation = "Regular password updates (every 3-6 months) reduce the risk of compromised credentials being used against you."
                },
                new()
                {
                    Question = "What is two-factor authentication (2FA)?",
                    Options = new List<string> { "Logging in twice", "Using two passwords", "A second layer of security beyond your password", "Having two accounts" },
                    CorrectOptionIndex = 2,
                    Explanation = "2FA requires a second verification step (like a code sent to your phone) in addition to your password, making accounts much harder to compromise."
                },
                new()
                {
                    Question = "True or False: You should download software from pop-up ads that claim your computer has a virus.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "Pop-up ads claiming your computer has a virus are a common scam. Never download software from pop-ups; use trusted antivirus software instead."
                },
                new()
                {
                    Question = "What should you do if you discover a data breach involving your account?",
                    Options = new List<string> { "Ignore it", "Change your password immediately", "Wait to see what happens", "Post about it on social media" },
                    CorrectOptionIndex = 1,
                    Explanation = "Change your password immediately and enable 2FA if available. Also check for any unauthorised activity on your account."
                },
                new()
                {
                    Question = "What is a phishing scam?",
                    Options = new List<string> { "A legitimate email from your bank", "A fake communication designed to steal personal information", "A type of computer virus", "A spam email about fishing" },
                    CorrectOptionIndex = 1,
                    Explanation = "Phishing scams use fake emails, messages, or websites that appear legitimate to trick you into revealing sensitive information."
                },
                new()
                {
                    Question = "True or False: It is safe to use the same password for multiple accounts if it is strong.",
                    Options = new List<string> { "True", "False" },
                    CorrectOptionIndex = 1,
                    Explanation = "If one account is compromised, all accounts using the same password become vulnerable. Use unique passwords for each account."
                },
                new()
                {
                    Question = "What is the best way to recognise a fake website?",
                    Options = new List<string> { "It has a nice design", "The URL has misspellings or unusual characters", "It loads quickly", "It has many images" },
                    CorrectOptionIndex = 1,
                    Explanation = "Fake websites often use URLs with slight misspellings (e.g., 'g00gle.com' instead of 'google.com') or unusual domain extensions."
                }
            };
        }

        public QuizQuestion Start()
        {
            _currentIndex = 0;
            _score = 0;
            IsActive = true;
            return _questions[0];
        }

        public (bool Correct, string Explanation, bool IsComplete) Answer(int optionIndex)
        {
            if (!IsActive || CurrentQuestion == null)
                return (false, "No active quiz.", true);

            var question = CurrentQuestion;
            bool correct = optionIndex == question.CorrectOptionIndex;
            if (correct) _score++;

            _currentIndex++;
            bool isComplete = _currentIndex >= _questions.Count;
            if (isComplete) IsActive = false;

            return (correct, question.Explanation, isComplete);
        }

        public void End()
        {
            IsActive = false;
        }

        public string GetScoreMessage()
        {
            double pct = (double)_score / _questions.Count * 100;
            if (pct >= 90) return $"Amazing, {_score}/{_questions.Count}! You're a cybersecurity expert!";
            if (pct >= 70) return $"Great job, {_score}/{_questions.Count}! You know your cybersecurity well.";
            if (pct >= 50) return $"Good effort, {_score}/{_questions.Count}. Keep learning to stay safe online!";
            return $"You scored {_score}/{_questions.Count}. Review the topics and try again to improve your cybersecurity knowledge.";
        }
    }
}
