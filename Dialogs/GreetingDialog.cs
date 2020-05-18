using System;
using System.Threading;
using System.Threading.Tasks;
using MyEchoBot.Models;
using MyEchoBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace MyEchoBot.Dialogs
{  
    // inhereting from component dialog(re-usable components)
    public class GreetingDialog:ComponentDialog
    { 
        // variable
        private readonly BotStateService _botStateService;

            //Injecting botstate service
        public GreetingDialog(string dialogId, BotStateService botStateService): base(dialogId)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));

            InitializeWaterfallDialog();
        }
        // waterfall dialog 
         private void InitializeWaterfallDialog()
        {   
            // create waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };
            // add new dialogs and pass the steps.
            AddDialog(new WaterfallDialog($"{nameof(GreetingDialog)}.mainFlow", waterfallSteps));
            AddDialog(new TextPrompt($"{nameof(GreetingDialog)}.name"));
            // set the starting dialog
            InitialDialogId = $"{nameof(GreetingDialog)}.mainFlow";
        }

         private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.Name))
            {
                return await stepContext.PromptAsync($"{nameof(GreetingDialog)}.name",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("What  is your name?")
                }, cancellationToken);
            }

            else
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }
        }
         private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile());
            if (string.IsNullOrEmpty(userProfile.Name))
            {   
                // set the name
                userProfile.Name = (string)stepContext.Result;
                //  save any state changes that might have occured during the turn
                await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            }
                // dialog will need to have logic flow a begin and end 
                
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Hi {0}. How can I help you today?", userProfile.Name)), cancellationToken);
            return await stepContext.EndDialogAsync(null, cancellationToken);

        }

        
    }
}