using Ai4Good_ConversationalAi.Common.Interfaces;
using Ai4Good_ConversationalAi.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Implementation.Repositories
{
    public class HubRepository : IHubRepository
    {
        private bool disposedValue;
        private List<HubConnection> _HubConnections = new List<HubConnection>();
        private List<Prompts> _Prompts = new List<Prompts>();
        private List<ScheduleNotification> _ScheduleNotification = new List<ScheduleNotification>();


        public Task<List<HubConnection>> GetConnections()
        {
            return Task.FromResult(_HubConnections);
        }
        
        public Task<HubConnection> GetConnections(string UserId)
        {
            return Task.FromResult(_HubConnections
                .Where(x => x.User.Equals(UserId))
                .OrderByDescending(x => x.LastChanged)
                .FirstOrDefault()
                );
        }

        public Task<List<Prompts>> GetPrompts(string intent)
        {
            var result = _Prompts.Where(x => x.Intent.Equals(intent, StringComparison.InvariantCultureIgnoreCase)).ToList();
            return Task.FromResult(result);
        }

        public Task<List<ScheduleNotification>> GetScheduledNotifications(string username)
        {
            //var result = _ScheduleNotification.Where(x => x. .Equals(username, StringComparison.InvariantCultureIgnoreCase)).ToList();
            
            return Task.FromResult(_ScheduleNotification);
        }

        public Task<int> RemoveConnection(string connectionId)
        {
            try
            {
                var item = _HubConnections.Where(x => x.ConnectionId.Equals(connectionId, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                _HubConnections.Remove(item);
            }
            catch (Exception)
            {
                return Task.FromResult(0);
            }

            return Task.FromResult(1);
        }

        public Task<int> SaveConnection(string connectionId, string username)
        {
            try
            {
                var item = new HubConnection();
                item.ConnectionId = connectionId;
                item.User = username;
                item.LastChanged = DateTime.UtcNow;

                _HubConnections.Remove(item);
            }
            catch (Exception)
            {
                return Task.FromResult(0);
            }

            return Task.FromResult(1);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _HubConnections = null;
                    _Prompts = null;
                    _ScheduleNotification = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~HubRepository()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

}
