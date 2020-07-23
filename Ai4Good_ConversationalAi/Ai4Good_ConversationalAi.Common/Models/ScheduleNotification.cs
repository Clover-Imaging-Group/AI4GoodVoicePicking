using System;
using System.Collections.Generic;
using System.Text;

namespace Ai4Good_ConversationalAi.Common.Models
{
    public class ScheduleNotification
    {
        public int Id { get; set; }
        public string Notification { get; set; }
        public ScheduleType ScheduleTypeId { get; set; }
        public DateTime BeginPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
    }
}
