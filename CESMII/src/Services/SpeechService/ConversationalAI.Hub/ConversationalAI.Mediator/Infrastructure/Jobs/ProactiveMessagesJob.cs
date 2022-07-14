using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ConversationalAI.Infrastructure.Interfaces.Repositories;
using ConversationalAI.Infrastructure.Interfaces.Services;
using ConversationalAI.Mediator.Hubs;
using ConversationalAI.Mediator.Infrastructure.Services.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace ConversationalAI.Mediator.Infrastructure.Jobs
{
    public class ProactiveMessagesJob: IJob
    {
        private readonly ILogger<ProactiveMessagesJob> _logger;
        private readonly IHubRepository _hubRepository;
        private readonly IHubContextService _hubContextService;
        private readonly ISpeechService _speechService;

        private readonly Dictionary<int, string> idleMessageOptions;

        public ProactiveMessagesJob(ILogger<ProactiveMessagesJob> logger, 
            IHubRepository hubRepository,
            IHubContextService hubContextService, ISpeechService speechService)
        {
            _logger = logger;
            _hubRepository = hubRepository;
            _hubContextService = hubContextService;
            _speechService = speechService;
            idleMessageOptions = new Dictionary<int, string>
            {
                { 1, "Do you need assistance?" },
                { 2, "How can I help you?" },
                { 3, "What can I do for you?" },
                { 4, "Are you there?" },
                { 5, "Are you okay?" }
            };
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var idleConnections =  await _hubRepository.GetIdleConnections();
            
            // Send proactive messages to idle connections
            foreach (var connection in idleConnections)
            {
                // await _hubRepository.UpdateConnection(connection.ConnectionId, connection.User);
                
                // await _hubContext.Clients.All.SendAsync("ConcurrentJobs", beginMessage);
                
                // await _hubContext.Clients.Client(connection.ConnectionId).SendAsync("ReceiveBroadcast", connection.User, "Do you need assistance?");

                // await _hub.Broadcast(connection.User, "Do you need assistance?", false);
                
                
                //get random message from messagesList
                var randomMessage = idleMessageOptions[new Random().Next(1, idleMessageOptions.Count)];

                var message = await _speechService.GetSpeech(randomMessage); ;
                await _hubContextService.Broadcast(connection.ConnectionId, connection.User, message, randomMessage);
            }
        }
        
    }
}