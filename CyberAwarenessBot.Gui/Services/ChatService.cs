using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CyberAwarenessBot.Gui.Models;

namespace CyberAwarenessBot.Gui.Services
{
    public class ChatService
    {
        private readonly string _userName;
        private readonly DatabaseService _db;
        private readonly ActivityLog _log;
        private readonly Dictionary<string, List<string>> _keywordResponses;
        private readonly Random _rng = new();
        private string _lastTopic = "";
        private string _favouriteTopic = "";

        public QuizGame? ActiveQuiz { get; set; }

        public ChatService(string userName, DatabaseService db, ActivityLog log)
        {
            _userName = userName;
            _db = db;
            _log = log;
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

        public string GetResponse(string userInput, out bool quizMode)
        {
            quizMode = false;
            if (string.IsNullOrWhiteSpace(userInput))
                return "I didn't quite catch that. Could you repeat?";

            string sentimentResponse = SentimentAnalyzer.DetectAndRespond(
                userInput, _userName,
                out bool sentimentDetected, out string sentimentTopic);

            if (sentimentDetected)
            {
                _lastTopic = sentimentTopic;
                _log.Add($"Sentiment detected: {sentimentTopic}");
                return sentimentResponse;
            }

            string lower = userInput.ToLower().Trim();

            var nlpResult = ProcessNlpCommand(lower, userInput);
            if (nlpResult != null)
                return nlpResult;

            if (lower.Contains("help") || lower.Contains("what can i ask"))
            {
                return "I can help with password safety, phishing, scams, privacy, and safe browsing. " +
                       "You can also:\n" +
                       "- 'add task: <title>' to create a cybersecurity task\n" +
                       "- 'show tasks' to view your tasks\n" +
                       "- 'start quiz' to test your knowledge\n" +
                       "- 'show log' to see recent activity";
            }
            if (lower.Contains("purpose") || lower.Contains("what do you do"))
            {
                return "My purpose is to educate South African citizens about cybersecurity threats and how to stay safe online.";
            }
            if (lower.Contains("how are you"))
            {
                return "I'm just a bot, but I'm ready to help you stay cyber-safe!";
            }

            if (IsFollowUp(lower, "another tip", "another", "more tips"))
            {
                if (!string.IsNullOrEmpty(_lastTopic) && _keywordResponses.ContainsKey(_lastTopic))
                    return _keywordResponses[_lastTopic][_rng.Next(_keywordResponses[_lastTopic].Count)];
                return "What topic would you like another tip on?";
            }

            if (lower.Contains("tell me more") || lower.Contains("explain more"))
            {
                if (!string.IsNullOrEmpty(_lastTopic))
                    return GetDetailedExplanation(_lastTopic);
                return "Which topic would you like me to explain further?";
            }

            foreach (var kvp in _keywordResponses)
            {
                if (lower.IndexOf(kvp.Key, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    _lastTopic = kvp.Key;
                    if (lower.Contains("interest") || lower.Contains("like") || lower.Contains("favourite"))
                    {
                        _favouriteTopic = kvp.Key;
                        _log.Add($"User interested in topic: {kvp.Key}");
                        return $"Great! I'll remember that you're interested in {kvp.Key}. " +
                               kvp.Value[_rng.Next(kvp.Value.Count)];
                    }
                    return kvp.Value[_rng.Next(kvp.Value.Count)];
                }
            }

            if (!string.IsNullOrEmpty(_favouriteTopic) && _rng.Next(4) == 0)
            {
                return $"As someone interested in {_favouriteTopic}, you might want to know: " +
                       _keywordResponses[_favouriteTopic][_rng.Next(_keywordResponses[_favouriteTopic].Count)];
            }

            return "I'm not sure I understand. Could you rephrase? Type 'help' for options.";
        }

        private string? ProcessNlpCommand(string lower, string raw)
        {
            if (string.IsNullOrWhiteSpace(lower)) return null;

            if (MatchCommand(lower, "start quiz", "play quiz", "take quiz", "begin quiz"))
            {
                ActiveQuiz = new QuizGame();
                var q = ActiveQuiz.Start();
                _log.Add("Quiz started");
                return $"Quiz started! Question {ActiveQuiz.CurrentQuestionNumber} of {ActiveQuiz.TotalQuestions}:\n\n{q.Question}\n\n" +
                       string.Join("\n", q.Options.Select((opt, i) => $"{i + 1}. {opt}")) +
                       "\n\nReply with the number of your answer.";
            }

            if (ActiveQuiz?.IsActive == true && int.TryParse(lower.Trim(), out int answerIndex))
            {
                if (answerIndex < 1 || answerIndex > ActiveQuiz.CurrentQuestion?.Options.Count)
                    return $"Please enter a number between 1 and {ActiveQuiz.CurrentQuestion?.Options.Count}.";

                var (correct, explanation, isComplete) = ActiveQuiz.Answer(answerIndex - 1);
                if (correct)
                {
                    var next = ActiveQuiz.CurrentQuestion;
                    _log.Add($"Quiz: correct answer on Q{ActiveQuiz.CurrentQuestionNumber - 1}");
                    if (isComplete)
                    {
                        string msg = $"Correct! {explanation}\n\n" + ActiveQuiz.GetScoreMessage();
                        _log.Add($"Quiz completed: score {ActiveQuiz.Score}/{ActiveQuiz.TotalQuestions}");
                        ActiveQuiz = null;
                        return msg;
                    }
                    return $"Correct! {explanation}\n\nNext question ({ActiveQuiz.CurrentQuestionNumber}/{ActiveQuiz.TotalQuestions}):\n{next!.Question}\n\n" +
                           string.Join("\n", next.Options.Select((opt, i) => $"{i + 1}. {opt}"));
                }
                else
                {
                    var next = ActiveQuiz.CurrentQuestion;
                    _log.Add($"Quiz: incorrect answer on Q{ActiveQuiz.CurrentQuestionNumber - 1}");
                    if (isComplete)
                    {
                        string msg = $"Incorrect. {explanation}\n\n" + ActiveQuiz.GetScoreMessage();
                        _log.Add($"Quiz completed: score {ActiveQuiz.Score}/{ActiveQuiz.TotalQuestions}");
                        ActiveQuiz = null;
                        return msg;
                    }
                    return $"Incorrect. {explanation}\n\nNext question ({ActiveQuiz.CurrentQuestionNumber}/{ActiveQuiz.TotalQuestions}):\n{next!.Question}\n\n" +
                           string.Join("\n", next.Options.Select((opt, i) => $"{i + 1}. {opt}"));
                }
            }

            if (MatchCommand(lower, "show log", "activity log", "what have you done", "show activity", "recent action"))
            {
                var entries = _log.GetRecent(10);
                if (entries.Count == 0)
                    return "No activity recorded yet.";
                var lines = entries.Select((e, i) => $"  {i + 1}. [{e.Timestamp:yyyy-MM-dd HH:mm}] {e.Description}");
                return $"Here's a summary of recent actions:\n{string.Join("\n", lines)}";
            }

            if (MatchCommand(lower, "show more log", "full log", "show all log"))
            {
                var entries = _log.GetAll();
                if (entries.Count == 0)
                    return "No activity recorded yet.";
                var lines = entries.Select((e, i) => $"  {i + 1}. [{e.Timestamp:yyyy-MM-dd HH:mm}] {e.Description}");
                return $"Full activity log:\n{string.Join("\n", lines)}";
            }

            if (MatchCommand(lower, "show task", "list task", "my task", "view task", "show all task"))
            {
                var tasks = _db.GetAllTasks();
                if (tasks.Count == 0)
                    return "You have no tasks yet. Let me know what task to add!";
                var lines = tasks.Select((t, i) =>
                    $"  {i + 1}. [{(t.IsCompleted ? "✓" : "○")}] ID:{t.Id} {t.Title}" +
                    (t.ReminderDate.HasValue ? $" (reminder: {t.ReminderDate:yyyy-MM-dd})" : ""));
                return $"Your cybersecurity tasks:\n{string.Join("\n", lines)}\n\nUse 'complete task <id>' or 'delete task <id>' to manage them.";
            }

            string[] taskPhrases = { "add task", "add a task", "create task", "create a task", "new task", "set task" };
            var addMatch = Regex.Match(lower, @"(?:add|create|set)\s+(?:a\s+|new\s+)?task\s*:?\s*(?:called\s+|named\s+|to\s+)?(.+?)(?:[,;]\s*remind\s+(?:me\s+)?(?:in|on|for)\s+(.+))?$");
            if (addMatch.Success || MatchCommand(lower, taskPhrases))
            {
                string title;
                string? reminderText = null;

                if (addMatch.Success)
                {
                    title = addMatch.Groups[1].Value.Trim();
                    reminderText = addMatch.Groups[2].Success ? addMatch.Groups[2].Value.Trim() : null;
                }
                else
                {
                    title = ExtractAfterPhrase(lower, taskPhrases);
                }

                if (string.IsNullOrEmpty(title))
                    return "What task would you like to add? Try 'add task: Enable two-factor authentication'.";

                var task = new CyberTask
                {
                    Title = Capitalize(title),
                    Description = $"Cybersecurity task created by {_userName}",
                    IsCompleted = false
                };

                if (reminderText != null)
                {
                    var reminderDays = ParseDays(reminderText);
                    task.ReminderDate = DateTime.Now.AddDays(reminderDays);
                }

                task.Id = _db.AddTask(task);
                _log.Add($"Task added: '{task.Title}'" + (task.ReminderDate.HasValue ? $" (reminder: {task.ReminderDate:yyyy-MM-dd})" : ""));

                string response = $"Task added: '{task.Title}'.";
                if (task.ReminderDate.HasValue)
                    response += $" I'll remind you on {task.ReminderDate:yyyy-MM-dd}.";
                else
                    response += " Would you like to set a reminder? Type 'remind me in <N> days'.";
                return response;
            }

            var remindToMatch = Regex.Match(lower, @"(?:set\s+(?:a\s+)?|create\s+(?:a\s+)?)?remind(?:er)?\s+(?:me\s+)?to\s+(.+?)(?:\s+(tomorrow|today))?\s*$");
            var remindInMatch = Regex.Match(lower, @"(?:set\s+(?:a\s+)?|create\s+(?:a\s+)?)?remind(?:er)?\s+(?:me\s+)?(?:in|on|for)\s+(.+?)(?:\s+to\s+(.+))?$");
            if (remindToMatch.Success || remindInMatch.Success || MatchCommand(lower, "remind me", "set a reminder", "create a reminder", "set reminder"))
            {
                string? actionText = null;
                string? timeStr = null;

                if (remindToMatch.Success)
                {
                    actionText = remindToMatch.Groups[1].Value.Trim();
                    timeStr = remindToMatch.Groups[2].Success ? remindToMatch.Groups[2].Value.Trim() : "7 days";
                }
                else if (remindInMatch.Success)
                {
                    timeStr = remindInMatch.Groups[1].Value.Trim();
                    actionText = remindInMatch.Groups[2].Success ? remindInMatch.Groups[2].Value.Trim() : null;
                }
                else
                {
                    return "When would you like me to remind you? Try 'remind me to update my password tomorrow'.";
                }

                int days = ParseDays(timeStr ?? "7 days");
                string title = actionText ?? "Cybersecurity task";

                var task = new CyberTask
                {
                    Title = Capitalize(title),
                    Description = $"Reminder created by {_userName}",
                    ReminderDate = DateTime.Now.AddDays(days),
                    IsCompleted = false
                };
                task.Id = _db.AddTask(task);
                _log.Add($"Reminder set: '{task.Title}' on {task.ReminderDate:yyyy-MM-dd}");
                return $"Reminder set for '{task.Title}' on {task.ReminderDate:yyyy-MM-dd}.";
            }

            var completeMatch = Regex.Match(lower, @"(?:mark\s+(?:as\s+)?)?complete\s+task\s*(?:#?\s*(\d+))?");
            if (completeMatch.Success)
            {
                if (!completeMatch.Groups[1].Success)
                    return "Which task would you like to mark as complete? Use 'complete task <id>'.";

                int id = int.Parse(completeMatch.Groups[1].Value);
                var tasks = _db.GetAllTasks();
                var task = tasks.FirstOrDefault(t => t.Id == id);
                if (task == null)
                    return $"Task with ID {id} not found. Use 'show tasks' to see your tasks.";

                task.IsCompleted = true;
                _db.UpdateTask(task);
                _log.Add($"Task completed: '{task.Title}'");
                return $"Task '{task.Title}' marked as complete. Well done!";
            }

            var deleteMatch = Regex.Match(lower, @"(?:delete|remove)\s+task\s*(?:#?\s*(\d+))?");
            if (deleteMatch.Success)
            {
                if (!deleteMatch.Groups[1].Success)
                    return "Which task would you like to delete? Use 'delete task <id>'.";

                int id = int.Parse(deleteMatch.Groups[1].Value);
                var tasks = _db.GetAllTasks();
                var task = tasks.FirstOrDefault(t => t.Id == id);
                if (task == null)
                    return $"Task with ID {id} not found. Use 'show tasks' to see your tasks.";

                _db.DeleteTask(id);
                _log.Add($"Task deleted: '{task.Title}'");
                return $"Task '{task.Title}' has been deleted.";
            }

            return null;
        }

        private bool MatchCommand(string lower, params string[] phrases)
        {
            return phrases.Any(p => lower.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private bool IsFollowUp(string input, params string[] phrases)
        {
            return phrases.Any(p => input.IndexOf(p, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private string ExtractAfterPhrase(string lower, string[] phrases)
        {
            foreach (var phrase in phrases)
            {
                int idx = lower.IndexOf(phrase, StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    string after = lower[(idx + phrase.Length)..].Trim();
                    after = after.TrimStart(':').Trim();
                    after = after.TrimEnd('.', '!', '?', ' ');
                    return string.IsNullOrWhiteSpace(after) ? "" : Capitalize(after);
                }
            }
            return "";
        }

        private int ParseDays(string text)
        {
            text = text.ToLower().Trim();
            if (text.Contains("tomorrow")) return 1;
            if (text.Contains("today")) return 0;

            var match = Regex.Match(text, @"(\d+)");
            if (!match.Success) return 7;

            int num = int.Parse(match.Groups[1].Value);
            if (text.Contains("day")) return num;
            if (text.Contains("week")) return num * 7;
            if (text.Contains("month")) return num * 30;
            return num;
        }

        private string Capitalize(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;
            return char.ToUpper(s[0]) + s[1..];
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

        public string LastTopic => _lastTopic;
        public string FavouriteTopic => _favouriteTopic;
    }
}
