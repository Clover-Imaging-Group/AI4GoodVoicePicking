using System;

namespace ConversationalAI.Infrastructure.Models
{
    public class ScheduleNotification
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Notification { get; set; }
        public ScheduleType ScheduleType { get; set; }
        public DateTime BeginPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
    }
}