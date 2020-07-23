using Ai4Good_ConversationalAi.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Implementation.HubChannels
{
    public class LoggingChannel : IConversationalHubChannel
    {
        public Guid ConversationalHubChannelId { get { return Guid.Parse("1A2B03C8-3365-4A26-83E5-455B4B5733ED"); } }

        public Task SendMessage(string user, string message, string connectionId)
        {
            //string utcTime = DateTime.UtcNow.ToString();
            //await Clients.All.SendAsync("Log", user, message, utcTime);
            return null;
        }
    }
}
