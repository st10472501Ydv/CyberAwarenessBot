namespace CyberAwarenessBot.Services
{
    public class ChatBot
    {
        private string userName = "";

        public void StartChat()
        {
            // Ask for the user's name
            Console.Write("  Please enter your name: ");
            userName = Console.ReadLine();

            // Personalised welcome message
            Console.WriteLine($"\n  Welcome, {userName}!");
            Console.WriteLine($"  Its great to have you here {userName}");
            Console.WriteLine("  I'm your Cybersecurity Awareness Bot.");
            Console.WriteLine("  Type 'help' to see what I can assist you with.\n");
        }
    }
}