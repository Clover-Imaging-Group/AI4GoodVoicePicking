using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Common.Interfaces
{
    public interface IConversationalHubChannel
    {
        public Guid ConversationalHubChannelId { get; }
        public Task SendMessage(string user, string message, string connectionId);
        
    }
}
