using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CyberAwarenessBot.Gui.Helpers;
using CyberAwarenessBot.Gui.Models;
using CyberAwarenessBot.Gui.Services;

namespace CyberAwarenessBot.Gui
{
    public partial class MainWindow : Window
    {
        private readonly DatabaseService _db = new();
        private readonly ActivityLog _log = new();
        private ChatService? _chatService;
        private string _userName = string.Empty;
        private bool _waitingForName = true;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                _db.Initialize();
                _log.Add("Database initialised successfully");
            }
            catch (Exception ex)
            {
                AppendToChat($"Bot: Database warning: {ex.Message}. Task features may be limited.");
                _log.Add("Database initialisation failed");
            }

            txtAsciiLogo.Text = AsciiArt.Logo;

            var greeting = new VoiceGreeting();
            try
            {
                await greeting.PlayAsync();
            }
            catch
            {
                AppendToChat("Bot: (Voice greeting unavailable)");
            }

            AppendToChat("Bot: Welcome! I am your Cybersecurity Awareness Assistant.");
            AppendToChat("Bot: To begin, please tell me your name.");

            _waitingForName = true;
            txtUserInput.IsEnabled = true;
            btnSend.IsEnabled = true;

            RefreshTaskList();
            RefreshActivityLog();
        }

        private void BtnSend_Click(object sender, RoutedEventArgs e)
        {
            ProcessInput();
        }

        private void txtUserInput_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
                ProcessInput();
        }

        private void ProcessInput()
        {
            string input = txtUserInput.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;

            AppendToChat("You: " + input);

            try
            {
                if (_waitingForName)
                {
                    _userName = input;
                    _waitingForName = false;
                    _chatService = new ChatService(_userName, _db, _log);
                    _log.Add($"User {_userName} started chat");
                    AppendToChat($"Bot: Nice to meet you, {_userName}!");
                    AppendToChat("Bot: I can answer cybersecurity questions, help with tasks, or quiz you. Type 'help' to see all options.");
                }
                else if (_chatService != null)
                {
                    string reply = _chatService.GetResponse(input, out bool quizMode);
                    AppendToChat("Bot: " + reply);

                    if (quizMode)
                    {
                        RefreshQuizTab();
                    }

                    RefreshTaskList();
                    RefreshActivityLog();

                    if (_chatService.ActiveQuiz != null)
                    {
                        RefreshQuizTab();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Chat error: {ex}");
                AppendToChat("Bot: Oops! Something went wrong. Please try again.");
            }

            txtUserInput.Clear();
        }

        public void AppendToChat(string message)
        {
            txtChatDisplay.AppendText(message + Environment.NewLine);
            txtChatDisplay.ScrollToEnd();
        }

        private void RefreshTaskList()
        {
            try
            {
                var tasks = _db.GetAllTasks();
                lstTasks.ItemsSource = tasks;
            }
            catch
            {
                // DB may not be available
            }
        }

        private void RefreshActivityLog()
        {
            var entries = _log.GetRecent(10);
            lstActivityLog.ItemsSource = entries;
        }

        private void BtnAddTask_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTaskTitle.Text.Trim();
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Please enter a task title.", "Task Required", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var task = new CyberTask
                {
                    Title = title,
                    Description = txtTaskDescription.Text.Trim(),
                    ReminderDate = dpTaskReminder.SelectedDate,
                    IsCompleted = false
                };
                _db.AddTask(task);
                _log.Add($"Task added via Tasks tab: '{title}'");
                txtTaskTitle.Clear();
                txtTaskDescription.Clear();
                dpTaskReminder.SelectedDate = null;
                RefreshTaskList();

                if (_chatService != null)
                    AppendToChat($"Bot: Task '{title}' has been added successfully." +
                        (task.ReminderDate.HasValue ? $" Reminder set for {task.ReminderDate:yyyy-MM-dd}." : ""));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to add task: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnCompleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                try
                {
                    var tasks = _db.GetAllTasks();
                    var task = tasks.FirstOrDefault(t => t.Id == id);
                    if (task != null)
                    {
                        task.IsCompleted = true;
                        _db.UpdateTask(task);
                        _log.Add($"Task completed: '{task.Title}'");
                        RefreshTaskList();
                        if (_chatService != null)
                            AppendToChat($"Bot: Task '{task.Title}' marked as complete!");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnDeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int id)
            {
                try
                {
                    var tasks = _db.GetAllTasks();
                    var task = tasks.FirstOrDefault(t => t.Id == id);
                    if (task != null)
                    {
                        _db.DeleteTask(id);
                        _log.Add($"Task deleted: '{task.Title}'");
                        RefreshTaskList();
                        if (_chatService != null)
                            AppendToChat($"Bot: Task '{task.Title}' has been deleted.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void BtnStartQuiz_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var quiz = new QuizGame();
                var q = quiz.Start();

                if (_chatService != null)
                    _chatService.ActiveQuiz = quiz;

                _log.Add("Quiz started from Quiz tab");
                DisplayQuizQuestion(quiz, q);
                btnStartQuiz.Content = "Restart Quiz";
                txtQuizScore.Text = $"Score: 0 / {quiz.TotalQuestions}";
                txtQuizFeedback.Text = "";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting quiz: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BtnQuizAnswer_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int index)
            {
                var quiz = _chatService?.ActiveQuiz;
                if (quiz == null || !quiz.IsActive)
                {
                    txtQuizFeedback.Text = "No active quiz. Click 'Start Quiz' to begin.";
                    return;
                }

                var (correct, explanation, isComplete) = quiz.Answer(index);
                _log.Add($"Quiz: {(correct ? "correct" : "incorrect")} on Q{quiz.CurrentQuestionNumber - 1}");
                txtQuizFeedback.Text = (correct ? "✅ Correct!" : "❌ Incorrect.") + " " + explanation;
                txtQuizScore.Text = $"Score: {quiz.Score} / {quiz.TotalQuestions}";

                if (isComplete)
                {
                    txtQuizQuestion.Text = quiz.GetScoreMessage();
                    quizOptionsPanel.ItemsSource = null;
                    txtQuizScore.Text = $"Final Score: {quiz.Score} / {quiz.TotalQuestions}";
                    _log.Add($"Quiz completed: score {quiz.Score}/{quiz.TotalQuestions}");
                    btnStartQuiz.Content = "Start Quiz";

                    if (_chatService != null)
                        _chatService.ActiveQuiz = null;

                    AppendToChat($"Bot: Quiz completed! Score: {quiz.Score}/{quiz.TotalQuestions}. {quiz.GetScoreMessage()}");
                }
                else
                {
                    var next = quiz.CurrentQuestion;
                    if (next != null)
                        DisplayQuizQuestion(quiz, next);
                }
                RefreshActivityLog();
            }
        }

        private void DisplayQuizQuestion(QuizGame quiz, QuizQuestion q)
        {
            txtQuizQuestion.Text = $"Question {quiz.CurrentQuestionNumber} of {quiz.TotalQuestions}:\n\n{q.Question}";
            var options = q.Options.Select((opt, i) => new { Text = $"{i + 1}. {opt}", Index = i }).ToList();
            quizOptionsPanel.ItemsSource = options;
        }

        private void BtnRefreshLog_Click(object sender, RoutedEventArgs e)
        {
            RefreshActivityLog();
        }

        private void BtnShowMoreLog_Click(object sender, RoutedEventArgs e)
        {
            var entries = _log.GetAll();
            lstActivityLog.ItemsSource = entries;
            btnShowMoreLog.IsEnabled = false;
            btnRefreshLog.IsEnabled = true;
        }

        private void RefreshQuizTab()
        {
            var quiz = _chatService?.ActiveQuiz;
            if (quiz != null && quiz.IsActive)
            {
                var q = quiz.CurrentQuestion;
                if (q != null)
                {
                    DisplayQuizQuestion(quiz, q);
                    btnStartQuiz.Content = "Restart Quiz";
                    txtQuizScore.Text = $"Score: {quiz.Score} / {quiz.TotalQuestions}";
                }
            }
        }
    }
}
