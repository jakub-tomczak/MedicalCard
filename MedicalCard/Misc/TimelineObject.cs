using System;

namespace MedicalCard.Misc
{
    public class TimelineObject
    {
        public TimelineEvent EventType { set => eventType = value; }
        public string EventTypeName { get => GetEventName(); }
        public string Code { get => code; set => code = value; }
        public string Header { get => header; set => header = value; }
        public DateTime Date { get => date; set => date = value; }
        public string Description { get => description; set => description = value; }

        private DateTime date;
        private string header;
        private string description;
        private string code;
        private TimelineEvent eventType;

        private string GetEventName()
        {
            switch (eventType)
            {
                case TimelineEvent.Medication:
                    return "Lekarstwo";
                case TimelineEvent.MedicationRequest:
                    return "Prośba o lekarstow";
                case TimelineEvent.ObservationMisc:
                    return "Obserwacja";
                case TimelineEvent.ObservationDeath:
                    return "Śmierć";
                default:
                    return "Inne";
            }
        }
    }
    public enum TimelineEvent
    {
        Medication,
        MedicationRequest,
        ObservationMisc,
        ObservationDeath
    }
}
