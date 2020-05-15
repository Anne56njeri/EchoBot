using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using MyEchoBot.Services;
using MyEchoBot.Models;


namespace MyEchoBot.Bots
{
    public class GreetinBot:ActivityHandler
    {   
        // Variables
         private readonly BotStateService _botStateService;
         
         public GreetinBot(BotStateService botStateService)
         {
             _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));

         }

         protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext,cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var welcomeText = "Hello and welcome!";
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
                }
            }
        }
        private async Task GetName(ITurnContext turnContext,CancellationToken cancellationToken){
            // this is a constructor
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(turnContext,()=> new UserProfile());

            ConversationData conversationData = await _botStateService.ConversationDataAccessor.GetAsync(turnContext,()=>new ConversationData());
            // Checking if the user name field is empty..
            if(!string.IsNullOrEmpty(userProfile.Name)){
                // if its not empty we greet the user
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("Hi {0}.How can I help you today ?",userProfile.Name)),cancellationToken);
            }
            else {
                // check whether we've prompted the user for name
                if(conversationData.PromptedUserForName){
                    // save the user
                    userProfile.Name = turnContext.Activity.Text?.Trim();
                    // thank the user
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("Thanks{0}.How can I help you today ?",userProfile.Name)),cancellationToken);
                    conversationData.PromptedUserForName = false;
                }
                else{
                    // prompt user for name...
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("What is your name ?")),cancellationToken);
                    // then set to user
                    conversationData.PromptedUserForName = true;
                }
                // save any state changes that might have occurres during the turn 
                await _botStateService.UserProfileAccessor.SetAsync(turnContext,userProfile);
                await _botStateService.ConversationDataAccessor.SetAsync(turnContext,conversationData);

                await _botStateService.UserState.SaveChangesAsync(turnContext);
                await _botStateService.ConversationState.SaveChangesAsync(turnContext);
            }

        }
        
    }
}