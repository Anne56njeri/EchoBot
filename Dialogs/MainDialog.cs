using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using MyEchoBot.Services
;
using Microsoft.Bot.Builder.Dialogs;

namespace MyEchoBot.Dialogs
{
    public class MainDialog:ComponentDialog
    {
         private readonly BotStateService _botStateService;


        public MainDialog(BotStateService botStateService)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));

            InitializeWaterfallDialog();
        }

          private void InitializeWaterfallDialog()
        {
            var waterfallSteps = new WaterfallStep[]
            {
                InitialSetupAsync,
                FinalStepAsync
            };
            // which dialog we are going to call...
            
            AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", _botStateService));
            AddDialog(new PizzaDialog($"{nameof(MainDialog)}.pizzaOrder", _botStateService));
            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));

            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
        }
         private async Task<DialogTurnResult> InitialSetupAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (Regex.Match(stepContext.Context.Activity.Text.ToLower(), "hi").Success)
            {
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);
            }
            else
            {
                return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.pizzaOrder", null, cancellationToken);
            }
        }
        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }



    }
}