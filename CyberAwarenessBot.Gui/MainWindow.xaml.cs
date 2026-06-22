using System;
using System.Windows;
using CyberAwarenessBot.Gui.Helpers;
using CyberAwarenessBot.Gui.Services;

namespace CyberAwarenessBot.Gui
{
    /// <summary>
    /// Main application window for the Cybersecurity Awareness Bot GUI.
    /// Manages the chat interface, user input, and coordinates voice/ASCII/chat services.
    /// </summary>
    public partial class MainWindow : Window
    {
        private string _userName = string.Empty;
        private bool _waitingForName = true;
        private ChatService? _chatService;

        /// <summary>Initialises the window and its XAML components.</summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the window Loaded event: displays the ASCII logo, plays the voice greeting,
        /// shows the welcome message, and enables the input controls.
        /// </summary>
        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
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

        /// <summary>
        /// Reads the user's input, determines whether we are still awaiting a name
        /// or in normal chat mode, and dispatches the response.
        /// </summary>
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
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Chat error: {ex}");
                AppendToChat("Bot: Oops! Something went wrong. Please try again.");
            }

            txtUserInput.Clear();
        }

        /// <summary>Appends a message to the chat display and auto-scrolls to the bottom.</summary>
        /// <param name="message">The message text to display.</param>
        public void AppendToChat(string message)
        {
            txtChatDisplay.AppendText(message + Environment.NewLine);
            txtChatDisplay.ScrollToEnd();
        }
    }
}