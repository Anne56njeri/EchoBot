// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text.RegularExpressions;
// using System.Threading;
// using System.Threading.Tasks;
// using MyEchoBot.Models;
// using MyEchoBot.Services;
// using Microsoft.Bot.Builder;
// using Microsoft.Bot.Builder.Dialogs;
// using Microsoft.Bot.Builder.Dialogs.Choices;

// namespace MyEchoBot.Dialogs
// {
//     public class BugReportDialog:ComponentDialog
//     {
//         private readonly BotStateService _botStateService;

//         public BugReportDialog(string dialogId, BotStateService botStateService) :base(dialogId)
//         {
//             _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));

//             InitializeWaterfallDialog();
//         }

//         private void InitializeWaterfallDialog()
//         {   
//             // create waterfall steps
//             var waterfallSteps = new WaterfallStep[]
//             {
//                 DescriptionStepAsync,
//                 CallbackTimeStepAsync,
//                 PhoneNumberStepAsync,
//                 BugStepAsync,
//                 SummaryStepAsync
//             };

            
//             AddDialog(new WaterfallDialog($"{nameof(BugReportDialog)}.mainFlow",waterfallSteps));
//             AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.description"));
//             AddDialog(new DateTimePrompt($"{nameof(BugReportDialog)}.callbackTime", CallbackTimeValidatorAsync));
//             AddDialog(new TextPrompt($"{nameof(BugReportDialog)}.phoneNumber", PhoneNumberValidatorAsync));
//             AddDialog(new ChoicePrompt($"{nameof(BugReportDialog)}.bug"));

//             // set the starting dialog 
//             InitialDialogId = $"{nameof(BugReportDialog)}.mainFlow";
//         }

//          private async Task<DialogTurnResult> DescriptionStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//         {
//             return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.description",
//                 new PromptOptions
//                 {
//                     Prompt = MessageFactory.Text("Enter a description for your report")
//                 }, cancellationToken);
//         }
//           private async Task<DialogTurnResult> CallbackTimeStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//         {
//             stepContext.Values["description"] = (string)stepContext.Result;

//             return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.callbackTime", 
//             new PromptOptions
//             {
//                 Prompt = MessageFactory.Text("Please enter in a callback time"),
//                 RetryPrompt = MessageFactory.Text("The value entered must be  between the hours of 9 am and 5 pm"),
//             }, cancellationToken);
//         }
//          private async Task<DialogTurnResult> PhoneNumberStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//         {
//             stepContext.Values["callbackTime"] = Convert.ToDateTime(((List<DateTimeResolution>)stepContext.Result).FirstOrDefault().Value);

//             // step context that holds
//             return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.phoneNumber",
            
//                 new PromptOptions
//                 {
//                     Prompt = MessageFactory.Text("Please enter in a phone number  that we can call back at"),
//                     RetryPrompt = MessageFactory.Text("Please enter a valid  phone number"),
//                 }, cancellationToken);
//         }
//         private async Task<DialogTurnResult> BugStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//         {
//             stepContext.Values["phoneNumber"] = (string)stepContext.Result;

//             return await stepContext.PromptAsync($"{nameof(BugReportDialog)}.bug",
//                  new PromptOptions
//                  {
//                      Prompt = MessageFactory.Text("Please enter the type of bug"),
//                      Choices = ChoiceFactory.ToChoices(new List<string> { "Security", "Crash", "Power", "Perfromance", "Usability", "Serious Bug","Other"}),
//                  }, cancellationToken);
               
//         }

//         private  async Task<DialogTurnResult> SummaryStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
//         {  
//             // value we get back from the choice prompt
//             stepContext.Values["bug"] = ((FoundChoice)stepContext.Result).Value;

//             var userProfile = await _botStateService.UserProfileAccessor.GetAsync(stepContext.Context, () => new UserProfile(), cancellationToken);

//             userProfile.Description = (string)stepContext.Values["description"].ToString();
//             userProfile.CallbackTime = (DateTime)stepContext.Values["callbackTime"];
//             userProfile.PhoneNumber = (string)stepContext.Values["phoneNumber"];
//             userProfile.Bug = (string)stepContext.Values["bug"];
//             // Show the summary to the user 
//             await stepContext.Context.SendActivityAsync(MessageFactory.Text($" Here is a summary of your bug report;"),cancellationToken);
//              await stepContext.Context.SendActivityAsync(MessageFactory.Text(String.Format("Description:{0}",userProfile.Description)),cancellationToken);

//             // save data in user state
//             await _botStateService.UserProfileAccessor.SetAsync(stepContext.Context, userProfile);
//             // waterfall step always finishes eith the end  of the waterfall so here is where it ends
//             return await stepContext.EndDialogAsync(cancellationToken: cancellationToken);

//         }

//         // our validators...
//         private Task<bool> CallbackTimeValidatorAsync(PromptValidatorContext<IList<DateTimeResolution>> promptContext, CancellationToken cancellationToken)
//         {
//             var valid = false;

//             if (promptContext.Recognized.Succeeded)
//             {
//                 var resolution = promptContext.Recognized.Value.First();
//                 DateTime selectedDate = Convert.ToDateTime(resolution.Value);
//                 TimeSpan start = new TimeSpan(9, 0, 0); // 10 oclock
//                 TimeSpan end = new TimeSpan(17, 0, 0);
//                 if((selectedDate.TimeOfDay >= start) && (selectedDate.TimeOfDay <= end))
//                 {
//                     valid = true;
//                 }
//             }

//             return Task.FromResult(valid);
//         }


//         private Task<bool> PhoneNumberValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
//         {
//             var valid = false;

//             if (promptContext.Recognized.Succeeded)
//             {
//                 valid = Regex.Match(promptContext.Recognized.Value, @"^(\+\d{1,2}\s)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$").Success;
//             }
//             return Task.FromResult(valid);
//         }

//     }
// }