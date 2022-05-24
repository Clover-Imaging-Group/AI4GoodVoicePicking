using System;

namespace ConversationalAI.Infrastructure.Models
{
    public partial class SpeechTranscript
    {
        public Guid SpeechTranscriptPk { get; set; }
        public Guid SessionId { get; set; }
        public string Message { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
    }
}