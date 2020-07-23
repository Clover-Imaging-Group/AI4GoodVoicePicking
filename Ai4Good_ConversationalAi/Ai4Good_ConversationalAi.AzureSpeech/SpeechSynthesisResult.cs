using System;
using System.Collections.Generic;
using System.Text;

namespace Ai4Good_ConversationalAi.AzureSpeech
{
    public class SpeechSynthesisResult
    {
        public string RecognitionStatus { get; set; }
        public string DisplayText { get; set; }
        public int Offset { get; set; }
        public int Duration { get; set; }
    }
}
