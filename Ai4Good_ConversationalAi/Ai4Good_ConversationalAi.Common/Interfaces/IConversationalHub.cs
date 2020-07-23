using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Common.Interfaces
{
    public interface IConversationalHub
    {
        public Task SendMessage(string user, string message);
        public Task Log(string user, string message);
        public Task OnConnectedAsync();
        public Task OnDisconnectedAsync(Exception exception);

    }
}
