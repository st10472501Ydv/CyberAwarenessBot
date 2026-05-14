using System;
using System.Windows;

namespace CyberAwarenessBot.Gui
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // ASCII art will be set here later
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