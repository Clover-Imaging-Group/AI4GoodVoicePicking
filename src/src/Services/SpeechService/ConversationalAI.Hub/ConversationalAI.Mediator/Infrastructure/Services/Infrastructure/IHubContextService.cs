using System.Threading.Tasks;

namespace ConversationalAI.Mediator.Infrastructure.Services.Infrastructure
{
    public interface IHubContextService
    {
        Task Log(string method, string user, string message, string originalMessage);
        Task Broadcast(string connectionId, string user, string message, string originalMessage);
        Task Broadcast(string user, string message, string originalMessage);
    }
}