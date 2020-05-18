using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using System;
using MyEchoBot.Models;
using Microsoft.Bot.Builder.Dialogs;

// State Accessor

namespace MyEchoBot.Services
{
    public class BotStateService
    {    
        // State Variables
        public UserState UserState{get;}
        public ConversationState ConversationState {get;}

        // IDs is nameof Botstateservice
        // For identifying 
        public static string ConversationDataId{get;} = $"{nameof(BotStateService)}.ConversationData";
        public static string UserProfileId{get; } = $"{nameof(BotStateService)}.UserProfile";
        public static string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";

        // Accessor 
        // Helps to access data inside state management bucket by pull,pushing and deleting data 
        public IStatePropertyAccessor<UserProfile> UserProfileAccessor{get;set;}
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor{get;set;}
        
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }

        // We inject conversation state..
        // Constructor 
        public BotStateService(ConversationState conversationState, UserState userState)
        {
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            intializeAccessors();

        }
        
        
        public void intializeAccessors()
        {

            // Intialize Converstioin State Accessors
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationDataId);
            //  intialize User state the properties we want to store using the IDs
            UserProfileAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);

            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);
        }
    }
}