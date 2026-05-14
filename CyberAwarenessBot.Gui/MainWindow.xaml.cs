using System;
using System.Windows;
using CyberAwarenessBot.Gui.Helpers;
using CyberAwarenessBot.Gui.Services;

namespace CyberAwarenessBot.Gui
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 1. Display the ASCII logo
            txtAsciiLogo.Text = AsciiArt.Logo;

            // 2. Play the voice greeting
            var greeting = new VoiceGreeting();
            try
            {
                await greeting.PlayAsync();
            }
            catch (Exception)
            {
                // If audio fails for any reason, tell the user
                AppendToChat("Bot: (Voice greeting unavailable)");
            }

            // 3. Welcome message
            AppendToChat("Bot: Welcome! I am your Cybersecurity Awareness Assistant.");
            AppendToChat("Bot: To begin, please tell me your name.");
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

            // For now, just echo
            AppendToChat("You: " + input);
            txtUserInput.Clear();
        }

        public void AppendToChat(string message)
        {
            txtChatDisplay.AppendText(message + Environment.NewLine);
            txtChatDisplay.ScrollToEnd();
        }
    }
}