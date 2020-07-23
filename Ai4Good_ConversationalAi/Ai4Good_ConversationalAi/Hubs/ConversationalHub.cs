using Ai4Good_ConversationalAi.Common.Interfaces;
using Ai4Good_ConversationalAi.Implementation.HubChannels;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ai4Good_ConversationalAi.Models;
using Microsoft.Bot.Builder.AI.Luis;

namespace Ai4Good_ConversationalAi.Hubs
{
    public class ConversationalHub : Hub, IConversationalHub
    {
        private string _connectionId;
        private string _currentUser;

        private readonly ITranscriptRepository _transcriptRepo;
        private readonly IHubRepository _hubRepo;
        private readonly ISpeechService _speechRepo;
        LuisRecognizer _dispatch { get; set; }
        List<BotIntent> _botIntents { get; set; }

        //ToDo: Revisit this once we have more channels
        //private List<IConversationalHubChannel> _channels;

        public ConversationalHub(
                     ITranscriptRepository transcriptRepository
                   , IHubRepository hubRepository
                   , ISpeechService speechRepository
                   , LuisRecognizer dispatch
                   , List<BotIntent> botIntents
            )
        {
            //ToDo: Revisit this once we have more channels
            //_channels = new List<IConversationalHubChannel>();

            _transcriptRepo = transcriptRepository;
            _hubRepo = hubRepository;
            _speechRepo = speechRepository;
            _dispatch = dispatch;
            _botIntents = botIntents;
        }

        public async Task SendMessage(string user, string message)
        {
            await Log(user, message);
            var speechResult = _speechRepo.GetSpeech(message);
            await Log(user+": SpeechService", speechResult);
        }

        public async Task GetText2Speech(string user, string message)
        {
            var speechResult = _speechRepo.GetSpeech(message);
            await Clients.Client(Context.ConnectionId).SendAsync("Text2SpeechResponse", user, speechResult);
            await Log(user + ": Text2SpeechService", speechResult);
        }

        public async Task Log(string user, string message)
        {
            string utcTime = DateTime.UtcNow.ToString();

            //ToDo: Generate a List of Administrators to receive logs.
            await Clients.All.SendAsync("Log", user, message, utcTime);            
        }
        public override async Task OnConnectedAsync()
        {
            await _hubRepo.SaveConnection(Context.ConnectionId, _currentUser);
            await base.OnConnectedAsync();

        }
        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await _hubRepo.RemoveConnection(Context.ConnectionId);
            return;
        }
    }
}
