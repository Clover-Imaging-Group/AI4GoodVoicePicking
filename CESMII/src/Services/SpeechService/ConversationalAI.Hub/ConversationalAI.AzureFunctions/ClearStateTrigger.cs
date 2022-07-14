using System;
using System.Threading.Tasks;
using ConversationalAI.AzureFunctions.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace ConversationalAI.AzureFunctions
{
    public static class ClearStateTrigger
    {
        [FunctionName("ClearStateTrigger")]
        public static async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.UtcNow}");
            
            var cosmosUri = Environment.GetEnvironmentVariable("CosmosUri");
            var cosmosKey = Environment.GetEnvironmentVariable("CosmosKey");
            var databaseId = Environment.GetEnvironmentVariable("CosmosDb");
            var stateCollection = Environment.GetEnvironmentVariable("ComosDbStateContainer");
            var timeoutCollection = Environment.GetEnvironmentVariable("ComosDbTimeoutContainer");
            var timeoutSeconds = int.Parse(Environment.GetEnvironmentVariable("ConversationTimeoutSeconds"));
            var botId = Environment.GetEnvironmentVariable("MicrosoftAppId");
            var appPassword = Environment.GetEnvironmentVariable("MicrosoftAppPassword");
            
            var clearConversationStateService = new ConversationService(cosmosUri, cosmosKey, databaseId, stateCollection, timeoutCollection, timeoutSeconds, botId, appPassword);
            await clearConversationStateService.SendProactiveMessages();
            
            log.LogInformation($"C# Timer trigger function completed at: {DateTime.Now}");
        }
        
        
    }
}