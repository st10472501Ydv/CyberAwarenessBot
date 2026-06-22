using System.Collections.Generic;

namespace CyberAwarenessBot.Gui.Models
{
    public class QuizQuestion
    {
        public string Question { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
        public int CorrectOptionIndex { get; set; }
        public string Explanation { get; set; } = string.Empty;
    }
}
