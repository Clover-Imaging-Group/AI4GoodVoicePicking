using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace ConversationalAI.Infrastructure.Interfaces.Services
{
    public interface ISpeechService
    {
        Task<string> GetSpeech(string message);
        string ConvertToBase64([NotNull] byte[] audioBytes);
        Task<string> ConvertTextToSpeechBase64([NotNull] string text);
        Task<byte[]> ConvertTextToSpeechAsync([NotNull] string text);
        Task<string> ConvertSpeechToTextAsync([NotNull]string audioBase64);
        Task<string> ConvertSpeechToTextAsync([NotNull] byte[] audioBytes);
    }
}