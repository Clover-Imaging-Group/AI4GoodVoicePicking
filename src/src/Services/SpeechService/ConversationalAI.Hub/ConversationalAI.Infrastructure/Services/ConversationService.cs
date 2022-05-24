using System;
using System.Threading;
using System.Threading.Tasks;
using ConversationalAI.Infrastructure.Interfaces.Services;
using ConversationalAI.Infrastructure.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Bot.Builder;
using Microsoft.Extensions.Configuration;

namespace ConversationalAI.Infrastructure.Services
{
    public class ConversationService : IConversationService
    {
        private readonly string _cosmosUri;
        private readonly string _cosmosKey;
        private readonly string _databaseId;
        private readonly string _collectionId;

        private static bool _ensuredCreated = false;
        private static readonly object LockObject = new object();
        
        private readonly ConversationState _conversationState;
        private readonly BotFrameworkAdapter _adapter;
        
        
        private readonly string _timeoutCollection;
        private readonly int _timeoutSeconds;
        private readonly string _botId;
        
        
        
        public ConversationService(IConfiguration config)
        {
            _cosmosUri = config["CosmosUri"];
            _cosmosKey = config["CosmosKey"];
            _databaseId = config["CosmosDb"];
            _collectionId = config["CosmosDbTimeoutContainer"];

            Initialize();
        }

        private void Initialize()
        {
            if (_ensuredCreated) return;
            
            lock (LockObject)
            {
                if (_ensuredCreated) return;
                
                using (var documentClient = new DocumentClient(new Uri(_cosmosUri), _cosmosKey))
                {
                    documentClient.CreateDatabaseIfNotExistsAsync(new Database { Id = _databaseId }).GetAwaiter().GetResult();
                    documentClient.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(_databaseId), new DocumentCollection { Id = _collectionId }).GetAwaiter().GetResult();
                }
                _ensuredCreated = true;
            }
        }

        public async Task AddOrUpdateConversationReference(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;
            var timeoutReference = new TimeoutConversationReference(activity.GetConversationReference());

            using var documentClient = new DocumentClient(new Uri(_cosmosUri), _cosmosKey);
            
            await documentClient.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseId, _collectionId), timeoutReference);
        }
        
        public async Task SendProactiveMessages()
        {
            using var documentClient = new DocumentClient(new Uri(_cosmosUri), _cosmosKey);
            
            var docs = documentClient.CreateDocumentQuery(
                UriFactory.CreateDocumentCollectionUri(_databaseId, _timeoutCollection),
                new SqlQuerySpec(
                    "SELECT * FROM TimeoutContainer r WHERE r.LastAccessed < @lastAccessedTimeout",
                    new SqlParameterCollection(new[]
                    {
                        new SqlParameter
                        {
                            Name = "@lastAccessedTimeout",
                            Value = DateTime.UtcNow.Subtract(TimeSpan.FromSeconds(_timeoutSeconds))
                        }
                    })));

            // var dialogStateProperty = _conversationState.CreateProperty<DialogState>(nameof(DialogState));
            foreach (var doc in docs)
            {
                var timeoutConversation = (TimeoutConversationReference)doc;
                await _adapter.ContinueConversationAsync(_botId,
                    timeoutConversation.ConversationReference, async (turnContext, cancellationToken) =>
                    {
                        await turnContext.SendActivityAsync(
                            "Hello.  Are you still there?  Please provide the requested information.",
                            cancellationToken: cancellationToken);

                        // NOTE: Uncomment below to clear the conversation state

                        // await dialogStateProperty.DeleteAsync(turnContext, cancellationToken);
                        // await _conversationState.SaveChangesAsync(turnContext, cancellationToken: cancellationToken);
                    }, CancellationToken.None);

                await documentClient.DeleteDocumentAsync(doc._self);
            }
        }
    }
}