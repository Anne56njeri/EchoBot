using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using System;

// This model is meant to hold data about our conversation...

namespace MyEchoBot.Models
{
    public class ConversationData
    {
        // Track whether we have already asked the user's name...
        public bool PromptedUserForName { get; set; } = false;
    }
}