using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MyEchoBot.Services;
using System;
using MyEchoBot.Helpers;

namespace MyEchoBot.Bots
{
    public class DialogBot<T> : ActivityHandler where T : Dialog
    { 
        // / Variables 
        protected readonly Dialog _dialog;
        protected readonly BotStateService _botStateService;
        protected readonly ILogger _logger;
        public DialogBot(BotStateService botStateService, T dialog, ILogger<DialogBot<T>> logger)
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));
            _dialog = dialog ?? throw new ArgumentNullException(nameof(dialog));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        // called when  a bot receives an activity...
        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            await base.OnTurnAsync(turnContext, cancellationToken);

            await _botStateService.UserState.SaveChangesAsync(turnContext, false, cancellationToken);
            await _botStateService.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // how we log information on our logger...
            _logger.LogInformation("Running dialog  with Message Activity");

            await _dialog.Run(turnContext, _botStateService.DialogStateAccessor, cancellationToken);
        }



    }
}