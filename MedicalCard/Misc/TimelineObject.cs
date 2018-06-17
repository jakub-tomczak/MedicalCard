using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalCard.Misc
{
    public class TimelineObject
    {
        public TimelineEvent EventType { set => eventType = value; }
        public string EventTypeName { get => nameof(eventType); }
        public string Code { get => code; set => code = value; }
        public string Header { get => header; set => header = value; }
        public DateTime Date { get => date; set => date = value; }
        public string Description { get => description; set => description = value; }

        private DateTime date;
        private string header;
        private string description;
        private string code;
        private TimelineEvent eventType;
    }
    public enum TimelineEvent
    {
        Medication,
        MedicationRequest,
        ObservationMisc,
        ObservationDeath
    }
}
