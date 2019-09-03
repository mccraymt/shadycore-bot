  using System;
using System.Text.RegularExpressions;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using ShadyBot.Services;
using System.Threading;
using System.Threading.Tasks;
using ShadyBot.Models;

namespace ShadyBot.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        #region Variables
        private readonly BotStateService _botStateService;
        private readonly BotServices _botServices;
        #endregion

        public MainDialog(BotStateService botStateService, BotServices botServices)
            : base($"{nameof(MainDialog)}")
        {
            _botStateService = botStateService ?? throw new ArgumentNullException(nameof(botStateService));
            _botServices = botServices ?? throw new ArgumentNullException(nameof(botServices));
            InitializeWaterfallDialog();
        }

        private void InitializeWaterfallDialog()
        {
            // Create Waterfall Steps
            var waterfallSteps = new WaterfallStep[]
            {
                InitialStepAsync,
                FinalStepAsync
            };

            // Add Named Dialogs

            AddDialog(new GreetingDialog($"{nameof(MainDialog)}.greeting", _botStateService));
            AddDialog(new BugReportDialog($"{nameof(MainDialog)}.bugReport", _botStateService));
            AddDialog(new BugReportDialog($"{nameof(MainDialog)}.bugType", _botStateService));


            AddDialog(new WaterfallDialog($"{nameof(MainDialog)}.mainFlow", waterfallSteps));

            // Set the starting Dialog
            InitialDialogId = $"{nameof(MainDialog)}.mainFlow";
        }

        private async Task<DialogTurnResult> InitialStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // First, we use the dispatch model to determine which cognitive service (LUIS or QAMaker) to use.
            var recognizerResult = await _botServices.Dispatch.RecognizeAsync(stepContext.Context, cancellationToken);

            // Top intent tell us which congnitive service to use.
            var topIntent = recognizerResult.GetTopScoringIntent();


            switch (topIntent.intent)
            {
                case "GreetingIntent":
                    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);

                case "NewBugReportIntent":
                    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null, cancellationToken);

                case "QueryBugTypeIntent":
                    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugType", null, cancellationToken);

                default:
                    _ = await stepContext.Context.SendActivityAsync(MessageFactory.Text($"I'm sorry I don't know what you mean."), cancellationToken);
                    break;

            }


            //if (Regex.Match(stepContext.Context.Activity.Text.ToLower(), "hi").Success)
            //{
            //    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.greeting", null, cancellationToken);
            //}
            //else 
            //{
            //    return await stepContext.BeginDialogAsync($"{nameof(MainDialog)}.bugReport", null, cancellationToken);
            //}

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}
