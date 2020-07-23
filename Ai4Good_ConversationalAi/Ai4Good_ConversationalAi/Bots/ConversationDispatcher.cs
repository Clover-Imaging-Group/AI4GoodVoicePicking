using Ai4Good_ConversationalAi.Helpers;
using Ai4Good_ConversationalAi.Models;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Bots
{

    public class ConversationDispatcher : IBot
    {
        //LuisRecognizer _dispatch { get; set; }
        List<BotIntent> _botIntents { get; set; }
        public async Task<string> OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            //await turnContext.SendActivityAsync($"You sent '{turnContext.Activity.Text}'", cancellationToken: cancellationToken);

            //var dispatchResult = await _dispatch.RecognizeAsync(turnContext, cancellationToken);
            //var topIntent = dispatchResult.GetTopScoringIntent();
            //var botIntent = _botIntents.Where(b => string.Equals(b.Intent, topIntent.intent)).FirstOrDefault();
            var botIntent = new BotIntent
            {
                HostName = "westus",
                Id = "871a0e23-83dc-43da-b1f2-a460bd6b1448",
                Intent = "EmergingTech_Ryerson",
                IsLuis = true,
                Key = "c37efd10cf26495e90d584da0fa2c9d3"
            };
            //if (botIntent.IsLuis)
            //{
            return await AccessLUIS(turnContext, cancellationToken, botIntent);
            //}
            //else
            //{
            //    return await AccessQnAMaker(turnContext, cancellationToken, botIntent);
            //};
        }

        //QnAMaker BotQNA;
        //private async Task<string> AccessQnAMaker(ITurnContext turnContext, CancellationToken cancellationToken, BotIntent intent)
        //{
        //    try
        //    {
        //        BotQNA = new QnAMaker(new QnAMakerEndpoint
        //        {
        //            EndpointKey = intent.Key,
        //            Host = intent.HostName,
        //            KnowledgeBaseId = intent.Id
        //        });
        //        var results = await BotQNA.GetAnswersAsync(turnContext);
        //        if (results.Any())
        //        {
        //            return JsonConvert.SerializeObject(new IntentWithScore { Result = results.First().Answer, Score = results.First().Score.ToString() });
        //            //await turnContext.SendActivityAsync(MessageFactory.Text("QnA Maker Returned: " + results.First().Answer), cancellationToken);
        //        }
        //        else
        //        {
        //            return JsonConvert.SerializeObject(new IntentWithScore { Result = "Sorry, could not find an answer in the Q and A system.", Score = "0.00" });
        //            //await turnContext.SendActivityAsync(MessageFactory.Text("Sorry, could not find an answer in the Q and A system."), cancellationToken);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonConvert.SerializeObject(new IntentWithScore { Result = ex.Message, Score = "-1.00" });
        //        //var x = ex.Message;
        //    }
        //}
        private async Task<string> AccessLUIS(ITurnContext turnContext, CancellationToken cancellationToken, BotIntent intent)
        {
            var luisIntent = await LuisHelper.ExecuteLuisQuery(intent, null, turnContext, cancellationToken);
            return JsonConvert.SerializeObject(luisIntent);
            //await turnContext.SendActivityAsync(MessageFactory.Text($"Luis Intent: {luisIntent.Result} || Score: {luisIntent.Score}"), cancellationToken);
            //LogIntent(luisIntent);
        }

        Task IBot.OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
