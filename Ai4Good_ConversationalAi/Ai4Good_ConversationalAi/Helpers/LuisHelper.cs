using Ai4Good_ConversationalAi.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Helpers
{
    public static class LuisHelper
    {
        public static async Task<IntentWithScore> ExecuteLuisQuery(BotIntent botIntent, ILogger logger, ITurnContext turnContext, CancellationToken cancellationToken)
        {
            try
            {
                var endPoint = $"https://{ botIntent.HostName}.api.cognitive.microsoft.com";
                var luisApplication = new LuisApplication(botIntent.Id, botIntent.Key, endPoint);

                var recognizer = new LuisRecognizer(luisApplication);

                //This actually calls to LUIS
                var recognizerResult = await recognizer.RecognizeAsync(turnContext, cancellationToken);
                var (intent, score) = recognizerResult.GetTopScoringIntent();
                return new IntentWithScore() { Result = intent, Score = score.ToString() };
            }
            catch (Exception e)
            {
                logger.LogWarning($"LUIS Exception: {e.Message} Check your LUIS configuration.");
            }
            return null;
        }
    }
}
