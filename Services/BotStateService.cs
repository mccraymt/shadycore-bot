using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using ShadyBot.Models;


namespace ShadyBot.Services
{
    public class BotStateService
    {
        #region Variables
        // State variable
        public ConversationState ConversationState { get; }
        public UserState UserState { get; }

        // IDs
        public static string ConversationStateId { get; } = $"{nameof(BotStateService)}.ConversationData";
        public static string DialogStateId { get; } = $"{nameof(BotStateService)}.DialogState";
        public static string UserProfileId { get; } = $"{nameof(BotStateService)}.UserProfile";

        // Accessors
        public IStatePropertyAccessor<ConversationData> ConversationDataAccessor { get; set; }
        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; set; }
        public IStatePropertyAccessor<UserProfile> UserProfilerAccessor { get; set; }
        #endregion

        public BotStateService(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(userState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));

            IntializeAccessors();
        }

        public void IntializeAccessors()
        {
            // Initialize Conversation State Accessor
            ConversationDataAccessor = ConversationState.CreateProperty<ConversationData>(ConversationStateId);
            DialogStateAccessor = ConversationState.CreateProperty<DialogState>(DialogStateId);

            // Initialize User State Accessor
            UserProfilerAccessor = UserState.CreateProperty<UserProfile>(UserProfileId);
          
        }

    }
}
