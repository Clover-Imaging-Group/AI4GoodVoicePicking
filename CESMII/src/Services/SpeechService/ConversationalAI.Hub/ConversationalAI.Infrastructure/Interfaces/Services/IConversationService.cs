using System.Threading.Tasks;
using Microsoft.Bot.Builder;

namespace ConversationalAI.Infrastructure.Interfaces.Services
{
    public interface IConversationService
    {
        Task AddOrUpdateConversationReference(ITurnContext turnContext);
        Task SendProactiveMessages();
    }
}