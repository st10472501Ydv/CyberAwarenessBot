using System;
using System.Windows;
using CyberAwarenessBot.Gui.Helpers;
using CyberAwarenessBot.Gui.Services;

namespace CyberAwarenessBot.Gui
{
    public partial class MainWindow : Window
    {
        private string _userName = string.Empty;
        private bool _waitingForName = true;
        private ChatService? _chatService;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Show ASCII logo
            txtAsciiLogo.Text = AsciiArt.Logo;

            // Play voice greeting
            var greeting = new VoiceGreeting();
            try
            {
                await greeting.PlayAsync();
            }
            catch
            {
                AppendToChat("Bot: (Voice greeting unavailable)");
            }

            // Welcome text
            AppendToChat("Bot: Welcome! I am your Cybersecurity Awareness Assistant.");
            AppendToChat("Bot: To begin, please tell me your name.");

            _waitingForName = true;

            // Enable input controls now that the welcome sequence is complete
            txtUserInput.IsEnabled = true;
            btnSend.IsEnabled = true;
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

            if (_waitingForName)
            {
                // Capture the user's name
                _userName = input;
                _waitingForName = false;

                _chatService = new ChatService(_userName);

                AppendToChat($"Bot: Nice to meet you, {_userName}!");
                AppendToChat("Bot: You can ask me about password safety, phishing, scams, privacy, or safe browsing.");
                AppendToChat("Bot: Type 'help' for options, or just ask a question.");
            }
            else if (_chatService != null)
            {
                string reply = _chatService.GetResponse(input);
                AppendToChat("Bot: " + reply);
            }

            txtUserInput.Clear();
        }

        public void AppendToChat(string message)
        {
            txtChatDisplay.AppendText(message + Environment.NewLine);
            txtChatDisplay.ScrollToEnd();
        }
    }
}