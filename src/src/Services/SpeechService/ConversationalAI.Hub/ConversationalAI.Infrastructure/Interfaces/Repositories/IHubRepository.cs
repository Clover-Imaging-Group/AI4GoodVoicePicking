using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ConversationalAI.Infrastructure.Models;

namespace ConversationalAI.Infrastructure.Interfaces.Repositories
{
    public interface IHubRepository: IDisposable
    {
        Task<int> SaveConnection(string connectionId, string username);
        Task<int> UpdateConnection(string connectionId, string username);
        Task<string> GetConnectionId(string username);
        Task<List<HubConnection>> GetConnections();
        Task<int> RemoveConnection(string connectionId);
        Task<List<HubConnection>> GetIdleConnections();
        Task<List<ScheduleNotification>> GetScheduledNotifications(string username);
        
    }
}