using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConversationalAI.Infrastructure.Interfaces.Repositories;
using ConversationalAI.Infrastructure.Models;

namespace ConversationalAI.HubService.Repositories
{       
    public class HubRepository : IHubRepository
    {
        private bool _disposedValue;
        private List<HubConnection> _hubConnections = new List<HubConnection>();
        private List<ScheduleNotification> _scheduleNotifications = new List<ScheduleNotification>();


        public Task<List<HubConnection>> GetConnections()
        {
            return Task.FromResult(_hubConnections);
        }
        
        public Task<HubConnection> GetConnections(string userId)
        {
            return Task.FromResult(_hubConnections
                .Where(x => x.User.Equals(userId))
                .OrderByDescending(x => x.LastChanged)
                .FirstOrDefault()
                );
        }

        public Task<List<ScheduleNotification>> GetScheduledNotifications(string username)
        {
            var result = _scheduleNotifications.Where(x => string.Equals(x.UserName, username, StringComparison.InvariantCultureIgnoreCase)).ToList();
            
            return Task.FromResult(_scheduleNotifications);
        }

        public Task<int> RemoveConnection(string connectionId)
        {
            try
            {
                var item = _hubConnections.FirstOrDefault(x => x.ConnectionId.Equals(connectionId, StringComparison.InvariantCultureIgnoreCase));
                _hubConnections.Remove(item);
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
                var item = new HubConnection
                {
                    ConnectionId = connectionId,
                    User = username,
                    LastChanged = DateTime.UtcNow
                };

                _hubConnections.Add(item);
            }
            catch (Exception)
            {
                return Task.FromResult(0);
            }

            return Task.FromResult(1);
        }

        public Task<int> UpdateConnection(string connectionId, string username)
        {
            try
            {
                var item = _hubConnections.FirstOrDefault(x =>
                    x.ConnectionId.Equals(connectionId, StringComparison.InvariantCultureIgnoreCase));


                if (item == null)
                {
                    return Task.FromResult(SaveConnection(connectionId, username).Result);
                }


                item.User = username;
                item.LastChanged = DateTime.UtcNow;
            }
            catch (Exception)
            {
                return Task.FromResult(0);
            }

            return Task.FromResult(1);
        }

        public Task<string> GetConnectionId(string username)
        {
            try
            {
                var item = _hubConnections.FirstOrDefault(x => x.User.Equals(username, StringComparison.InvariantCultureIgnoreCase));
                if (item != null)
                {
                    return Task.FromResult(item.ConnectionId);
                }
            }
            catch (Exception)
            {
                return Task.FromResult(string.Empty);
            }

            return Task.FromResult(string.Empty);
        }
        
        
        public Task<List<HubConnection>> GetIdleConnections()
        {
            var result = _hubConnections.Where(x => x.LastChanged < DateTime.UtcNow.AddMinutes(-1)).ToList();
            
            return Task.FromResult(result);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _hubConnections = null;
                    _scheduleNotifications = null;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
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