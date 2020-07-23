using Ai4Good_ConversationalAi.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Common.Interfaces
{
    public interface IHubRepository: IDisposable
    {
        Task<int> SaveConnection(string connectionId, string username);
        Task<List<HubConnection>> GetConnections();
        Task<int> RemoveConnection(string connectionId);
        Task<List<Prompts>> GetPrompts(string intent);
        Task<List<ScheduleNotification>> GetScheduledNotifications(string username);
    }
}
