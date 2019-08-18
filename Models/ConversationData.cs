using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShadyBot.Models
{
    public class ConversationData
    {
        // Track whether we have already asked the user's name;
        public bool PromptUserForName { get; set; } = false;
    }
}
