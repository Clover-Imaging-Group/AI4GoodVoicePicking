using System;

namespace ConversationalAI.Infrastructure.Models
{
    public class HubConnection
    {
        public string ConnectionId { get; set; }
        public string User { get; set; }
        public DateTime LastChanged { get; set; }
    }
}