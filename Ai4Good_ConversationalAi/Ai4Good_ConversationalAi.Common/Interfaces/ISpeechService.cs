using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading.Tasks;

namespace Ai4Good_ConversationalAi.Common.Interfaces
{
    public interface ISpeechService
    {
        string GetSpeech(string message);
        string ConvertToBase64([NotNull] byte[] audioBytes);
        string ConvertTextToSpeechBase64([NotNull] string text);
        byte[] ConvertTextToSpeech([NotNull] string text);
        Task<string> ConvertSpeechToText([NotNull] byte[] audioBytes, string lang = "en-US");
    }
}
