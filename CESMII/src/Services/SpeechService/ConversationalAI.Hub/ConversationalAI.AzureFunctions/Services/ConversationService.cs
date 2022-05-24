using System;
using System.Threading;
using System.Threading.Tasks;
using ConversationalAI.AzureFunctions.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector.Authentication;

namespace ConversationalAI.AzureFunctions.Services
{
    public class ConversationService
    {
        private readonly string _cosmosUri;
        private readonly string _cosmosKey;
        private readonly string _databaseId;
        private readonly string _timeoutCollection;
        private readonly int _timeoutSeconds;
        private readonly string _botId;

        private readonly ConversationState _conversationState;
        private readonly BotFrameworkAdapter _adapter;

        public ConversationService(string cosmosUri, string cosmosKey, string databaseId, string stateCollection,
            string timeoutCollection, int timeoutSeconds, string botId, string appPassword)
        {
            _cosmosUri = cosmosUri;
            _cosmosKey = cosmosKey;
            _databaseId = databaseId;
            _timeoutCollection = timeoutCollection;
            _timeoutSeconds = timeoutSeconds;
            _botId = botId;

            var options = new CosmosDbPartitionedStorageOptions()
            {
                AuthKey = _cosmosKey,
                ContainerId = stateCollection,
                CosmosDbEndpoint = _cosmosUri,
                DatabaseId = _databaseId,
                CompatibilityMode = false,
            };

            _conversationState = new ConversationState(new CosmosDbPartitionedStorage(options));
            
            _adapter = new BotFrameworkAdapter(new SimpleCredentialProvider(_botId, appPassword));
        }

        
    }
}