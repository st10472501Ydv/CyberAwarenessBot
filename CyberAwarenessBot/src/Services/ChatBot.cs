namespace CyberAwarenessBot.Services
{
    public class ChatBot
    {
        private string userName = "";

        public void StartChat()
        {
            Console.Write("Please enter your name: ");
            userName = Console.ReadLine();

            Console.WriteLine($"\nWelcome, {userName}.");
            Console.WriteLine($"Hello, {userName}.");
            Console.WriteLine("Cybersecurity Awareness Bot.");
            Console.WriteLine("Type 'help' for options.");
            Console.WriteLine("Type 'exit' to quit.\n");

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
                    Console.WriteLine("\nBot: Ready to assist.");
                }
                else if (input.Contains("purpose") || input.Contains("what do you do"))
                {
                    Console.WriteLine("\nBot: Cybersecurity Awareness Bot.");
                    Console.WriteLine("Provides information on online safety.");
                }
                else if (input.Contains("what can i ask") || input.Contains("help"))
                {
                    Console.WriteLine("\nYou can ask about:");
                    Console.WriteLine("- Password safety");
                    Console.WriteLine("- Phishing scams");
                    Console.WriteLine("- Safe browsing");
                }
                else if (input.Contains("password"))
                {
                    Console.WriteLine("\nPassword Safety Tips:");
                    Console.WriteLine("- Use at least 12 characters");
                    Console.WriteLine("- Mix letters, numbers and symbols");
                    Console.WriteLine("- Do not reuse passwords across sites");
                    Console.WriteLine("- Consider using a password manager");
                }
                else if (input.Contains("phishing"))
                {
                    Console.WriteLine("\nPhishing Awareness:");
                    Console.WriteLine("- Phishing emails may pretend to be from banks or trusted brands");
                    Console.WriteLine("- Do not click suspicious links in emails");
                    Console.WriteLine("- Check the sender's email address");
                    Console.WriteLine("- When in doubt, contact the organisation directly");
                }
                else if (input.Contains("safe browsing") || input.Contains("browsing"))
                {
                    Console.WriteLine("\nSafe Browsing Tips:");
                    Console.WriteLine("- Look for HTTPS in the website address");
                    Console.WriteLine("- Avoid clicking pop-up ads");
                    Console.WriteLine("- Keep your browser updated");
                    Console.WriteLine("- Do not download files from untrusted websites");
                }
                else if (input == "exit")
                {
                    Console.WriteLine($"\nGoodbye {userName}. Stay safe online.");
                    break;
                }
                else
                {
                    Console.WriteLine("\nI did not understand that. Please rephrase.");
                    Console.WriteLine("Type 'help' for options.");
                }
            }
        }
    }
}
