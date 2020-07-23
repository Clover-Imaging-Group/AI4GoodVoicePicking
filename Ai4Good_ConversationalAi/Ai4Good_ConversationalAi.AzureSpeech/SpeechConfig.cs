using System;
using System.Collections.Generic;
using System.Text;

namespace Ai4Good_ConversationalAi.AzureSpeech
{
    public class SpeechConfig
    {
        public string AppName { get; set; }
        public string AudioFormat { get; set; }
        public string AzureRegion { get; set; }
        public string APIKey { get; set; }
        public string Language { get; set; }
        public string VoiceName { get; set; }
        public string SynthesisEndPoint
        {
            get
            {
                return $"https://{AzureRegion}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language={Language}";
            }
        }
        public string TextToSpeechEndPoint
        {
            get
            {
                return $"https://{AzureRegion}.tts.speech.microsoft.com/cognitiveservices/v1";
            }
        }
        public string TokenEndPoint
        {
            get
            {
                return $"https://{AzureRegion}.api.cognitive.microsoft.com/sts/v1.0/issuetoken";
            }
        }
    }
}
