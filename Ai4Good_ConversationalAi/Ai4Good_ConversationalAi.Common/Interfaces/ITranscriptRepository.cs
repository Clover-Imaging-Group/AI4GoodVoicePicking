using Ai4Good_ConversationalAi.Common.Models;
using System;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Common.Interfaces
{
    public interface ITranscriptRepository: IDisposable
    {
        Task<int> LogTask(SpeechTranscript status);
    }
}
