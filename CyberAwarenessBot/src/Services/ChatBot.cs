using CyberAwarenessBot.UI;

namespace CyberAwarenessBot.Services
{
    public class ChatBot
    {
        private string userName = "";

        public void StartChat()
        {
            Console.Write("Please enter your name: ");
            userName = Console.ReadLine();

            ConsoleHelper.PrintDivider();
            ConsoleHelper.TypeWrite($"Welcome, {userName}.", ConsoleColor.White);
            ConsoleHelper.TypeWrite($"Hello, {userName}.", ConsoleColor.White);
            ConsoleHelper.TypeWrite("Cybersecurity Awareness Bot.", ConsoleColor.Cyan);
            ConsoleHelper.TypeWrite("Type 'help' for options.", ConsoleColor.White);
            ConsoleHelper.TypeWrite("Type 'exit' to quit.", ConsoleColor.White);
            ConsoleHelper.PrintDivider();

            StartConversation();
        }

        public void StartConversation()
        {
            string input = "";

            while (input != "exit")
            {
                Console.Write($"\n{userName}: ");
                input = (Console.ReadLine() ?? string.Empty).ToLower();

                if (input.Contains("how are you"))
                {
                    ConsoleHelper.PrintDivider();
                    ConsoleHelper.TypeWrite("Ready to assist.", ConsoleColor.Cyan);
                    ConsoleHelper.PrintDivider();
                }
                else if (input.Contains("purpose") || input.Contains("what do you do"))
                {
                    ConsoleHelper.PrintDivider();
                    ConsoleHelper.TypeWrite("Cybersecurity Awareness Bot.", ConsoleColor.Cyan);
                    ConsoleHelper.TypeWrite("Provides information on online safety.", ConsoleColor.White);
                    ConsoleHelper.PrintDivider();
                }
                else if (input.Contains("what can i ask") || input.Contains("help"))
                {
                    ConsoleHelper.PrintDivider();
                    ConsoleHelper.TypeWrite("You can ask about:", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Password safety", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Phishing scams", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Safe browsing", ConsoleColor.White);
                    ConsoleHelper.PrintDivider();
                }
                else if (input.Contains("password"))
                {
                    ConsoleHelper.PrintDivider();
                    ConsoleHelper.TypeWrite("Password Safety Tips:", ConsoleColor.Cyan);
                    ConsoleHelper.TypeWrite("- Use at least 12 characters", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Mix letters, numbers and symbols", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Do not reuse passwords across sites", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Consider using a password manager", ConsoleColor.White);
                    ConsoleHelper.PrintDivider();
                }
                else if (input.Contains("phishing"))
                {
                    ConsoleHelper.PrintDivider();
                    ConsoleHelper.TypeWrite("Phishing Awareness:", ConsoleColor.Cyan);
                    ConsoleHelper.TypeWrite("- Phishing emails may pretend to be from banks or trusted brands", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Do not click suspicious links in emails", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Check the sender's email address", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- When in doubt, contact the organisation directly", ConsoleColor.White);
                    ConsoleHelper.PrintDivider();
                }
                else if (input.Contains("safe browsing") || input.Contains("browsing"))
                {
                    ConsoleHelper.PrintDivider();
                    ConsoleHelper.TypeWrite("Safe Browsing Tips:", ConsoleColor.Cyan);
                    ConsoleHelper.TypeWrite("- Look for HTTPS in the website address", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Avoid clicking pop-up ads", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Keep your browser updated", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("- Do not download files from untrusted websites", ConsoleColor.White);
                    ConsoleHelper.PrintDivider();
                }
                else if (input == "exit")
                {
                    ConsoleHelper.PrintDivider();
                    ConsoleHelper.TypeWrite($"Goodbye {userName}.", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("Stay safe online.", ConsoleColor.White);
                    ConsoleHelper.PrintDivider();
                    break;
                }
                else
                {
                    ConsoleHelper.PrintDivider();
                    ConsoleHelper.TypeWrite("I did not understand that. Please rephrase.", ConsoleColor.White);
                    ConsoleHelper.TypeWrite("Type 'help' for options.", ConsoleColor.White);
                    ConsoleHelper.PrintDivider();
                }
            }
        }
    }
}
