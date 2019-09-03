using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Bot.Builder.AI.Luis;
namespace ShadyBot.Services
{
    public class BotServices
    {
        public LuisRecognizer Dispatch { get; private set; }
        public BotServices(IConfiguration configuration)
        {
            Dispatch = new LuisRecognizer(
                new LuisApplication(
                    configuration["LuisAppId"],
                    configuration["LuisAPIKey"],
                    $"https://{configuration["LuisAPIHostName"]}.api.cognitive.microsoft.com"
                ),
                new LuisPredictionOptions { IncludeAllIntents = true, IncludeInstanceData = true },
                true
            );
        }
    }
}
