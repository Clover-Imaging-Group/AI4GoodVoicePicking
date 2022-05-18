using System;
using System.Threading.Tasks;
using ConversationalAI.Infrastructure.Interfaces.Repositories;
using ConversationalAI.Infrastructure.Interfaces.Services;
using ConversationalAI.Mediator.Infrastructure;
using ConversationalAI.Mediator.Infrastructure.Services.Infrastructure;
using Microsoft.AspNetCore.SignalR;

namespace ConversationalAI.Mediator.Hubs
{
    public class ConversationalHub : Hub
    {
        private readonly IHubRepository _hubRepo;
        private readonly ISpeechService _speechService;
        private readonly IHubContextService _hubContextService;
        
        public ConversationalHub(
            IHubRepository hubRepository,
            ISpeechService speechService,
            IHubContextService hubContextService)
        {
            //TODO: Revisit this once we have more channels
            //_channels = new List<IConversationalHubChannel>();
            
            _hubRepo = hubRepository;
            _speechService = speechService;
            _hubContextService = hubContextService;
        }

        
        private async Task Broadcast(string user, string message, string originalMessage)
        {
            // await _hubRepo.UpdateConnection(Context.ConnectionId, user);
            // var connectionId = await _hubRepo.GetConnectionId(user);
            //
            // if (connectionId != null) 
            //     await Clients.Client(connectionId).SendAsync("ReceiveBroadcast", isSpeechResult ? $"{user}: Text2SpeechService" : user, message);
            
            await _hubContextService.Broadcast(Context.ConnectionId, user, message, originalMessage);
        }

        private async Task Broadcast(string user, string message)
        {
            await _hubContextService.Broadcast(Context.ConnectionId, user, message);
        }
        
        public async Task GetTextFromSpeech(string user, string base64Speech)
        {
            await _hubContextService.Log(nameof(GetTextFromSpeech), user, string.Empty, base64Speech);
            
            var text = await _speechService.ConvertSpeechToTextAsync(base64Speech);
            
            await _hubContextService.Log("TranslatedText", user, text, "base64speech");
            
            await Broadcast(user, text, "base64speech");
        }
        
        
        public async Task GetSpeechFromText(string user, string message)
        {   
            await _hubContextService.Log(nameof(GetSpeechFromText), user, string.Empty, message);

            var speechResult = await _speechService.GetSpeech(message);
            
            await Broadcast(user, speechResult, message);
        }

        public async Task ConnectToHub(string user)
        {
            await GetSpeechFromText(user, "Connected to Hub");
        }

        public async Task SendMessage(string user, string message)
        {
            
            await _hubContextService.Log(nameof(SendMessage), user, string.Empty, message);

            await GetSpeechFromText(user, message);
        }
        
        public override async Task OnConnectedAsync()
        {
            await _hubRepo.SaveConnection(Context.ConnectionId, "");
            await base.OnConnectedAsync();

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _hubRepo.RemoveConnection(Context.ConnectionId);
        }
    }
}