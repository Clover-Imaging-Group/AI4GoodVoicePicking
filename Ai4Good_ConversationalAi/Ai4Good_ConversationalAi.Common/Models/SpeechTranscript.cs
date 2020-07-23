using System;

namespace Ai4Good_ConversationalAi.Common.Models
{
    public partial class SpeechTranscript
    {
        public Guid SpeechTranscriptPK { get; set; }
        public Guid SessionID { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}
