using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ShadyBot.Models;
using ShadyBot.Services;

namespace ShadyBot.Bots
{
    public class GreetingBot: ActivityHandler
    {
        #region Variables
        private readonly BotStateService _botStateService;
        #endregion

        public GreetingBot(BotStateService botStateService)
        {
            _botStateService = botStateService ?? throw new System.ArgumentNullException(nameof(botStateService));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await GetName(turnContext, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="membersAdded"></param>
        /// <param name="turnContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await GetName(turnContext, cancellationToken);
                }
            }
        }

        private async Task GetName(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            UserProfile userProfile = await _botStateService.UserProfilerAccessor.GetAsync(turnContext, () => new UserProfile());
            ConversationData conversationData   = await _botStateService.ConversationDataAccessor.GetAsync(turnContext, () => new ConversationData());
            if (!string.IsNullOrEmpty(userProfile.Name))
            {
                await turnContext.SendActivityAsync(MessageFactory.Text(string.Format("Hi, {0}. How can I help you today?", userProfile.Name)), cancellationToken );
            }
            else
            {
                if (conversationData.PromptUserForName)
                {
                    // Set the name to what the user provided
                    userProfile.Name = turnContext.Activity.Text?.Trim();

                    // Acknowledge that we got their name.
                    await turnContext.SendActivityAsync(MessageFactory.Text(string.Format($"Thanks {0}. How can I help you today?", userProfile.Name)), cancellationToken);

                    // Reset the flag to allow the bot to go through the cycle again.
                    conversationData.PromptUserForName = true; 
                }
                else
                {
                    // Prompt user for their name;
                    await turnContext.SendActivityAsync(MessageFactory.Text($"What is you name?"), cancellationToken);

                    // Set the flag to true, so we don't prompt in the next turn;
                    conversationData.PromptUserForName = true;
                }

                // Save any state changes that might have occured during the turn.
                await _botStateService.UserProfilerAccessor.SetAsync(turnContext, userProfile);
                await _botStateService.ConversationDataAccessor.SetAsync(turnContext, conversationData);

                await _botStateService.UserState.SaveChangesAsync(turnContext);
                await _botStateService.ConversationState.SaveChangesAsync(turnContext);
            }
        }
    }
}
