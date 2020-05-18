using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MyEchoBot.Models;
using MyEchoBot.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;

namespace MyEchoBot.Dialogs
{   
    // It inherits from parent class
    public class PizzaDialog:ComponentDialog
    {
        private readonly BotStateService _botStateService;

        public PizzaDialog(string dialogId, BotStateService botStateService) :base(dialogId)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));

            InitializeWaterfallDialog();
        }
        // Intialize the waterflow dialog...
        private void InitializeWaterfallDialog()
        {   
            // create waterfall steps
            var waterfallSteps = new WaterfallStep[]
            {
               PizzaMenuAsync,
               DescriptionStepAsync,
               LocationStepAsync,
               SummaryStepAsync
            };

            
            AddDialog(new WaterfallDialog($"{nameof(PizzaDialog)}.mainFlow",waterfallSteps));
            AddDialog(new ChoicePrompt($"{nameof(PizzaDialog)}.order"));
            AddDialog(new TextPrompt($"{nameof(PizzaDialog)}.instruction"));
            AddDialog(new TextPrompt($"{nameof(PizzaDialog)}.location"));

            // set the starting dialog 
            InitialDialogId = $"{nameof(PizzaDialog)}.mainFlow";
        }
        private async Task<DialogTurnResult> PizzaMenuAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            return await stepContext.PromptAsync($"{nameof(PizzaDialog)}.order",
            new PromptOptions {
                 Prompt = MessageFactory.Text("Hello  please enter the pizza you would like to order:)"),
                 Choices = ChoiceFactory.ToChoices(new List<string> { "All meaty", "All Chicken", "Chicken BBQ", "Vegan Pizza", "MayCorn"}),
            }, cancellationToken);
        }
                    
    
               
        
        private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["order"] = ((FoundChoice)stepContext.Result).Value;

            return await stepContext.PromptAsync($"{nameof(PizzaDialog)}.instruction",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text("Enter any instructions  for your Pizza")
                }, cancellationToken);
        }
          private async Task<DialogTurnResult> LocationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            stepContext.Values["instruction"] = (string)stepContext.Result;

            return await stepContext.PromptAsync($"{nameof(PizzaDialog)}.location", 
            new PromptOptions
            {
                Prompt = MessageFactory.Text("Please enter your location"),
            }, cancellationToken);
        }
        
        

        private  async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {  
            // value we get back from the choice prompt
            stepContext.Values["location"] = (string)stepContext.Result;

            var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

            userProfile.Order = (string)stepContext.Values["order"];
            userProfile.Instruction = (string)stepContext.Values["instruction"].ToString();
            userProfile.Location = (string)stepContext.Values["location"];
            // Show the summary to the user 
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($" Here is a summary of your order;"),cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Order:{0}",userProfile.Order)),cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Instructions:{0}",userProfile.Instruction)),cancellationToken);
            await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Location:{0}",userProfile.Location)),cancellationToken);


            // save data in user state
            await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
            // waterfall step always finishes eith the end  of the waterfall so here is where it ends
            return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

        }

        // our validators...
        private Task<bool> CallbackTimeValidatorAsync(PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;

            if (promptContext.Recognized.Succeeded)
            {
                var resolution = promptContext.Recognized.Value.First();
                DateTime selectedDate = Convert.ToDateTime(resolution.Value);
                TimeSpan start = new TimeSpan(9, 0, 0); // 10 oclock
                TimeSpan end = new TimeSpan(17, 0, 0);
                if((selectedDate.TimeOfDay >= start) && (selectedDate.TimeOfDay <= end))
                {
                    valid = true;
                }
            }

            return Task.FromResult(valid);
        }


        private Task<bool> PhoneNumberValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            var valid = false;

            if (promptContext.Recognized.Succeeded)
            {
                valid = Regex.Match(promptContext.Recognized.Value, @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$").Success;
            }
            return Task.FromResult(valid);
        }
        
    }
}